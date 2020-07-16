using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using ASME.WebEnd.Entities;

namespace ASME.WebEnd.Services
{
    public interface IOrderBook
    {
        void OrderAdded(string id, Order order);
        void OrderRemoved(string id);

        Order[] GetActiveOrders();
    }

    public class OrderBook : IOrderBook
    {
        private readonly BlockingCollection<Action> _processingQueue = new BlockingCollection<Action>();
        private readonly ConcurrentDictionary<string, Order> _orders = new ConcurrentDictionary<string, Order>();

        public OrderBook()
        {
            Task.Run(() =>
            {
                foreach (var editAction in _processingQueue.GetConsumingEnumerable())
                {
                    editAction();
                }
            });
        }

        public void OrderAdded(string id, Order order)
        {
            _processingQueue.Add(() => _orders.TryAdd(id, order));
        }

        public void OrderRemoved(string id)
        {
            _processingQueue.Add(() => _orders.TryRemove(id, out _));
        }

        public Order[] GetActiveOrders()
        {
            return _orders.ToArray().Select(o => o.Value).ToArray();
        }
    }
}