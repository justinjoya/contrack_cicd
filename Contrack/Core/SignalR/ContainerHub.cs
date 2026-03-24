using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Contrack
{
    public class ContainerHub : Hub
    {
        public Task JoinGroup(string locationUuid, string modelUuid)
        {
            var groupName = $"LOC_{locationUuid}_MOD_{modelUuid}";
            return Groups.Add(Context.ConnectionId, groupName);
        }

        public Task LeaveGroup(string locationUuid, string modelUuid)
        {
            var groupName = $"LOC_{locationUuid}_MOD_{modelUuid}";
            return Groups.Remove(Context.ConnectionId, groupName);
        }
    }
}