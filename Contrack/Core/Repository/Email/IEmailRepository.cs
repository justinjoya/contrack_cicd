using System.Collections.Generic;

namespace Contrack
{
    public interface IEmailRepository
    {
        string SaveEmailLog(EmailDTO email, string hubId, string loginId);
        void UpdateEmailStatus(string logUuid, int statusId, string message, string hubId);
        List<string> GetEmails(string loginId);
        Result AddEmail(string loginId, string email);
        string GetVendorEmailByDetailID(int detailId);
    }
}