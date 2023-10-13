namespace MyThreadPool;

/// <summary>
/// Interface that represents a Task data structure. 
/// </summary>
/// <typeparam name="TResult">Function's return type</typeparam>
public interface IMyTask<TResult>
{
    /// <summary>
    /// Checks if task is completed.
    /// </summary>
    public bool IsCompleted { get; }

    /// <summary>
    /// Function's computing result.
    /// </summary>
    public TResult? Result { get; }

    /// <summary>
    /// Applies the second function to the result of the first function and returns a new task.
    /// </summary>
    /// <param name="function">Second function</param>
    /// <typeparam name="TNewResult"></typeparam>
    /// <returns>New continuation task.</returns>
    public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult?, TNewResult> function);
}