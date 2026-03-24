using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
using Contrack;

namespace Contrack
{
    public class AgencyRepository : CustomException, IAgencyRepository
    {
        public Result SaveAgency(AgencyDTO agency)
        {
            Result result = new Result();
            try
            {
                using (SqlDB Db = new SqlDB())
                {
                    DataTable tbl = Db.GetDataTable("SELECT * from masters.saveagency(" +
                        "'" + Common.Escape(agency.agencyname) + "'," +
                        "'" + Common.Escape(agency.agencyshortname) + "'," +
                        "'" + Common.Escape(agency.imono) + "'," +
                        "'" + Common.Escape(agency.address) + "'," +
                        "'" + Common.Escape(agency.billingaddress) + "'," +
                        "'" + Common.Escape(agency.email) + "'," +
                        "'" + Common.Escape(agency.phone) + "'," +
                        "'" + Common.LoginID + "'," +
                        "'" + Common.HubID + "'," +
                        "" + Common.GetUUID(agency.uuid) + "," +
                        //(string.IsNullOrEmpty(uuid) ? "null," : ("'" + uuid + "',")) +
                        "'" + Common.Decrypt(agency.agencyid.EncryptedValue) + "'," +
                        "'" + agency.accountsemail + "'," +
                        "'{" + string.Join(",", agency.countryList.Select(x => Common.Decrypt(x)).ToArray()) + "}'," +
                        "'{" + string.Join(",", agency.portList.Select(x => Common.Decrypt(x)).ToArray()) + "}'," +
                        "'" + agency.websites + "'" +
                        ");");
                    if (tbl.Rows.Count != 0)
                    {
                        if (tbl.Rows[0][0].ToString() == "1")
                        {
                            result = Common.SuccessMessage(tbl.Rows[0][1].ToString());
                            result.TargetID = Convert.ToInt32(tbl.Rows[0][2].ToString());
                        }
                        else
                            result = Common.ErrorMessage(tbl.Rows[0][1].ToString());
                    }
                    else
                        result = Common.ErrorMessage("Cannot save Agency");
                }
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.Message);
                RecordException(ex);
            }
            return result;
        }

