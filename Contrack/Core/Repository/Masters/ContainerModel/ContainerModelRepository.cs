using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Contrack
{
    public class ContainerModelRepository : CustomException, IContainerModelRepository
    {
        public Result SaveContainerModel(ContainerModelDTO model)
        {
            Result result = new Result();
            try
            {
                using (SqlDB Db = new SqlDB(DatabaseCollection.Contrack))
                {
                    string query = "SELECT * FROM masters.container_model_save(" +
                                   "p_modelid := '" + Common.Decrypt(model.modelid.EncryptedValue) + "'," +
                                   "p_hub_id := '" + Common.HubID + "'," +
                                   "p_sizeid := '" + Common.Decrypt(model.sizeid.EncryptedValue) + "'," +
                                   "p_iso_code := '" + Common.Escape(model.iso_code) + "'," +
                                   "p_description := '" + Common.Escape(model.description) + "'," +
                                   "p_typeid := '" + Common.Decrypt(model.typeid.EncryptedValue) + "'," +
                                   "p_user_id := '" + Common.LoginID + "'" +
                                   ");";

                    DataTable tbl = Db.GetDataTable(query);
                    if (tbl.Rows.Count > 0)
                    {
                        string resultId = tbl.Rows[0][0].ToString();
                        string message = tbl.Rows[0][1].ToString();
                        if (resultId == "1")
                        {
                            result = Common.SuccessMessage(message);
                            result.TargetID = Convert.ToInt32(tbl.Rows[0][2]);
                        }
                        else
                        {
                            result = Common.ErrorMessage(message);
                        }
                    }
                    else
                    {
                        result = Common.ErrorMessage("Cannot save container model");
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

        public Result DeleteContainerModel(int containermodelid)
        {
            Result result = new Result();
            try
            {
                using (SqlDB Db = new SqlDB(DatabaseCollection.Contrack))
                {
                    string query = "SELECT * FROM masters.container_model_delete(" +
                                   "p_modelid := '" + containermodelid + "'," +
                                   "p_hub_id := '" + Common.HubID + "'," +
                                   "p_user_id := '" + Common.LoginID + "'" +
                                   ");";

                    DataTable tbl = Db.GetDataTable(query);
                    if (tbl.Rows.Count > 0)
                    {
                        string resultId = tbl.Rows[0][0].ToString();
                        string message = tbl.Rows[0][1].ToString();

                        if (resultId == "1")
                        {
                            result = Common.SuccessMessage(message);
                        }
                        else
                        {
                            result = Common.ErrorMessage(message);
                        }
                    }
                    else
                    {
                        result = Common.ErrorMessage("Cannot delete container model");
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

        public List<ContainerTypeDTO> GetContainerModels(ContainerModelFilter filter)
        {
            List<ContainerTypeDTO> list = new List<ContainerTypeDTO>();
            try
            {
                using (SqlDB Db = new SqlDB(DatabaseCollection.Contrack))
                {
                    string filterJson = JsonConvert.SerializeObject(filter);
                    DataTable tbl = Db.GetDataTable(
                    "SELECT * FROM masters.container_type_list(" +
                    "p_hubid := '" + Common.HubID + "', " +
                    "p_filters := '" + filterJson + "'::jsonb" +
                    ");");

                    var rows = tbl.AsEnumerable();

                    list = rows
                        .GroupBy(t => Common.ToInt(t["typeid"]))
                        .Select(typeGrp =>
                        {
                            var firstType = typeGrp.First();
                            var allModels = typeGrp
                                .Where(m => Common.ToInt(m["modelid"]) > 0)
                                .GroupBy(m => Common.ToInt(m["modelid"]))
                                .Select(modelGrp =>
                                {
                                    var firstModel = modelGrp.First();
                                    int sizeId = Common.ToInt(firstModel["sizeid"]);

                                    if (sizeId <= 0)
                                        return null;

                                    return new ContainerModelDTO
                                    {
                                        modelid = new EncryptedData()
                                        {
                                            NumericValue = Common.ToInt(firstModel["modelid"]),
                                            EncryptedValue = Common.Encrypt(Common.ToInt(firstModel["modelid"]))
                                        },
                                        modeluuid = Common.ToString(firstModel["modeluuid"]),
                                        typeid = new EncryptedData()
                                        {
                                            NumericValue = typeGrp.Key,
                                            EncryptedValue = Common.Encrypt(typeGrp.Key)
                                        },
                                        sizeid = new EncryptedData()
                                        {
                                            NumericValue = sizeId,
                                            EncryptedValue = Common.Encrypt(sizeId)
                                        },
                                        iso_code = Common.ToString(firstModel["iso_code"]),
                                        description = Common.ToString(firstModel["description"]),
                                        createdat = Common.ToDateTime(firstModel["createdat"])
                                    };
                                })
                                .Where(m => m != null)
                                .ToList();

                            var sizes = allModels
                                .GroupBy(m => m.sizeid.NumericValue)
                                .Select(sizeGrp =>
                                {
                                    int sizeId = sizeGrp.Key;

                                    var sizeRow = rows.FirstOrDefault(r =>
                                        Common.ToInt(r["sizeid"]) == sizeId);

                                    if (sizeRow == null)
                                        return null;

                                    var sizeDto = new ContainerSizeDTO
                                    {
                                        sizeid = new EncryptedData()
                                        {
                                            NumericValue = sizeId,
                                            EncryptedValue = Common.Encrypt(sizeId)
                                        },
                                        sizename = Common.ToString(sizeRow["sizename"]),
                                        length = Common.ToString(sizeRow["length"]),
                                        width = Common.ToString(sizeRow["width"]),
                                        height = Common.ToString(sizeRow["height"]),
                                        sizeorderby = Common.ToInt(sizeRow["sizeorderby"]),
                                        models = sizeGrp.ToList()
                                    };

                                    if (sizeDto.models == null || sizeDto.models.Count == 0)
                                        return null;

                                    return sizeDto;
                                })
                                .Where(s => s != null)
                                .OrderBy(s => s.sizeorderby)
                                .ToList();

                            return new ContainerTypeDTO
                            {
                                containertypeid = new EncryptedData()
                                {
                                    NumericValue = typeGrp.Key,
                                    EncryptedValue = Common.Encrypt(typeGrp.Key)
                                },
                                typeuuid = Common.ToString(firstType["typeuuid"]),
                                typename = Common.ToString(firstType["typename"]),
                                typeshortname = Common.ToString(firstType["typeshortname"]),
                                createdat = Common.ToDateTime(firstType["createdat"]),
                                icon = new Icon()
                                {
                                    iconid = new EncryptedData()
                                    {
                                        NumericValue = Common.ToInt(firstType["iconid"]),
                                        EncryptedValue = Common.Encrypt(Common.ToInt(firstType["iconid"]))
                                    },
                                    icon = Common.ToString(firstType["icon"])
                                },
                                sizes = sizes ?? new List<ContainerSizeDTO>()
                            };
                        })
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return list;
        }

        public ContainerModelDTO GetContainerModelByID(int modelid)
        {
            ContainerModelDTO model = new ContainerModelDTO();
            try
            {
                using (SqlDB Db = new SqlDB(DatabaseCollection.Contrack))
                {
                    DataTable tbl = Db.GetDataTable(
                        "SELECT * FROM masters.container_model_get_byid(" +
                        "p_hubid := '" + Common.HubID + "'," +
                        "p_modelid := '" + modelid + "'" + ");");

                    if (tbl.Rows.Count > 0)
                    {
                        DataRow dr = tbl.Rows[0];

                        model = new ContainerModelDTO()
                        {
                            modelid = new EncryptedData()
                            {
                                NumericValue = Common.ToInt(dr["modelid"]),
                                EncryptedValue = Common.Encrypt(Common.ToInt(dr["modelid"]))
                            },
                            modeluuid = Common.ToString(dr["modeluuid"]),
                            sizeid = new EncryptedData()
                            {
                                NumericValue = Common.ToInt(dr["sizeid"]),
                                EncryptedValue = Common.Encrypt(Common.ToInt(dr["sizeid"]))
                            },
                            iso_code = Common.ToString(dr["iso_code"]),
                            description = Common.ToString(dr["description"]),
                            typeid = new EncryptedData()
                            {
                                NumericValue = Common.ToInt(dr["typeid"]),
                                EncryptedValue = Common.Encrypt(Common.ToInt(dr["typeid"]))
                            },
                            createdat = Common.ToDateTime(dr["createdat"])
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return model;
        }

        public ContainerModelExtendedDTO GetContainerModelByUUID(string uuid)
        {
            ContainerModelExtendedDTO model = new ContainerModelExtendedDTO();
            try
            {
                using (SqlDB Db = new SqlDB(DatabaseCollection.Contrack))
                {
                    DataTable tbl = Db.GetDataTable(
                        "SELECT * FROM masters.container_model_get_byuuid(" +
                        "p_hubid := '" + Common.HubID + "'," +
                        "p_modeluuid := '" + uuid + "'" + ");");

                    if (tbl.Rows.Count == 0)
                        return null;

                    var rows = tbl.AsEnumerable();

                    model = rows
                        .GroupBy(r => Common.ToInt(r["modelid"]))
                        .Select(g =>
                        {
                            var first = g.First();

                            return new ContainerModelExtendedDTO
                            {
                                containertypeid = new EncryptedData()
                                {
                                    NumericValue = Common.ToInt(first["typeid"]),
                                    EncryptedValue = Common.Encrypt(Common.ToInt(first["typeid"]))
                                },
                                typeuuid = Common.ToString(first["typeuuid"]),
                                typename = Common.ToString(first["typename"]),
                                typeshortname = Common.ToString(first["typeshortname"]),
                                iconid = new EncryptedData()
                                {
                                    NumericValue = Common.ToInt(first["iconid"]),
                                    EncryptedValue = Common.Encrypt(Common.ToInt(first["iconid"]))
                                },
                                icon = Common.ToString(first["icon"]),
                                sizeid = new EncryptedData()
                                {
                                    NumericValue = Common.ToInt(first["sizeid"]),
                                    EncryptedValue = Common.Encrypt(Common.ToInt(first["sizeid"]))
                                },
                                sizename = Common.ToString(first["sizename"]),
                                length = Common.ToString(first["length"]),
                                width = Common.ToString(first["width"]),
                                height = Common.ToString(first["height"]),
                                sizeorderby = Common.ToInt(first["sizeorderby"]),
                                modelid = new EncryptedData()
                                {
                                    NumericValue = Common.ToInt(first["modelid"]),
                                    EncryptedValue = Common.Encrypt(Common.ToInt(first["modelid"]))
                                },
                                modeluuid = Common.ToString(first["modeluuid"]),
                                iso_code = Common.ToString(first["iso_code"]),
                                description = Common.ToString(first["description"]),
                            };
                        })
                        .FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return model;
        }

        public List<ContainerModelExtendedDTO> GetContainerModelList(ContainerModelFilter filter)
        {
            List<ContainerModelExtendedDTO> list = new List<ContainerModelExtendedDTO>();
            try
            {
                using (SqlDB Db = new SqlDB(DatabaseCollection.Contrack))
                {
                    string filterJson = JsonConvert.SerializeObject(filter);

                    DataTable tbl = Db.GetDataTable(
                    "SELECT * FROM masters.container_model_list(" +
                    "p_hubid := '" + Common.HubID + "', " +
                    "p_filters := '" + filterJson + "'::jsonb" +
                    ");");

                    list = tbl.AsEnumerable().Select(r => new ContainerModelExtendedDTO
                    {
                        modelid = new EncryptedData
                        {
                            NumericValue = Common.ToInt(r["modelid"]),
                            EncryptedValue = Common.Encrypt(Common.ToInt(r["modelid"]))
                        },
                        modeluuid = Common.ToString(r["modeluuid"]),
                        iso_code = Common.ToString(r["iso_code"]),
                        description = Common.ToString(r["description"]),
                        createdat = Common.ToDateTime(r["createdat"]),
                        sizeid = new EncryptedData
                        {
                            NumericValue = Common.ToInt(r["sizeid"]),
                            EncryptedValue = Common.Encrypt(Common.ToInt(r["sizeid"]))
                        },
                        sizename = Common.ToString(r["sizename"]),
                        length = Common.ToString(r["length"]),
                        width = Common.ToString(r["width"]),
                        height = Common.ToString(r["height"]),
                        sizeorderby = Common.ToInt(r["sizeorderby"]),
                        containertypeid = new EncryptedData
                        {
                            NumericValue = Common.ToInt(r["typeid"]),
                            EncryptedValue = Common.Encrypt(Common.ToInt(r["typeid"]))
                        },
                        typeuuid = Common.ToString(r["typeuuid"]),
                        typename = Common.ToString(r["typename"]),
                        typeshortname = Common.ToString(r["typeshortname"]),
                        iconid = new EncryptedData
                        {
                            NumericValue = Common.ToInt(r["iconid"]),
                            EncryptedValue = Common.Encrypt(Common.ToInt(r["iconid"]))
                        },
                        icon = Common.GetIconPath(Common.ToInt(r["iconid"])),
                        AvailableCount = Common.ToInt(r["available_count"]),
                        BookedCount = Common.ToInt(r["booked_count"]),
                        BlockedCount = Common.ToInt(r["blocked_count"]),
                        RepairCount = Common.ToInt(r["damaged_count"]),
                        TotalCount = Common.ToInt(r["total_count"]),

                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }

            return list;
        }
        public List<ContainerModelExtendedDTO> GetContainerModelsByTypeSize(string typeid, string sizeid)
        {
            List<ContainerModelExtendedDTO> list = new List<ContainerModelExtendedDTO>();

            try
            {
                using (SqlDB Db = new SqlDB(DatabaseCollection.Contrack))
                {
                    DataTable tbl = Db.GetDataTable(
                            "SELECT * FROM masters.container_model_list_bytypesize(" +
                            "p_hubid := '" + Common.HubID + "', " +
                            "p_typeid := '" + Common.Decrypt(typeid) + "', " +
                            "p_sizeid := '" + Common.Decrypt(sizeid) + "'" +
                            ");"
                        );

                    list = tbl.AsEnumerable().Select(r => new ContainerModelExtendedDTO
                    {
                        modelid = new EncryptedData
                        {
                            NumericValue = Common.ToInt(r["modelid"]),
                            EncryptedValue = Common.Encrypt(Common.ToInt(r["modelid"]))
                        },
                        modeluuid = Common.ToString(r["modeluuid"]),
                        iso_code = Common.ToString(r["iso_code"]),
                        description = Common.ToString(r["description"]),
                        createdat = Common.ToDateTime(r["createdat"]),
                        sizeid = new EncryptedData
                        {
                            NumericValue = Common.ToInt(r["sizeid"]),
                            EncryptedValue = Common.Encrypt(Common.ToInt(r["sizeid"]))
                        },
                        sizename = Common.ToString(r["sizename"]),
                        length = Common.ToString(r["length"]),
                        width = Common.ToString(r["width"]),
                        height = Common.ToString(r["height"]),
                        sizeorderby = Common.ToInt(r["sizeorderby"]),
                        containertypeid = new EncryptedData
                        {
                            NumericValue = Common.ToInt(r["typeid"]),
                            EncryptedValue = Common.Encrypt(Common.ToInt(r["typeid"]))
                        },
                        typeuuid = Common.ToString(r["typeuuid"]),
                        typename = Common.ToString(r["typename"]),
                        typeshortname = Common.ToString(r["typeshortname"]),
                        iconid = new EncryptedData
                        {
                            NumericValue = Common.ToInt(r["iconid"]),
                            EncryptedValue = Common.Encrypt(Common.ToInt(r["iconid"]))
                        },
                        icon = Common.ToString(r["icon"]),
                        AvailableCount = 0,
                        BookedCount = 0,
                        RepairCount = 0,
                        TotalCount = 0
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