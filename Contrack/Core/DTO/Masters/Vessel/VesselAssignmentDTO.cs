using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Contrack
{
    public class VesselAssignmentDTO
    {
        public EncryptedData vesselassignmentid { get; set; } = new EncryptedData();
        public string assignmentuuid { get; set; } = "";
        public EncryptedData agencyid { get; set; } = new EncryptedData();
        public string agencyname { get; set; } = "";
        public string agencyuuid { get; set; } = "";
    }
}