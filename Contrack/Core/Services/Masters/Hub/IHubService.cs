using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contrack
{
    public interface IHubService
    {
        HubDTO GetHubByID(int HubID);
    }
}