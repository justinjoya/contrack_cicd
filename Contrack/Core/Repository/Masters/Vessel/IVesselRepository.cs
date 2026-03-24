using System.Collections.Generic;

namespace Contrack
{
    public interface IVesselRepository
    {
        List<VesselDTO> ValidateVessel(string vesselname, string imono, string mmsino);
        Result SaveVessel(VesselDTO vessel);
        Result MoveVesselToHub(VesselAssignmentDTO vassignment);
        VesselDTO GetVesselByUUID(string uuid);
        List<VesselDTO> GetVesselList(VesselFilter filter);
        Result DeleteVessel(VesselAssignmentDTO assignment);
        List<VesselContactDTO> GetContacts(string vesselAssignmentId);
        VesselContactDTO GetContactById(string picId);
        Result SaveContact(VesselContactDTO contact);
        Result MakePrimary(string picIdInc);
        Result DeleteContact(string picIdInc);

    }
}
