using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimistShop.Models.DbTables
{
    public class Order
    {
        public int OrderID { get; set; }

        //User Relationship
        public int UserID { get; set; }
        public User? User { get; set; }

        public DateTime OrderDate { get; set; }
        public string OrderStatus { get; set; }
        public int OrderTotal { get; set; }
        public bool IsPaid { get; set; }
        public string PaymentMethod { get; set; }
        public string OrderDescription { get; set; }

        //OrderContain Relationship
        public ICollection<OrderContain>? OrderContain { get; set; }
    }
}
