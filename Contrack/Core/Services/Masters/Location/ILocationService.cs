using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contrack
{
    public interface ILocationService
    {
        void SaveLocation(Location location);
        void DisableLocation(int LocationID);
        LocationDTO GetLocationByID(int LocationID);
        LocationDTO GetLocationByUUID(string locationUUID);
        List<LocationTypeDTO> GetLocationTypes();
        List<Location> GetLocationList(LocationListFilter filter);
        List<PortGroupDTO> GetGroupedPorts();
        PortGroupDTO GetPortById(string EncryptedPortID);
    }
}