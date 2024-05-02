using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimistShop.Models.DbTables
{
    public class ClothesContain
    {
        public int ClothesContainID { get; set; }

        //Cart Relationship
        public int CartID { get; set; }
        public Cart? Cart { get; set; }

        //Food Relationship
        public int ClothesID { get; set; }
        public Clothes? Clothes { get; set; }

        public int Count { get; set; }

    }
}
