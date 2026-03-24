using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contrack
{
    public interface ICustomizeColumnRepository
    {
        List<CustomizeColumnDTO> GetMenus(int menutype);

        Result Save(CustomizeColumnDTO customizecolumn);

    }

}