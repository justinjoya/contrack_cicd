using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Linq;
using System.Web;

namespace Contrack
{
    public class TrackingDownload
    {
        public static string TrackingListDownload(TrackingFilterPage filter, string filename)
        {
            string filepath = string.Empty;
            try
            {
                string outpath = Common.DocumentsFolder;
                if (!Directory.Exists(outpath))
                    Directory.CreateDirectory(outpath);

                filepath = Path.Combine(Common.DocumentsFolder, filename);
                filter.noofrows = -1;
                ITrackingRepository repo = new TrackingRepository();
                TrackingService _service = new TrackingService(repo);
                List<TrackingListDTO> list = _service.GetTrackingList(filter);
                DataTable tbl = TrackingToDataTable(list);

                DataSet dss = new DataSet();
                dss.Tables.Add(tbl);
                Excel exls = new Excel();
                exls.DataTable_To_Excel(dss, filepath);
            }
            catch (Exception ex)
            {
                //throw new Exception("Export Failed: " + ex.Message);
            }
            return filepath;
        }
        private static DataTable TrackingToDataTable(List<TrackingListDTO> list)
        {
            DataTable tbl_export = new DataTable();

            tbl_export.Columns.Add("Move");
            tbl_export.Columns.Add("Location");
            tbl_export.Columns.Add("Port Code");
            tbl_export.Columns.Add("Container No");
            tbl_export.Columns.Add("Size/Type");
            tbl_export.Columns.Add("Status");
            tbl_export.Columns.Add("Damage");
            tbl_export.Columns.Add("Booking No");
            tbl_export.Columns.Add("Customer");
            tbl_export.Columns.Add("Next Activity");
            tbl_export.Columns.Add("Next Location");

            if (list != null)
            {
                foreach (var item in list)
                {
                    tbl_export.Rows.Add(
                        item.movesname,
                        item.locationname,
                        item.location_portcode,
                        item.containerno,
                        item.containersizetype,
                        item.isempty ? "Empty" : "Full",
                        item.isdamaged ? "Yes" : "No",
                        item.bookingno,
                        item.customername,
                        item.nextmovename,
                        item.nextlocationname
                    );
                }
            }
            return tbl_export;
        }
    }
}