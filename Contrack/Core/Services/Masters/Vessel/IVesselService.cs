using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contrack
{
    public interface IVesselService
    {
        List<VesselDTO> ValidateVessel(string vesselname, string imono, string mmsino);
        void SaveVessel(VesselDTO vessel);
       Result  MoveVesselToHub(VesselAssignmentDTO assignment);
        VesselDTO GetVesselByUUID(string uuid);
        List<Vessel> GetVesselList(VesselFilter filter);
        void DeleteVessel(VesselAssignmentDTO assignment);
        List<VesselContactDTO> GetContacts(string vesselAssignmentId);
        VesselContactDTO GetContactById(string picId);
        Result SaveContact(VesselContactDTO contact);
        void MakePrimary(string picIdInc);
        void DeleteContact(string picIdInc);
    }
}