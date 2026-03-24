using SelectPdf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Web;

namespace Contrack
{
    public class Html2PDFConvertor
    {
        public static void HTMLToPdf(string URL, string outpath, string footerurl, bool addPageNumbers, string agencyName)
        {
            try
            {

                GlobalProperties.LicenseKey = "U3hic2FmYnNkYHNhZ31jc2BifWJhfWpqamo=";

                PdfPageSize pageSize = (PdfPageSize)Enum.Parse(typeof(PdfPageSize),
                    "A4", true);

                PdfPageOrientation pdfOrientation =
                    (PdfPageOrientation)Enum.Parse(typeof(PdfPageOrientation),
                    "Portrait", true);

                RenderingEngine renderingEngine =
               (RenderingEngine)Enum.Parse(typeof(RenderingEngine), "Blink", true);


                int webPageWidth = 1024;

                int webPageHeight = 0;

                // instantiate a html to pdf converter object
                HtmlToPdf converter = new HtmlToPdf();

                if (!string.IsNullOrEmpty(footerurl))
                {
                    converter.Options.DisplayFooter = true;
                    converter.Footer.DisplayOnFirstPage = true;
                    converter.Footer.DisplayOnOddPages = true;
                    converter.Footer.DisplayOnEvenPages = true;
                    converter.Footer.Height = 100;

                    // add some html content to the header
                    PdfHtmlSection footerHtml = new PdfHtmlSection(footerurl);
                    footerHtml.AutoFitHeight = HtmlToPdfPageFitMode.AutoFit;
                    converter.Footer.Add(footerHtml);
                }

                // set converter options
                converter.Options.JavaScriptEnabled = true;
                converter.Options.PdfPageSize = pageSize;
                converter.Options.PdfPageOrientation = pdfOrientation;
                converter.Options.WebPageWidth = webPageWidth;
                converter.Options.WebPageHeight = webPageHeight;
                converter.Options.RenderingEngine = renderingEngine;
                converter.Options.BlinkEnginePath = HttpContext.Current.Server.MapPath("~/Includes/Chromium");

                PdfDocument doc = converter.ConvertUrl(URL);

                //if (addPageNumbers)
                //{
                //    Font titleFont = new Font("Arial", 9, FontStyle.Bold, GraphicsUnit.Point);
                //    Font detailFont = new Font("Arial", 8, FontStyle.Regular, GraphicsUnit.Point);

                //    PdfFont pdfTitleFont = doc.AddFont(titleFont);
                //    PdfFont pdfDetailFont = doc.AddFont(detailFont);

                //    Color detailsGray = System.Drawing.ColorTranslator.FromHtml("#555555");
                //    Color footerShadeColor = System.Drawing.ColorTranslator.FromHtml("#F0F0F0");

                //    Color lightBlueLineColor = System.Drawing.ColorTranslator.FromHtml("#90CAF9");

                //    for (int i = 0; i < doc.Pages.Count; i++)
                //    {
                //        PdfPage page = doc.Pages[i];
                //        float pageMargin = 40f;
                //        float pageHeight = page.PageSize.Height;
                //        float pageWidth = page.PageSize.Width;

                //        float footerHeight = 40f;
                //        float footerStartY = pageHeight - footerHeight;

                //        PdfRectangleElement backgroundShade = new PdfRectangleElement(0, footerStartY, pageWidth, footerHeight);
                //        backgroundShade.BackColor = footerShadeColor;
                //        page.Add(backgroundShade);

                //        PdfLineElement line = new PdfLineElement(0, footerStartY, pageWidth, footerStartY);
                //        line.ForeColor = lightBlueLineColor;
                //        page.Add(line);

                //        float textY = footerStartY + (footerHeight / 2) - (detailFont.Height / 2);

                //        if (!string.IsNullOrEmpty(agencyName))
                //        {
                //            PdfTextElement agencyElement = new PdfTextElement(pageMargin, textY, agencyName, pdfTitleFont);
                //            agencyElement.ForeColor = detailsGray;
                //            page.Add(agencyElement);
                //        }

                //        string pageText = $"Page {i + 1} of {doc.Pages.Count}";
                //        SizeF pageTextSize = GetTextSize(pageText, detailFont);
                //        float pageX = pageWidth - pageMargin - pageTextSize.Width;
                //        PdfTextElement pageElement = new PdfTextElement(pageX, textY, pageText, pdfDetailFont);
                //        pageElement.ForeColor = detailsGray;
                //        page.Add(pageElement);
                //    }
                //}

                doc.Save(outpath);

                doc.Close();


            }
            catch (Exception ex)
            {
                Common.WriteLog("Html to PDF :" + ex.ToString());
            }
        }
        public static string HTMLToPdf2(string URL, string outpath, string footerurl, bool addPageNumbers, string agencyName)
        {
            string output = "";
            try
            {

                GlobalProperties.LicenseKey = "U3hic2FmYnNkYHNhZ31jc2BifWJhfWpqamo=";

                PdfPageSize pageSize = (PdfPageSize)Enum.Parse(typeof(PdfPageSize),
                    "A4", true);

                PdfPageOrientation pdfOrientation =
                    (PdfPageOrientation)Enum.Parse(typeof(PdfPageOrientation),
                    "Portrait", true);

                RenderingEngine renderingEngine =
               (RenderingEngine)Enum.Parse(typeof(RenderingEngine), "Blink", true);


                int webPageWidth = 1024;

                int webPageHeight = 0;

                // instantiate a html to pdf converter object
                HtmlToPdf converter = new HtmlToPdf();

                if (!string.IsNullOrEmpty(footerurl))
                {
                    converter.Options.DisplayFooter = true;
                    converter.Footer.DisplayOnFirstPage = true;
                    converter.Footer.DisplayOnOddPages = true;
                    converter.Footer.DisplayOnEvenPages = true;
                    converter.Footer.Height = 100;

                    // add some html content to the header
                    PdfHtmlSection footerHtml = new PdfHtmlSection(footerurl);
                    footerHtml.AutoFitHeight = HtmlToPdfPageFitMode.AutoFit;
                    converter.Footer.Add(footerHtml);
                }

                // set converter options
                converter.Options.JavaScriptEnabled = true;
                converter.Options.PdfPageSize = pageSize;
                converter.Options.PdfPageOrientation = pdfOrientation;
                converter.Options.WebPageWidth = webPageWidth;
                converter.Options.WebPageHeight = webPageHeight;
                converter.Options.RenderingEngine = renderingEngine;
                converter.Options.BlinkEnginePath = HttpContext.Current.Server.MapPath("~/Includes/Chromium");

                PdfDocument doc = converter.ConvertUrl(URL);

                if (addPageNumbers)
                {
                    Font titleFont = new Font("Arial", 9, FontStyle.Bold, GraphicsUnit.Point);
                    Font detailFont = new Font("Arial", 8, FontStyle.Regular, GraphicsUnit.Point);

                    PdfFont pdfTitleFont = doc.AddFont(titleFont);
                    PdfFont pdfDetailFont = doc.AddFont(detailFont);

                    Color detailsGray = System.Drawing.ColorTranslator.FromHtml("#555555");
                    Color footerShadeColor = System.Drawing.ColorTranslator.FromHtml("#F0F0F0");

                    Color lightBlueLineColor = System.Drawing.ColorTranslator.FromHtml("#90CAF9");

                    for (int i = 0; i < doc.Pages.Count; i++)
                    {
                        PdfPage page = doc.Pages[i];
                        float pageMargin = 40f;
                        float pageHeight = page.PageSize.Height;
                        float pageWidth = page.PageSize.Width;

                        float footerHeight = 40f;
                        float footerStartY = pageHeight - footerHeight;

                        PdfRectangleElement backgroundShade = new PdfRectangleElement(0, footerStartY, pageWidth, footerHeight);
                        backgroundShade.BackColor = footerShadeColor;
                        page.Add(backgroundShade);

                        PdfLineElement line = new PdfLineElement(0, footerStartY, pageWidth, footerStartY);
                        line.ForeColor = lightBlueLineColor;
                        page.Add(line);

                        float textY = footerStartY + (footerHeight / 2) - (detailFont.Height / 2);

                        if (!string.IsNullOrEmpty(agencyName))
                        {
                            PdfTextElement agencyElement = new PdfTextElement(pageMargin, textY, agencyName, pdfTitleFont);
                            agencyElement.ForeColor = detailsGray;
                            page.Add(agencyElement);
                        }

                        string pageText = $"Page {i + 1} of {doc.Pages.Count}";
                        SizeF pageTextSize = GetTextSize(pageText, detailFont);
                        float pageX = pageWidth - pageMargin - pageTextSize.Width;
                        PdfTextElement pageElement = new PdfTextElement(pageX, textY, pageText, pdfDetailFont);
                        pageElement.ForeColor = detailsGray;
                        page.Add(pageElement);
                    }
                }

                doc.Save(outpath);

                doc.Close();


            }
            catch (Exception ex)
            {
                output = ex.ToString();
            }
            return output;
        }

        private static SizeF GetTextSize(string text, Font font)
        {
            using (Bitmap bmp = new Bitmap(1, 1))
            using (Graphics g = Graphics.FromImage(bmp))
            {
                return g.MeasureString(text, font);
            }
        }
    }
}