using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Contrack
{
    public interface IVendorRepository
    {
        Result SaveVendor(VendorDTO vendor);
        Result SaveCustomAttribute(List<KeyValuePair> attributes, int vendorid, string vendoruuid);
        Result SaveBankInfo(List<CustomBankInfo> banks, int vendorid, string vendoruuid);

        VendorDTO GetVendorByUUID(string vendorUUID);
        VendorDTO GetVendorByID(int vendorId);
        VendorDTO GetVendorByDetailID(int detailid);
        List<VendorDTO> GetVendorLogsByUUID(string vendorUUID);
        List<VendorDTO> GetVendorList(VendorFilter filter);
        List<DocumentDTO> GetDocumentListUUID(string uuid);
        Result DeleteVendor(VendorDTO vendor);
        Result SaveContact(VendorContact contact);
        Result MakePrimaryContact(string picidinc);
        VendorContact GetContactByID(string picid);
        List<VendorContact> GetContactList(string vendorid);
        Result DeleteContact(string picidinc);

    }
}
