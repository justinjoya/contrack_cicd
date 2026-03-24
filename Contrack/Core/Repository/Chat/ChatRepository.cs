using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Helpers;

namespace Contrack
{
    public class ChatRepository : CustomException, IChatRepository
    {
        public Result SaveMessage(ChatDTO chat)
        {
            Result result = new Result();
            try
            {
                using (SqlDB Db = new SqlDB())
                {

                    DataTable tbl = Db.GetDataTable("SELECT * from procurement.save_chat_detail(" +
                        "p_type := '" + chat.type + "'," +
                        "p_hub_id := '" + Common.HubID + "'," +
                        "p_user_id := '" + (chat.autoapprove ? -1 : Common.LoginID) + "'," +
                        "p_message_text := '" + Common.Escape(chat.message_text) + "'," +
                        "p_attachment_path := '" + chat.attachment_path + "'," +
                        "p_target_id := '" + Common.Decrypt(chat.target_id) + "'" +
                        ");");
                    if (tbl.Rows.Count != 0)
                    {
                        if (Common.ToInt(tbl.Rows[0][0]) > 0)
                        {
                            result = Common.SuccessMessage(tbl.Rows[0][1].ToString());
                        }
                        else
                            result = Common.ErrorMessage(tbl.Rows[0][1].ToString());
                    }
                    else
                        result = Common.ErrorMessage("Cannot Save Message");

                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
                result = Common.ErrorMessage(ex.Message);
            }
            return result;
        }

        public List<ChatDTO> GetChatList(int targetid, string type)
        {
            List<ChatDTO> list = new List<ChatDTO>();
            try
            {
                using (SqlDB Db = new SqlDB())
                {

                    DataTable tbl = Db.GetDataTable("SELECT * FROM procurement.get_chat_details('" + Common.HubID + "','" + targetid + "','" + type + "');");

                    list = (from DataRow dr in tbl.Rows
                            select new ChatDTO()
                            {
                                message_text = Common.ToString(dr["messagetext"]),
                                created_at = FormatConvertor.ToClientDateTimeFormat(Common.ToDateTime(dr["createdat"])),
                                created_by = Common.ToInt(dr["userid"]),
                                attachment_path = Common.ToString(dr["attachmentpath"]),
                                created_by_name = Common.ToString(dr["username"]),
                                target_id = Common.ToString(Common.ToInt(dr["targetid"])),
                                type = Common.ToString(dr["type"]),
                            }).ToList();

                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return list;
        }
    }
}