using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;

namespace Contrack
{
    public interface IChatRepository
    {
        List<ChatDTO> GetChatList(int targetid, string type);
        Result SaveMessage(ChatDTO chat);
    }
}
