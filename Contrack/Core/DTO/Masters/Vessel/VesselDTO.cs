using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Contrack
{
    public class VesselExtra
    {
        public string shortcode { get; set; } = "";
        public string color { get; set; } = "";
        public string bgcolor { get; set; } = "";
    }
    public class VesselDTO
    {
        public EncryptedData vesselid { get; set; } = new EncryptedData();
        public string vesseluuid { get; set; } = "";
        public string imono { get; set; } = "";
        public string mmsino { get; set; } = "";
        public VesselAssignmentDTO vassignment { get; set; } = new VesselAssignmentDTO();
        public VesselExtra extras { get; set; } = new VesselExtra();
        public string vesselname { get; set; } = "";
        public string vesseltypeid { get; set; } = "";
        public string subtypeid { get; set; } = "";
        public string flagcountryid { get; set; } = "";
        public string portofregistryid { get; set; } = "";
        public decimal grosstonnage { get; set; } = 0;
        public decimal nettonnage { get; set; } = 0;
        public decimal deadweight { get; set; } = 0;
        public decimal lengthoverall { get; set; } = 0;
        public decimal lengthbp { get; set; } = 0;
        public decimal breadth { get; set; } = 0;
        public decimal depth { get; set; } = 0;
        public decimal summerdraft { get; set; } = 0;
        public decimal enginepowerkw { get; set; } = 0;
        public decimal totalauxenginepowerkw { get; set; } = 0;
        public bool scrubberinstalled { get; set; } = false;
        public bool soxcompliant { get; set; } = false;
        public bool sternthrusters { get; set; } = false;
        public bool bowthrusters { get; set; } = false;
        public string specialinstructions { get; set; } = "";
        public DateTime createdat { get; set; }
        public List<VesselContactDTO> Contacts { get; set; } = new List<VesselContactDTO>();
        public int row_index { get; set; } = 0;
        public int totalnoofrows { get; set; } = 0;
        public string flagcountry { get; set; } = "";
        public string portofregistry { get; set; } = "";
        public string vesseltype { get; set; } = "";
        public string vesselsubtype { get; set; } = "";
        public string vesselpicname { get; set; } = "";
        public string vesselpicposition { get; set; } = "";
        //public AgencyDTO agency { get; set; } = new AgencyDTO();
        public string TypeEncrypted { get; set; } = "";

    }
}