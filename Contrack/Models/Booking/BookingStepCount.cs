using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Contrack
{

    public class BookingStepCount
    {
        public string bookingno { get; set; } = "";
        public string bookinguuid { get; set; } = "";
        public string document_type { get; set; } = "";
        public int count { get; set; } = 0;

        public static List<BookingStepCount> GetBookingCount(string refid)
        {
            List<BookingStepCount> result = new List<BookingStepCount>();
            try
            {
                using (SqlDB Db = new SqlDB(DatabaseCollection.Contrack))
                {
                    DataTable tbl = Db.GetDataTable("SELECT * FROM  booking.booking_step_count('" + refid + "','" + Common.HubID + "');");
                    result = (from DataRow dr in tbl.Rows
                              select new BookingStepCount()
                              {
                                  count = Common.ToInt(dr["count"]),
                                  document_type = Common.ToString(dr["document_type"]),
                                  bookingno = Common.ToString(dr["bookingno"]),
                                  bookinguuid = Common.ToString(dr["bookinguuid"]),
                              }).ToList();
                }
            }
            catch (Exception ex)
            { }
            return result;
        }
    }
}