using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Contrack
{
    public class ContainerAdditionalServicesDTO
    {
        public EncryptedData additionalserviceid { get; set; } = new EncryptedData();
        public string additionalserviceuuid { get; set; } = "";
        public string servicename { get; set; } = "";
        public string description { get; set; } = "";
        public int orderby { get; set; } = 0;
    }
}