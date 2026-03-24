using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace Contrack
{
    public class Login
    {
        public LoginDTO login { get; set; } = new LoginDTO();
        public Result result { get; set; }
        public HubDTO HubInfo { get; set; } = new HubDTO();
    }
}