using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;

namespace Contrack
{
    public class PricingRepository : CustomException, IPricingRepository
    {
        public Result SaveHeader(PricingHeaderDTO header)
        {
            Result result = new Result();
            try
            {
                using (SqlDB Db = new SqlDB(DatabaseCollection.Contrack))
                {
                    DataTable tbl = Db.GetDataTable("SELECT * from booking.quotation_pricing_header_save(" +
                     "p_hub_id := '" + Common.HubID + "'," +
                     "p_pricing_id := '" + Common.Decrypt(header.PricingID.EncryptedValue) + "'," +
                     "p_pol := '" + Common.Decrypt(header.POL.PortID.EncryptedValue) + "'," +
                     "p_pod := '" + Common.Decrypt(header.POD.PortID.EncryptedValue) + "'," +
                     "p_currency := '" + header.Currency + "'," +
                     "p_template_desc := '" + Common.Escape(header.Description) + "'," +
                     "p_template_no := ''," +
                     "p_user_id := '" + Common.LoginID + "'" +
                     ");");
                    if (tbl.Rows.Count != 0)
                    {
                        if (tbl.Rows[0][0].ToString() == "1")
                        {
                            result = Common.SuccessMessage(tbl.Rows[0][1].ToString());
                            result.TargetUUID = Convert.ToString(tbl.Rows[0][2].ToString());
                        }
                        else
                            result = Common.ErrorMessage(tbl.Rows[0][1].ToString());
                    }
                    else
                        result = Common.ErrorMessage("Cannot save template");
                }
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.Message);
                RecordException(ex);
            }
            return result;
        }

