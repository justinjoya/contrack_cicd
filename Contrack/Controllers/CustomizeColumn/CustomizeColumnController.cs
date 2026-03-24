using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Contrack
{
    public class CustomizeColumnController : Controller
    {
        private readonly CustomizeColumnService _customizecolumnService;
        public CustomizeColumnController()
        {
            ICustomizeColumnRepository repo = new CustomizeColumRepository();
            _customizecolumnService = new CustomizeColumnService(repo);
        }

        public JsonResult SaveMenus(CustomizeColumnDTO model)
        {
            _customizecolumnService.Save(model);
            return Json(_customizecolumnService.result, JsonRequestBehavior.AllowGet);
        }

    }
}