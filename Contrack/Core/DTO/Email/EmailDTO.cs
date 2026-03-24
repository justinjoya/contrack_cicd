using System.Collections.Generic;
using System.Web.Mvc;

namespace Contrack
{
    public class EmailDTO
    {
        [AllowHtml]
        public List<string> EmailTo { get; set; } = new List<string>();
        [AllowHtml]
        public List<string> CC { get; set; } = new List<string>();
        [AllowHtml]
        public List<string> BCC { get; set; } = new List<string>();
        public string Subject { get; set; } = "";
        [AllowHtml]
        public string Body { get; set; } = "";
        public List<AttachmentDTO> Attachments { get; set; } = new List<AttachmentDTO>();
        public string EmailType { get; set; } = "";
        public string ID { get; set; } = ""; // TargetUUID
        public bool IsHtml { get; set; } = true;
        public string RedirectUrl { get; set; } = "";
        public Result result { get; set; } = new Result();
        public int AgencyID { get; set; } = 0;
        public int PIID { get; set; } = 0;
        public string LogUUID { get; set; } = "";
    }

    public class AttachmentDTO
    {
        public string AttachmentPath { get; set; } = "";
        public string Name { get; set; } = "";
        public string PhysicalPath { get; set; } = "";
    }
}