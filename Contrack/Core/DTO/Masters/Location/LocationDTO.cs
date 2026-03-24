using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contrack
{
    public class LocationDTO
    {
        public EncryptedData LocationID { get; set; } = new EncryptedData();
        public string LocationUUID { get; set; } = "";
        public EncryptedData CountryID { get; set; } = new EncryptedData();
        public EncryptedData PortID { get; set; } = new EncryptedData();
        public string LocationCode { get; set; } = "";
        public string LocationName { get; set; } = "";
        public LocationTypeDTO LocationType { get; set; } = new LocationTypeDTO();
        public string Address { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Email { get; set; } = "";
        public DateTime CreatedAt { get; set; } = new DateTime();
        public int AvailableCount { get; set; } = 0;
        public int BookedCount { get; set; } = 0;
        public int BlockedCount { get; set; } = 0;
        public int DamagedCount { get; set; } = 0;
        public int TotalCount { get; set; } = 0;
        public string CountryName { get; set; } = "";
        public string CountryFlag { get; set; } = "";
        public string PortName { get; set; } = "";
        public string PortCode { get; set; } = "";
        public int totalnoofrows { get; set; }
    }


}