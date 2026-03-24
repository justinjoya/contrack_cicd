using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Contrack
{
    public class VesselService : CustomException, IVesselService
    {
        public Result result = new Result();
        private readonly IVesselRepository _repo;

        public VesselService(IVesselRepository repo)
        {
            _repo = repo;
        }

        public void SaveVessel(VesselDTO vessel)
        {
            try
            {
                if (Common.Decrypt(vessel.TypeEncrypted) == 1)
                    vessel.vassignment.agencyid.EncryptedValue = Common.Encrypt(0);

                result = _repo.SaveVessel(vessel);
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage($"Unexpected error while saving vessel: {ex.Message}");
                RecordException(ex);
            }
        }

        public void DeleteVessel(VesselAssignmentDTO assignment)
        {
            try
            {
                result = _repo.DeleteVessel(assignment);
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.ToString());
                RecordException(ex);
            }
        }


        public Result MoveVesselToHub(VesselAssignmentDTO assignment)
        {
            Result result = new Result();
            try
            {
                result = _repo.MoveVesselToHub(assignment);
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.ToString());
                RecordException(ex);
            }
            return result;
        }


        public VesselDTO GetVesselByUUID(string uuid)
        {
            VesselDTO vessel = new VesselDTO();
            try
            {
                vessel = _repo.GetVesselByUUID(uuid);
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return vessel;
        }

        public List<VesselDTO> ValidateVessel(string vesselname, string imono, string mmsino)
        {
            try
            {
                return _repo.ValidateVessel(vesselname, imono, mmsino);
            }
            catch (Exception ex)
            {
                RecordException(ex);
                return new List<VesselDTO>(); // Return empty list on error
            }
        }
        public List<Vessel> GetVesselList(VesselFilter filter)
        {
            List<VesselDTO> list = new List<VesselDTO>();
            try
            {
                list = _repo.GetVesselList(filter);
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }

            return list.Select(x => new Vessel()
            {
                vessel = x,
                menus = new MasterMenus()
                {
                    edit = true // PagePermission.GetPermissions().Vendor.edit
                }
            }).ToList();
        }
        public List<VesselContactDTO> GetContacts(string vesselAssignmentId)
        {
            List<VesselContactDTO> contacts = new List<VesselContactDTO>();
            try
            {
                contacts = _repo.GetContacts(vesselAssignmentId);
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return contacts;
        }

        public VesselContactDTO GetContactById(string picId)
        {
            VesselContactDTO contact = null;
            try
            {
                contact = _repo.GetContactById(picId);
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return contact;
        }

        public Result SaveContact(VesselContactDTO contact)
        {
            Result result = new Result();
            try
            {
                result = _repo.SaveContact(contact);
            }
            catch (Exception ex)
            {
                RecordException(ex);
                result = Common.ErrorMessage(ex.Message);
            }
            return result;
        }


        public void MakePrimary(string picIdInc)
        {
            try
            {
                result = _repo.MakePrimary(picIdInc);
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage($"Unexpected error while setting primary: {ex.Message}");
                RecordException(ex);
            }
        }

        public void DeleteContact(string picIdInc)
        {
            try
            {
                result = _repo.DeleteContact(picIdInc);
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage($"Unexpected error while deleting contact: {ex.Message}");
                RecordException(ex);
            }
        }

    }
}
