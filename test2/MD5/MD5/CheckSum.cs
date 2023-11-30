namespace MD5;

using System.Text;

public static class CheckSum
{
    /// <summary>
    /// Computes check-sum for a file or directory.
    /// </summary>
    /// <param name="path">The path to the file or directory.</param>
    /// <returns>The check-sum of the file or directory.</returns>
    public static byte[] ComputeCheckSum(string path)
    {
        if (!Directory.Exists(path) && !File.Exists(path))
        {
            throw new DirectoryNotFoundException();
        }

        // just file
        if (File.Exists(path))
        {
            var resultFileHash = ComputeFileHash(path);
            return resultFileHash;
        }

        // directory
        var resultHash = ComputeDirectoryEntriesHash(path);
        return resultHash;
    }

    private static byte[] ComputeDirectoryEntriesHash(string path)
    {
        using var md5 = System.Security.Cryptography.MD5.Create();
        var subdirectories = Directory.GetDirectories(path);
        var filesInDirectory = Directory.GetFiles(path);
        var hashes = new List<byte[]>();

        Array.Sort(subdirectories);
        Array.Sort(filesInDirectory);

        foreach (var file in filesInDirectory)
        {
            var hash = ComputeFileHash(file);
            hashes.Add(hash);
        }

        foreach (var subdirectory in subdirectories)
        {
            var currentSubDirectoryHash = ComputeDirectoryEntriesHash(subdirectory);
            hashes.Add(currentSubDirectoryHash);
        }

        var resultHash = ConcatenateHashesAndNameOfDirectory(hashes, path);
        return resultHash;
    }

    private static byte[] ComputeFileHash(string path)
    {
        using var stream = File.OpenRead(path);
        using var md5 = System.Security.Cryptography.MD5.Create();

        var hash = md5.ComputeHash(stream);
        return hash;
    }

    private static byte[] ConcatenateHashesAndNameOfDirectory(List<byte[]> hashes, string path)
    {
        var bytesOfDirectoryName = Encoding.ASCII.GetBytes(path);

        var lengthOfResultBytesArray = bytesOfDirectoryName.Length;
        foreach (var hash in hashes)
        {
            lengthOfResultBytesArray += hash.Length;
        }
        var resultByteArray = new byte[lengthOfResultBytesArray];

        // directory name
        var offset = 0;
        Buffer.BlockCopy(bytesOfDirectoryName, 0, resultByteArray, offset, bytesOfDirectoryName.Length);
        offset += bytesOfDirectoryName.Length;

        // all other stuff
        foreach (var hash in hashes)
        {
            Buffer.BlockCopy(hash, 0, resultByteArray, offset, hash.Length);
            offset += hash.Length;
        }

        return resultByteArray;
    }

}