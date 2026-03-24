//using MailKit.Security;
//using MimeKit;
//using System;
//using System.IO;
//using System.Linq;
//using System.Net;
//using System.Text;

//namespace Contrack
//{
//    public class EmailService : CustomException, IEmailService
//    {
//        private readonly IEmailRepository _emailRepo;
//        private readonly IPOService _poService;

//        public EmailService(IEmailRepository emailRepo, IPOService poService)
//        {
//            _emailRepo = emailRepo;
//            _poService = poService;
//        }

//        public EmailDTO PrepareEmail(string type, string refid, string hubId, string loginId)
//        {
//            EmailDTO email = new EmailDTO();
//            try
//            {
//                if (type == "PO")
//                {
//                    PurchaseOrder po = _poService.GetPOByUUID(refid, hubId);
//                    email = PreparePOMail(po, true);
//                }
//                // Implement "RFQ" here later if needed using IPurchaseIntentService
//            }
//            catch (Exception ex) { RecordException(ex); }
//            return email;
//        }

//        private EmailDTO PreparePOMail(PurchaseOrder po, bool autoemail = true)
//        {
//            EmailDTO email = new EmailDTO();
//            email.ID = po.pouuid;
//            email.AgencyID = po.PI.AgencyDetailID.NumericValue;
//            email.PIID = po.PI.PurchaseIntentID;

//            string deliverydate = string.IsNullOrEmpty(po.PI.DeliveryDate) ? "N/A" : Common.ToDateTimeString(po.PI.DeliveryDate, "MMM dd, yyyy");
//            email.Subject = $"Purchase Order {po.pocode}, {po.vendor_name}, expected delivery date: {deliverydate}";

//            try
//            {
//                string vendorEmail = _emailRepo.GetVendorEmailByDetailID(po.vendordetailid.NumericValue);
//                string[] splitparams = { ",", ";" };
//                email.EmailTo = vendorEmail.Split(splitparams, StringSplitOptions.None).Where(x => !string.IsNullOrEmpty(x)).ToList();

//                string requestername = Common.LoginName;
//                string requesterEmail = Common.Email;
//                var sb = new StringBuilder();

//                var sellamount = po.Details.Sum(y => (y.quantity * y.vendorprice + (y.quantity * y.vendorprice * y.tax / 100)));
//                string vesselName = po.PI.VesselList != null ? string.Join(", ", po.PI.VesselList.Select(x => x.Text)) : "N/A";
//                if (string.IsNullOrEmpty(vesselName)) vesselName = "N/A";

//                if (autoemail)
//                {
//                    sb.AppendLine("<!DOCTYPE html><html><head><meta charset=\"UTF-8\"><title>Purchase Order Notification</title></head>");
//                    sb.AppendLine("<body style=\"font-family:Segoe UI, sans-serif; font-size:15px; color:#2a2a2a; line-height:1.6; margin:0; padding:20px; background-color:#ffffff;\">");
//                    sb.AppendLine("<div style=\"max-width:700px; margin:0 auto;\">");
//                    sb.AppendLine($"<p><strong style=\"color:#3b3bff;\">Kind Attn: {po.vendor_name}</strong></p>");
//                    sb.AppendLine("<p>Dear Sir/Madam,</p>");
//                    sb.AppendLine($"<p>We are pleased to share the <strong>Purchase Order (PO) {po.pocode}</strong> for the goods/services as per our agreement.</p>");
//                    sb.AppendLine("<p><strong>PO Details:</strong></p>");
//                    sb.AppendLine("<div style=\"background-color:#f8fafd; border-left:5px solid #23a36c; padding:15px; border-radius:8px; margin:20px 0;\">");
//                    sb.AppendLine($"<p><strong>PO Number:</strong> {po.pocode}</p>");
//                    sb.AppendLine($"<p><strong>PO Date:</strong> {Common.ToDateTimeString(po.createdat, "MMM dd, yyyy")}</p>");
//                    sb.AppendLine($"<p><strong>Ship Name:</strong> {vesselName}</p>");
//                    sb.AppendLine($"<p><strong>Total Amount:</strong> {po.vendorcurrency} {sellamount.ToString("0.00")}</p>");
//                    sb.AppendLine($"<p><strong>Delivery Deadline:</strong> {deliverydate}</p>");
//                    sb.AppendLine($"<p><strong>Billing Address:</strong> {po.billto}</p>");
//                    sb.AppendLine("</div>");
//                    sb.AppendLine("<p><strong>Attachments:</strong></p><ul><li>PO Copy (PDF/Excel) for your reference</li></ul>");
//                    sb.AppendLine($"<p>For any clarifications, feel free to contact <strong>{requestername}</strong> at <a href=\"mailto:{requesterEmail}\">{requesterEmail}</a>.</p>");
//                    sb.AppendLine("<p>Best Regards,</p>");
//                    sb.AppendLine($"<p><strong>{po.PI.AgencyName}</strong></p></div></body></html>");
//                }
//                email.Body = sb.ToString();
//                email.Attachments.Add(new AttachmentDTO()
//                {
//                    AttachmentPath = $"/Download/PDFDownload?refid={po.pouuid}&type=po&code={po.pocode}&Download=1",
//                    Name = po.pocode + ".pdf"
//                });
//            }
//            catch (Exception ex) { RecordException(ex); }
//            return email;
//        }

