using System.Collections.Concurrent;

namespace mass_upload_via_s3_csharp;

public static class UploadProgress
{
    private static readonly ConcurrentDictionary<int, (int BulkUploadId, long Bytes)> InFlight = new();

    public static void Set(int bulkUploadId, int fileId, long bytes) => InFlight[fileId] = (bulkUploadId, bytes);

    public static void Remove(int fileId) => InFlight.TryRemove(fileId, out _);

    public static long Bytes(int bulkUploadId) =>
        InFlight.Values.Where(x => x.BulkUploadId == bulkUploadId).Sum(x => x.Bytes);
}
