namespace Lazy;

public class SingleThreadLazy<T> : ILazy<T>
{
    private Func<T>? supplier;
    private T? result;
    private bool isResultReady;
    private Exception? thrownException;

    public SingleThreadLazy(Func<T> supplier)
    {
        this.supplier = supplier;
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

        return result;
    }
}