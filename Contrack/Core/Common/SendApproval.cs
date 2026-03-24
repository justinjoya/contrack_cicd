using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contrack
{
    public class SendApproval
    {
        public EncryptedData ID { get; set; } = new EncryptedData();
        public string UUID { get; set; } = "";
        public EncryptedData AssignedTo { get; set; } = new EncryptedData();
        public string Comments { get; set; } = "";
        public int ApproveStatus { get; set; } = 0; // - Send for approva, Approved, Rejected
        public int ApproveOrder { get; set; } = 1; //Approval Level
        public string ApprovalType { get; set; } = ""; //HOD Approval, Finance, etc
    }
}