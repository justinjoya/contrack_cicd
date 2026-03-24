using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contrack
{
    public class ChatDTO
    {
        public string message_text { get; set; } = "";
        public string attachment_path { get; set; } = "";
        public FormattedValue<DateTime> created_at { get; set; } = new FormattedValue<DateTime>();
        public int created_by { get; set; } = 0;
        public string type { get; set; } = "";
        public string target_id { get; set; } = "";
        public string created_by_name { get; set; } = "";
        public string approver { get; set; } = "";
        public bool autoapprove { get; set; } = false;
    }
}