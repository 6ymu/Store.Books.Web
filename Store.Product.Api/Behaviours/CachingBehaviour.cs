
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;
using Store.Product.Api.Interfaces;
using System.Text;
using Serilog.Core;
using Newtonsoft.Json;

namespace Store.Product.Api.Behaviours
{
    public class CachingBehaviour<TRequest, TResponse>: IPipelineBehavior<TRequest,TResponse> where TRequest: ICacheableQuery
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger _logger;
        private readonly CacheConfig _settings;
        
        public CachingBehaviour(IDistributedCache cache,
            ILogger<CachingBehaviour<TRequest,TResponse>> logger,
            IOptions<CacheConfig> settings)
        {
            _cache = cache;
            _logger = logger;
            //_settings = settings;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            TResponse response;
            if (request.BypassCache) return await next();
            async Task<TResponse> GetResponseAndAddToCache()
            {
                response = await next();
                var slidingExpiration = request.SlidingExpiration == null ? TimeSpan.FromHours(2) : request.SlidingExpiration;
                var options = new DistributedCacheEntryOptions { SlidingExpiration = slidingExpiration };
                var serializedData = Encoding.Default.GetBytes(JsonConvert.SerializeObject(response));
                await _cache.SetAsync(request.CacheKey, serializedData, options, cancellationToken);
                return response;
            }

            var cachedResponse = await _cache.GetAsync(request.CacheKey, cancellationToken);
            if(cachedResponse != null)
            {
                response = JsonConvert.DeserializeObject<TResponse>(Encoding.Default.GetString(cachedResponse));
                _logger.LogInformation($"fetched from cache->' {request.CacheKey}'.");
            }
            else
            {
                response = await GetResponseAndAddToCache();
                _logger.LogInformation($"added to cache->' {request.CacheKey}'.");
            }
            return response;

        }

        public class CacheConfig
        {
        }
    }
}
