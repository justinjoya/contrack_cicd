using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;

namespace Contrack
{
    public class VendorService : CustomException, IVendorService
    {
        public Result result = new Result();
        private readonly IVendorRepository _repo;
        public VendorService(IVendorRepository repo)
        {
            _repo = repo;
        }

        public void SaveVendor(VendorDTO vendor)
        {

            try
            {
                if (!string.IsNullOrEmpty(vendor.emailtemp))
                {
                    if (!string.IsNullOrEmpty(vendor.contactemail))
                        vendor.contactemail = vendor.contactemail + ";" + vendor.emailtemp;
                    else
                        vendor.contactemail = vendor.emailtemp;
                }

                if (!string.IsNullOrEmpty(vendor.accountsemailtemp))
                {
                    if (!string.IsNullOrEmpty(vendor.accountsemail))
                        vendor.accountsemail = vendor.accountsemail + ";" + vendor.accountsemailtemp;
                    else
                        vendor.accountsemail = vendor.accountsemailtemp;
                }

                if (Common.Decrypt(vendor.TypeEncrypted) == 1)
                    vendor.agency.agencyid.EncryptedValue = Common.Encrypt(0);

                result = _repo.SaveVendor(vendor);
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.ToString());
                RecordException(ex);
            }
        }

        public void SaveCustomAttribute(List<KeyValuePair> attributes, int vendorid, string vendoruuid)
        {
            try
            {
                result = _repo.SaveCustomAttribute(attributes, vendorid, vendoruuid);
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.ToString());
                RecordException(ex);
            }
        }
        public void SaveBankInfo(List<CustomBankInfo> bankInfo, int vendorid, string vendoruuid)
        {
            try
            {
                result = _repo.SaveBankInfo(bankInfo, vendorid, vendoruuid);
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.ToString());
                RecordException(ex);
            }
        }
        public void DeleteVendor(Vendor vendor)
        {
            try
            {
                result = _repo.DeleteVendor(vendor.vendor);
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.ToString());
                RecordException(ex);
            }
        }

        public VendorDTO GetVendorByUUID(string uuid)
        {
            VendorDTO vendor = new VendorDTO();
            try
            {
                vendor = _repo.GetVendorByUUID(uuid);
                SessionManager.CurrentVendor = vendor;
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return vendor;
        }

        public VendorDTO GetVendorByID(int vendorid)
        {
            VendorDTO vendor = new VendorDTO();
            try
            {
                vendor = _repo.GetVendorByID(vendorid);
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return vendor;
        }

        public VendorDTO GetVendorByDetailID(int detailId)
        {
            VendorDTO vendor = new VendorDTO();
            try
            {
                vendor = _repo.GetVendorByDetailID(detailId);
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return vendor;
        }

        public List<VendorDTO> GetVendorLogsByUUID(string uuid)
        {
            List<VendorDTO> list = new List<VendorDTO>();
            try
            {
                list = _repo.GetVendorLogsByUUID(uuid);
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return list;
        }

        public List<DocumentDTO> GetDocumentListUUID(string uuid)
        {
            List<DocumentDTO> list = new List<DocumentDTO>();
            try
            {
                list = _repo.GetDocumentListUUID(uuid);
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return list;
        }

        public List<Vendor> GetVendorList(VendorFilter filter)
        {
            List<VendorDTO> list = new List<VendorDTO>();
            try
            {
                list = _repo.GetVendorList(filter);
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }

            return list.Select(x => new Vendor()
            {
                vendor = x,
                menus = new MasterMenus()
                {
                    edit = true
                }
            }).ToList();
        }
        public void SaveContact(VendorContact contact)
        {
            try
            {
                if (Common.Decrypt(contact.vendorid.EncryptedValue) == 0)
                {
                    result = Common.ErrorMessage("Invalid Vendor");
                    return;
                }
                result = _repo.SaveContact(contact);
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.ToString());
                RecordException(ex);
            }
        }

        public void MakePrimaryContact(string picidinc)
        {
            try
            {
                result = _repo.MakePrimaryContact(picidinc);
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.ToString());
                RecordException(ex);
            }
        }

        public VendorContact GetContactByID(string picid)
        {
            try
            {
                return _repo.GetContactByID(picid);
            }
            catch (Exception ex)
            {
                RecordException(ex);
                return new VendorContact();
            }
        }

        public List<VendorContact> GetContactList(string vendorid)
        {
            try
            {
                return _repo.GetContactList(vendorid);
            }
            catch (Exception ex)
            {
                RecordException(ex);
                return new List<VendorContact>();
            }
        }

        public void DeleteContact(string picidinc)
        {
            try
            {
                result = _repo.DeleteContact(picidinc);
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.ToString());
                RecordException(ex);
            }
        }
    }
}
