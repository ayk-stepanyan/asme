using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ASME.WebEnd.Entities;

namespace ASME.WebEnd.Services
{
    public interface IOrderValidator
    {
        IEnumerable<ValidationResult> Validate(Order order);
    }

    public class OrderValidator : IOrderValidator
    {
        private readonly HashSet<string> _validSecurityCodes;

        public OrderValidator(ISecuritiesRepository securitiesRepository)
        {
            _validSecurityCodes = new HashSet<string>(securitiesRepository.GetSecurityCodes());
        }

        public IEnumerable<ValidationResult> Validate(Order order)
        {
            if (!_validSecurityCodes.Contains(order.Security))
                yield return new ValidationResult($"Unexpected security code: {order.Security}");
            if (order.Side != Constants.OrderSideBuy && order.Side != Constants.OrderSideSell)
                yield return new ValidationResult($"Unexpected order side: {order.Side}");
            if (order.OrderType != Constants.OrderTypeIoc && order.OrderType != Constants.OrderTypeGtc)
                yield return new ValidationResult($"Unexpected order type: {order.OrderType}");
            if (order.Quantity <= 0)
                yield return new ValidationResult("Quantity should be more than zero");
            if (order.Price <= 0)
                yield return new ValidationResult("Price should be more than zero");
        }
    }
}