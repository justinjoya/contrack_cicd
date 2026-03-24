using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contrack
{
    public class ShuntingServiceDTO
    {
        public EncryptedData serviceid { get; set; } = new EncryptedData();
        public EncryptedData type { get; set; } = new EncryptedData();
        public string servicename { get; set; }
        public int orderby { get; set; }
    }
}