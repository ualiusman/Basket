using Basket.API.Data;
using Basket.API.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Basket.API.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IBasketContext _context;

        public BasketRepository(IBasketContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<bool> DeleteBasket(string userName)
        {
            return await _context
                         .Redis
                         .KeyDeleteAsync(userName);
        }

        public async Task<BasketCart> GetBasket(string userName)
        {
            var basket = await _context
                                .Redis
                                .StringGetAsync(userName);
            if (basket.IsNullOrEmpty)
            {
                return null;
            }
            return JsonConvert.DeserializeObject<BasketCart>(basket);
        }

        public async Task<BasketCart> UpdateBasket(BasketCart basketCart)
        {
            var created = await _context
                                 .Redis
                                 .StringSetAsync(basketCart.UserName, JsonConvert.SerializeObject(basketCart));

            if (!created)
            {
                return null;
            }

            return await GetBasket(basketCart.UserName);
        }
    }
}
