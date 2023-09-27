namespace Lazy;

public class MultiThreadLazy<T> : ILazy<T>
{
    private Func<T>? supplier;
    private T? result;
    private volatile bool isResultReady;
    private Exception? thrownException;
    private readonly object lockObject;

    public MultiThreadLazy(Func<T> supplier)
    {
        this.supplier = supplier;
        this.lockObject = new object();
        this.isResultReady = false;
    }

    public T? Get()
    {
        if (thrownException != null)
        {
            throw thrownException;
        }

        if (supplier == null)
        {
            throw new InvalidOperationException("Supplier can't be null!");
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

