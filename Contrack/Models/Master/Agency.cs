using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contrack
{
    public class Agency
    {
        public MasterMenus menu { get; set; } = new MasterMenus();
        public AgencyDTO agency { get; set; } = new AgencyDTO();
        public Result result { get; set; }
        public string emailtemp { get; set; } = "";
        public string accountsemailtemp { get; set; } = "";
    }

    public class AgencyLog
    {
        public Agency Info { get; set; } = new Agency();
        public List<Agency> Logs { get; set; } = new List<Agency>();
    }
}