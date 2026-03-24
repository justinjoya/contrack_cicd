using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contrack
{
    public interface ICommonRepository
    {
        List<IconDTO> GetIcons();
    }
}