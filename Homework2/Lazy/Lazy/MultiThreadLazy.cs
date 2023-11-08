namespace Lazy;

/// <summary>
/// Class that represents multi thread version of Lazy interface.
/// </summary>
/// <typeparam name="T">Generic type of result of calculating.</typeparam>
public class MultiThreadLazy<T> : ILazy<T>
{
    private Func<T?>? supplier;
    private T? result;
    private volatile bool isResultReady;
    private Exception? thrownException;
    private readonly object lockObject;

    /// <summary>
    /// Standard constructor for MultiThreadLazy class.
    /// </summary>
    /// <param name="supplier">Function that calculate the result.</param>
    public MultiThreadLazy(Func<T> supplier)
    {
        this.supplier = supplier ?? throw new ArgumentException("Supplier can't be null!");
        this.lockObject = new object();
        this.isResultReady = false;
    }

    /// <inheritdoc/>
    public T? Get()
    {
        if (thrownException != null)
        {
            throw thrownException;
        }

        // for not to lock if you don't need to
        if (!isResultReady)
        {
            lock (lockObject)
            {
                if (!isResultReady)
                {
                    try
                    {
                        result = supplier!();
                    }
                    catch (Exception ex)
                    {
                        thrownException = ex;
                        throw;
                    }
                    finally
                    {
                        isResultReady = true;
                        supplier = null;
                    }
                }
            }
        }

        return result;
    }
}

