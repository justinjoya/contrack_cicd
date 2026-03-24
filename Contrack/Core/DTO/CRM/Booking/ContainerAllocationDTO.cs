using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contrack
{
    public class ContainerAllocationDTO
    {
        public string LocationName { get; set; } = "";
        public EncryptedData LocationId { get; set; } = new EncryptedData();
        public string LocationUuid { get; set; } = "";
        public string LocationType { get; set; } = "";
        public int LocationTypeIcon { get; set; } = 0;
        public string PortName { get; set; } = "";
        public string CountryName { get; set; } = "";
        public string CountryFlag { get; set; } = "";
        public string PortCode { get; set; } = "";
        public string CountryCode { get; set; } = "";
        public List<ContainerAllocationModelDTO> Models { get; set; } = new List<ContainerAllocationModelDTO>();

    }
    public class ContainerAllocationModelDTO
    {
        public EncryptedData ReservationID { get; set; } = new EncryptedData();
        public string ContainerTypeName { get; set; } = "";
        public string ContainerSizeName { get; set; } = "";

        public EncryptedData ContainerModelId { get; set; } = new EncryptedData();
        public string ContainerModelUuid { get; set; } = "";
        public string ContainerModelIso { get; set; } = "";

        public int AvailableCount { get; set; } = 0;
        public int BlockedCount { get; set; } = 0;
        public int ComingSoon { get; set; } = 0;
        public int SelectedCount { get; set; } = 0;
    }
}