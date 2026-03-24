using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Contrack
{
    public class ContainerSizeRepository : CustomException, IContainerSizeRepository
    {
        public List<ContainerSizeDTO> GetContainerSizes()
        {
            List<ContainerSizeDTO> list = new List<ContainerSizeDTO>();
            try
            {
                using (SqlDB Db = new SqlDB(DatabaseCollection.Contrack))
                {
                    DataTable tbl = Db.GetDataTable(
                        "SELECT * FROM masters.container_size_list();"
                    );

                    list = (from DataRow dr in tbl.Rows
                            select new ContainerSizeDTO()
                            {
                                sizeid = new EncryptedData()
                                {
                                    NumericValue = Common.ToInt(dr["sizeid"]),
                                    EncryptedValue = Common.Encrypt(Common.ToInt(dr["sizeid"]))
                                },
                                sizename = Common.ToString(dr["sizename"]),
                                sizeorderby = Common.ToInt(dr["orderby"]),
                                length = Common.ToString(dr["length"]), 
                                width = Common.ToString(dr["width"]),
                                height = Common.ToString(dr["height"]),
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
