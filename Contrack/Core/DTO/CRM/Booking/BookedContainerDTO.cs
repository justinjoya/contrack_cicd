using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contrack
{
    public class ShipmentConfirmationDTO
    {
        public string BookingUUID { get; set; } = "";
        public string ContainerUuid { get; set; } = "";
        public string Stamp { get; set; } = "";
        public decimal Weight { get; set; } = 0;
    }
    public class BookedContainerDTO
    {
        public int RowIndex { get; set; } = 0;
        public long TotalCount { get; set; } = 0;
        public EncryptedData ContainerId { get; set; } = new EncryptedData();
        public bool IsChecked { get; set; } = false;
        public string ContainerUuid { get; set; } = "";
        public string ContainerNo { get; set; } = "";

        public string IsoCode { get; set; } = "";
        public string ContainerTypeName { get; set; } = "";
        public string ContainerSizeName { get; set; } = "";

        public int OperatorCode { get; set; } = 0;
        public bool IsDamaged { get; set; } = false;

        public string VoyageUuid { get; set; } = "";
        public string VoyageNumber { get; set; } = "";
        public string VesselName { get; set; } = "";

        public string PolPortCode { get; set; } = "";
        public string PolPortName { get; set; } = "";
        public string PolCountryCode { get; set; } = "";
        public string PolCountryName { get; set; } = "";
        public string PolCountryFlag { get; set; } = "";

        public string PodPortCode { get; set; } = "";
        public string PodPortName { get; set; } = "";
        public string PodCountryCode { get; set; } = "";
        public string PodCountryName { get; set; } = "";
        public string PodCountryFlag { get; set; } = "";

        public string CurrentLocationName { get; set; } = "";

        public string CurrentPortName { get; set; } = "";
        public string CurrentCountryName { get; set; } = "";
        public string CurrentCountryFlag { get; set; } = "";
        public string MoveName { get; set; } = "";
        public int MoveIconID { get; set; } = 0;
        public string LastMoveDate { get; set; } = "";
        public bool IsEmpty { get; set; } = true;

        public int ContainerTypeID { get; set; } = 0;
        public int ContainerSizeID { get; set; } = 0;

        public string Stamp { get; set; } = "";
        public decimal Weight { get; set; } = 0;

    }
}