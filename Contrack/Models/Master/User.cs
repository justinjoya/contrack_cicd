using Contrack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contrack
{
    public class User
    {
        public MasterMenus menus { get; set; } = new MasterMenus();
        public UserDTO user { get; set; } = new UserDTO();
        public Result result { get; set; }
    }
}