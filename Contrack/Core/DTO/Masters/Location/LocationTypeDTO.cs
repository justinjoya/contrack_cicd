using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contrack
{
    public class LocationTypeDTO
    {
        public EncryptedData LocationTypeID { get; set; } = new EncryptedData();
        public string LocationTypeName { get; set; } = "";
        public Icon Icon { get; set; } = new Icon(); 
    }
}