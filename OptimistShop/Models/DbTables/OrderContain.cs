using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimistShop.Models.DbTables
{
    public class OrderContain
    {
        public int OrderContainID { get; set; }

        //Order Relationship
        public int OrderID { get; set; }
        public Order? Order { get; set; }

        //Food Relationship
        public int ClothesID { get; set; }
        public Clothes? Clothes { get; set; }

        public int Count { get; set; }
    }
}
