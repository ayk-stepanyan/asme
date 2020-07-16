using System;
using CommandLine;

namespace ASME.ConsoleClient
{
    public class Options
    {
        [Option('u', "url", HelpText = "URL to WebEnd")]
        public string Url { get; set; }
    }

    [Verb("RUN")]
    public class RunOptions : Options { }

    public class OrderDetailsOptions : Options
    {
        [Option('s', "security", Required = true, HelpText = "Security for the order. Valid values: AA01, AA02, AA03, AA04, AA05, AA06, AA07, AA08, AA09, AA10")]
        public string Security { get; set; }

        [Option('b', "side", Required = true, HelpText = "Side of the order. Valid values: BUY, SELL")]
        public string Side { get; set; }

        [Option('t', "type", Required = true, HelpText = "Order type. Valid values: IOC, GTC")]
        public string OrderType { get; set; }

        [Option('q', "quantity", Required = true, HelpText = "Order quantity")]
        public int Quantity { get; set; }

        [Option('p', "price", Required = true, HelpText = "Order price")]
        public int Price { get; set; }
    }

    [Verb("GET")]
    public class GetOrdersOptions : Options { }

    [Verb("PUT")]
    public class NewOrderOptions : OrderDetailsOptions { }

    [Verb("UPDATE")]
    public class AmendOrderOptions : OrderDetailsOptions
    {
        [Option('i', "id", Required = true, HelpText = "Id for the order to amend")]
        public string Id { get; set; }
    }

    [Verb("DELETE")]
    public class CancelOrderOptions : Options
    {
        [Option('i', "id", Required = true, HelpText = "Id for the order to cancel")]
        public string Id { get; set; }
    }

    [Verb("SELECT")]
    public class GetTransactionsOptions : Options
    {
        [Option('f', "from", Required = true, HelpText = "From time for the transaction search")]
        public DateTimeOffset From { get; set; }

        [Option('t', "till", Required = true, HelpText = "Till time for the transaction search")]
        public DateTimeOffset Till { get; set; }
    }
}