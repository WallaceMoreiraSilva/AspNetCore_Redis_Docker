using AspNetCore_Redis_Docker.Data;
using AspNetCore_Redis_Docker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore_Redis_Docker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IDistributedCache distributedCache;
        private readonly ContextBase context;

        public OrdersController(ContextBase context, IDistributedCache distributedCache)
        {
            this.context = context;
            this.distributedCache = distributedCache;
        }

        [HttpGet("normal")]
        public async Task<IActionResult> GetAllOrders()
        {
            var orderList = await context.Paises.ToListAsync();
            return Ok(orderList);
        }

        [HttpGet("redis")]
        public async Task<IActionResult> GetAllOrdersUsingRedisCache()
        {
            var cacheKey = "orderList";
            string serializedOrderList;
            var orderList = new List<Pais>();

            var redisOrderList = await distributedCache.GetAsync(cacheKey);

            if (redisOrderList != null)
            {
                serializedOrderList = Encoding.UTF8.GetString(redisOrderList);
                orderList = JsonConvert.DeserializeObject<List<Pais>>(serializedOrderList);
            }
            else
            {
                orderList = await context.Paises.ToListAsync();

                serializedOrderList = JsonConvert.SerializeObject(orderList);

                redisOrderList = Encoding.UTF8.GetBytes(serializedOrderList);

                var options = new DistributedCacheEntryOptions()
                    .SetAbsoluteExpiration(DateTime.Now.AddMinutes(10))
                    .SetSlidingExpiration(TimeSpan.FromMinutes(2));

                await distributedCache.SetAsync(cacheKey, redisOrderList, options);
            }
            return Ok(orderList);
        }
    }
}
