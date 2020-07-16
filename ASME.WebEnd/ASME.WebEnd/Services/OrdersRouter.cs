using System;
using System.Collections.Generic;
using System.Linq;
using ASME.WebEnd.Entities;

namespace ASME.WebEnd.Services
{
    public interface IOrderRouter
    {
        OrderResult CreateOrder(Order order);
        bool CancelOrder(string orderId);
    }

    public class OrdersRouter : IOrderRouter
    {
        private readonly IIdGenerator _idGenerator;
        private readonly Dictionary<string, IExecutionEngine> _executionEngines;

        public OrdersRouter(IIdGenerator idGenerator, ISecuritiesRepository securitiesRepository, IServiceProvider serviceProvider)
        {
            _idGenerator = idGenerator;
            _executionEngines = securitiesRepository.GetSecurityCodes()
                .ToDictionary(s => _idGenerator.GetIdPrefix(s), s => (IExecutionEngine)serviceProvider.GetService(typeof(IExecutionEngine)));
        }

        public OrderResult CreateOrder(Order order)
        {
            return _executionEngines[_idGenerator.GetIdPrefix(order.Security)].CreateOrder(_idGenerator.GenerateId(order.Security), order);
        }

        public bool CancelOrder(string orderId)
        {
            return _executionEngines.TryGetValue(orderId[..1], out var ee) && ee.CancelOrder(orderId);
        }
    }
}