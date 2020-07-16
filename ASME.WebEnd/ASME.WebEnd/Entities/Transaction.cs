using System;

namespace ASME.WebEnd.Entities
{
    public class Transaction
    {
        public string Security { get; set; }
        public int Quantity { get; set; }
        public int Price { get; set; }
        public DateTimeOffset Timestamp { get; set; }
    }
}