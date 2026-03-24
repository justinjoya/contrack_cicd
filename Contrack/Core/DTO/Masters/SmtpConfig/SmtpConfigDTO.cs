using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contrack
{
    public class SmtpConfigDTO
    {
        public EncryptedData smtp_id { get; set; } = new EncryptedData();
        public string config_name { get; set; } = "";
        public string smtp_host { get; set; } = "";
        public int smtp_port { get; set; } = 587;
        public string smtp_username { get; set; } = "";
        public string smtp_password { get; set; } = "";
        public string encryption_type { get; set; } = "";
        public string from_email { get; set; } = "";
        public string reply_email { get; set; } = "";
        public bool is_deleted { get; set; } = false;
        public bool is_default { get; set; } = false;
        public long created_by { get; set; } = 0;
        public string CreatedFor { get; set; }
        public DateTime created_at { get; set; } = new DateTime();
        public string TypeEncrypted { get; set; } = "";
        public string HubName { get; set; }
        public AgencyDTO agency { get; set; } = new AgencyDTO();
        public string created_username { get; set; } = "";
        public string deleted_username { get; set; } = "";
    }
}