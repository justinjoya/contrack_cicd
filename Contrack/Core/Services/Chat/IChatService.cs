using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contrack
{
    public interface IChatService
    {
        List<ChatDTO> GetChatList(int targetid, string type);
        void SaveMessage(ChatDTO chat);
    }
}