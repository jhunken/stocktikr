
namespace stocktikr.API.DAL
{
    using System;
    using System.Collections.Generic;
    using System.Text;


    public partial class NYSE
    {

        public string Symbol { get; set; }
        public string Description { get; set; }

        public decimal LastPrice { get; set; }
        public decimal Price
        {
            get
            {
                //get random "price" based on ascii values of stock symbol
                byte[] asciiBytes = Encoding.ASCII.GetBytes(Symbol);
                decimal price = 0;
                for (int i = 0; i < asciiBytes.Length; i++)
                {
                    price += asciiBytes[i];
                }
                
                //we have base price. Let's add random variance
                Random random = new Random();
                decimal percentChange = (decimal)(random.NextDouble())/100;
                price = (price * percentChange) + price;
                price = Math.Round(price, 2);
                return price;
            }

        }

    }
}
