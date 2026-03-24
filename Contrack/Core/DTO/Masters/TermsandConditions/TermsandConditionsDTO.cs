using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Contrack
{
    public class TermsandConditionsDTO
    {

        public EncryptedData TermId { get; set; } = new EncryptedData();
        public string TypeEncrypted { get; set; }

        public string TermUuid { get; set; }
        public int? HubId { get; set; }

        public string Type { get; set; }

        public string TermsText { get; set; }
        public bool IsDeleted { get; set; }
        public int? DeletedBy { get; set; }
        public DateTime? DeletedAt { get; set; } = new DateTime();
        public long CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = new DateTime();
        public AgencyDTO agency { get; set; } = new AgencyDTO();
        public string UserName { get; set; }
        public string HubName { get; set; }
    }
}