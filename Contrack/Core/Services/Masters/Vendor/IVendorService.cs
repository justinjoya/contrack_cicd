using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contrack
{
    public interface IVendorService
    {
        void SaveVendor(VendorDTO vendor);
        void SaveCustomAttribute(List<KeyValuePair> attributes, int vendorid, string vendoruuid);
        void SaveBankInfo(List<CustomBankInfo> banks, int vendorid, string vendoruuid);
        void DeleteVendor(Vendor vendor);
        VendorDTO GetVendorByUUID(string uuid);
        VendorDTO GetVendorByID(int vendorId);
        VendorDTO GetVendorByDetailID(int detailId);
        List<VendorDTO> GetVendorLogsByUUID(string uuid);
        List<DocumentDTO> GetDocumentListUUID(string uuid);
        List<Vendor> GetVendorList(VendorFilter filter);
        void SaveContact(VendorContact contact);
        void MakePrimaryContact(string picidinc);
        VendorContact GetContactByID(string picid);
        List<VendorContact> GetContactList(string vendorid);
        void DeleteContact(string picidinc);
    }
}
