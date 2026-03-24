using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Contrack
{
    public class VendorInvoiceRepository : CustomException, IVendorInvoiceRepository
    {
        public Result SaveVendorInvoice(string headerJson, string detailsJson)
        {
            Result result = new Result();
            try
            {
                using (SqlDB Db = new SqlDB())
                {
                    DataTable tbl = Db.GetDataTable("SELECT * from procurement.save_vendor_invoice(" +
                        "p_header_json := '" + Common.Escape(headerJson) + "'," +
                        "p_details_json :='" + Common.Escape(detailsJson) + "'," +
                        "p_hub_id := '" + Common.HubID + "'," +
                        "p_created_by := '" + Common.LoginID + "'" +
                        ");");
                    if (tbl.Rows.Count != 0 && tbl.Rows[0][0].ToString() == "1")
                    {
                        result = Common.SuccessMessage(tbl.Rows[0][1].ToString());
                        result.TargetUUID = Convert.ToString(tbl.Rows[0][2].ToString()); // InvoiceUUID
                    }
                    else
                        result = Common.ErrorMessage(tbl.Rows.Count != 0 ? tbl.Rows[0][1].ToString() : "Cannot save Vendor Invoice");
                }
            }
            catch (Exception ex) { RecordException(ex); result = Common.ErrorMessage(ex.Message); }
            return result;
        }

        public Result DeleteVendorInvoice(string invoiceUuid)
        {
            Result result = new Result();
            try
            {
                using (SqlDB Db = new SqlDB())
                {
                    DataTable tbl = Db.GetDataTable("SELECT * from procurement.delete_vendor_invoice(" +
                        "p_invoice_uuid := '" + invoiceUuid + "'," +
                        "p_hub_id := '" + Common.HubID + "'," +
                        "p_userid := '" + Common.LoginID + "'" +
                        ");");
                    if (tbl.Rows.Count != 0 && Common.ToInt(tbl.Rows[0][0]) > 0)
                        result = Common.SuccessMessage(tbl.Rows[0][1].ToString());
                    else
                        result = Common.ErrorMessage(tbl.Rows.Count != 0 ? tbl.Rows[0][1].ToString() : "Cannot Delete Vendor Invoice");
                }
            }
            catch (Exception ex) { RecordException(ex); result = Common.ErrorMessage(ex.Message); }
            return result;
        }

        public Result QuoteApproval(string invoiceUuid, int approvalStatus, string approver)
        {
            Result result = new Result();
            try
            {
                using (SqlDB Db = new SqlDB())
                {
                    DataTable tbl = Db.GetDataTable("SELECT * from procurement.approvevendorinvoice(" +
                        "p_hubid := '" + Common.HubID + "'," +
                        "p_invoiceuuid := '" + invoiceUuid + "'," +
                        "p_approvestatus := '" + approvalStatus + "'," +
                        "p_userid := '" + Common.LoginID + "'," +
                        "p_approver := '" + Common.Decrypt(approver) + "'" +
                        ");");
                    if (tbl.Rows.Count != 0 && Common.ToInt(tbl.Rows[0][0]) > 0)
                        result = Common.SuccessMessage("Success");
                    else
                        result = Common.ErrorMessage(tbl.Rows.Count != 0 ? tbl.Rows[0][1].ToString() : "Cannot process request");
                }
            }
            catch (Exception ex) { RecordException(ex); result = Common.ErrorMessage(ex.Message); }
            return result;
        }

        public Result QuoteApprovalBulk(List<string> invoiceUuids)
        {
            Result result = new Result();
            try
            {
                using (SqlDB Db = new SqlDB())
                {
                    DataTable tbl = Db.GetDataTable("SELECT * from sandbox.create_pick_selection(" +
                        "p_hubid := '" + Common.HubID + "'," +
                        "p_uuids := ARRAY['" + string.Join("','", invoiceUuids.ToArray()) + "']::uuid[]," +
                        "p_createdby := '" + Common.LoginID + "'" +
                        ");");
                    if (tbl.Rows.Count != 0 && Common.ToInt(tbl.Rows[0][0]) > 0)
                    {
                        result = Common.SuccessMessage("Success");
                        result.TargetUUID = Convert.ToString(tbl.Rows[0][2].ToString());
                    }
                    else
                        result = Common.ErrorMessage(tbl.Rows.Count != 0 ? tbl.Rows[0][1].ToString() : "Cannot process request");
                }
            }
            catch (Exception ex) { RecordException(ex); result = Common.ErrorMessage(ex.Message); }
            return result;
        }

        public Result BulkApproveReject(string invoiceUuid, bool isAccept, string comments)
        {
            Result result = new Result();
            try
            {
                using (SqlDB Db = new SqlDB())
                {
                    DataTable tbl = Db.GetDataTable("SELECT * from procurement.approvebulkvendorinvoice(" +
                        "p_hubid := '" + Common.HubID + "'," +
                        "p_pickuuid := '" + invoiceUuid + "'," +
                        "p_message := '" + Common.Escape(comments) + "'," +
                        "p_approvestatus := '" + (isAccept ? 1 : 2) + "'," +
                        "p_userid := '" + Common.LoginID + "'" +
                        ");");
                    if (tbl.Rows.Count != 0 && Common.ToInt(tbl.Rows[0][0]) > 0)
                        result = Common.SuccessMessage("Success");
                    else
                        result = Common.ErrorMessage(tbl.Rows.Count != 0 ? tbl.Rows[0][1].ToString() : "Cannot process request");
                }
            }
            catch (Exception ex) { RecordException(ex); result = Common.ErrorMessage(ex.Message); }
            return result;
        }

        public Result SendBulkApproval(string invoiceUuid, string approver, string message)
        {
            Result result = new Result();
            try
            {
                using (SqlDB Db = new SqlDB())
                {
                    DataTable tbl = Db.GetDataTable("SELECT * from procurement.approvebulkvendorinvoice(" +
                        "p_hubid := '" + Common.HubID + "'," +
                        "p_pickuuid := '" + invoiceUuid + "'," +
                        "p_message := '" + Common.Escape(message) + "'," +
                        "p_approvestatus := '0'," +
                        "p_userid := '" + Common.LoginID + "'," +
                        "p_approver := '" + Common.Decrypt(approver) + "'" +
                        ");");
                    if (tbl.Rows.Count != 0 && Common.ToInt(tbl.Rows[0][0]) > 0)
                        result = Common.SuccessMessage("Success");
                    else
                        result = Common.ErrorMessage(tbl.Rows.Count != 0 ? tbl.Rows[0][1].ToString() : "Cannot process request");
                }
            }
            catch (Exception ex) { RecordException(ex); result = Common.ErrorMessage(ex.Message); }
            return result;
        }

        public Result RecordPayment(string invoiceUuid, decimal paidAmount)
        {
            Result result = new Result();
            try
            {
                using (SqlDB Db = new SqlDB())
                {
                    DataTable tbl = Db.GetDataTable("SELECT * from procurement.record_vendor_invoice(" +
                        "p_hubid := '" + Common.HubID + "'," +
                        "p_invoiceuuid := '" + invoiceUuid + "'," +
                        "p_paidamount := '" + paidAmount + "'," +
                        "p_updatedby := '" + Common.LoginID + "'" +
                        ");");
                    if (tbl.Rows.Count != 0 && Common.ToInt(tbl.Rows[0][0]) > 0)
                        result = Common.SuccessMessage("Success");
                    else
                        result = Common.ErrorMessage(tbl.Rows.Count != 0 ? tbl.Rows[0][1].ToString() : "Cannot process request");
                }
            }
            catch (Exception ex) { RecordException(ex); result = Common.ErrorMessage(ex.Message); }
            return result;
        }

        public Result CancelVendorInvoice(string invoiceUuid, int invoiceId)
        {
            Result result = new Result();
            try
            {
                using (SqlDB Db = new SqlDB())
                {
                    DataTable tbl = Db.GetDataTable("select * from Procurement.cancel_vendor_invoice('" + Common.HubID + "','" + invoiceUuid + "','" + invoiceId + "','" + Common.LoginID + "');");
                    if (tbl.Rows.Count != 0 && Common.ToInt(tbl.Rows[0][0]) > 0)
                        result = Common.SuccessMessage("Success");
                    else
                        result = Common.ErrorMessage(tbl.Rows.Count != 0 ? tbl.Rows[0][1].ToString() : "Cannot process request");
                }
            }
            catch (Exception ex) { RecordException(ex); result = Common.ErrorMessage(ex.Message); }
            return result;
        }

        public void UpdateDocumentUUID(string invoiceUuid, string documentUuid)
        {
            try
            {
                using (SqlDB Db = new SqlDB())
                {
                    Db.Execute("Update procurement.vendor_invoice_header set documentuuid = '" + documentUuid + "' where vendorinvoiceuuid='" + invoiceUuid + "' and hubid='" + Common.HubID + "'");
                }
            }
            catch (Exception ex) { RecordException(ex); }
        }

        public VendorInvoiceDTO GetVendorInvoiceByUUID(string uuid)
        {
            VendorInvoiceDTO vi = new VendorInvoiceDTO();
            try
            {
                using (SqlDB Db = new SqlDB())
                {
                    DataTable tbl = Db.GetDataTable("SELECT * FROM  procurement.get_vendor_inv_uuid('" + Common.HubID + "','" + uuid + "','" + Common.LoginID + "');");
                    if (tbl.Rows.Count != 0)
                    {
                        vi.InvoiceDate = Common.ToDateTimeString(tbl.Rows[0]["h_invoicedate"], "yyyy-MM-dd");
                        vi.PI = new PurchaseIntentDTO() { PurchaseIntentID = Common.ToInt(tbl.Rows[0]["h_piid"]), PurchaseIntentIDInc = Common.Encrypt(Common.ToInt(tbl.Rows[0]["h_piid"])) };
                        vi.InvoiceID = new EncryptedData() { NumericValue = Common.ToInt(tbl.Rows[0]["h_vendorinvoiceid"]), EncryptedValue = Common.Encrypt(Common.ToInt(tbl.Rows[0]["h_vendorinvoiceid"])) };
                        vi.InvoiceUUID = Common.ToString(tbl.Rows[0]["h_vendorinvoiceuuid"]);
                        vi.InvoiceNo = Common.ToString(tbl.Rows[0]["h_vendorinvoiceno"]);
                        vi.AgencyDetailID = new EncryptedData() { EncryptedValue = Common.Encrypt(Common.ToInt(tbl.Rows[0]["h_agencydetailid"])), NumericValue = Common.ToInt(tbl.Rows[0]["h_agencydetailid"]) };
                        vi.VendorDetailID = new EncryptedData() { EncryptedValue = Common.Encrypt(Common.ToInt(tbl.Rows[0]["h_vendordetailid"])), NumericValue = Common.ToInt(tbl.Rows[0]["h_vendordetailid"]) };
                        vi.VesselAssignmentID = Common.ToInt64Array(tbl.Rows[0]["h_vesseldetailid"]).Select(x => Common.Encrypt(Convert.ToInt32(x))).ToList();

                        vi.DueDate = Common.ToDateTimeString(tbl.Rows[0]["h_duedate"], "yyyy-MM-dd");
                        vi.Currency = Common.ToString(tbl.Rows[0]["h_currency"]);
                        vi.InvoiceAmount = Common.ToDecimal(tbl.Rows[0]["h_invoiceamount"]);
                        vi.PaidAmount = Common.ToDecimal(tbl.Rows[0]["h_paidamount"]);
                        vi.CreatedAt = Common.ToDateTime(tbl.Rows[0]["h_createdat"]);
                        vi.POUUID = Common.ToString(tbl.Rows[0]["h_pouuid"]);
                        vi.JobID = Common.ToString(tbl.Rows[0]["h_jobid"]);
                        vi.AgencyName = Common.ToString(tbl.Rows[0]["agency_name"]);
                        vi.VendorName = Common.ToString(tbl.Rows[0]["vendor_name"]);
                        vi.POCode = Common.ToString(tbl.Rows[0]["po_code"]);
                        vi.StatusName = Common.ToString(tbl.Rows[0]["status_desc"]);
                        vi.InvoiceDocument.documentuuid = Common.ToString(tbl.Rows[0]["h_documentuuid"]);
                        vi.Status = Common.ToInt(tbl.Rows[0]["h_status"]);
                        vi.Remarks = Common.ToString(tbl.Rows[0]["h_remarks"]);
                        vi.HODApprover = Common.ToString(tbl.Rows[0]["approvedby1_username"]);
                        vi.FinanceApprover = Common.ToString(tbl.Rows[0]["approvedby2_username"]);
                        vi.CreatedUser = Common.ToString(tbl.Rows[0]["createdby_username"]);
                        vi.Approvedby1 = Common.ToInt(tbl.Rows[0]["h_approvedby1"]);
                        vi.POApprover = Common.ToString(tbl.Rows[0]["h_poapprover"]);

                        vi.Details = (from drinner in tbl.AsEnumerable()
                                      where Common.ToString(drinner["d_vendorinvoicedetailuuid"]) != ""
                                      select new VendorInvoiceDetail()
                                      {
                                          Type = string.IsNullOrEmpty(Common.ToString(drinner["d_jobtypedetailuuid"])) ? 2 : 1,
                                          JobTypeID = Common.ToInt(drinner["d_jobtypeid"]),
                                          InvoiceDetailID = Common.ToInt(drinner["d_vendorinvoicedetailid"]),
                                          InvoiceDetailUUID = Common.ToString(drinner["d_vendorinvoicedetailuuid"]),
                                          JobTypeDetailUUID = Common.ToString(drinner["d_jobtypedetailuuid"]),
                                          ServiceName = Common.ToString(drinner["d_operation"]),
                                          Order = Common.ToInt(drinner["d_sortorder"]),
                                          Description = string.IsNullOrEmpty(Common.ToString(drinner["d_jobtypedetailuuid"])) ? Common.ToString(drinner["d_description"]) : Common.ToString(drinner["d_jobdetaildesc"]),
                                          ArticleIMPACode = Common.ToString(drinner["d_articlecode"]),
                                          PartNo = Common.ToString(drinner["d_partno"]),
                                          Make = Common.ToString(drinner["d_make"]),
                                          Model = Common.ToString(drinner["d_model"]),
                                          UOM = Common.ToString(drinner["d_uom"]),
                                          POQty = Common.ToDecimal(drinner["d_poquantity"]),
                                          POPrice = Common.ToDecimal(drinner["d_poprice"]),
                                          InvoiceQty = Common.ToDecimal(drinner["d_invoicequantity"]),
                                          InvoicePrice = Common.ToDecimal(drinner["d_invoiceprice"]),
                                          PODetailID = Common.ToInt(drinner["d_podetailid"]),
                                          Remarks = Common.ToString(drinner["d_itemremarks"]),
                                      }).ToList();
                    }
                }
            }
            catch (Exception ex) { RecordException(ex); }
            return vi;
        }

        public List<VIPaymentSummary> GetPaymentSummary(string invoiceUuid)
        {
            List<VIPaymentSummary> list = new List<VIPaymentSummary>();
            try
            {
                using (SqlDB Db = new SqlDB())
                {
                    DataTable tbl = Db.GetDataTable("SELECT * FROM  procurement.get_invoice_batch_details('" + invoiceUuid + "','" + Common.HubID + "');");
                    list = (from DataRow dr in tbl.Rows
                            select new VIPaymentSummary()
                            {
                                BatchID = Common.ToInt(dr["batchid"]),
                                BatchUUID = Common.ToString(dr["batchuuid"]),
                                BatchNo = Common.ToString(dr["batchcode"]),
                                CreatedAt = Common.ToDateTime(dr["createddate"]),
                                BatchCreatedAt = Common.ToDateTime(dr["batchcreatedDate"]),
                                BatchCurrency = Common.ToString(dr["paymentcurrency"]),
                                PaidAmount = Common.ToDecimal(dr["paidamount"]),
                                BalanceAmount = Common.ToDecimal(dr["balanceamount"]),
                                ConversionRate = Common.ToDecimal(dr["conversionrate"]),
                                PaymentMethod = Common.ToString(dr["paymentmethod"]),
                                Status = Common.ToString(dr["status"]),
                            }).ToList();
                }
            }
            catch (Exception ex) { RecordException(ex); }
            return list;
        }

        public List<string> GetAutoApproveInvoices(List<string> invoiceUuids)
        {
            List<string> result = new List<string>();
            try
            {
                using (SqlDB Db = new SqlDB())
                {
                    DataTable tbl = Db.GetDataTable("SELECT * from procurement.check_auto_approvable_vendor_invoices(" +
                        "p_hubid := '" + Common.HubID + "'," +
                        "p_invoiceuuids := ARRAY['" + string.Join("','", invoiceUuids.ToArray()) + "']::uuid[]" +
                        ");");
                    result = (from DataRow dr in tbl.Rows select Common.ToString(dr["vendorinvoiceuuid"])).ToList();
                }
            }
            catch (Exception ex) { RecordException(ex); }
            return result;
        }
        public List<PIVendorInvoice> GetVendorInvoiceListByPI(int piid, string hubId)
        {
            List<PIVendorInvoice> list = new List<PIVendorInvoice>();
            try
            {
                using (SqlDB Db = new SqlDB())
                {
                    DataTable tbl = Db.GetDataTable($"SELECT * FROM procurement.get_vendor_invoice_list_by_piid('{piid}','{hubId}');");
                    list = (from DataRow dr in tbl.Rows
                            select new PIVendorInvoice()
                            {
                                VendorInvoiceID = Common.ToInt(dr["vendorinvoiceid"]),
                                VendorInvoiceUUID = Common.ToString(dr["vendorinvoiceuuid"]),
                                VendorInvoiceNo = Common.ToString(dr["vendorinvoiceno"]),
                                InvoiceAmount = Common.ToDecimal(dr["invoiceamount"]),
                                Currency = Common.ToString(dr["currency"]),
                                VendorName = Common.ToString(dr["vendor_name"]),
                                StatusName = Common.ToString(dr["statusname"]),
                                CreatedAt = Common.ToDateTime(dr["createdat"])
                            }).ToList();
                }
            }
            catch (Exception ex) { RecordException(ex); }
            return list;
        }
        public List<InvoiceCurrencies> GetCurrencies(string invoiceUuid)
        {
            List<InvoiceCurrencies> result = new List<InvoiceCurrencies>();
            try
            {
                using (SqlDB Db = new SqlDB())
                {
                    DataTable tbl = Db.GetDataTable("SELECT * FROM procurement.get_vendor_invoice_amount_by_pickid('" + Common.HubID + "','" + invoiceUuid + "');");
                    result = (from DataRow dr in tbl.Rows
                              select new InvoiceCurrencies()
                              {
                                  amount = Common.ToDecimal(dr["amount"]),
                                  invoicecount = Common.ToInt(dr["invoicecount"]),
                                  currency = Common.ToString(dr["currency"]),
                              }).ToList();
                }
            }
            catch (Exception ex) { RecordException(ex); }
            return result;
        }
    }
}