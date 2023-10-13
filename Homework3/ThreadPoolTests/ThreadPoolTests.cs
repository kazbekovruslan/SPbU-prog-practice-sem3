using System.ComponentModel;

namespace ThreadPoolTests;

public class MyThreadPoolTests
{
    private MyThreadPool.MyThreadPool myThreadPool = null!;

    [SetUp]
    public void Setup()
    {
        myThreadPool = new(Environment.ProcessorCount);
    }

    [Test]
    public void ResultAfterSumbit_ShouldGiveExpectedValue()
    {
        var task = myThreadPool.Submit(() => (1 + 1) * 2);
        Assert.That(task.Result, Is.EqualTo(4));
    }

    [Test]
    public void ResultAfterShutdown_ShouldGiveExpectedValue()
    {
        var task = myThreadPool.Submit(() => (1 + 1) * 2);
        myThreadPool.Shutdown();
        Assert.That(task.Result, Is.EqualTo(4));
    }

    [Test]
    public void ContinueWith_ShouldGiveExpectedValue()
    {
        for (int i = 0; i < 10; ++i)
        {
            var localI = i;
            var task = myThreadPool.Submit(() => localI).ContinueWith(x => x + 1);
            Assert.That(task.Result, Is.EqualTo(localI + 1));
        }
    }

    [Test]
    public void SubmitAfterShutDown_ShouldThrowException()
    {
        myThreadPool.Shutdown();
        Assert.Throws<InvalidOperationException>(() => myThreadPool.Submit(() => 1));
    }

    [Test]
    public void ContinueWithAfterShutDown()
    {
        var task = myThreadPool.Submit(() => 1);
        myThreadPool.Shutdown();
        Assert.Throws<InvalidOperationException>(() => task.ContinueWith(x => x + 1));
    }
}