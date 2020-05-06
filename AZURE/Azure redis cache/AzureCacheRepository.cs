using System;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Framework.Exceptions;
using LoggerLibrary.Logger;
using StackExchange.Redis;

namespace AzureStorage.AzureCache
{
    public class AzureCacheRepository : IAzureCacheRepository, IDisposable
    {
        private readonly Lazy<ConnectionMultiplexer> _lazyConnection;
        private IDatabase MultiplexerDatabase => _lazyConnection.Value.GetDatabase();
        private readonly ILogger _logger;

        public AzureCacheRepository(ILogger logger)
        {
            _logger = logger;
            _lazyConnection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect("secret-key"));
        }

        public Result<string> GetValue(string key)
        {
            try
            {
                return Result.Ok<string>(MultiplexerDatabase.StringGet(key));
            }
            catch (Exception ex)
            {
                _logger.Log(new SFCommonException($"Error getting value from azure cache.{ex.GetExceptionDetailMessage()}", ex));
                return Result.Fail<string>("Error getting value from azure cache");
            }
        }

        public Result<byte[]> GetBinaryValue(string key)
        {
            try
            {
                return Result.Ok<byte[]>(MultiplexerDatabase.StringGet(key));
            }
            catch (Exception ex)
            {
                _logger.Log(new SFCommonException($"Error getting value from azure cache.{ex.GetExceptionDetailMessage()}", ex));
                return Result.Fail<byte[]>("Error getting value from azure cache");
            }
        }

        public Result<bool> SetCache(string key, string value, TimeSpan? expiry)
        {
            try
            {
                return Result.Ok(MultiplexerDatabase.StringSet(key, value, expiry));
            }
            catch (Exception ex)
            {
                _logger.Log(new SFCommonException($"Error to set value in azure cache. {ex.GetExceptionDetailMessage()}", ex));
                return Result.Fail<bool>("Error to set value in azure cache.");
            }
        }

        public Result<bool> SetCache(string key, byte[] value, TimeSpan? expiry)
        {
            try
            {
                return Result.Ok(MultiplexerDatabase.StringSet(key, value, expiry));
            }
            catch (Exception ex)
            {
                _logger.Log(new SFCommonException($"Error to set value in azure cache. {ex.GetExceptionDetailMessage()}", ex));
                return Result.Fail<bool>("Error to set value in azure cache.");
            }
        }

        public async Task<Result<bool>> IsKeyExistsAsync(string key)
        {            
            try
            {
                return Result.Ok(await MultiplexerDatabase.KeyExistsAsync(key));
            }
            catch (Exception ex)
            {
                _logger.Log(new SFCommonException($"Error to communicate with azure cache. {ex.GetExceptionDetailMessage()}", ex));
                return Result.Fail<bool>("Error to communicate with azure cache.");
            }
        }

        public void Dispose()
        {
            _lazyConnection?.Value?.Dispose();
        }
    }
}