        public Result CloneHeader(PricingHeaderDTO header)
        {
            Result result = new Result();
            try
            {
                using (SqlDB Db = new SqlDB(DatabaseCollection.Contrack))
                {
                    DataTable tbl = Db.GetDataTable("SELECT * from booking.quotation_pricing_header_clone(" +
                     "p_hubid := '" + Common.HubID + "'," +
                     "p_source_pricing_id := '" + Common.Decrypt(header.PricingID.EncryptedValue) + "'," +
                     "p_pol := '" + Common.Decrypt(header.POL.PortID.EncryptedValue) + "'," +
                     "p_pod := '" + Common.Decrypt(header.POD.PortID.EncryptedValue) + "'," +
                     "p_currency := '" + header.Currency + "'," +
                     "p_templatedesc := '" + Common.Escape(header.Description) + "'," +
                     "p_templateno := ''," +
                     "p_createdby := '" + Common.LoginID + "'" +
                     ");");
                    if (tbl.Rows.Count != 0)
                    {
                        if (tbl.Rows[0][0].ToString() == "1")
                        {
                            result = Common.SuccessMessage(tbl.Rows[0][1].ToString());
                            result.TargetUUID = Convert.ToString(tbl.Rows[0][2].ToString());
                        }
                        else
                            result = Common.ErrorMessage(tbl.Rows[0][1].ToString());
                    }
                    else
                        result = Common.ErrorMessage("Cannot clone template");
                }
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.Message);
                RecordException(ex);
            }
            return result;
        }

        public Result SaveType(PricingTypeDTO currency)
        {
            Result result = new Result();
            try
            {
                using (SqlDB Db = new SqlDB(DatabaseCollection.Contrack))
                {
                    DataTable tbl = Db.GetDataTable("SELECT * from booking.quotation_pricing_type_save(" +
                     "p_type_id := '" + Common.Decrypt(currency.TypeID.EncryptedValue) + "'," +
                     "p_pricing_uuid := '" + currency.PricingUUID + "'," +
                     "p_hubid := '" + Common.HubID + "'," +
                     "p_type := '" + Common.Decrypt(currency.TransferType.EncryptedValue) + "'" +
                     ");");
                    if (tbl.Rows.Count != 0)
                    {
                        if (tbl.Rows[0][0].ToString() == "1")
                        {
                            result = Common.SuccessMessage(tbl.Rows[0][1].ToString());
                            result.TargetUUID = Convert.ToString(tbl.Rows[0][2].ToString());
                        }
                        else
                            result = Common.ErrorMessage(tbl.Rows[0][1].ToString());
                    }
                    else
                        result = Common.ErrorMessage("Cannot save currency");
                }
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.Message);
                RecordException(ex);
            }
            return result;
        }

        public Result CloneType(PricingTypeDTO currency)
        {
            Result result = new Result();
            try
            {
                using (SqlDB Db = new SqlDB(DatabaseCollection.Contrack))
                {
                    DataTable tbl = Db.GetDataTable("SELECT * from booking.quotation_pricing_currency_clone(" +
                     "p_source_currency_id := '" + Common.Decrypt(currency.TypeID.EncryptedValue) + "'," +
                     "p_hubid := '" + Common.HubID + "'," +
                     "p_type := '" + currency.TypeID.EncryptedValue + "'" +
                     ");");
                    if (tbl.Rows.Count != 0)
                    {
                        if (tbl.Rows[0][0].ToString() == "1")
                        {
                            result = Common.SuccessMessage(tbl.Rows[0][1].ToString());
                            result.TargetUUID = Convert.ToString(tbl.Rows[0][2].ToString());
                        }
                        else
                            result = Common.ErrorMessage(tbl.Rows[0][1].ToString());
                    }
                    else
                        result = Common.ErrorMessage("Cannot save type");
                }
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.Message);
                RecordException(ex);
            }
            return result;
        }

        public Result SaveDetail(PricingDetailDTO detail)
        {
            Result result = new Result();
            try
            {
                using (SqlDB Db = new SqlDB(DatabaseCollection.Contrack))
                {
                    DataTable tbl = Db.GetDataTable("SELECT * from booking.quotation_pricing_detail_save(" +
                     "p_detail_id := '" + Common.Decrypt(detail.DetailID.EncryptedValue) + "'," +
                     "p_type_id := '" + Common.Decrypt(detail.TypeID.EncryptedValue) + "'," +
                     "p_line_item_uuid := '" + detail.LineItemUUID + "'," +
                     "p_containertypeid := '" + Common.Decrypt(detail.ContainerTypeID.EncryptedValue) + "'," +
                     "p_containersizeid := '" + Common.Decrypt(detail.ContainerSizeID.EncryptedValue) + "'," +
                     "p_fullempty := '" + detail.FullEmpty + "'," +
                     "p_uom := '" + detail.UOM + "'," +
                     "p_isfrightcharges := '" + detail.IsFrightCharges + "'," +
                     "p_amount := '" + detail.Amount + "'," +
                     "p_comments := '" + Common.Escape(detail.Comments) + "'" +
                     ");");
                    if (tbl.Rows.Count != 0)
                    {
                        if (tbl.Rows[0][0].ToString() == "1")
                        {
                            result = Common.SuccessMessage(tbl.Rows[0][1].ToString());
                            result.TargetUUID = Convert.ToString(tbl.Rows[0][2].ToString());
                        }
                        else
                            result = Common.ErrorMessage(tbl.Rows[0][1].ToString());
                    }
                    else
                        result = Common.ErrorMessage("Cannot save line item");
                }
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.Message);
                RecordException(ex);
            }
            return result;
        }

        public Result SaveCustomer(PricingCustomerDTO customer)
        {
            Result result = new Result();
            try
            {
                using (SqlDB Db = new SqlDB(DatabaseCollection.Contrack))
                {
                    DataTable tbl = Db.GetDataTable("SELECT * from booking.quotation_pricing_client_save(" +
                     "p_pricing_id := '" + Common.Decrypt(customer.PricingID.EncryptedValue) + "'," +
                     "p_hub_id := '" + Common.HubID + "'," +
                     "p_clientid := '" + Common.Decrypt(customer.ClientID.EncryptedValue) + "'" +
                     ");");
                    if (tbl.Rows.Count != 0)
                    {
                        if (tbl.Rows[0][0].ToString() == "1")
                        {
                            result = Common.SuccessMessage(tbl.Rows[0][1].ToString());
                            result.TargetUUID = Convert.ToString(tbl.Rows[0][2].ToString());
                        }
                        else
                            result = Common.ErrorMessage(tbl.Rows[0][1].ToString());
                    }
                    else
                        result = Common.ErrorMessage("Cannot save client");
                }
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.Message);
                RecordException(ex);
            }
            return result;
        }
        public Result DeleteCustomer(PricingCustomerDTO customer)
        {
            Result result = new Result();
            try
            {
                using (SqlDB Db = new SqlDB(DatabaseCollection.Contrack))
                {
                    DataTable tbl = Db.GetDataTable("SELECT * from booking.quotation_pricing_client_delete(" +
                     "p_pricing_id := '" + Common.Decrypt(customer.PricingID.EncryptedValue) + "'," +
                     "p_hub_id := '" + Common.HubID + "'," +
                     "p_clientid := '" + Common.Decrypt(customer.ClientID.EncryptedValue) + "'" +
                     ");");
                    if (tbl.Rows.Count != 0)
                    {
                        if (tbl.Rows[0][0].ToString() == "1")
                        {
                            result = Common.SuccessMessage(tbl.Rows[0][1].ToString());
                            result.TargetUUID = Convert.ToString(tbl.Rows[0][2].ToString());
                        }
                        else
                            result = Common.ErrorMessage(tbl.Rows[0][1].ToString());
                    }
                    else
                        result = Common.ErrorMessage("Cannot delete client");
                }
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.Message);
                RecordException(ex);
            }
            return result;
        }
        public Result DeleteDetail(string typeuuid, string detailuuid)
        {
            Result result = new Result();
            try
            {
                using (SqlDB Db = new SqlDB(DatabaseCollection.Contrack))
                {
                    DataTable tbl = Db.GetDataTable("SELECT * from booking.quotation_pricing_detail_delete(" +
                     "p_typeuuid := '" + typeuuid + "'," +
                     "p_detailuuid := '" + detailuuid + "'" +
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
                        result = Common.ErrorMessage("Cannot delete lineitem");
                }
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.Message);
                RecordException(ex);
            }
            return result;
        }


        public PricingHeaderDTO GetHeader(string pricinguuid)
        {
            PricingHeaderDTO pricing = new PricingHeaderDTO();
            try
            {
                using (SqlDB Db = new SqlDB(DatabaseCollection.Contrack))
                {

                    DataTable tbl = Db.GetDataTable("SELECT * FROM booking.quotation_pricing_header_get('" + pricinguuid + "','" + Common.HubID + "');");
                    if (tbl.Rows.Count > 0)
                    {
                        pricing = new PricingHeaderDTO()
                        {
                            PricingID = new EncryptedData()
                            {
                                EncryptedValue = Common.Encrypt(Common.ToInt(tbl.Rows[0]["pricingid"])),
                                NumericValue = Common.ToInt(tbl.Rows[0]["pricingid"]),
                            },
                            PricingUUID = Common.ToString(tbl.Rows[0]["pricinguuid"]),
                            POL = new PortDTO()
                            {
                                PortID = new EncryptedData()
                                {
                                    EncryptedValue = Common.Encrypt(Common.ToInt(tbl.Rows[0]["pol_id"])),
                                    NumericValue = Common.ToInt(tbl.Rows[0]["pol_id"]),
                                },
                                PortName = Common.ToString(tbl.Rows[0]["pol_portname"]),
                                PortCode = Common.ToString(tbl.Rows[0]["pol_portcode"]),
                                CountryName = Common.ToString(tbl.Rows[0]["pol_countryname"]),
                                CountryCode = Common.ToString(tbl.Rows[0]["pol_countrycode"]),
                                Flag = Common.ToString(tbl.Rows[0]["pol_countryflag"]),
                            },
                            POD = new PortDTO()
                            {
                                PortID = new EncryptedData()
                                {
                                    EncryptedValue = Common.Encrypt(Common.ToInt(tbl.Rows[0]["pod_id"])),
                                    NumericValue = Common.ToInt(tbl.Rows[0]["pod_id"]),
                                },
                                PortName = Common.ToString(tbl.Rows[0]["pod_portname"]),
                                PortCode = Common.ToString(tbl.Rows[0]["pod_portcode"]),
                                CountryName = Common.ToString(tbl.Rows[0]["pod_countryname"]),
                                CountryCode = Common.ToString(tbl.Rows[0]["pod_countrycode"]),
                                Flag = Common.ToString(tbl.Rows[0]["pod_countryflag"]),
                            },
                            Currency = Common.ToString(tbl.Rows[0]["currency"]),

                            Description = Common.ToString(tbl.Rows[0]["templatedesc"]),
                            TemplateNo = Common.ToString(tbl.Rows[0]["templateno"]),
                            CreatedAt = FormatConvertor.ToClientDateTimeFormat(Common.ToDateTime(tbl.Rows[0]["createdat"])),
                            CreatedByName = Common.ToString(tbl.Rows[0]["created_name"]),
                            Types = (from DataRow dr in tbl.Rows
                                     select new PricingTypeDTO
                                     {
                                         TypeID = new EncryptedData()
                                         {
                                             EncryptedValue = Common.Encrypt(Common.ToInt(dr["typeid"])),
                                             NumericValue = Common.ToInt(dr["typeid"]),
                                         },
                                         TypeUUID = Common.ToString(dr["typeuuid"]),
                                         TransferType = new EncryptedData()
                                         {
                                             EncryptedValue = Common.Encrypt(Common.ToInt(dr["transfertype"])),
                                             NumericValue = Common.ToInt(dr["transfertype"]),
                                         },
                                     }).ToList()
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return pricing;
        }
        public PricingTypeDTO GetTransferType(string typeuuid)
        {
            PricingTypeDTO currency = new PricingTypeDTO();
            try
            {
                using (SqlDB Db = new SqlDB(DatabaseCollection.Contrack))
                {

                    DataTable tbl = Db.GetDataTable("SELECT * FROM booking.quotation_pricing_type_get('" + typeuuid + "','" + Common.HubID + "');");
                    if (tbl.Rows.Count > 0)
                    {
                        currency = new PricingTypeDTO()
                        {
                            Details = (from DataRow dr in tbl.Rows
                                       select new PricingDetailDTO
                                       {
                                           DetailID = new EncryptedData()
                                           {
                                               EncryptedValue = Common.Encrypt(Common.ToInt(dr["detailid"])),
                                               NumericValue = Common.ToInt(dr["detailid"]),
                                           },
                                           DetailUUID = Common.ToString(dr["detailuuid"]),
                                           LineItemUUID = Common.ToString(dr["lineitemuuid"]),
                                           LineItemDesc = Common.ToString(dr["description"]),
                                           Amount = Common.ToDecimal(dr["amount"]),
                                           ContainerTypeID = new EncryptedData()
                                           {
                                               EncryptedValue = Common.Encrypt(Common.ToInt(dr["containertypeid"])),
                                               NumericValue = Common.ToInt(dr["containertypeid"]),
                                           },
                                           ContainerSizeID = new EncryptedData()
                                           {
                                               EncryptedValue = Common.Encrypt(Common.ToInt(dr["containersizeid"])),
                                               NumericValue = Common.ToInt(dr["containersizeid"]),
                                           },
                                           FullEmpty = Common.ToString(dr["empty_full"]),
                                           TypeSizeCombined = Common.ToString(dr["containertypesizename"]),
                                           UOM = Common.ToString(dr["uom"]),
                                           IsFrightCharges = Common.ToBool(dr["isfrightcharges"]),
                                           Comments = Common.ToString(dr["comments"]),
                                       }).ToList()
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return currency;
        }
        public PricingTypeDTO GetPricingByBookingUUID(string bookinguuid, string currency, string clientdetailid)
        {
            PricingTypeDTO type = new PricingTypeDTO();
            try
            {
                using (SqlDB Db = new SqlDB(DatabaseCollection.Contrack))
                {

                    DataTable tbl = Db.GetDataTable("SELECT * FROM booking.quotation_pricing_get_bookinguuid('" + bookinguuid + "','" + Common.HubID + "','" + Common.Decrypt(clientdetailid) + "','" + currency + "');");
                    if (tbl.Rows.Count > 0)
                    {
                        type = new PricingTypeDTO()
                        {
                            Details = (from DataRow dr in tbl.Rows
                                       select new PricingDetailDTO
                                       {
                                           DetailID = new EncryptedData()
                                           {
                                               EncryptedValue = Common.Encrypt(Common.ToInt(dr["detailid"])),
                                               NumericValue = Common.ToInt(dr["detailid"]),
                                           },
                                           DetailUUID = Common.ToString(dr["detailuuid"]),
                                           LineItemUUID = Common.ToString(dr["lineitemuuid"]),
                                           LineItemDesc = Common.ToString(dr["description"]),
                                           Amount = Common.ToDecimal(dr["amount"]),
                                           ContainerTypeID = new EncryptedData()
                                           {
                                               EncryptedValue = Common.Encrypt(Common.ToInt(dr["containertypeid"])),
                                               NumericValue = Common.ToInt(dr["containertypeid"]),
                                           },
                                           ContainerSizeID = new EncryptedData()
                                           {
                                               EncryptedValue = Common.Encrypt(Common.ToInt(dr["containersizeid"])),
                                               NumericValue = Common.ToInt(dr["containersizeid"]),
                                           },
                                           FullEmpty = Common.ToString(dr["empty_full"]),
                                           TypeSizeCombined = Common.ToString(dr["containertypesizename"]),
                                           UOM = Common.ToString(dr["uom"]),
                                           IsFrightCharges = Common.ToBool(dr["isfrightcharges"]),
                                           Comments = Common.ToString(dr["comments"]),
                                           Qty = Common.ToInt(dr["qty"]),
                                       }).ToList()
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return type;
        }
        public List<PricingCustomerDTO> GetClientsList(string pricinguuid)
        {
            List<PricingCustomerDTO> list = new List<PricingCustomerDTO>();
            try
            {
                using (SqlDB Db = new SqlDB(DatabaseCollection.Contrack))
                {
                    string query = $"SELECT * FROM booking.quotation_pricing_client_get(" +
                                   $"p_hub_id := {Common.HubID}, " +
                                   $"p_pricinguuid := {Common.GetUUID(pricinguuid)});";

                    DataTable tbl = Db.GetDataTable(query);
                    if (tbl != null)
                    {
                        foreach (DataRow dr in tbl.Rows)
                        {
                            list.Add(new PricingCustomerDTO()
                            {
                                ClientID = new EncryptedData
                                {
                                    NumericValue = Common.ToInt(dr["clientid"]),
                                    EncryptedValue = Common.Encrypt(Common.ToInt(dr["clientid"]))
                                },
                                ClientName = Common.ToString(dr["name"])
                            });
                        }
                    }
                }
            }
            catch (Exception ex) { RecordException(ex); }
            return list;
        }
        public List<PricingHeaderDTO> GetPricingList(PricingListFilter filter)
        {
            List<PricingHeaderDTO> list = new List<PricingHeaderDTO>();
            try
            {
                using (SqlDB Db = new SqlDB(DatabaseCollection.Contrack))
                {
                    string jsonFilters = JsonConvert.SerializeObject(filter);
                    string query = $"SELECT * FROM booking.quotation_pricing_list(" +
                                   $"p_hubid := {Common.HubID}, " +
                                   $"p_filters := '{Common.Escape(jsonFilters)}'::jsonb, " +
                                   $"p_userid := {Common.LoginID});";

                    DataTable tbl = Db.GetDataTable(query);
                    if (tbl != null)
                    {
                        foreach (DataRow dr in tbl.Rows)
                        {
                            list.Add(ParsePricingList(dr));
                        }
                    }
                }
            }
            catch (Exception ex) { RecordException(ex); }
            return list;
        }
        private PricingHeaderDTO ParsePricingList(DataRow dr)
        {
            return new PricingHeaderDTO()
            {
                RowCount = new TableCounts
                {
                    row_index = Common.ToInt(dr["row_index"]),
                    totalnoofrows = Common.ToInt(dr["total_count"])
                },
                PricingID = new EncryptedData
                {
                    NumericValue = Common.ToInt(dr["pricingid"]),
                    EncryptedValue = Common.Encrypt(Common.ToInt(dr["pricingid"]))
                },
                PricingUUID = Common.ToString(dr["pricinguuid"]),
                Description = Common.ToString(dr["templatedesc"]),
                TemplateNo = Common.ToString(dr["templateno"]),
                POL = new PortDTO
                {
                    PortName = Common.ToString(dr["pol_name"]),
                    PortCode = Common.ToString(dr["pol_code"]),
                    Flag = Common.ToString(dr["pol_flag"])
                },
                POD = new PortDTO
                {
                    PortName = Common.ToString(dr["pod_name"]),
                    PortCode = Common.ToString(dr["pod_code"]),
                    Flag = Common.ToString(dr["pod_flag"])
                },
                //TransferTypeName = IDReferences.GetTransferType(Common.ToInt(dr["transfertype_id"])),
                CreatedAt = FormatConvertor.ToClientDateTimeFormat(Common.ToDateTime(dr["createdat"])),
                CreatedByName = Common.ToString(dr["createdby_name"]),
                ClientCount = Common.ToInt(dr["client_count"]),
                Currency = Common.ToString(dr["currency"]),
                //CurrencyCount = Common.ToInt(dr["currency_count"])
            };
        }

        public PricingDetailDTO GetDetail(string typeid, string detailid)
        {
            PricingDetailDTO detail = new PricingDetailDTO();
            try
            {
                using (SqlDB Db = new SqlDB(DatabaseCollection.Contrack))
                {

                    DataTable tbl = Db.GetDataTable("SELECT * FROM booking.quotation_pricing_detail_get('" + Common.Decrypt(typeid) + "','" + Common.Decrypt(detailid) + "');");
                    if (tbl.Rows.Count > 0)
                    {
                        detail = new PricingDetailDTO()
                        {
                            DetailID = new EncryptedData()
                            {
                                EncryptedValue = Common.Encrypt(Common.ToInt(tbl.Rows[0]["detailid"])),
                                NumericValue = Common.ToInt(tbl.Rows[0]["detailid"]),
                            },
                            TypeID = new EncryptedData()
                            {
                                EncryptedValue = Common.Encrypt(Common.ToInt(tbl.Rows[0]["typeid"])),
                                NumericValue = Common.ToInt(tbl.Rows[0]["typeid"]),
                            },
                            ContainerTypeID = new EncryptedData()
                            {
                                EncryptedValue = Common.Encrypt(Common.ToInt(tbl.Rows[0]["containertypeid"])),
                                NumericValue = Common.ToInt(tbl.Rows[0]["containertypeid"]),
                            },
                            ContainerSizeID = new EncryptedData()
                            {
                                EncryptedValue = Common.Encrypt(Common.ToInt(tbl.Rows[0]["containersizeid"])),
                                NumericValue = Common.ToInt(tbl.Rows[0]["containersizeid"]),
                            },
                            DetailUUID = Common.ToString(tbl.Rows[0]["detailuuid"]),
                            LineItemUUID = Common.ToString(tbl.Rows[0]["lineitemuuid"]),
                            Amount = Common.ToDecimal(Common.ToDecimal(tbl.Rows[0]["amount"]).ToString("0.00")),
                            FullEmpty = Common.ToString(tbl.Rows[0]["empty_full"]),
                            UOM = Common.ToString(tbl.Rows[0]["uom"]),
                            IsFrightCharges = Common.ToBool(tbl.Rows[0]["isfrightcharges"]),
                            Comments = Common.ToString(tbl.Rows[0]["comments"]),
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return detail;
        }
        public PricingDetailDTO GetPricingByLineItemUUID(string bookinguuid, string clientdetailid, string currency, string lineitemuuid, string typeid, string sizeid, string fullempty)
        {
            PricingDetailDTO output = new PricingDetailDTO();
            try
            {
                using (SqlDB Db = new SqlDB(DatabaseCollection.Contrack))
                {

                    DataTable tbl = Db.GetDataTable("SELECT * from booking.quotation_pricing_get_lineitem(" +
                          "p_booking := '" + bookinguuid + "'," +
                          "p_hubid := '" + Common.HubID + "'," +
                          "p_createfor := '" + Common.Decrypt(clientdetailid) + "'," +
                          "p_currency := '" + currency + "'," +
                          "p_lineitemuuid := '" + lineitemuuid + "'," +
                          "p_typeid := '" + Common.Decrypt(typeid) + "'," +
                          "p_sizeid := '" + Common.Decrypt(sizeid) + "'," +
                          "p_fullempty := '" + fullempty + "'" +
                          ");");
                    if (tbl.Rows.Count > 0)
                    {
                        output = new PricingDetailDTO()
                        {
                            Amount = Common.ToDecimal(Common.ToDecimal(tbl.Rows[0]["amount"]).ToString("0.00")),
                            UOM = Common.ToString(tbl.Rows[0]["uom"]),
                        };
                    }

                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return output;
        }
    }
}