        public Result SaveCustomAttribute(List<KeyValuePair> attr, int agencyid, string uuid)
        {
            Result result = new Result();
            try
            {
                using (SqlDB Db = new SqlDB())
                {
                    string customattribute = JsonConvert.SerializeObject(attr);

                    DataTable tbl = Db.GetDataTable("SELECT * from masters.updatecustomattributes(" +
                        "'" + Common.HubID + "'," +
                        "'" + agencyid + "'," +
                        "" + Common.GetUUID(uuid) + "," +
                        "'" + Common.LoginID + "'," +
                        "'" + Common.Escape(customattribute) + "'" +
                        ");");
                    if (tbl.Rows.Count != 0)
                    {
                        if (tbl.Rows[0][0].ToString() == "1")
                        {
                            result = Common.SuccessMessage(tbl.Rows[0][1].ToString());
                        }
                        else
                            result = Common.ErrorMessage(tbl.Rows[0][1].ToString());
                    }
                    else
                        result = Common.ErrorMessage("Cannot save Agency");
                }
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.Message);
                RecordException(ex);
            }
            return result;
        }

        public Result SaveBankInfo(List<CustomBankInfo> BankInfo, int agencyid, string uuid)
        {
            Result result = new Result();
            try
            {
                using (SqlDB Db = new SqlDB())
                {
                    //BankInfo.ForEach(x => x.BankAttributes.ForEach(y => y.UUID = ""));
                    //BankInfo.ForEach(x => x.BankAttributes.RemoveAll(y => string.IsNullOrEmpty(y.KeyName)));
                    string bankinfo = JsonConvert.SerializeObject(BankInfo);

                    DataTable tbl = Db.GetDataTable("SELECT * from masters.updatebankaccount(" +
                        "'" + Common.HubID + "'," +
                        "'" + agencyid + "'," +
                        "" + Common.GetUUID(uuid) + "," +
                        "'" + Common.LoginID + "'," +
                        "'" + Common.Escape(bankinfo) + "'" +
                        ");");
                    if (tbl.Rows.Count != 0)
                    {
                        if (tbl.Rows[0][0].ToString() == "1")
                        {
                            result = Common.SuccessMessage(tbl.Rows[0][1].ToString());
                        }
                        else
                            result = Common.ErrorMessage(tbl.Rows[0][1].ToString());
                    }
                    else
                        result = Common.ErrorMessage("Cannot save Agency");
                }
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.Message);
                RecordException(ex);
            }
            return result;
        }

        public Result DeleteAgency(int agencyid, string uuid)
        {
            Result result = new Result();
            try
            {
                using (SqlDB Db = new SqlDB())
                {

                    DataTable tbl = Db.GetDataTable("SELECT * from masters.DeleteAgency(" +
                        "'" + agencyid + "'," +
                        "" + Common.GetUUID(uuid) + "," +
                       "'" + Common.LoginID + "'," +
                       "'" + Common.HubID + "');");
                    if (tbl.Rows.Count != 0)
                    {
                        if (tbl.Rows[0][0].ToString() == "1")
                        {
                            result = Common.SuccessMessage(tbl.Rows[0][1].ToString());
                        }
                        else
                            result = Common.ErrorMessage(tbl.Rows[0][1].ToString());
                    }
                    else
                        result = Common.ErrorMessage("Cannot Delete Agency");
                }
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.Message);
                RecordException(ex);
            }
            return result;
        }

        public AgencyDTO GetAgencyByID(int ID)
        {
            AgencyDTO agency = new AgencyDTO();
            try
            {
                if (ID != 0)
                {
                    using (SqlDB Db = new SqlDB())
                    {
                        DataTable tbl = Db.GetDataTable("SELECT * FROM  masters.getagencybyid('" + ID + "','" + Common.HubID + "');");
                        if (tbl.Rows.Count != 0)
                        {
                            agency.agencyid = new EncryptedData()
                            {
                                EncryptedValue = Common.Encrypt(Common.ToInt(tbl.Rows[0]["agencyid"])),
                                NumericValue = Common.ToInt(tbl.Rows[0]["agencyid"])
                            };
                            agency.agencydetailid = Common.ToInt(tbl.Rows[0]["agencydetailid"]);
                            agency.uuid = Common.ToString(tbl.Rows[0]["uuid"]);
                            agency.agencyname = Common.ToString(tbl.Rows[0]["agencyname"]);
                            agency.agencyshortname = Common.ToString(tbl.Rows[0]["agencyshortname"]);
                            agency.imono = Common.ToString(tbl.Rows[0]["imono"]);
                            agency.address = Common.ToString(tbl.Rows[0]["address"]);
                            agency.billingaddress = Common.ToString(tbl.Rows[0]["billingaddress"]);
                            agency.email = Common.ToString(tbl.Rows[0]["email"]);
                            agency.phone = Common.ToString(tbl.Rows[0]["phone"]);
                            agency.hcreatedat = Common.ToDateTime(tbl.Rows[0]["hcreatedat"]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return agency;
        }

        public AgencyDTO GetAgencyByUUID(string agencyuuid)
        {
            AgencyDTO agency = new AgencyDTO();
            try
            {
                if (agencyuuid != "")
                {
                    agency = ParseAgency("SELECT * FROM  masters.getagencybyuuid('" + agencyuuid + "','" + Common.HubID + "');");
                }
            }
            catch (Exception ex)
            { }
            return agency;
        }

        public AgencyDTO GetAgencyByDetailID(int detailid)
        {
            AgencyDTO agency = new AgencyDTO();
            try
            {
                if (detailid != 0)
                {
                    agency = ParseAgency("SELECT * FROM  masters.GetAgencyByDetailID('" + Common.HubID + "','" + detailid + "');");
                }
            }
            catch (Exception ex)
            { }
            return agency;
        }
        private AgencyDTO ParseAgency(string qry)
        {
            AgencyDTO agency = new AgencyDTO();
            try
            {
                using (SqlDB Db = new SqlDB())
                {
                    DataTable tbl = Db.GetDataTable(qry);
                    if (tbl.Rows.Count != 0)
                    {
                        agency.agencyid = new EncryptedData()
                        {
                            EncryptedValue = Common.Encrypt(Common.ToInt(tbl.Rows[0]["agencyid"])),
                            NumericValue = Common.ToInt(tbl.Rows[0]["agencyid"])
                        };
                        agency.agencydetailid = Common.ToInt(tbl.Rows[0]["agencydetailid"]);
                        agency.uuid = Common.ToString(tbl.Rows[0]["uuid"]);
                        agency.agencyname = Common.ToString(tbl.Rows[0]["agencyname"]);
                        agency.agencyshortname = Common.ToString(tbl.Rows[0]["agencyshortname"]);
                        agency.imono = Common.ToString(tbl.Rows[0]["imono"]);
                        agency.address = Common.ToString(tbl.Rows[0]["address"]);
                        agency.billingaddress = Common.ToString(tbl.Rows[0]["billingaddress"]);
                        agency.email = Common.ToString(tbl.Rows[0]["email"]);
                        agency.accountsemail = Common.ToString(tbl.Rows[0]["accountsemail"]);
                        agency.phone = Common.ToString(tbl.Rows[0]["phone"]);
                        agency.websites = Common.ToString(tbl.Rows[0]["websites"]);
                        agency.hcreatedat = Common.ToDateTime(tbl.Rows[0]["hcreatedat"]);
                        agency.countryList = Common.ToIntArray(tbl.Rows[0]["country"]).Select(x => Common.Encrypt(x)).ToList();
                        agency.portList = Common.ToIntArray(tbl.Rows[0]["port"]).Select(x => Common.Encrypt(x)).ToList();
                        try
                        {
                            agency.CustomAttributes = JsonConvert.DeserializeObject<List<KeyValuePair>>(Common.ToString(tbl.Rows[0]["customattributes"]));
                            if (agency.CustomAttributes == null)
                                agency.CustomAttributes = new List<KeyValuePair>();

                            agency.BankInfo = JsonConvert.DeserializeObject<List<CustomBankInfo>>(Common.ToString(tbl.Rows[0]["bankaccounts"]));
                            if (agency.BankInfo == null)
                            {
                                agency.BankInfo = new List<CustomBankInfo>();
                            }
                            agency.BankInfo.ForEach(x => x.FillBankKeyValues());
                            SessionManager.CurrenyAgency = new Agency { agency = agency };
                        }
                        catch (Exception)
                        { }

                    }
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }

            return agency;
        }

        public List<AgencyDTO> GetAgencyList()
        {
            List<AgencyDTO> list = new List<AgencyDTO>();
            try
            {
                using (SqlDB Db = new SqlDB())
                {
                    DataTable tbl = Db.GetDataTable("SELECT * FROM  masters.getagencylist('" + Common.HubID + "','" + Common.LoginID + "');");
                    list = (from DataRow dr in tbl.Rows
                            select new AgencyDTO()
                            {
                                agencyid = new EncryptedData()
                                {
                                    EncryptedValue = Common.Encrypt(Common.ToInt(dr["agencyid"])),
                                    NumericValue = Common.ToInt(dr["agencyid"])
                                },
                                agencydetailid = Common.ToInt(dr["agencydetailid"]),
                                uuid = Common.ToString(dr["uuid"]),
                                agencyname = Common.ToString(dr["agencyname"]),
                                agencyshortname = Common.ToString(dr["agencyshortname"]),
                                extras = new AgencyExtra()
                                {
                                    agencyshortcode = Common.GetShortcode(Common.ToString(dr["agencyname"])),
                                },

                                imono = Common.ToString(dr["imono"]),
                                address = Common.ToString(dr["address"]),
                                billingaddress = Common.ToString(dr["billingaddress"]),
                                email = Common.ToString(dr["email"]),
                                phone = Common.ToString(dr["phone"]),
                                hcreatedat = Common.ToDateTime(dr["hcreatedat"]),
                            }).ToList();

                    try
                    {
                        list.ForEach(x =>
                        {
                            x.hcreatedat = x.hcreatedat.AddMinutes(Convert.ToDouble(SessionManager.TimeOffSet));
                            var colors = Common.GetColorFromName(x.extras.agencyshortcode);
                            x.extras.agencycolor = colors.Color;
                            x.extras.agencybgcolor = colors.Bg;
                        }
                        );
                    }
                    catch (Exception)
                    { }
                }
            }
            catch (Exception ex)
            { }
            return list;
        }
        public List<AgencyDTO> GetAgencyModificationList(string uuid)
        {
            List<AgencyDTO> list = new List<AgencyDTO>();
            try
            {
                using (SqlDB Db = new SqlDB())
                {
                    DataTable tbl = Db.GetDataTable("SELECT * FROM  masters.getagencylogsbyuuid('" + uuid + "','" + Common.HubID + "');");
                    list = (from DataRow dr in tbl.Rows
                            select new AgencyDTO()
                            {
                                agencyid = new EncryptedData()
                                {
                                    EncryptedValue = Common.Encrypt(Common.ToInt(dr["agencyid"])),
                                    NumericValue = Common.ToInt(dr["agencyid"])
                                },
                                agencydetailid = Common.ToInt(dr["agencydetailid"]),
                                uuid = Common.ToString(dr["uuid"]),
                                agencyname = Common.ToString(dr["agencyname"]),
                                agencyshortname = Common.ToString(dr["agencyshortname"]),
                                imono = Common.ToString(dr["imono"]),
                                address = Common.ToString(dr["address"]),
                                billingaddress = Common.ToString(dr["billingaddress"]),
                                email = Common.ToString(dr["email"]),
                                accountsemail = Common.ToString(dr["accountsemail"]),
                                phone = Common.ToString(dr["phone"]),
                                websites = Common.ToString(dr["websites"]),
                                hcreatedat = Common.ToDateTime(dr["agencycreatedat"]),
                                ModifiedBy = Common.ToString(dr["ModifiedBy"]),
                                //countryList = Common.ToIntArray(dr["country"]).Select(x => Common.Encrypt(x)).ToList(),
                                //portList = Common.ToIntArray(dr["port"]).Select(x => Common.Encrypt(x)).ToList(),
                                AgencyModifiedAt = Common.ToDateTime(dr["AgencyModifiedAt"]),
                                CustomAttributes = JsonConvert.DeserializeObject<List<KeyValuePair>>(Common.ToString(dr["customattributes"])),
                                BankInfo = JsonConvert.DeserializeObject<List<CustomBankInfo>>(Common.ToString(dr["bankaccounts"])),
                            }).ToList();

                    try
                    {
                        //double serveroffset = Convert.ToDouble(Db.GetValue("SELECT EXTRACT(timezone FROM CURRENT_TIMESTAMP) / 60 AS timezone_offset_minutes;"));
                        //list.ForEach(x => x.AgencyModifiedAt = x.AgencyModifiedAt.AddMinutes(-1 * serveroffset).AddMinutes(Convert.ToDouble(SessionManager.TimeOffSet)));
                        list.ForEach(x => x.AgencyModifiedAt = x.AgencyModifiedAt.AddMinutes(Convert.ToDouble(SessionManager.TimeOffSet)));
                    }
                    catch (Exception)
                    { }

                }
            }
            catch (Exception ex)
            { }
            return list;
        }
    }
}