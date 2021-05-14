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
        private readonly NorthwindContext context;

        public OrdersController(NorthwindContext context, IDistributedCache distributedCache)
        {
            this.context = context;
            this.distributedCache = distributedCache;
        }

        [HttpGet("normal")]
        public async Task<IActionResult> GetAllOrders()
        {
            var orderList = await context.Orders.ToListAsync();
            return Ok(orderList);
        }

        [HttpGet("redis")]
        public async Task<IActionResult> GetAllOrdersUsingRedisCache()
        {
            var cacheKey = "orderList";
            string serializedOrderList;
            var orderList = new List<Order>();

            var redisOrderList = await distributedCache.GetAsync(cacheKey);

            if (redisOrderList != null)
            {
                serializedOrderList = Encoding.UTF8.GetString(redisOrderList);
                orderList = JsonConvert.DeserializeObject<List<Order>>(serializedOrderList);
            }
            else
            {
                orderList = await context.Orders.ToListAsync();

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
