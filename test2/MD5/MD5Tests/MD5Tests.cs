namespace MD5Tests;

using MD5;

public class Tests
{
    [Test]
    public void CheckSumShouldBeEqualInTwoComputes()
    {
        var path = "../../../../MD5";
        var checkSum1 = CheckSum.ComputeCheckSum(path);
        var checkSum2 = CheckSum.ComputeCheckSum(path);
        Assert.That(checkSum1, Is.EqualTo(checkSum2));
    }

    [Test]
    public void CheckSumParallelShouldBeEqualInTwoComputes()
    {
        var path = "../../../../MD5";
        var checkSum1 = CheckSum.ComputeCheckSumParallel(path);
        var checkSum2 = CheckSum.ComputeCheckSumParallel(path);
        Assert.That(checkSum1, Is.EqualTo(checkSum2));
    }

    [Test]
    public void CheckSumAndCheckSumParallelShouldBeEqual()
    {
        var path = "../../../../MD5";
        var checkSum1 = CheckSum.ComputeCheckSum(path);
        var checkSum2 = CheckSum.ComputeCheckSumParallel(path);
        Assert.That(checkSum1, Is.EqualTo(checkSum2));
    }
}