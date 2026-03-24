using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Web;
using System.Web.Mvc;

namespace Contrack
{
    public class SmtpConfigRepository : CustomException, ISmtpConfigRepository
    {
        public List<SmtpConfigDTO> GetSmtpConfigList()
        {
            List<SmtpConfigDTO> list = new List<SmtpConfigDTO>();

            try
            {
                using (SqlDB Db = new SqlDB())
                {
                    DataTable tbl = Db.GetDataTable("SELECT * FROM  masters.get_smtp_config_list('" + Common.HubID + "');");
                    list = (from DataRow dr in tbl.Rows
                            select new SmtpConfigDTO()
                            {
                                HubName = Common.ToString(dr["hubname"]),
                                agency = new AgencyDTO()
                                {
                                    agencyid = new EncryptedData()
                                    {
                                        NumericValue = Common.ToInt(dr["agencyid"]),
                                        EncryptedValue = Common.Encrypt(Common.ToInt(dr["agencyid"]))
                                    },
                                    agencyname = Common.ToString(dr["agencyname"])
                                },
                                smtp_id = new EncryptedData()
                                {
                                    NumericValue = Common.ToInt(dr["smtpid"]),
                                    EncryptedValue = Common.Encrypt(Common.ToInt(dr["smtpid"]))
                                },
                                config_name = Common.ToString(dr["config_name"]),
                                smtp_host = Common.ToString(dr["smtp_host"]),
                                smtp_port = Common.ToInt(dr["smtp_port"]),
                                smtp_username = Common.ToString(dr["smtp_username"]),
                                encryption_type = Common.ToString(dr["encryption_type"]),
                                from_email = Common.ToString(dr["from_email"]),
                                reply_email = Common.ToString(dr["reply_to_email"]),
                                is_deleted = Common.ToBool(dr["is_deleted"]),
                                is_default = Common.ToBool(dr["is_default"]),
                                created_username = Common.ToString(dr["created_by_username"]),
                                created_at = Common.ToDateTime(dr["created_at"]),
                                deleted_username = Common.ToString(dr["deleted_by_username"])
                            }).ToList();

                    try
                    {
                        list.ForEach(x => x.created_at = x.created_at.AddMinutes(Convert.ToDouble(SessionManager.TimeOffSet)));
                    }
                    catch (Exception)
                    { }
                }
            }
            catch
            {
            }
            return list;
        }

