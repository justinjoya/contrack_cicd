using System;

namespace Contrack
{
    public class PurchaseOrderListDTO
    {
        public TableCounts rowcount { get; set; } = new TableCounts();
        public EncryptedData poid { get; set; } = new EncryptedData();
        public string pouuid { get; set; } = "";
        public string pocode { get; set; } = "";
        public string vendor_name { get; set; } = "";
        public string agencyname { get; set; } = "";
        public long no_of_items { get; set; } = 0;
        public decimal total_amount { get; set; } = 0;
        public string currency { get; set; } = "USD";
        public FormattedValue<DateTime> issuedate { get; set; } = FormatConvertor.ToDateTimeFormat(DateTime.MinValue);
        public string createdby_fullname { get; set; } = "";
        public FormattedValue<int> status { get; set; } = new FormattedValue<int>();
        public bool isdirectpo { get; set; } = true;
    }

}