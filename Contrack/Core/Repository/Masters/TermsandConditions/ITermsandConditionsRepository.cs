using System.Collections.Generic;

namespace Contrack
{
    public interface ITermsandConditionsRepository
    {
        Result SaveTermsandConditions(TermsandConditionsDTO termsandconditions);
        TermsandConditionsDTO GetTermAndConditionsByUUID(string TermUuid);
        List<TermsandConditionsDTO> GetTermsAndConditionsList();
        Result DeleteTermsandConditions(string TermUuid);
        TermsandConditionsDTO GetTermsAndConditionsbyTypeandAgency(string Type, string Agency);

    }
}
