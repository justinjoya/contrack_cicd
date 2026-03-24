using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contrack
{
    public interface ILocationRepository
    {
        Result SaveLocation(LocationDTO location);
        Result DisableLocation(int LocationID); // Soft Delete
        LocationDTO GetLocationByID(int LocationID);
        LocationDTO GetLocationByUUID(string locationUUID);
        List<LocationTypeDTO> GetLocationTypes();
        List<LocationDTO> GetLocationList(LocationListFilter filter);
        List<PortGroupDTO> GetGroupedPorts(string search = "");
        PortGroupDTO GetPortById(int PortID);
    }
}