        public SmtpConfigDTO GetSmtpConfigById(string refid)
        {
            SmtpConfigDTO smtpconfig = new SmtpConfigDTO();
            try
            {
                if (refid != "")
                {
                    smtpconfig = ParseSmtpConfig("SELECT * FROM  masters.Get_SMTP_Configuration('" + Common.Decrypt(refid) + "','" + Common.HubID + "');");
                }
            }
            catch (Exception ex)
            {
            }
            return smtpconfig;
        }
        private SmtpConfigDTO ParseSmtpConfig(string qry)
        {
            SmtpConfigDTO smtpconfig = new SmtpConfigDTO();
            try
            {
                using (SqlDB Db = new SqlDB())
                {
                    DataTable tbl = Db.GetDataTable(qry);
                    if (tbl.Rows.Count != 0)
                    {
                        smtpconfig.agency = new AgencyDTO()
                        {
                            agencyid = new EncryptedData()
                            {
                                NumericValue = Common.ToInt(tbl.Rows[0]["agencyid"]),
                                EncryptedValue = Common.Encrypt(Common.ToInt(tbl.Rows[0]["agencyid"]))
                            }
                        };
                        smtpconfig.smtp_id = new EncryptedData()
                        {
                            NumericValue = Common.ToInt(tbl.Rows[0]["smtpid"]),
                            EncryptedValue = Common.Encrypt(Common.ToInt(tbl.Rows[0]["smtpid"]))
                        };
                        smtpconfig.config_name = Common.ToString(tbl.Rows[0]["config_name"]);
                        smtpconfig.smtp_host = Common.ToString(tbl.Rows[0]["smtp_host"]);
                        smtpconfig.smtp_port = Common.ToInt(tbl.Rows[0]["smtp_port"]);
                        smtpconfig.smtp_username = Common.ToString(tbl.Rows[0]["smtp_username"]);
                        smtpconfig.smtp_password = Common.ToString(tbl.Rows[0]["smtp_password"]);
                        smtpconfig.encryption_type = Common.ToString(tbl.Rows[0]["encryption_type"]);
                        smtpconfig.from_email = Common.ToString(tbl.Rows[0]["from_email"]);
                        smtpconfig.reply_email = Common.ToString(tbl.Rows[0]["reply_to_email"]);
                        smtpconfig.is_deleted = Common.ToBool(tbl.Rows[0]["is_deleted"]);
                        smtpconfig.is_default = Common.ToBool(tbl.Rows[0]["is_default"]);
                        smtpconfig.created_by = Common.ToLong(tbl.Rows[0]["created_by"]);
                        smtpconfig.created_at = Common.ToDateTime(tbl.Rows[0]["created_at"]);
                    }
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }

            return smtpconfig;
        }

        public Result SaveSmtpConfig(SmtpConfigDTO smtpconfig)
        {
            Result result = new Result();
            try
            {
                using (SqlDB Db = new SqlDB())
                {
                    DataTable tbl = Db.GetDataTable("SELECT * FROM masters.save_smtp_configuration(" +
                         "p_hubid := " + Common.HubID + "," +
                         "p_config_name := '" + Common.Escape(smtpconfig.config_name) + "'," +
                         "p_smtp_host := '" + Common.Escape(smtpconfig.smtp_host) + "'," +
                         "p_smtp_username := '" + Common.Escape(smtpconfig.smtp_username) + "'," +
                         "p_smtp_password := '" + Common.Escape(smtpconfig.smtp_password) + "'," +
                         "p_from_email := '" + Common.Escape(smtpconfig.from_email) + "'," +
                         "p_smtpid := " + Common.Decrypt(smtpconfig.smtp_id.EncryptedValue) + "," +
                         "p_agencyid := " + Common.Decrypt(smtpconfig.agency.agencyid.EncryptedValue) + "," +
                         "p_smtp_port := " + smtpconfig.smtp_port + "," +
                         "p_encryption_type := '" + Common.Escape(smtpconfig.encryption_type) + "'," +
                         "p_reply_to_email := '" + Common.Escape(smtpconfig.reply_email) + "'," +
                         "p_is_default := " + smtpconfig.is_default + "," +
                         "p_created_by := " + (Common.Decrypt(smtpconfig.smtp_id.EncryptedValue) == 0 ? Common.LoginID.ToString() : "NULL") + "," +
                          "p_updated_by := " + (Common.Decrypt(smtpconfig.smtp_id.EncryptedValue) > 0 ? Common.LoginID.ToString() : "NULL") + "" +
                     ");");

                    if (tbl.Rows.Count > 0)
                    {
                        if (tbl.Rows[0][0].ToString() == "1")
                            result = Common.SuccessMessage(tbl.Rows[0][1].ToString());
                        else
                            result = Common.ErrorMessage(tbl.Rows[0][1].ToString());
                    }
                    else
                    {
                        result = Common.ErrorMessage("Cannot Save terms and Conditions.");
                    }
                }
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.Message);
                RecordException(ex);
            }
            return result;
        }

        public Result DeleteSmtpConfig(SmtpConfigDTO smtpconfig)
        {
            Result result = new Result();

            try
            {
                using (SqlDB Db = new SqlDB())
                {
                    DataTable tbl = Db.GetDataTable("SELECT * FROM masters.delete_smtp_configuration('" +
                    smtpconfig.smtp_id.NumericValue + "', '" + Common.LoginID + "')");

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
                        result = Common.ErrorMessage("Cannot Delete SMTP Configuration");
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