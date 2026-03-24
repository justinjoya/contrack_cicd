using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contrack
{
    public interface ICustomizeColumnService
    {
        List<CustomizeColumn> GetMenus(int menutype);

        void Save(CustomizeColumnDTO customizecolumn);
    }
}