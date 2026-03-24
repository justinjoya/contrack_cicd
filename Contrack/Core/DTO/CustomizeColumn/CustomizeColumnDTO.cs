using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contrack
{
    public class CustomizeColumnDTO
    {
        public string MenuType { get; set; }
        public long MenuTypeId { get; set; }
        public long menuid { get; set; }
        public long UserMenuID { get; set; }
        public List<long> menuIds { get; set; } = new List<long>();
        public string column_name { get; set; }
        public bool is_default { get; set; }
        public bool MappedToUser { get; set; }
        public int display_order { get; set; }
        
    }
}