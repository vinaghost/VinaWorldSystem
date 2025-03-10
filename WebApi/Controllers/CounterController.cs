﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using WebApi.Context;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CounterController : ControllerBase
    {
        private readonly ILogger<CounterController> _logger;
        private readonly IDistributedCache _cache;
        private readonly AppDbContext _context;

        public CounterController(ILogger<CounterController> logger, IDistributedCache cache, AppDbContext context)
        {
            _logger = logger;
            _cache = cache;
            _context = context;
        }

        [HttpGet(Name = "GetCounter")]
        public string Get()
        {
            string key = "Counter";
            string? result = null;
            try
            {
                var counterStr = _cache.GetString(key);
                if (int.TryParse(counterStr, out int counter))
                {
                    counter++;
                }
                else
                {
                    counter = 0;
                }
                result = counter.ToString();
                _cache.SetString(key, result);
            }
            catch (RedisConnectionException)
            {
                result = "Redis cache is not found.";
            }
            return result;
        }
    }
}