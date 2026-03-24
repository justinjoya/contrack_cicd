using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;
using Newtonsoft.Json;

namespace Contrack
{
    public class VendorRepository : CustomException, IVendorRepository
    {
        private readonly IDocumentRepository _docRepository;
        public VendorRepository(IDocumentRepository docRepository)
        {
            _docRepository = docRepository;
        }
        public Result SaveVendor(VendorDTO vendor)
        {
            Result result = new Result();
            try
            {
                using (SqlDB Db = new SqlDB())
                {
                    DataTable tbl = Db.GetDataTable("SELECT * FROM masters.savevendor(" +
                                    "p_vendor_code := ''," +
                                    "p_legal_name :='" + Common.Escape(vendor.legalname) + "'," +
                                    "p_registered_address := '" + Common.Escape(vendor.registeredaddress) + "'," +
                                    "p_billing_address := '" + Common.Escape(vendor.billingaddress) + "'," +
                                    "p_contact_email := '" + Common.Escape(vendor.contactemail) + "'," +
                                    "p_phone :='" + Common.Escape(vendor.phone) + "'," +
                                    "p_created_by := '" + Common.LoginID + "'," +
                                    "p_hub_id := '" + Common.HubID + "'," +
                                    "p_uuid:= " + Common.GetUUID(vendor.vendoruuid) + "," +
                                    "p_vendor_id :=" + Common.Decrypt(vendor.vendorid.EncryptedValue) + "," +
                                    "p_accounts_email := '" + vendor.accountsemail + "'," +
                                    "p_preferred_currency := '" + vendor.preferredcurrency + "'," +
                                    "p_standing_instructions := '" + Common.Escape(vendor.standinginstructions) + "'," +
                                    "p_payment_terms := '" + vendor.paymentterms + "'," +
                                    "p_agency_id := '" + Common.Decrypt(vendor.agency.agencyid.EncryptedValue) + "'" +
                                    ");");


                    if (tbl.Rows.Count != 0)
                    {
                        if (tbl.Rows[0][0].ToString() == "1")
                        {
                            if (vendor.vendorid.NumericValue == 0)
                            {
                                vendor.vendorid.NumericValue = Convert.ToInt32(tbl.Rows[0][2].ToString());
                                vendor = GetVendorByID(vendor.vendorid.NumericValue);
                                result = Common.SuccessMessage(vendor.vendoruuid);
                            }
                            else
                                result = Common.SuccessMessage(tbl.Rows[0][1].ToString());
                        }
                        else
                            result = Common.ErrorMessage(tbl.Rows[0][1].ToString());
                    }
                    else
                        result = Common.ErrorMessage("Cannot save Vendor");
                }
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.Message);
                RecordException(ex);
            }
            return result;
        }

