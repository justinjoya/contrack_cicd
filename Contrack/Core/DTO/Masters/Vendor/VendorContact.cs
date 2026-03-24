using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Contrack
{
    public class VendorContact
    {
        public EncryptedData picid { get; set; } = new EncryptedData();
        public EncryptedData vendorid { get; set; } = new EncryptedData();
        
        public string fullname { get; set; } = "";
        public string email { get; set; } = "";
        public string phone { get; set; } = "";
        public string position { get; set; } = "";
        public bool isprimary { get; set; } = false;
    }
}