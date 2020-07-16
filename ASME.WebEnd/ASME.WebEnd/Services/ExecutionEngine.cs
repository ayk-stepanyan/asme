using System;
using System.Collections.Generic;
using System.Linq;
using ASME.WebEnd.Entities;

namespace ASME.WebEnd.Services
{
    public interface IExecutionEngine
    {
        OrderResult CreateOrder(string id, Order order);
        bool CancelOrder(string id);
    }

    public class ExecutionEngine : IExecutionEngine
    {
        private readonly object _syncRoot = new object();

        private readonly LinkedList<(string Id, Order Order)> _buyOrders = new LinkedList<(string Id, Order Order)>();
        private readonly LinkedList<(string Id, Order Order)> _sellOrders = new LinkedList<(string Id, Order Order)>();
        private readonly Dictionary<string, LinkedListNode<(string Id, Order Order)>> _orders = new Dictionary<string, LinkedListNode<(string Id, Order Order)>>();

        private readonly IOrderBook _orderBook;
        private readonly ITransactionLedger _transactionLedger;

        public ExecutionEngine(IOrderBook orderBook, ITransactionLedger transactionLedger)
        {
            _orderBook = orderBook;
            _transactionLedger = transactionLedger;
        }

        public OrderResult CreateOrder(string id, Order order)
        {
            lock (_syncRoot)
            {
                var result = new OrderResult();
                if (order.Side == Constants.OrderSideBuy)
                {
                    result.Transactions = TryFillBuyOrder(order).ToArray();
                    if (order.Quantity > 0 && order.OrderType == Constants.OrderTypeGtc)
                    {
                        result.OrderId = id;
                        _orderBook.OrderAdded(id, order);
                        PlaceBuyOrder(id, order);
                    }
                }

                if (order.Side == Constants.OrderSideSell)
                {
                    result.Transactions = TryFillSellOrder(order).ToArray();
                    if (order.Quantity > 0 && order.OrderType == Constants.OrderTypeGtc)
                    {
                        result.OrderId = id;
                        _orderBook.OrderAdded(id, order);
                        PlaceSellOrder(id, order);
                    }
                }

                return result;
            }
        }

        public bool CancelOrder(string id)
        {
            lock (_syncRoot)
            {
                if (!_orders.TryGetValue(id, out var orderNode)) return false;

                orderNode.List.Remove(orderNode);
                _orderBook.OrderRemoved(id);
                _orders.Remove(id);
                return true;
            }
        }

        private void PlaceBuyOrder(string id, Order order)
        {
            var currNode = _buyOrders.Last;
            while (currNode != null && order.Price < currNode.Value.Order.Price) currNode = currNode.Previous;
            var node = currNode == null ? _buyOrders.AddFirst((id, order)) : _buyOrders.AddAfter(currNode, (id, order));

            _orders.Add(id, node);
        }

        private IEnumerable<Transaction> TryFillSellOrder(Order order)
        {
            if (_buyOrders.Count == 0) yield break;
            if (_buyOrders.Last.Value.Order.Price < order.Price) yield break;

            var candidate = _buyOrders.First;
            while (candidate != null)
            {
                while (candidate != null && candidate.Value.Order.Price < order.Price) candidate = candidate.Next;
                if (candidate == null) yield break;

                var quantity = Math.Min(order.Quantity, candidate.Value.Order.Quantity);
                candidate.Value.Order.Quantity -= quantity;
                order.Quantity -= quantity;

                if (candidate.Value.Order.Quantity == 0)
                {
                    var candidateId = candidate.Value.Id;
                    candidate = candidate.Next;
                    CancelOrder(candidateId);
                }

                yield return CreateTransaction(order, quantity);
                if (order.Quantity == 0) yield break;
            }
        }

        private void PlaceSellOrder(string id, Order order)
        {
            var currNode = _sellOrders.Last;
            while (currNode != null && order.Price > currNode.Value.Order.Price) currNode = currNode.Previous;

            var node = currNode == null ? _sellOrders.AddFirst((id, order)) : _sellOrders.AddAfter(currNode, (id, order));

            _orders.Add(id, node);
        }

        private IEnumerable<Transaction> TryFillBuyOrder(Order order)
        {
            if (_sellOrders.Count == 0) yield break;
            if (_sellOrders.Last.Value.Order.Price > order.Price) yield break;

            var candidate = _sellOrders.First;
            while (candidate != null)
            {
                while (candidate != null && candidate.Value.Order.Price > order.Price) candidate = candidate.Next;
                if (candidate == null) yield break;

                var quantity = Math.Min(order.Quantity, candidate.Value.Order.Quantity);
                candidate.Value.Order.Quantity -= quantity;
                order.Quantity -= quantity;

                if (candidate.Value.Order.Quantity == 0)
                {
                    var candidateId = candidate.Value.Id;
                    candidate = candidate.Next;
                    CancelOrder(candidateId);
                }

                yield return CreateTransaction(order, quantity);
                if (order.Quantity == 0) yield break;
            }
        }

        private Transaction CreateTransaction(Order order, int quantity)
        {
            var result = new Transaction { Security = order.Security, Quantity = quantity, Price = order.Price, Timestamp = DateTimeOffset.UtcNow };
            _transactionLedger.OrderExecuted(result);

            return result;
        }
    }
}