using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contrack
{
    public class LocationListFilter
    {
        [JsonIgnore]
        public string CountryID { get; set; } = "0";

        [JsonIgnore]
        public string PortID { get; set; } = "0";

        [JsonIgnore]
        public string LocationTypeID { get; set; } = "0";

        [JsonProperty("searchtext")]
        public string SearchText { get; set; } = "";
    }
}