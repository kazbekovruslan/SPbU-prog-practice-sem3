namespace Lazy;

/// <summary>
/// Class that represents single thread version of Lazy interface.
/// </summary>
/// <typeparam name="T">Generic type of result of calculating.</typeparam>
public class SingleThreadLazy<T> : ILazy<T>
{
    private Func<T?>? supplier;
    private T? result;
    private bool isResultReady;
    private Exception? thrownException;

    /// <summary>
    /// Standard constructor for SingleThreadLazy class.
    /// </summary>
    /// <param name="supplier">Function that calculate the result.</param>
    public SingleThreadLazy(Func<T> supplier)
    {
        this.supplier = supplier ?? throw new ArgumentException("Supplier can't be null!");
        this.isResultReady = false;
    }

    /// <inheritdoc/>
    public T? Get()
    {
        if (thrownException != null)
        {
            throw thrownException;
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