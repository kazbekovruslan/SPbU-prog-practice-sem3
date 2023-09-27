namespace LazyTests;

public class Tests
{
    private static readonly Random Rand = new();

    private static int CounterOfCalls { get; set; }

    private static IEnumerable<TestCaseData> LaziesWithNormalFunctions()
    {
        yield return new TestCaseData(new SingleThreadLazy<int>(() => Rand.Next()));
        yield return new TestCaseData(new MultiThreadLazy<int>(() => Rand.Next()));
    }

    private static IEnumerable<TestCaseData> LaziesWithCounterInSupplier()
    {
        yield return new TestCaseData(new SingleThreadLazy<int>(() =>
        {
            CounterOfCalls = 0;
            ++CounterOfCalls;
            return 0;
        }));
        yield return new TestCaseData(new MultiThreadLazy<int>(() =>
        {
            CounterOfCalls = 0;
            ++CounterOfCalls;
            return 0;
        }));
    }

    private static IEnumerable<TestCaseData> LaziesWithExceptionInSupplier()
    {
        yield return new TestCaseData(new SingleThreadLazy<int>(() => throw new Exception()));
        yield return new TestCaseData(new MultiThreadLazy<int>(() => throw new Exception()));
    }

    [TestCaseSource(nameof(LaziesWithExceptionInSupplier))]
    public void LaziesWithExceptionInSupplierShouldThrowException(ILazy<int> lazy)
    {
        Assert.Throws<Exception>(() => lazy.Get());
        Assert.Throws<Exception>(() => lazy.Get());
    }

    [TestCaseSource(nameof(LaziesWithNormalFunctions))]
    public void FirstAndSecondCallOfLazyShouldGiveSameResults(ILazy<int> lazy)
    {
        var firstCall = lazy.Get();
        var secondCall = lazy.Get();

        Assert.That(firstCall, Is.EqualTo(secondCall));
    }

    [TestCaseSource(nameof(LaziesWithCounterInSupplier))]
    public void SupplierShouldBeEvaluatedOnlyOnce(ILazy<int> lazy)
    {
        lazy.Get();
        lazy.Get();
        Assert.That(CounterOfCalls, Is.EqualTo(1));
    }

    [Test]
    public void MultiThreadLazyShouldWorkInParallel()
    {
        var resultValueOfLazyGet = Rand.Next();
        var multiThreadLazy = new MultiThreadLazy<int>(() => resultValueOfLazyGet);

        var processorsCount = Environment.ProcessorCount;
        var threads = new Thread[processorsCount];
        var resultsOfLazyGet = new int[processorsCount];

        for (var i = 0; i < processorsCount; i++)
        {
            var local = i;
            threads[i] = new Thread(() =>
            {
                resultsOfLazyGet[local] = multiThreadLazy.Get();
            });
            threads[i].Start();
        }

        foreach (var thread in threads)
        {
            thread.Join();
        }

        for (var i = 0; i < processorsCount; i++)
        {
            Assert.That(resultsOfLazyGet[i], Is.EqualTo(resultValueOfLazyGet));
        }
    }
}