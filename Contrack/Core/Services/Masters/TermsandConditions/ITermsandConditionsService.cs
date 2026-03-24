using System.Collections.Generic;

namespace Contrack
{
    public interface ITermsandConditionsService
    {
        void SaveTermsandConditions(TermsandConditionsDTO termsandconditions);
        TermsandConditionsDTO GetTermAndConditionsByUUID(string TermUuid);
        List<TermsandConditions> GetTermsAndConditionsList(string search);
        void DeleteTermsandConditions(string TermUuid);
        TermsandConditionsDTO GetTermsAndConditionsbyTypeandAgency(string Type, string Agency);
    }
}
