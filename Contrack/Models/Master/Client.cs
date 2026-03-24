using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Contrack
{
    public class Client
    {     
        public ClientDTO client { get; set; } = new ClientDTO();
        public MasterMenus menus { get; set; } = new MasterMenus();
        public Result result { get; set; }
    }
    public class ClientLog
    {
        public ClientDTO Info { get; set; } = new ClientDTO();
        public List<ClientDTO> Logs { get; set; } = new List<ClientDTO>();
    }
}