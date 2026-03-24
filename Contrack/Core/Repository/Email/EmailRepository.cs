using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;

namespace Contrack
{
    public class EmailRepository : CustomException, IEmailRepository
    {
        public string SaveEmailLog(EmailDTO email, string hubId, string loginId)
        {
            string logUuid = "";
            try
            {
                using (SqlDB Db = new SqlDB())
                {
                    string attachmentsJson = JsonConvert.SerializeObject(email.Attachments ?? new List<AttachmentDTO>());
                    string functionCall = "SELECT * from logs.save_email_notification(" +
                             $"p_hubid := {hubId}," +
                             $"p_createdby := '{Common.Escape(loginId)}'," +
                             $"p_type := '{Common.Escape(email.EmailType)}'," +
                             $"p_emailtitle := '{Common.Escape(email.Subject)}'," +
                             $"p_emailsubject := '{Common.Escape(email.Subject)}'," +
                             $"p_emailbody := '{Common.Escape(email.Body)}'," +
                             $"p_emailfrom := 'notification@inc2.net'," +
                             $"p_emailto := '{Common.Escape(string.Join(";", email.EmailTo))}'," +
                             $"p_smtpid := 1," +
                             $"p_emailtargetuuid := {Common.GetUUID(email.ID)}," +
                             $"p_agencyid := {email.AgencyID}," +
                             $"p_piid := {email.PIID}," +
                             $"p_emailcc := '{Common.Escape(string.Join(";", email.CC))}'," +
                             $"p_attachments := '{Common.Escape(attachmentsJson)}'::jsonb" +
                             ");";

                    DataTable tbl = Db.GetDataTable(functionCall);

                    if (tbl.Rows.Count > 0 && Common.ToInt(tbl.Rows[0]["resultid"]) == 1)
                    {
                        logUuid = Common.ToString(tbl.Rows[0]["emaillog_uuid"]);
                    }
                }
            }
            catch (Exception ex) { RecordException(ex); }
            return logUuid;
        }

        public void UpdateEmailStatus(string logUuid, int statusId, string message, string hubId)
        {
            try
            {
                using (SqlDB Db = new SqlDB())
                {
                    string sql = "UPDATE logs.email_notification_log SET " +
                                 $"status = {statusId}, " +
                                 $"response = '{Common.Escape(message)}', " +
                                 "updatedat = NOW() " +
                                 $"WHERE emailloguuid = {Common.GetUUID(logUuid)} AND hubid = {hubId};";
                    Db.Execute(sql);
                }
            }
            catch (Exception ex) { RecordException(ex); }
        }

        public List<string> GetEmails(string loginId)
        {
            return new List<string>(); 
        }

        public Result AddEmail(string loginId, string email)
        {
            return Common.SuccessMessage("Saved"); 
        }

        public string GetVendorEmailByDetailID(int detailId)
        {
            string email = "";
            try
            {
                using (SqlDB Db = new SqlDB())
                {
                    DataTable tbl = Db.GetDataTable($"SELECT contactemail FROM masters.vendordetails WHERE vendordetailid = {detailId}");
                    if (tbl.Rows.Count > 0) email = Common.ToString(tbl.Rows[0]["contactemail"]);
                }
            }
            catch (Exception ex) { RecordException(ex); }
            return email;
        }
    }
}