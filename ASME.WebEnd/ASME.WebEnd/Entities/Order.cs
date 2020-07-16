namespace ASME.WebEnd.Entities
{
    public class Order
    {
        public string Security { get; set; }
        public string Side { get; set; }
        public string OrderType { get; set; }
        public int Quantity { get; set; }
        public int Price { get; set; }
    }
}