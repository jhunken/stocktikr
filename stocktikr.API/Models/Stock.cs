using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace stocktikr.API.Models
{
    public class Stock
    {
        string path = HttpContext.Current.Server.MapPath("~/resources/symbolNames.txt");
        public IEnumerable<Stock> GetStocks()
        {

            string[] allLines = File.ReadAllLines(path);
            IEnumerable<Stock> stocks = from s in allLines
                                        let data = s.Split('\t')
                                        select new Stock
                                        {
                                            Symbol = data[0],
                                            Description = data[1]
                                        };
            return stocks;


        }
        public decimal GetRandomPrice(Stock stock)
        {

            //get random "price" based on ascii values of stock symbol
            byte[] asciiBytes = Encoding.ASCII.GetBytes(stock.Symbol);
            decimal price = 0;
            for (int i = 0; i < asciiBytes.Length; i++)
            {
                price += asciiBytes[i];
            }

            //we have base price. Let's add random variance
            Random random = new Random();
            decimal percentChange = (decimal)(random.NextDouble()) / 100;
            price = (price * percentChange) + price;
            price = Math.Round(price, 2);
            return price;


        }
        public decimal Price { get; set; }
        public string Symbol { get; set; }
        public string Description { get; set; }

        public decimal LastPrice { get; set; }



    }
}