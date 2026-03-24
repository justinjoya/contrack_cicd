using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Contrack
{
    public class VendorExtra
    {
        public string vendorshortcode { get; set; } = "";
        public string vendorcolor { get; set; } = "";
        public string vendorbgcolor { get; set; } = "";
    }

    public class VendorDTO
    {
        public EncryptedData vendorid { get; set; } = new EncryptedData();
        public VendorExtra extras { get; set; } = new VendorExtra();
        public int vendordetailid { get; set; } = 0;
        public string vendorcode { get; set; } = "";
        public string vendoruuid { get; set; } = "";
        public AgencyDTO agency { get; set; } = new AgencyDTO();
        public string TypeEncrypted { get; set; } = "";
        public int hubid { get; set; } = 0;
        public string legalname { get; set; } = "";
        public string contactemail { get; set; } = "";
        public string phone { get; set; } = "";
        public string accountsemail { get; set; } = "";
        public string emailtemp { get; set; } = "";
        public string email { get; set; } = "";
        public string accountsemailtemp { get; set; } = "";
        public string registeredaddress { get; set; } = "";
        public string billingaddress { get; set; } = "";
        public int paymentterms { get; set; } = 0;
        public string preferredcurrency { get; set; } = "";
        public string standinginstructions { get; set; } = "";
        public string picname { get; set; } = "";
        public string picshort { get; set; } = "";
        public string picemail { get; set; } = "";
        public string picdesignation { get; set; } = "";
        public DateTime createdat { get; set; } = new DateTime();
        public string ModifiedBy { get; set; } = "";
        public DateTime ModifiedAt { get; set; } = new DateTime();
        public List<KeyValuePair> CustomAttributes { get; set; } = new List<KeyValuePair>();
        public List<CustomBankInfo> BankInfo { get; set; } = new List<CustomBankInfo>();
        public List<VendorContact> Contacts { get; set; } = new List<VendorContact>();
        public List<DocumentDTO> Documents { get; set; } = new List<DocumentDTO>();
        public int row_index { get; set; } = 0;
        public int totalnoofrows { get; set; } = 0;
        public int Count { get; set; } = 0;
    }
}