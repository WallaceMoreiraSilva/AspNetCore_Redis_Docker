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
    public class PaisesController : ControllerBase
    {
        private readonly IDistributedCache distributedCache;
        private readonly ContextBase context;

        public PaisesController(ContextBase context, IDistributedCache distributedCache)
        {
            this.context = context;
            this.distributedCache = distributedCache;
        }

        [HttpGet("normal")]
        public async Task<IActionResult> GetAllPaises()
        {
            var paisesList = await context.Paises.ToListAsync();
            return Ok(paisesList);
        }

        [HttpGet("redis")]
        public async Task<IActionResult> GetAllPaisesUsingRedisCache()
        {
            var cacheKey = "paisesList";
            string serializedPaisesList;
            var paisesList = new List<Pais>();

            var redisPaisesList = await distributedCache.GetAsync(cacheKey);

            if (redisPaisesList != null)
            {
                serializedPaisesList = Encoding.UTF8.GetString(redisPaisesList);
                paisesList = JsonConvert.DeserializeObject<List<Pais>>(serializedPaisesList);
            }
            else
            {
                paisesList = await context.Paises.ToListAsync();

                serializedPaisesList = JsonConvert.SerializeObject(paisesList);

                redisPaisesList = Encoding.UTF8.GetBytes(serializedPaisesList);

                var options = new DistributedCacheEntryOptions()
                    .SetAbsoluteExpiration(DateTime.Now.AddMinutes(10))
                    .SetSlidingExpiration(TimeSpan.FromMinutes(2));

                await distributedCache.SetAsync(cacheKey, redisPaisesList, options);
            }
            return Ok(paisesList);
        }
    }
}
