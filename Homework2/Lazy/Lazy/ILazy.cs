namespace Lazy;

/// <summary>
/// Interface for lazy evaluation.
/// </summary>
/// <typeparam name="T">Generic type of result of calculating.</typeparam>
public interface ILazy<T>
{
    /// <summary>
    /// Provides access to the result of calculating.
    /// </summary>
    /// <returns>Result of calculating by supplier.</returns>
    T? Get();
}