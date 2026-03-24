using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contrack
{
    public class Document
    {
        public int documenttypeid { get; set; } = 0;
        public string parentuuid { get; set; } = "";
        public string targetuuid { get; set; } = "";
        public int targetid { get; set; } = 0;
        public List<DocumentDTO> Documents = new List<DocumentDTO>();
        public Document()
        { }
        public Document(int p_documenttypeid, string p_parentuuid, string p_targetuuid, int p_targetid)
        {
            documenttypeid = p_documenttypeid;
            parentuuid = p_parentuuid;
            targetuuid = p_targetuuid;
            targetid = p_targetid;
        }
        public void GetDocuments()
        {
            //Documents = new DocumentDTO().GetDocumentListUUID(targetuuid, documenttypeid);
        }
    }
}