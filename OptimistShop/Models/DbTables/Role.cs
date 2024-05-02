using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimistShop.Models.DbTables
{
    public class Role
    {
        public int RoleID { get; set; }
        public string? RoleName { get; set; }

        //User Relationship
        public ICollection<User>? User { get; set; }
    }
}
