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
            //chave que vou usar para obter o valor do cache
            var cacheKey = "paisesList";
            //armazeno os dados serializados
            string serializedPaisesList;
            //crio uma instacia dos meus paises que quero obter
            var paisesList = new List<Pais>();

            //Pega os dados do cache
            var redisPaisesList = await distributedCache.GetAsync(cacheKey);

            if (redisPaisesList != null) //significa que eu tenho dados no cache
            {
                serializedPaisesList = Encoding.UTF8.GetString(redisPaisesList);
                //Desserializa e retorna os dados no cache
                paisesList = JsonConvert.DeserializeObject<List<Pais>>(serializedPaisesList);
            }
            else
            {
                //obtem os dados da tabela
                paisesList = await context.Paises.ToListAsync();

                //serializa os dados da tabela
                serializedPaisesList = JsonConvert.SerializeObject(paisesList);

                //converte a string fonte codificada para UTF-8
                redisPaisesList = Encoding.UTF8.GetBytes(serializedPaisesList);
                               
                //Defini o prazo maximo de expiração do cache => SetAbsoluteExpiration . Pra nao ter o risco de ter dados desatualizados
                //Defini uma janela de expiração do cache de 2 minutos
                var options = new DistributedCacheEntryOptions()
                    .SetAbsoluteExpiration(DateTime.Now.AddMinutes(10)) //o tempo máximo que um item pode ser mantido no cache. Neste caso defini 10 minutos
                    .SetSlidingExpiration(TimeSpan.FromMinutes(2)); //indica o tempo que o cache pode ficar inativo, antes de ser removido

                //coloca os dados da tabela no cache
                await distributedCache.SetAsync(cacheKey, redisPaisesList, options);
            }
            return Ok(paisesList);
        }
    }
}
