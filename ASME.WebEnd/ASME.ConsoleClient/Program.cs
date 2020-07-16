using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ASME.WebClient;
using CommandLine;

namespace ASME.ConsoleClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<RunOptions, GetOrdersOptions, NewOrderOptions, AmendOrderOptions, CancelOrderOptions, GetTransactionsOptions>(args)
                .WithParsed<RunOptions>(o => Run(o))
                .WithParsed<GetOrdersOptions>(o => Execute(o).Wait())
                .WithParsed<NewOrderOptions>(o => Execute(o).Wait())
                .WithParsed<AmendOrderOptions>(o => Execute(o).Wait())
                .WithParsed<CancelOrderOptions>(o => Execute(o).Wait())
                .WithParsed<GetTransactionsOptions>(o => Execute(o).Wait());
        }

        private static void Run(RunOptions options)
        {
            var client = new Client(options.Url, new HttpClient());
            while (true)
            {
                Console.WriteLine();
                Console.Write("> ");

                var command = Console.ReadLine();
                if (command == "EXIT") break;

                Parser.Default
                    .ParseArguments<GetOrdersOptions, NewOrderOptions, AmendOrderOptions, CancelOrderOptions, GetTransactionsOptions>(command.Split(' '))
                    .WithParsed<GetOrdersOptions>(o => Execute(o, client).Wait())
                    .WithParsed<NewOrderOptions>(o => Execute(o, client).Wait())
                    .WithParsed<AmendOrderOptions>(o => Execute(o, client).Wait())
                    .WithParsed<CancelOrderOptions>(o => Execute(o, client).Wait())
                    .WithParsed<GetTransactionsOptions>(o => Execute(o, client).Wait());
            }
        }

        private static async Task Execute(GetOrdersOptions options, Client client = null)
        {
            client ??= new Client(options.Url, new HttpClient());

            try
            {
                var orders = await client.GetOrdersAsync();
                Console.WriteLine($"Found {orders.Count} orders:");
                foreach (var order in orders) Console.WriteLine($"{order.Security} - {order.Side} {order.Quantity} units @ {order.Price}");
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
            }
        }

        private static async Task Execute(NewOrderOptions options, Client client = null)
        {
            client ??= new Client(options.Url, new HttpClient());

            try
            {
                var orderResult = await client.NewOrderAsync(Create(options));
                Console.WriteLine("Order execution results:");
                Console.WriteLine($"\tTransactions: {(orderResult.Transactions?.Count == 0 ? "none" : "")}");
                foreach (var transaction in orderResult.Transactions ?? new Transaction[0])
                {
                    Console.WriteLine($"\t\t{transaction.Timestamp}: {transaction.Security} - {transaction.Quantity} units @ {transaction.Price}");
                }

                Console.WriteLine($"\tOrderId = {(string.IsNullOrEmpty(orderResult.OrderId) ? "none" : orderResult.OrderId)}");
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
            }
        }

        private static async Task Execute(AmendOrderOptions options, Client client = null)
        {
            client ??= new Client(options.Url, new HttpClient());

            try
            {
                var orderResult = await client.AmendOrderAsync(options.Id, Create(options));
                Console.WriteLine("Order amendment results:");
                Console.WriteLine($"\tTransactions: {(orderResult.Transactions?.Count == 0 ? "none" : "")}");
                foreach (var transaction in orderResult.Transactions ?? new Transaction[0])
                {
                    Console.WriteLine($"\t\t{transaction.Timestamp}: {transaction.Security} - {transaction.Quantity} units @ {transaction.Price}");
                }

                Console.WriteLine($"\tOrderId = {(string.IsNullOrEmpty(orderResult.OrderId) ? "none" : orderResult.OrderId)}");
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
            }
        }

        private static async Task Execute(CancelOrderOptions options, Client client = null)
        {
            client ??= new Client(options.Url, new HttpClient());

            try
            {
                var orderResult = await client.CancelOrderAsync(options.Id);
                Console.WriteLine($"Order cancelled: {orderResult}");
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
            }
        }

        private static async Task Execute(GetTransactionsOptions options, Client client = null)
        {
            client ??= new Client(options.Url, new HttpClient());

            try
            {
                var transactions = await client.FindTransactionsAsync(options.From, options.Till) ?? new Transaction[0];
                Console.WriteLine($"Found {transactions.Count} transactions");
                foreach (var transaction in transactions)
                {
                    Console.WriteLine($"\t{transaction.Timestamp}: {transaction.Security} - {transaction.Quantity} units @ {transaction.Price}");
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
            }
        }

        private static Order Create(OrderDetailsOptions options)
        {
            return new Order
            {
                Security = options.Security,
                Side = options.Side,
                OrderType = options.OrderType,
                Quantity = options.Quantity,
                Price = options.Price
            };
        }
    }
}