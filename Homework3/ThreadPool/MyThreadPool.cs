using System;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace MyThreadPool;

public class MyThreadPool
{
    private readonly Thread[] threads;
    private CancellationTokenSource cancellationTokenSource;
    private BlockingCollection<Action> tasks;
    private object lockObject;

    public MyThreadPool(int threadAmount)
    {
        if (threadAmount < 1)
        {
            throw new ArgumentException("Amount of threads can't be negative!");
        }

        threads = new Thread[threadAmount];
        cancellationTokenSource = new CancellationTokenSource();
        tasks = new BlockingCollection<Action>();
        lockObject = new();

        InitThreads();
    }

    private void InitThreads()
    {
        for (var i = 0; i < threads.Length; i++)
        {
            threads[i] = new(() =>
            {
                foreach (var task in tasks.GetConsumingEnumerable())
                {
                    task();
                }
            });
            threads[i].Start();
        }
    }

    public IMyTask<TResult> Submit<TResult>(Func<TResult> function)
    {
        if (cancellationTokenSource.IsCancellationRequested)
        {
            throw new InvalidOperationException("Thread pool is already shut downed!");
        }

        lock (lockObject)
        {
            var task = new MyTask<TResult>(function, this);
            tasks.Add(task.ComputeResult);

            return task;
        }
    }

    public void Shutdown()
    {
        lock (lockObject)
        {
            cancellationTokenSource.Cancel();
            tasks.CompleteAdding();
            // tasks.IsAddingCompleted
        }

        foreach (var thread in threads)
        {
            thread.Join();
        }
    }

    public class MyTask<TResult> : IMyTask<TResult>
    {
        private volatile bool isCompleted;
        private TResult? result;
        private readonly Func<TResult> taskFunction;
        private Exception? taskFuncException;
        private readonly MyThreadPool threadPool;
        private readonly object taskLockObject;
        private readonly ManualResetEvent resultIsCompletedEvent;
        private ConcurrentQueue<Action> continuationTasks;

        public MyTask(Func<TResult> taskFunction, MyThreadPool threadPool)
        {
            this.taskFunction = taskFunction;
            this.threadPool = threadPool;
            isCompleted = false;
            taskLockObject = new object();
            resultIsCompletedEvent = new ManualResetEvent(false);
            continuationTasks = new ConcurrentQueue<Action>();
        }

        public bool IsCompleted => isCompleted;

        public TResult? Result
        {
            get
            {
                if (!isCompleted)
                {
                    resultIsCompletedEvent.WaitOne();
                }

                if (taskFuncException != null)
                {
                    throw new AggregateException(taskFuncException);
                }

                return result;
            }
        }

        public void ComputeResult()
        {
            lock (taskLockObject)
            {
                try
                {
                    result = taskFunction();
                }
                catch (Exception ex)
                {
                    taskFuncException = ex;
                }
                finally
                {
                    isCompleted = true;
                    resultIsCompletedEvent.Set();

                    while (!continuationTasks.IsEmpty)
                    {
                        if (continuationTasks.TryDequeue(out var result))
                        {
                            threadPool.tasks.Add(result);
                        }
                    }
                }
            }
        }

        public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult?, TNewResult> function)
        {
            if (threadPool.cancellationTokenSource.IsCancellationRequested)
            {
                throw new InvalidOperationException();
            }

            lock (taskLockObject)
            {
                if (isCompleted)
                {
                    return threadPool.Submit(() => function(Result));
                }
                var continuation = new MyTask<TNewResult>(() => function(Result), threadPool);
                continuationTasks.Enqueue(continuation.ComputeResult);
                return continuation;
            }
        }
    }
}