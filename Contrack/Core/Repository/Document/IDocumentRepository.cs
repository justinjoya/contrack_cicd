using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;

namespace Contrack
{
    public interface IDocumentRepository
    {
        Result UploadFile(DocumentDTO doc, HttpPostedFileBase file);
        Result SaveDocument(DocumentDTO document);
        List<DocumentDTO> GetDocumentListUUID(string uuid, int type);
        DocumentDTO GetDocumentByUUID(string docuuid);
        Result DeleteDocument(string docuuid);
    }
}
