using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contrack
{
    public class BookingAllocationAcquireDTO
    {
        public EncryptedData ContainerID { get; set; } = new EncryptedData();
        public string AllocationBookingUUID { get; set; } = "";
    }
    public class AcquireDTO
    {
        public string containerid { get; set; } = "";
        public bool isdeleted { get; set; } = false;
    }
    
}