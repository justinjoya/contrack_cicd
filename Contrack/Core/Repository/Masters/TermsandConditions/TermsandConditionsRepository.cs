using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Contrack
{
    public class TermsandConditionsRepository : CustomException, ITermsandConditionsRepository
    {
        public Result SaveTermsandConditions(TermsandConditionsDTO termsandconditions)
        {
            Result result = new Result();
            try
            {
                var jsonData = new
                {
                    type = Common.Escape(termsandconditions.Type),
                    terms_text = Common.Escape(termsandconditions.TermsText),
                    agency_id = Common.Decrypt(termsandconditions.agency.agencyid.EncryptedValue)
                };
                var jsonPayload = Newtonsoft.Json.JsonConvert.SerializeObject(jsonData);
                using (SqlDB Db = new SqlDB())
                {
                    string query = "SELECT * FROM masters.save_terms_condition(" +
                       "p_termid := '" + Common.Decrypt(termsandconditions.TermId.EncryptedValue) + "'," +
                        "p_hubid := " + Common.HubID + "," +
                        "p_createdby := " + Common.LoginID + "," +
                        "p_details := '" + Common.Escape(jsonPayload) + "');";

                    DataTable tbl = Db.GetDataTable(query);
                    if (tbl.Rows.Count > 0)
                    {
                        if (tbl.Rows[0][0].ToString() == "1")
                            result = Common.SuccessMessage(tbl.Rows[0][1].ToString());
                        else
                            result = Common.ErrorMessage(tbl.Rows[0][1].ToString());
                    }
                    else
                        result = Common.ErrorMessage("Cannot save terms and conditions.");
                }
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.Message);
                RecordException(ex);
            }
            return result;
        }

        public Result DeleteTermsandConditions(string TermUuid)
        {
            Result result = new Result();
            try
            {
                using (SqlDB Db = new SqlDB())
                {
                    string query = "SELECT * FROM masters.soft_delete_term('" + TermUuid + "', '" + Common.LoginID + "');";
                    DataTable tbl = Db.GetDataTable(query);
                    if (tbl.Rows.Count > 0)
                    {
                        if (tbl.Rows[0][0].ToString() == "1")
                            result = Common.SuccessMessage(tbl.Rows[0][1].ToString());
                        else
                            result = Common.ErrorMessage(tbl.Rows[0][1].ToString());
                    }
                    else
                        result = Common.ErrorMessage("Cannot delete terms and conditions.");
                }
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.Message);
                RecordException(ex);
            }
            return result;
        }

        public TermsandConditionsDTO GetTermAndConditionsByUUID(string TermUuid)
        {
            TermsandConditionsDTO termsandconditions = new TermsandConditionsDTO();
            try
            {
                if (!string.IsNullOrEmpty(TermUuid))
                {
                    termsandconditions = ParseTermsAndConditions("SELECT * FROM masters.get_terms_condition('" + Common.HubID + "','" + TermUuid + "');");
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return termsandconditions;
        }

        private TermsandConditionsDTO ParseTermsAndConditions(string qry)
        {
            TermsandConditionsDTO termsandconditions = new TermsandConditionsDTO();
            try
            {
                using (SqlDB Db = new SqlDB())
                {
                    DataTable tbl = Db.GetDataTable(qry);
                    if (tbl.Rows.Count != 0)
                    {
                        termsandconditions.agency = new AgencyDTO()
                        {
                            agencyid = new EncryptedData()
                            {
                                NumericValue = Common.ToInt(tbl.Rows[0]["agency_id"]),
                                EncryptedValue = Common.Encrypt(Common.ToInt(tbl.Rows[0]["agency_id"]))
                            }
                        };
                        termsandconditions.TermId = new EncryptedData()
                        {
                            NumericValue = Common.ToInt(tbl.Rows[0]["term_id"]),
                            EncryptedValue = Common.Encrypt(Common.ToInt(tbl.Rows[0]["term_id"])),
                        };
                        termsandconditions.TermUuid = Common.ToString(tbl.Rows[0]["term_uuid"]);
                        termsandconditions.Type = Common.ToString(tbl.Rows[0]["type"]);
                        termsandconditions.TermsText = Common.ToString(tbl.Rows[0]["terms_text"]);
                        termsandconditions.HubId = Common.ToInt(tbl.Rows[0]["hub_id"]);
                    }
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return termsandconditions;
        }

        public List<TermsandConditionsDTO> GetTermsAndConditionsList()
        {
            List<TermsandConditionsDTO> list = new List<TermsandConditionsDTO>();
            var loginSession = SessionManager.LoginSession;
            try
            {
                using (SqlDB Db = new SqlDB())
                {
                    string query = "SELECT * FROM masters.get_terms_list('" + Common.HubID + "');";
                    DataTable tbl = Db.GetDataTable(query);
                    list = (from DataRow dr in tbl.Rows
                            select new TermsandConditionsDTO()
                            {
                                TermId = new EncryptedData()
                                {
                                    NumericValue = Common.ToInt(dr["term_id"]),
                                    EncryptedValue = Common.Encrypt(Common.ToInt(dr["term_id"]))
                                },
                                TermUuid = Common.ToString(dr["term_uuid"]),
                                HubId = Common.ToInt(dr["hub_id"]),
                                HubName = dr.Table.Columns.Contains("hub_name") ? Common.ToString(dr["hub_name"]) : Common.HubName,
                                agency = new AgencyDTO()
                                {
                                    agencyname = dr.Table.Columns.Contains("agency_name") ? Common.ToString(dr["agency_name"]) : ""
                                },
                                Type = Common.ToString(dr["type"]),
                                TermsText = Common.ToString(dr["terms_text"]),
                                UserName = Common.ToString(dr["created_by_user"]),
                                CreatedAt = Common.ToDateTime(dr["created_at"])
                            }).ToList();
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return list;
        }

        public TermsandConditionsDTO GetTermsAndConditionsbyTypeandAgency(string Type, string Agency)
        {
            TermsandConditionsDTO termsandconditions = new TermsandConditionsDTO();
            try
            {
                using (SqlDB Db = new SqlDB())
                {
                    string query = "SELECT * FROM masters.get_terms_condition_by_typeandagency('" + Common.HubID + "','" + Common.ToString(Type) + "','" + Common.Decrypt(Agency) + "');";
                    DataTable tbl = Db.GetDataTable(query);
                    if (tbl.Rows.Count != 0)
                    {
                        termsandconditions.agency = new AgencyDTO()
                        {
                            agencyid = new EncryptedData()
                            {
                                NumericValue = Common.ToInt(tbl.Rows[0]["agency_id"]),
                                EncryptedValue = Common.Encrypt(Common.ToInt(tbl.Rows[0]["agency_id"]))
                            }
                        };
                        termsandconditions.TermId = new EncryptedData()
                        {
                            NumericValue = Common.ToInt(tbl.Rows[0]["term_id"]),
                            EncryptedValue = Common.Encrypt(Common.ToInt(tbl.Rows[0]["term_id"])),
                        };
                        termsandconditions.TermUuid = Common.ToString(tbl.Rows[0]["term_uuid"]);
                        termsandconditions.Type = Common.ToString(tbl.Rows[0]["type"]);
                        termsandconditions.TermsText = Common.ToString(tbl.Rows[0]["terms_text"]);
                        termsandconditions.HubId = Common.ToInt(tbl.Rows[0]["hub_id"]);
                    }
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return termsandconditions;
        }
    }
}

