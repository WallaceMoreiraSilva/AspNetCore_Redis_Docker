using System;

namespace AspNetCore_Redis_Docker.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public char CustomerId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime? RequiredDate { get; set; }
        public DateTime? ShippedDate { get; set; }
        public int ShipVia { get; set; }
        public decimal Freight { get; set; }
        public string ShipName { get; set; }
        public string ShipAddress { get; set; }
        public string ShipCity { get; set; }
        public string ShipRegion { get; set; }
    }
}
