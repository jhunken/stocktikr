using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using System.Web;
using System.Text;

using System.IO;
using stocktikr.API.Models;

namespace stocktikr.API.Controllers
{
    public class SymbolController : ApiController
    {

        private Stock _stock = new Stock();
        // GET api/symbol
        public IEnumerable<Stock> Get()
        {


            return _stock.GetStocks();
            
        }

        // GET api/symbol?term=
        //used by jQueryUI Autocomplete as source for search term
        public IEnumerable<AutoCompleteModel> Get(string term)
        {

            var stocks = from s in _stock.GetStocks()
                         where s.Symbol.Contains(term) || s.Description.Contains(term)
                         select new AutoCompleteModel
                         {
                             label = s.Symbol + " - " + s.Description,
                             value = s.Symbol
                         };
            if (stocks != null)
            {
                return stocks;
            }
            return null;
        }

        public JsonResult GetBySymbol(string symbol)
        {
            //get stock from database, then add random price
            var stock = (from s in _stock.GetStocks()
                         where s.Symbol.ToLower() == symbol.ToLower()
                         select s).FirstOrDefault();
            if (stock != null)
            {
                stock.Price = stock.GetRandomPrice(stock);
                return new JsonResult { Data = new { success = true, stock = stock }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            else
            {
                return new JsonResult { Data = new { success = false }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }

        }

        
        public JsonResult PostBulkUpdate(List<Stock> stocks)
        {
            //receive list of symbols for bulk price update
            if (stocks.Count() > 0)
            {
                List<Stock> updatedStocks = new List<Stock>();
                foreach (var stock in stocks)
                {
                    var dbStock = (from s in _stock.GetStocks()
                                   where s.Symbol.ToLower() == stock.Symbol.ToLower()
                                 select s).FirstOrDefault();
                    if (dbStock != null)
                    {
                        //dbStock.LastPrice = stock.Price;
                        dbStock.Price = dbStock.GetRandomPrice(stock);
                        updatedStocks.Add(dbStock);
                    }

                }
                return new JsonResult { Data = new { success = true, stocks = updatedStocks }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            else
            {
                return new JsonResult { Data = new { success = false }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }
    }
}

public class AutoCompleteModel
{
    public string label { get; set; }
    public string value { get; set; }
}