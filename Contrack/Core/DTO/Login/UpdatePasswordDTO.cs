using Contrack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contrack
{
    public class UpdatePasswordDTO
    {
        public EncryptedData UserID { get; set; } = new EncryptedData();
        public string Password { get; set; } = "";
        public string Salt { get; set; } = "";
    }
}