//        public Result SendEmail(EmailDTO email, string hubId, string loginId, string urlSchemeAndServer)
//        {
//            email.result = Common.ErrorMessage("Cannot send email");
//            email.LogUUID = _emailRepo.SaveEmailLog(email, hubId, loginId);

//            try
//            {
//                string mail_server = "smtpout.secureserver.net";
//                string mail_user = "notification@inc2.net";
//                string mail_pass = "F7r@8!zLp2QxT9m";
//                int mail_port = 465;

//                var message = new MimeMessage();
//                message.From.Add(new MailboxAddress("Notifications", mail_user));
//                message.ReplyTo.Add(new MailboxAddress("Notifications", mail_user));
//                message.Subject = email.Subject;

//                foreach (var toGroup in email.EmailTo)
//                    foreach (var to in toGroup.Split(','))
//                        if (!string.IsNullOrWhiteSpace(to)) message.To.Add(MailboxAddress.Parse(to.Trim()));

//                foreach (var ccGroup in email.CC)
//                    foreach (var cc in ccGroup.Split(','))
//                        if (!string.IsNullOrWhiteSpace(cc)) message.Cc.Add(MailboxAddress.Parse(cc.Trim()));

//                var builder = new BodyBuilder { HtmlBody = email.IsHtml ? email.Body : null, TextBody = !email.IsHtml ? email.Body : null };

//                foreach (var attach in email.Attachments.Where(x => !string.IsNullOrEmpty(x.AttachmentPath) || !string.IsNullOrEmpty(x.PhysicalPath)))
//                {
//                    byte[] fileBytes = null;
//                    if (!string.IsNullOrEmpty(attach.PhysicalPath) && File.Exists(attach.PhysicalPath))
//                        fileBytes = File.ReadAllBytes(attach.PhysicalPath);
//                    else if (!string.IsNullOrEmpty(attach.AttachmentPath))
//                        using (WebClient web = new WebClient())
//                            fileBytes = web.DownloadData(urlSchemeAndServer + attach.AttachmentPath + "&HubID=" + Common.Encrypt(Common.ToInt(hubId)));

//                    if (fileBytes != null)
//                        builder.Attachments.Add(attach.Name.Contains('.') ? attach.Name : attach.Name + ".pdf", new MemoryStream(fileBytes));
//                }

//                message.Body = builder.ToMessageBody();

//                using (var client = new MailKit.Net.Smtp.SmtpClient())
//                {
//                    client.Timeout = 180000;
//                    client.Connect(mail_server, mail_port, SecureSocketOptions.SslOnConnect);
//                    client.Authenticate(mail_user, mail_pass);
//                    client.Send(message);
//                    client.Disconnect(true);

//                    email.result = Common.SuccessMessage("Email has been sent to " + string.Join(",", email.EmailTo) + " Successfully!");
//                    _emailRepo.UpdateEmailStatus(email.LogUUID, 2, "Email has been sent successfully!", hubId);
//                }
//            }
//            catch (Exception ex)
//            {
//                string message = ex.Message + (ex.InnerException != null ? " --> " + ex.InnerException.Message : "");
//                email.result = Common.ErrorMessage(message);
//                _emailRepo.UpdateEmailStatus(email.LogUUID, 3, message, hubId);
//            }
//            return email.result;
//        }
//    }
//}