        public Result SaveCustomAttribute(List<KeyValuePair> attributes, int vendorid, string vendorUUID)
        {
            Result result = new Result();
            try
            {
                using (SqlDB Db = new SqlDB())
                {
                    string customattribute = JsonConvert.SerializeObject(attributes);
                    DataTable tbl = Db.GetDataTable("SELECT * FROM masters.updatevendorcustomattributes(" +
                        "'" + Common.HubID + "'," +
                        "'" + vendorid + "'," +
                        "" + Common.GetUUID(vendorUUID) + "," +
                        "'" + Common.LoginID + "'," +
                        "'" + Common.Escape(customattribute) + "'" +
                        ");");

                    if (tbl.Rows.Count != 0)
                    {
                        result = tbl.Rows[0][0].ToString() == "1"
                            ? Common.SuccessMessage(tbl.Rows[0][1].ToString())
                            : Common.ErrorMessage(tbl.Rows[0][1].ToString());
                    }
                    else
                        result = Common.ErrorMessage("Cannot save Vendor Custom Attributes");
                }
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.Message);
                RecordException(ex);
            }
            return result;
        }


        public Result SaveBankInfo(List<CustomBankInfo> bankInfo, int vendorid, string vendoruuid)
        {
            Result result = new Result();
            try
            {
                using (SqlDB Db = new SqlDB())
                {
                    string bankinfo = JsonConvert.SerializeObject(bankInfo);

                    string query = "SELECT * FROM masters.updatevendorbankaccount(" +
                                   "'" + Common.HubID + "'," +
                                   "'" + vendorid + "'," +
                                   "" + Common.GetUUID(vendoruuid) + "," +
                                   "'" + Common.LoginID + "'," +
                                   "'" + Common.Escape(bankinfo) + "'::jsonb" +
                                   ");";

                    DataTable tbl = Db.GetDataTable(query);

                    if (tbl.Rows.Count > 0)
                    {
                        if (tbl.Rows[0][0].ToString() == "1")
                            result = Common.SuccessMessage(tbl.Rows[0][1].ToString());
                        else
                            result = Common.ErrorMessage(tbl.Rows[0][1].ToString());
                    }
                    else
                        result = Common.ErrorMessage("Cannot save bank info");
                }
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.Message);
                RecordException(ex);
            }
            return result;
        }
        public Result DeleteVendor(VendorDTO vendor)
        {
            Result result = new Result();
            try
            {
                using (SqlDB Db = new SqlDB())
                {
                    DataTable tbl = Db.GetDataTable("SELECT * FROM masters.deletevendor(" +
                        "'" + Common.Decrypt(vendor.vendorid.EncryptedValue) + "'," +
                        "" + Common.GetUUID(vendor.vendoruuid) + "," +
                        "'" + Common.LoginID + "'," +
                        "'" + Common.HubID + "');");

                    if (tbl.Rows.Count != 0)
                    {
                        result = tbl.Rows[0][0].ToString() == "1"
                            ? Common.SuccessMessage(tbl.Rows[0][1].ToString())
                            : Common.ErrorMessage(tbl.Rows[0][1].ToString());
                    }
                    else
                        result = Common.ErrorMessage("Cannot Delete Vendor");
                }
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.Message);
                RecordException(ex);
            }
            return result;
        }


        public VendorDTO GetVendorByUUID(string uuid)
        {
            VendorDTO vendor = null;
            try
            {
                vendor = ParseVendor(
                    "SELECT * FROM masters.getvendorbyuuid('" + uuid + "','" + Common.HubID + "');",
                    true
                );
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
                ParseVendor("SELECT * FROM  masters.getvendorbyid('" + vendorid + "','" + Common.HubID + "');");
            }
            catch (Exception ex) { RecordException(ex); }
            return vendor;
        }

        public VendorDTO GetVendorByDetailID(int detailid)
        {
            VendorDTO vendor = new VendorDTO();
            try
            {
                ParseVendor("SELECT * FROM  masters.GetVendorByDetailID('" + Common.HubID + "','" + detailid + "');");
            }
            catch (Exception ex) { RecordException(ex); }
            return vendor;
        }

        private VendorDTO ParseVendor(string qry, bool fetchdocuments = false)
        {
            VendorDTO vendor = new VendorDTO();

            using (SqlDB Db = new SqlDB())
            {
                DataTable tbl = Db.GetDataTable(qry);
                if (tbl.Rows.Count != 0)
                {
                    DataRow dr = tbl.Rows[0];

                    vendor.agency = new AgencyDTO()
                    {
                        agencyid = new EncryptedData()
                        {
                            NumericValue = Common.ToInt(dr["agencyid"]),
                            EncryptedValue = Common.Encrypt(Common.ToInt(dr["agencyid"]))
                        },
                        uuid = Common.ToString(dr["agencyuuid"]),
                        agencyname = Common.ToString(dr["agencyname"])
                    };

                    vendor.vendorid = new EncryptedData()
                    {
                        NumericValue = Common.ToInt(dr["vendorid"]),
                        EncryptedValue = Common.Encrypt(Common.ToInt(dr["vendorid"]))
                    };
                    vendor.vendorcode = Common.ToString(dr["vendorcode"]);
                    vendor.hubid = Common.ToInt(dr["hubid"]);
                    vendor.vendoruuid = Common.ToString(dr["vendoruuid"]);
                    vendor.legalname = Common.ToString(dr["legalname"]);
                    vendor.contactemail = Common.ToString(dr["contactemail"]);
                    vendor.phone = Common.ToString(dr["phone"]);
                    vendor.accountsemail = Common.ToString(dr["accountsemail"]);
                    vendor.registeredaddress = Common.ToString(dr["registeredaddress"]);
                    vendor.billingaddress = Common.ToString(dr["billingaddress"]);
                    vendor.paymentterms = Common.ToInt(dr["paymentterms"]);
                    vendor.preferredcurrency = Common.ToString(dr["preferredcurrency"]);
                    vendor.standinginstructions = Common.ToString(dr["standinginstructions"]).Trim();
                    try
                    {
                        vendor.CustomAttributes = JsonConvert.DeserializeObject<List<KeyValuePair>>(Common.ToString(dr["customattributes"])) ?? new List<KeyValuePair>();
                        vendor.BankInfo = JsonConvert.DeserializeObject<List<CustomBankInfo>>(Common.ToString(dr["bankaccounts"])) ?? new List<CustomBankInfo>();
                        vendor.BankInfo.ForEach(x => x.FillBankKeyValues());
                    }
                    catch (Exception ex)
                    {
                        vendor.CustomAttributes = new List<KeyValuePair>();
                        vendor.BankInfo = new List<CustomBankInfo>();
                        RecordException(ex);
                    }

                    vendor.agency.agencyname = vendor.agency.agencyid.NumericValue > 0
                        ? vendor.agency.agencyname
                        : Common.HubName;

                    if (fetchdocuments)
                        vendor.Documents = GetDocumentListUUID(vendor.vendoruuid);
                }
            }
            return vendor;
        }

        public List<VendorDTO> GetVendorLogsByUUID(string uuid)
        {
            List<VendorDTO> list = new List<VendorDTO>();
            try
            {
                if (uuid != "")
                {
                    using (SqlDB Db = new SqlDB())
                    {
                        DataTable tbl = Db.GetDataTable("SELECT * FROM  masters.getvendorlogsbyuuid('" + uuid + "','" + Common.HubID + "');");
                        list = (from DataRow dr in tbl.Rows
                                select new VendorDTO()
                                {
                                    agency = new AgencyDTO()
                                    {
                                        agencyid = new EncryptedData()
                                        {
                                            NumericValue = Common.ToInt(dr["agencyid"]),
                                            EncryptedValue = Common.Encrypt(Common.ToInt(dr["agencyid"]))
                                        },
                                        uuid = Common.ToString(dr["agencyuuid"]),
                                    },
                                    vendorid = new EncryptedData()
                                    {
                                        NumericValue = Common.ToInt(dr["vendorid"]),
                                        EncryptedValue = Common.Encrypt(Common.ToInt(dr["vendorid"]))
                                    },
                                    vendorcode = Common.ToString(dr["vendorcode"]),
                                    hubid = Common.ToInt(dr["hubid"]),
                                    vendoruuid = Common.ToString(dr["vendoruuid"]),
                                    legalname = Common.ToString(dr["legalname"]),
                                    contactemail = Common.ToString(dr["email"]),
                                    phone = Common.ToString(dr["phone"]),
                                    accountsemail = Common.ToString(dr["accountsemail"]),
                                    registeredaddress = Common.ToString(dr["address"]),
                                    billingaddress = Common.ToString(dr["billingaddress"]),
                                    paymentterms = Common.ToInt(dr["paymentterms"]),
                                    preferredcurrency = Common.ToString(dr["preferredcurrency"]),
                                    standinginstructions = Common.ToString(dr["standinginstructions"]).Trim(),
                                    ModifiedAt = Common.ToDateTime(dr["vendormodifiedat"]),
                                    createdat = Common.ToDateTime(dr["createdat"]),
                                    ModifiedBy = Common.ToString(dr["modifiedby"]),
                                    CustomAttributes = Common.ConvertJson<List<KeyValuePair>>(Common.ToString(dr["customattributes"])),
                                    BankInfo = Common.ConvertJson<List<CustomBankInfo>>(Common.ToString(dr["bankaccounts"])),
                                }).ToList();
                        try
                        {
                            list.ForEach(x => x.ModifiedAt = x.ModifiedAt.AddMinutes(Convert.ToDouble(SessionManager.TimeOffSet)));
                        }
                        catch (Exception ex)
                        { RecordException(ex); }
                    }
                }
            }
            catch (Exception ex)
            { RecordException(ex); }
            return list;
        }

        public List<DocumentDTO> GetDocumentListUUID(string uuid)
        {
            List<DocumentDTO> list = new List<DocumentDTO>();
            try
            {
                list = _docRepository.GetDocumentListUUID(uuid, AttachmentFileTypes.Vendor);
            }
            catch (Exception ex)
            { RecordException(ex); }
            return list;
        }

        public List<VendorDTO> GetVendorList(VendorFilter filter)
        {
            List<VendorDTO> list = new List<VendorDTO>();
            try
            {
                using (SqlDB Db = new SqlDB())
                {
                    string query = "SELECT * FROM masters.getvendorlist(" +
                        "p_hub_id := '" + Common.HubID + "'," +
                        "p_agency_id :='{" + Common.Decrypt(filter.AgencyID) + "}'," +
                        "p_search := '" + Common.Escape(filter.Search) + "'," +
                        "p_limit := '" + filter.limit + "'," +
                        "p_offset := '" + filter.offset + "'," +
                        "p_user_ids :='" + Common.LoginID + "'," +
                        "p_sort_column := '" + filter.sorting + "'," +
                        "p_sort_direction:= '" + filter.sortingorder + "'" +
                        ");";

                    DataTable tbl = Db.GetDataTable(query);

                    list = (from DataRow dr in tbl.Rows
                            select new VendorDTO()
                            {
                                row_index = Common.ToInt(dr["row_index"]),
                                vendorid = new EncryptedData()
                                {
                                    NumericValue = Common.ToInt(dr["vendorid"]),
                                    EncryptedValue = Common.Encrypt(Common.ToInt(dr["vendorid"]))
                                },
                                vendoruuid = Common.ToString(dr["vendoruuid"]),
                                legalname = Common.ToString(dr["legalname"]),
                                extras = new VendorExtra()
                                {
                                    vendorshortcode = Common.GetShortcode(Common.ToString(dr["legalname"])),

                                },
                                vendorcode = Common.ToString(dr["vendorcode"]),
                                contactemail = Common.ToString(dr["contactemail"]),
                                phone = Common.ToString(dr["phone"]),
                                accountsemail = Common.ToString(dr["accountsemail"]),
                                registeredaddress = Common.ToString(dr["registeredaddress"]),
                                billingaddress = Common.ToString(dr["billingaddress"]),
                                agency = new AgencyDTO()
                                {
                                    agencyid = new EncryptedData()
                                    {
                                        NumericValue = Common.ToInt(dr["agencyid"]),
                                        EncryptedValue = Common.Encrypt(Common.ToInt(dr["agencyid"]))
                                    },
                                    agencyname = Common.ToString(dr["agencyname"]),
                                },

                                picname = Common.ToString(dr["fullname"]),
                                picshort = Common.GetShortcode(Common.ToString(dr["fullname"])),
                                picdesignation = Common.ToString(dr["picposition"]),
                                paymentterms = Common.ToInt(dr["paymentterms"]),
                                preferredcurrency = Common.ToString(dr["preferredcurrency"]),
                                totalnoofrows = Common.ToInt(dr["total_count"]),
                                createdat = Common.ToDateTime(dr["createdat"]),
                                BankInfo = JsonConvert.DeserializeObject<List<CustomBankInfo>>(Common.ToString(dr["bankaccounts"])),
                            }).ToList();

                    try
                    {
                        list.ForEach(x =>
                        {
                            x.createdat = x.createdat.AddMinutes(Convert.ToDouble(SessionManager.TimeOffSet));
                            x.agency.agencyname = x.agency.agencyid.NumericValue > 0 ? x.agency.agencyname : Common.HubName;
                            var colors = Common.GetColorFromName(x.extras.vendorshortcode);
                            x.extras.vendorcolor = colors.Color;
                            x.extras.vendorbgcolor = colors.Bg;
                        });
                    }
                    catch (Exception) { }
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return list;
        }
        public Result SaveContact(VendorContact contact)
        {
            Result result = new Result();
            try
            {
                using (SqlDB Db = new SqlDB())
                {
                    string query = "SELECT * from masters.savevendorpic(" +
                                   "p_vendorid := '" + Common.Decrypt(contact.vendorid.EncryptedValue) + "'," +
                                   "p_fullname :='" + Common.Escape(contact.fullname) + "'," +
                                   "p_email := '" + Common.Escape(contact.email) + "'," +
                                   "p_phone := '" + Common.Escape(contact.phone) + "'," +
                                   "p_position := '" + Common.Escape(contact.position) + "'," +
                                   "p_isprimary :='" + contact.isprimary + "'," +
                                   "p_createdby := '" + Common.LoginID + "'," +
                                   "p_picid := '" + Common.Decrypt(contact.picid.EncryptedValue) + "'," +
                                   "p_hub_id := '" + Common.HubID + "'" +
                                   ");";

                    DataTable tbl = Db.GetDataTable(query);
                    if (tbl.Rows.Count != 0)
                    {
                        if (tbl.Rows[0][0].ToString() == "1")
                            result = Common.SuccessMessage(tbl.Rows[0][1].ToString());
                        else
                            result = Common.ErrorMessage(tbl.Rows[0][1].ToString());
                    }
                    else
                        result = Common.ErrorMessage("Cannot save Vendor");
                }
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.Message);
                RecordException(ex);
            }
            return result;
        }

        public Result MakePrimaryContact(string picidinc)
        {
            Result result = new Result();
            try
            {
                using (SqlDB Db = new SqlDB())
                {
                    string query = "SELECT * from masters.updatevendorprimarypic(" +
                                   "p_hub_id := '" + Common.HubID + "'," +
                                   "p_picid :='" + Common.Decrypt(picidinc) + "'" +
                                   ");";

                    DataTable tbl = Db.GetDataTable(query);

                    if (tbl.Rows.Count != 0)
                    {
                        if (tbl.Rows[0][0].ToString() == "1")
                            result = Common.SuccessMessage(tbl.Rows[0][1].ToString());
                        else
                            result = Common.ErrorMessage(tbl.Rows[0][1].ToString());
                    }
                    else
                        result = Common.ErrorMessage("Cannot update primary contact");
                }
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.Message);
                RecordException(ex);
            }
            return result;
        }
        public VendorContact GetContactByID(string picid)
        {
            VendorContact contact = new VendorContact();
            try
            {
                if (!string.IsNullOrEmpty(picid))
                {
                    using (SqlDB Db = new SqlDB())
                    {
                        DataTable tbl = Db.GetDataTable("SELECT * FROM  masters.getvendorpicbyid('" + Common.HubID + "','" + Common.Decrypt(picid) + "');");
                        if (tbl.Rows.Count != 0)
                        {
                            contact.picid = new EncryptedData()
                            {
                                EncryptedValue = Common.Encrypt(Common.ToInt(tbl.Rows[0]["picid"])),
                                NumericValue = Common.ToInt(tbl.Rows[0]["picid"])
                            };
                            contact.fullname = Common.ToString(tbl.Rows[0]["fullname"]);
                            contact.email = Common.ToString(tbl.Rows[0]["email"]);
                            contact.phone = Common.ToString(tbl.Rows[0]["phone"]);
                            contact.position = Common.ToString(tbl.Rows[0]["position"]);
                            contact.isprimary = Common.ToBool(tbl.Rows[0]["isprimary"]);
                            contact.vendorid = new EncryptedData()
                            {
                                EncryptedValue = Common.Encrypt(Common.ToInt(tbl.Rows[0]["vendorid"])),
                                NumericValue = Common.ToInt(tbl.Rows[0]["vendorid"])
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return contact;
        }

        public List<VendorContact> GetContactList(string vendorid)
        {
            List<VendorContact> list = new List<VendorContact>();
            try
            {
                using (SqlDB Db = new SqlDB())
                {
                    DataTable tbl = Db.GetDataTable("SELECT * FROM  masters.getvendorpiclist('" + Common.Decrypt(vendorid) + "','" + Common.HubID + "');");
                    list = (from DataRow dr in tbl.Rows
                            select new VendorContact()
                            {
                                picid = new EncryptedData()
                                {
                                    EncryptedValue = Common.Encrypt(Common.ToInt(dr["picid"])),
                                    NumericValue = Common.ToInt(dr["picid"])
                                },
                                fullname = Common.ToString(dr["fullname"]),
                                email = Common.ToString(dr["email"]),
                                phone = Common.ToString(dr["phone"]),
                                position = Common.ToString(dr["position"]),
                                isprimary = Common.ToBool(dr["isprimary"])
                            }).ToList();
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return list;
        }
        public Result DeleteContact(string picidinc)
        {
            Result result = new Result();
            try
            {
                using (SqlDB Db = new SqlDB())
                {
                    string query = "SELECT * from masters.vendorpicsoftdelete(" +
                                   "p_hub_id := '" + Common.HubID + "'," +
                                   "p_deleted_by := '" + Common.LoginID + "'," +
                                   "p_picid :='" + Common.Decrypt(picidinc) + "'" +
                                   ");";

                    DataTable tbl = Db.GetDataTable(query);

                    if (tbl.Rows.Count != 0)
                    {
                        if (tbl.Rows[0][0].ToString() == "1")
                            result = Common.SuccessMessage(tbl.Rows[0][1].ToString());
                        else
                            result = Common.ErrorMessage(tbl.Rows[0][1].ToString());
                    }
                    else
                        result = Common.ErrorMessage("Cannot delete Contact");
                }
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.Message);
                RecordException(ex);
            }
            return result;
        }

    }
}
