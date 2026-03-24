using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contrack
{
    public class LocationService : CustomException, ILocationService
    {
        public Result result = new Result();
        private readonly ILocationRepository _repo;
        public LocationService(ILocationRepository repo)
        {
            _repo = repo;
        }
        public void SaveLocation(Location location)
        {
            try
            {
                result = _repo.SaveLocation(location.location);
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.ToString());
                RecordException(ex);
            }
        }
        public void DisableLocation(int LocationID)
        {
            result = _repo.DisableLocation(LocationID);
        }
        public LocationDTO GetLocationByID(int LocationID)
        {
            return _repo.GetLocationByID(LocationID);
        }
        public LocationDTO GetLocationByUUID(string locationUUID)
        {
            return _repo.GetLocationByUUID(locationUUID);
        }

        public List<LocationTypeDTO> GetLocationTypes()
        {
            return _repo.GetLocationTypes();
        }
        public PortGroupDTO GetPortById(string EncryptedPortID)
        {
            int portId = Common.Decrypt(EncryptedPortID);
            if (portId > 0)
            {
                return _repo.GetPortById(portId);
            }
            return new PortGroupDTO();
        }

        public List<PortGroupDTO> GetGroupedPorts()
        {
            return _repo.GetGroupedPorts();
        }
        public List<Location> GetLocationList(LocationListFilter filter)
        {
            List<LocationDTO> dtoList = _repo.GetLocationList(filter);

            return dtoList.Select(dto => new Location()
            {
                location = dto,
                menu = new MasterMenus()
                {
                    edit = true
                }
            }).ToList();
        }
    }
}