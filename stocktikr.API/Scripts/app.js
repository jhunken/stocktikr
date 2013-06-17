//load data from localStorage if available
var hasStorage = (function () {
    try {
        localStorage.setItem('test', 'test');
        localStorage.removeItem('test');
        return true;
    } catch (e) {
        return false;
    }
}());

var stockData = [];
if (hasStorage) {

    var lsData = localStorage.getItem('stockTikr');
    try {
        var temp = JSON.parse(lsData);
        if (temp != null) {
            delete temp.__ko_mapping__;
            stockData = ko.mapping.fromJS(temp);
            
        }

    } catch (e) {

    }

}



//knockout viewmodel
var stockViewModel = function () {
    var self = this;
    self.stockToAdd = ko.observable();
    self.allStocks = ko.observableArray(stockData());

    self.addStock = function (event, ui) {
        $(event.target).val("");

        var stockSymbol = ui.item.value;
        //get product object
        var url = '/api/Symbol/GetBySymbol';
        $.getJSON(url, { symbol: stockSymbol }, function (data) {
            if (data.Data.success == true) {
                self.stockToAdd = ko.mapping.fromJS(data.Data.stock);
                self.allStocks.push(self.stockToAdd);
            }
        });
        return false;
    };
    self.updateStockPrice = function (stock) {
        var match = ko.utils.arrayFirst(self.allStocks(), function (item) {
            return item.Symbol() === stock.Symbol;
        });
        if (!match)
            return 'error';
        else {
            match.LastPrice(match.Price());
            match.Price(stock.Price);
        }
    };
    self.removeStock = function (stock) { self.allStocks.remove(stock) };

    self.updateStocks = function () {
        if (self.allStocks().length > 0) {


            var url = '/api/Symbol/PostBulkUpdate';
            var stocks = ko.mapping.toJSON(self.allStocks());
            $.ajax({
                url: url,
                type: 'POST',
                data: stocks,
                contentType: 'application/json',
                processData: false,
                dataType: 'json',
                success: function (data) {
                    if (data.Data.success == true) {
                        var stocks = data.Data.stocks;
                        stocks.forEach(function (element, index, array) {
                            self.updateStockPrice(element);
                        });
                        localStorage.setItem('stockTikr', ko.toJSON(self.allStocks));

                    }
                }
            });


        }
    };

    setInterval(self.updateStocks, 3000);
};

ko.bindingHandlers.ko_autocomplete = {
    init: function (element, params) {
        $(element).autocomplete(params());
    },
    update: function (element, params) {
        $(element).autocomplete("option", "source", params().source);
    }


};

ko.applyBindings(new stockViewModel());

