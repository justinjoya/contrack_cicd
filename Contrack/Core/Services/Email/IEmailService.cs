using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
namespace Contrack
{
    public interface IEmailService
    {
        EmailDTO PrepareEmail(string type, string refid, string hubId, string loginId);
        Result SendEmail(EmailDTO email, string hubId, string loginId, string urlSchemeAndServer);
    }
}