using System;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace MyThreadPool;

class MyThreadPool
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

    private class MyTask<TResult> : IMyTask<TResult>
    {
        private bool isCompleted;
        private TResult? result;
        private readonly Func<TResult> taskFunction;
        private readonly Exception? taskFuncException;
        private readonly MyThreadPool threadPool;

        public MyTask(Func<TResult> taskFunction, MyThreadPool threadPool)
        {
            this.taskFunction = taskFunction;
            this.threadPool = threadPool;
            isCompleted = false;
        }
        public bool IsCompleted => isCompleted;

        public TResult? Result
        {
        }

        public void ComputeResult()
        {
        }

        public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> function)
        {
        }
    }
}