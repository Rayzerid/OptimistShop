using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace OptimistShop.Models.DbTables
{
    public class Clothes
    {
        public int ClothesID { get; set; }
        public string? ClothesName { get; set; }
        public int ClothesPrice { get; set; }
        public string? ClothesDescription { get; set; }
        public string? ClothesType { get; set; }
        public string? ClothesCategory { get; set; }
        public byte[]? ClothesImage { get; set; }
    }
}
