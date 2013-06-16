using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using System.Web;
using System.Text;
using stocktikr.API.DAL;

namespace stocktikr.API.Controllers
{
    public class SymbolController : ApiController
    {
        private StockDataContext db = new StockDataContext();

        // GET api/symbol
        public IEnumerable<NYSE> Get()
        {

            var stocks = from s in db.NYSEs
                         select s;

            return stocks.ToList();


        }

        // GET api/symbol?term=
        //used by jQueryUI Autocomplete as source for search term
        public IEnumerable<AutoCompleteModel> Get(string term)
        {

            var stocks = from s in db.NYSEs
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
            var stock = (from s in db.NYSEs
                         where s.Symbol.ToLower() == symbol.ToLower()
                         select s).FirstOrDefault();
            if (stock != null)
            {

                return new JsonResult { Data = new { success = true, stock = stock }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            else
            {
                return new JsonResult { Data = new { success = false }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }

        }

        
        public JsonResult PostBulkUpdate(List<NYSE> stocks)
        {
            //receive list of symbols for bulk price update
            if (stocks.Count() > 0)
            {
                List<NYSE> updatedStocks = new List<NYSE>();
                foreach (var stock in stocks)
                {
                    var dbStock = (from s in db.NYSEs
                                   where s.Symbol.ToLower() == stock.Symbol.ToLower()
                                 select s).FirstOrDefault();
                    if (dbStock != null)
                    {
                        //dbStock.LastPrice = stock.Price;
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