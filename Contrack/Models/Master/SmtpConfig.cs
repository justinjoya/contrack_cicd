using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contrack
{
    public class SmtpConfig
    {
        public MasterMenus menus { get; set; } = new MasterMenus();
        public SmtpConfigDTO smtpconfig { get; set; } = new SmtpConfigDTO();
        public Result result { get; set; } = new Result();
    }
}