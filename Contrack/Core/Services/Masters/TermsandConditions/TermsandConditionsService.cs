using System;
using System.Collections.Generic;
using System.Linq;
namespace Contrack
{
    public class TermsandConditionsService : CustomException, ITermsandConditionsService
    {
        private readonly ITermsandConditionsRepository _repo;
        public Result result = new Result();
        public TermsandConditionsService(ITermsandConditionsRepository repo)
        {
            _repo = repo;
        }
        public void SaveTermsandConditions(TermsandConditionsDTO termsandconditions)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(termsandconditions.TermsText))
                {
                    result = Common.ErrorMessage("Please enter the terms and conditions.");
                }
                else
                {
                    if (Common.Decrypt(termsandconditions.TypeEncrypted) == 1)
                        termsandconditions.agency.agencyid.EncryptedValue = Common.Encrypt(0);

                    var agencyid = Common.Decrypt(termsandconditions.agency.agencyid.EncryptedValue);
                    var termsUuid = Common.GetUUID(termsandconditions.TermUuid);
                    result = _repo.SaveTermsandConditions(termsandconditions);
                }
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.Message);
                RecordException(ex);
            }
        }
        public void DeleteTermsandConditions(string TermUuid)
        {
            try
            {
                result = _repo.DeleteTermsandConditions(TermUuid);
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.Message);
                RecordException(ex);
            }
        }

        public TermsandConditionsDTO GetTermAndConditionsByUUID(string TermUuid)
        {
            return _repo.GetTermAndConditionsByUUID(TermUuid);
        }

        public List<TermsandConditions> GetTermsAndConditionsList(string search)
        {
            List<TermsandConditionsDTO> list = _repo.GetTermsAndConditionsList();
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                if (!string.IsNullOrEmpty(search))
                {
                    search = search.ToLower();
                    list = list.Where(x => x.Type.ToLower().Contains(search)
                    || x.HubName.ToLower().Contains(search)
                    || x.agency.agencyname.ToLower().Contains(search)
                    || x.UserName.ToLower().Contains(search)).ToList();
                }
            }
            return list.Select(x => new TermsandConditions()
            {
                termsandConditions = x,
                menu = new MasterMenus()
                {
                    edit = true
                }
            }).ToList();
        }
        public TermsandConditionsDTO GetTermsAndConditionsbyTypeandAgency(string Type, string Agency)
        {
            return _repo.GetTermsAndConditionsbyTypeandAgency(Type, Agency);
        }
    }
}
