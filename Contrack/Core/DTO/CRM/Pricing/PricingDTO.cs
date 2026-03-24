using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contrack
{
    public class PricingHeaderDTO
    {
        public EncryptedData PricingID { get; set; } = new EncryptedData();
        public string PricingUUID { get; set; } = "";
        public PortDTO POL { get; set; } = new PortDTO();
        public PortDTO POD { get; set; } = new PortDTO();
        public EncryptedData TransferType { get; set; } = new EncryptedData();
        public string TransferTypeName { get; set; } = "";
        public string Currency { get; set; } = "";
        public string TemplateNo { get; set; } = "";
        public string Description { get; set; } = "";
        public FormattedValue<DateTime> CreatedAt { get; set; } = new FormattedValue<DateTime>();
        public string CreatedByName { get; set; } = "";
        public List<PricingTypeDTO> Types { get; set; } = new List<PricingTypeDTO>();
        public int IsClone { get; set; } = 0;
        public int TypeCount { get; set; } = 0;
        public TableCounts RowCount { get; set; } = new TableCounts();
        public int ClientCount { get; set; }
    }
    public class PricingTypeDTO
    {
        public EncryptedData PricingID { get; set; } = new EncryptedData();
        public string PricingUUID { get; set; } = "";
        public EncryptedData TypeID { get; set; } = new EncryptedData();
        public string TypeUUID { get; set; } = "";
        public EncryptedData TransferType { get; set; } = new EncryptedData();
        public string TransferTypeName { get; set; } = "";
        public bool IsCurrent { get; set; } = false;
        public int IsClone { get; set; } = 0;
        public List<PricingDetailDTO> Details { get; set; } = new List<PricingDetailDTO>();
    }
    public class PricingDetailDTO
    {
        public EncryptedData TypeID { get; set; } = new EncryptedData();
        public EncryptedData DetailID { get; set; } = new EncryptedData();
        public string DetailUUID { get; set; } = "";
        public string LineItemUUID { get; set; } = "";
        public string LineItemDesc { get; set; } = "";
        public decimal Amount { get; set; } = 0;
        public EncryptedData ContainerTypeID { get; set; } = new EncryptedData();
        public EncryptedData ContainerSizeID { get; set; } = new EncryptedData();
        public string TypeSizeCombined { get; set; } = "";
        public string TypeSizeCombinedValue { get; set; } = "";
        public string TypeSizeName { get; set; } = "";
        public string FullEmpty { get; set; } = "";
        public string UOM { get; set; } = "";
        public bool IsFrightCharges { get; set; } = false;
        public string Comments { get; set; } = "";
        public int Qty { get; set; } = 0; // for quotatioon
    }

    public class PricingCustomerDTO
    {
        public EncryptedData PricingID { get; set; } = new EncryptedData();
        public EncryptedData ClientID { get; set; } = new EncryptedData();
        public string ClientName { get; set; } = "";
    }
}