using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contrack
{
    public class JobTypeDTO
    {
        public EncryptedData jobtypeid { get; set; } = new EncryptedData();
        public string jobtypename { get; set; } = "";
        public string jobshortcode { get; set; } = "";
        public bool useasmaster { get; set; } = false;
        public int lineitemcount { get; set; }
        public List<JobTypeDetailDTO> JobTypeDetails { get; set; } = new List<JobTypeDetailDTO>();
    }

    public class JobTypeDetailDTO
    {
        public EncryptedData jobtypedetailid { get; set; } = new EncryptedData();
        public string jobtypedetailuuid { get; set; } = "";
        public string description { get; set; } = "";
        public bool isdeleted { get; set; } = false;
        public string jobtypename { get; set; } = "";
        public long totalcount { get; set; } = 0;
    }
}