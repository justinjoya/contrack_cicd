using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Contrack
{
    public class DocumentDTO
    {
        public int documentid { get; set; } = 0;
        public string documentuuid { get; set; } = "";
        public int documenttypeid { get; set; } = 0;
        public string documenttype { get; set; } = "";
        public string parentuuid { get; set; } = "";
        public string targetuuid { get; set; } = "";
        public int targetid { get; set; } = 0;
        public string filename { get; set; } = "";
        public string filepath { get; set; } = "";
        public string fileextension { get; set; } = "";
        public Int64 filesize { get; set; } = 0;
        public DateTime createdat { get; set; } = new DateTime();
        public string createdbyname { get; set; } = "";
        public DocumentDTO()
        { }
        public DocumentDTO(int p_documenttypeid, string p_parentuuid, string p_targetuuid, int p_targetid)
        {
            documenttypeid = p_documenttypeid;
            parentuuid = p_parentuuid;
            targetuuid = p_targetuuid;
            targetid = p_targetid;
        }
    }
}