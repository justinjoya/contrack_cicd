using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contrack
{
    public class ChatService : CustomException, IChatService
    {
        public Result result = new Result();
        private readonly IChatRepository _repo;
        public ChatService(IChatRepository repo)
        {
            _repo = repo;
        }

        public List<ChatDTO> GetChatList(int targetid, string type)
        {
            return _repo.GetChatList(targetid, type);
        }
        public void SaveMessage(ChatDTO chat)
        {
            result = _repo.SaveMessage(chat);
        }
    }
}