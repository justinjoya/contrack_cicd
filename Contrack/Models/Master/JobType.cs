using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contrack
{
    public class JobType
    {
        public MasterMenus menu { get; set; } = new MasterMenus();
        public JobTypeDTO jobtype { get; set; } = new JobTypeDTO();
        public Result result { get; set; }
    }

    // The 'JobTypeDetails' wrapper class has been completely removed.
}