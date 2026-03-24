using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Contrack
{
    public class PurchaseOrderRepository : CustomException, IPurchaseOrderRepository
    {
        public Result SaveDirectPurchaseOrder(PurchaseOrderDTO po)
        {
            Result result = new Result();
            try
            {
                bool isNewPo = string.IsNullOrEmpty(po.pouuid);
                string detailsJson = JsonConvert.SerializeObject(po.Details ?? new List<PurchaseOrderDetailDTO>());
                using (SqlDB Db = new SqlDB())
                {
                    string query = "SELECT * FROM procurement.save_direct_po(" +
                                   $"p_dpoid := {Common.Decrypt(po.dpoid.EncryptedValue)}, " +
                                   $"p_dpo_uuid := {Common.GetUUID(po.dpouuid)}, " +
                                   $"p_hubid := {Common.HubID}, " +
                                   $"p_agencydetailid := {Common.Decrypt(po.agencydetailid.EncryptedValue)}, " +
                                   $"p_jobid := null, " +
                                   $"p_port := null, " +
                                   $"p_arrivaldate := null, " +
                                   $"p_departure := null, " +
                                   $"p_deliverydate := null, " +
                                   $"p_priority := null, " +
                                   $"p_jobtypes := null, " +
                                   $"p_clientdetailid := null, " +
                                   $"p_vesseldetailid := null, " +
                                   $"p_requester := null, " +
                                   $"p_salutation := null, " +
                                   $"p_remarks := '{Common.Escape(po.comments)}', " +
                                   $"p_createdby := {Common.LoginID}, " +
                                   $"p_details := '{Common.Escape(detailsJson)}'::jsonb, " +
                                   $"p_vendor_detail_id := {Common.Decrypt(po.vendordetailid.EncryptedValue)}, " +
                                   $"p_vendor_currency := '{Common.Escape(po.vendorcurrency)}', " +
                                   $"p_appid := {Common.MyAppID});";
                    DataTable tbl = Db.GetDataTable(query);
                    if (tbl.Rows.Count != 0)
                    {
                        if (tbl.Rows[0][0].ToString() == "1")
                        {
                            result = Common.SuccessMessage(tbl.Rows[0][1].ToString());
                            po.pouuid = Convert.ToString(tbl.Rows[0][4]);
                            result.TargetUUID = po.pouuid;

                            if (!isNewPo)
                            {
                                UpdatePOHeader(po);
                            }
                        }
                        else
                        {
                            result = Common.ErrorMessage(tbl.Rows[0][1].ToString());
                        }
                    }
                    else
                    {
                        result = Common.ErrorMessage("Cannot create Direct PO.");
                    }
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
                result = Common.ErrorMessage("System Error: " + ex.Message);
            }
            return result;
        }
        public Result UpdatePOHeader(PurchaseOrderDTO po)
        {
            Result result = new Result();
            try
            {
                using (SqlDB Db = new SqlDB())
                {
                    string query = "SELECT * FROM procurement.update_po_header(" +
                        $"p_pouuid := '{Common.Escape(po.pouuid)}', " +
                        $"p_hubid := {Common.HubID}, " +
                        $"p_billing_address := '{Common.Escape(po.billto)}', " +
                        $"p_remarks := '{Common.Escape(po.comments)}', " +
                        $"p_deliveryaddress := '{Common.Escape(po.deliveryaddress)}', " +
                        $"p_terms_conditions := '{Common.Escape(po.terms)}', " +
                        $"p_invoice_instructions := '{Common.Escape(po.invoiceinstruction)}', " +
                        $"p_vendoraddress := '{Common.Escape(po.vendoraddress)}', " +
                        $"p_updatedby := {Common.LoginID});";

                    DataTable tbl = Db.GetDataTable(query);
                    if (tbl.Rows.Count != 0)
                    {
                        if (tbl.Rows[0][0].ToString() == "1")
                        {
                            result = Common.SuccessMessage(tbl.Rows[0][1].ToString());
                        }
                        else
                        {
                            result = Common.ErrorMessage(tbl.Rows[0][1].ToString());
                        }
                    }
                    else
                    {
                        result = Common.ErrorMessage("Cannot update PO header.");
                    }
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
                result = Common.ErrorMessage("System Error: " + ex.Message);
            }
            return result;
        }
        public PurchaseOrderDTO GetPurchaseOrderByUUID(string pouuid)
        {
            PurchaseOrderDTO model = new PurchaseOrderDTO();
            try
            {
                using (SqlDB Db = new SqlDB())
                {
                    DataTable tbl = Db.GetDataTable($"SELECT * FROM procurement.get_po_by_uiid('{pouuid}', {Common.HubID}, {Common.LoginID});");
                    if (tbl != null && tbl.Rows.Count > 0)
                    {
                        model = ParsePO(tbl.Rows[0]);
                        if (model.isdirectpo)
                        {
                            string quotationUuid = Common.ToString(tbl.Rows[0]["quotationuuid"]);
                            if (!string.IsNullOrEmpty(quotationUuid))
                            {
                                DataTable dpoTbl = Db.GetDataTable($"SELECT * FROM procurement.get_direct_po_by_uuid('{quotationUuid}', {Common.HubID}, {Common.LoginID});");
                                if (dpoTbl != null && dpoTbl.Rows.Count > 0)
                                {
                                    model.dpoid = new EncryptedData { NumericValue = Common.ToInt(dpoTbl.Rows[0]["dpoid"]), EncryptedValue = Common.Encrypt(Common.ToInt(dpoTbl.Rows[0]["dpoid"])) };
                                    model.dpouuid = Common.ToString(dpoTbl.Rows[0]["dpo_uuid"]);
                                    model.agencydetailid = new EncryptedData { NumericValue = Common.ToInt(dpoTbl.Rows[0]["agencydetailid"]), EncryptedValue = Common.Encrypt(Common.ToInt(dpoTbl.Rows[0]["agencydetailid"])) };
                                    model.agencyname = Common.ToString(dpoTbl.Rows[0]["agency_name"]);

                                    if (string.IsNullOrEmpty(model.comments))
                                    {
                                        model.comments = Common.ToString(dpoTbl.Rows[0]["remarks"]);
                                    }
                                }
                            }
                        }

                        model.Details = ParsePODetails(tbl);
                    }
                }
            }
            catch (Exception ex) { RecordException(ex); }
            return model;
        }
        public PurchaseOrderDTO GetPurchaseOrderByID(string poid)
        {
            PurchaseOrderDTO model = new PurchaseOrderDTO();
            try
            {
                long decryptedId = Common.Decrypt(poid);
                if (decryptedId <= 0) return model;

                using (SqlDB Db = new SqlDB())
                {
                    DataTable tbl = Db.GetDataTable($"SELECT * FROM procurement.po_get_byid({decryptedId}, {Common.HubID});");
                    if (tbl != null && tbl.Rows.Count > 0)
                    {
                        model = ParsePO(tbl.Rows[0]);
                        model.Details = ParsePODetails(tbl);
                    }
                }
            }
            catch (Exception ex) { RecordException(ex); }
            return model;
        }
        public List<PurchaseOrderListDTO> GetPurchaseOrderList(PurchaseOrderFilterPage filter)
        {
            List<PurchaseOrderListDTO> list = new List<PurchaseOrderListDTO>();
            try
            {
                using (SqlDB Db = new SqlDB())
                {
                    string jsonFilters = JsonConvert.SerializeObject(filter);
                    string query = $"SELECT * FROM procurement.get_po_list({Common.HubID}, '{Common.Escape(jsonFilters)}'::jsonb, {Common.LoginID});";

                    DataTable tbl = Db.GetDataTable(query);
                    if (tbl != null && tbl.Rows.Count > 0)
                    {
                        list = (from DataRow dr in tbl.Rows
                                select ParsePOList(dr)).ToList();
                    }
                }
            }
            catch (Exception ex) { RecordException(ex); }
            return list;
        }
        public List<PurchaseOrderStatusCountDTO> GetPurchaseOrderStatusCount(PurchaseOrderFilterPage filter)
        {
            List<PurchaseOrderStatusCountDTO> list = new List<PurchaseOrderStatusCountDTO>();
            try
            {
                using (SqlDB Db = new SqlDB())
                {
                    string jsonFilters = JsonConvert.SerializeObject(filter);
                    string query = $"SELECT * FROM procurement.get_po_list_status_count({Common.HubID}, '{Common.Escape(jsonFilters)}'::jsonb, {Common.LoginID});";

                    DataTable tbl = Db.GetDataTable(query);
                    if (tbl != null && tbl.Rows.Count > 0)
                    {
                        list = (from DataRow dr in tbl.Rows
                                select new PurchaseOrderStatusCountDTO
                                {
                                    status_code = Common.ToInt(dr["status_id"]),
                                    status_count = Common.ToLong(dr["status_count"])
                                }).ToList();
                    }
                }
            }
            catch (Exception ex) { RecordException(ex); }
            return list;
        }
        private PurchaseOrderListDTO ParsePOList(DataRow dr)
        {
            return new PurchaseOrderListDTO()
            {
                rowcount = new TableCounts
                {
                    row_index = Common.ToInt(dr["row_index"]),
                    totalnoofrows = Common.ToInt(dr["total_count"])
                },
                poid = new EncryptedData { NumericValue = Common.ToInt(dr["poid"]), EncryptedValue = Common.Encrypt(Common.ToInt(dr["poid"])) },
                pouuid = Common.ToString(dr["pouuid"]),
                pocode = Common.ToString(dr["pocode"]),
                vendor_name = Common.ToString(dr["vendor_name"]),
                agencyname = Common.ToString(dr["agencyname"]),
                no_of_items = Common.ToLong(dr["no_of_items"]),
                total_amount = Common.ToDecimal(dr["total_amount"]),
                currency = Common.ToString(dr["currency"]),
                issuedate = FormatConvertor.ToDateTimeFormat(Common.ToDateTime(dr["issuedate"])),
                createdby_fullname = Common.ToString(dr["createdby_fullname"]),
                status = FormatConvertor.ToStatus(Common.ToInt(dr["status"]), StatusEnum.PurchaseOrder),
                isdirectpo = Common.ToBool(dr["isdirectpo"])
            };
        }
        private PurchaseOrderDTO ParsePO(DataRow dr)
        {
            return new PurchaseOrderDTO()
            {
                poid = new EncryptedData { NumericValue = Common.ToInt(dr["poid"]), EncryptedValue = Common.Encrypt(Common.ToInt(dr["poid"])) },
                pouuid = Common.ToString(dr["pouuid"]),
                pocode = Common.ToString(dr["pocode"]),
                vendordetailid = new EncryptedData { NumericValue = Common.ToInt(dr["vendordetailid"]), EncryptedValue = Common.Encrypt(Common.ToInt(dr["vendordetailid"])) },
                vendor_name = Common.ToString(dr["vendor_name"]),
                vendorcurrency = Common.ToString(dr["vendorcurrency"]),
                comments = Common.ToString(dr["comments"]),
                terms = Common.ToString(dr["terms"]),
                invoiceinstruction = Common.ToString(dr["invoiceinstruction"]),
                vendoraddress = Common.ToString(dr["vendoraddress"]),
                billto = Common.ToString(dr["billto"]),
                deliveryaddress = Common.ToString(dr["deliveryaddress"]),
                isdirectpo = Common.ToBool(dr["isdirectpo"]),
                createdat = FormatConvertor.ToDateTimeFormat(Common.ToDateTime(dr["createdat"])),
                issuedate = FormatConvertor.ToDateTimeFormat(Common.ToDateTime(dr["issuedate"])),
                status_desc = Common.ToString(dr["status_desc"]),
                createdby_username = Common.ToString(dr["createdby_username"])
            };
        }
        private List<PurchaseOrderDetailDTO> ParsePODetails(DataTable tbl)
        {
            List<PurchaseOrderDetailDTO> details = new List<PurchaseOrderDetailDTO>();
            foreach (DataRow dr in tbl.Rows)
            {
                if (!string.IsNullOrEmpty(Common.ToString(dr["podetailuuid"])))
                {
                    details.Add(new PurchaseOrderDetailDTO()
                    {
                        podetailid = new EncryptedData { NumericValue = Common.ToInt(dr["podetailid"]), EncryptedValue = Common.Encrypt(Common.ToInt(dr["podetailid"])) },
                        podetailuuid = Common.ToString(dr["podetailuuid"]),
                        quantity = Common.ToDecimal(dr["quantity"]),
                        tax = Common.ToDecimal(dr["tax"]),
                        sortorder = Common.ToInt(dr["sortorder"]),
                        vendorprice = Common.ToDecimal(dr["vendorprice"]),
                        itemname = Common.ToString(dr["itemname"]),
                        article_impacode = Common.ToString(dr["article_impacode"]),
                        partno = Common.ToString(dr["partno"]),
                        make = Common.ToString(dr["make"]),
                        model = Common.ToString(dr["model"]),
                        uom = Common.ToString(dr["uom"]),
                        remarks = Common.ToString(dr["remarks"]),
                        jobtype = Common.ToInt(dr["jobtype"]),
                        jobtypedetailuuid = Common.ToString(dr["jobtypedetailuuid"]),
                        Type = string.IsNullOrEmpty(Common.ToString(dr["jobtypedetailuuid"])) ? 2 : 1
                    });
                }
            }
            return details;
        }
    }
}