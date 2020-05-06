using System;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;

namespace AzureStorage.AzureCache
{
    public interface IAzureCacheRepository
    {
        Result<string> GetValue(string key);
        Result<byte[]> GetBinaryValue(string key);
        Result<bool> SetCache(string key, string value, TimeSpan? expiry);
        Result<bool> SetCache(string key, byte[] value, TimeSpan? expiry);
        Task<Result<bool>> IsKeyExistsAsync(string key);
    }
}
