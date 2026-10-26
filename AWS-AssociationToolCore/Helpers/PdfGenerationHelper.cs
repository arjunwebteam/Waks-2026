using iText.Html2pdf;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout.Font;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.IO;
using Path = System.IO.Path;

namespace AWS_AssociationToolCore.Helpers
{
    public class PdfGenerationHelper
    {
        /// <summary>
        /// Generates PDF bytes from Razor view for MULTIPLE tickets (3 per page)
        /// </summary>
        public static byte[] GeneratePdfBytesFromViewMultipleTickets(
            Controller controller,
            string viewName,
            List<AWS_AssociationToolCore.Models.TicketPdfViewModel> ticketModels,
            IWebHostEnvironment env)
        {
            try
            {
                string htmlContent = RenderViewToString(controller, viewName, ticketModels);

                using (MemoryStream ms = new MemoryStream())
                {
                    ConverterProperties converterProperties = new ConverterProperties();

                    // ✅ Core way
                    string baseUri = env.WebRootPath + Path.DirectorySeparatorChar;
                    converterProperties.SetBaseUri(baseUri);

                    // Font Provider
                    FontProvider fontProvider = new FontProvider();
                    fontProvider.AddStandardPdfFonts();
                    fontProvider.AddSystemFonts();
                    converterProperties.SetFontProvider(fontProvider);

                    using (PdfWriter writer = new PdfWriter(ms))
                    {
                        writer.SetCloseStream(false);
                        using (PdfDocument pdf = new PdfDocument(writer))
                        {
                            // A4 Portrait
                            pdf.SetDefaultPageSize(PageSize.A4);
                            HtmlConverter.ConvertToPdf(htmlContent, pdf, converterProperties);
                        }
                    }
                    return ms.ToArray();
                }
            }
            catch (Exception ex)
            {
                string errorMsg = $"PDF Generation Error: {ex.Message}";
                if (ex.InnerException != null)
                    errorMsg += $" | Inner Exception: {ex.InnerException.Message}";
                throw new Exception(errorMsg, ex);
            }
        }

        /// <summary>
        /// Generates PDF bytes from Razor view (single ticket)
        /// </summary>
        public static byte[] GeneratePdfBytesFromView(
            Controller controller,
            string viewName,
            object model,
            IWebHostEnvironment env)  // ✅ Added env parameter
        {
            try
            {
                string htmlContent = RenderViewToString(controller, viewName, model);

                using (MemoryStream ms = new MemoryStream())
                {
                    ConverterProperties converterProperties = new ConverterProperties();

                    // ✅ Core way
                    string baseUri = env.WebRootPath + Path.DirectorySeparatorChar;
                    converterProperties.SetBaseUri(baseUri);

                    // Font Provider
                    FontProvider fontProvider = new FontProvider();
                    fontProvider.AddStandardPdfFonts();
                    fontProvider.AddSystemFonts();
                    converterProperties.SetFontProvider(fontProvider);

                    using (PdfWriter writer = new PdfWriter(ms))
                    {
                        writer.SetCloseStream(false);
                        using (PdfDocument pdf = new PdfDocument(writer))
                        {
                            // Landscape format for single ticket
                            PageSize customPageSize = new PageSize(850, 300);
                            pdf.SetDefaultPageSize(customPageSize);
                            HtmlConverter.ConvertToPdf(htmlContent, pdf, converterProperties);
                        }
                    }
                    return ms.ToArray();
                }
            }
            catch (Exception ex)
            {
                string errorMsg = $"PDF Generation Error: {ex.Message}";
                if (ex.InnerException != null)
                    errorMsg += $" | Inner Exception: {ex.InnerException.Message}";
                throw new Exception(errorMsg, ex);
            }
        }

        /// <summary>
        /// Merges multiple PDF byte arrays into a single PDF
        /// </summary>
        public static byte[] MergePdfs(List<byte[]> pdfBytesList)
        {
            if (pdfBytesList == null || pdfBytesList.Count == 0)
                throw new ArgumentException("PDF list cannot be null or empty");

            using (MemoryStream mergedStream = new MemoryStream())
            {
                using (PdfWriter writer = new PdfWriter(mergedStream))
                {
                    writer.SetCloseStream(false);
                    using (PdfDocument mergedDoc = new PdfDocument(writer))
                    {
                        foreach (byte[] pdfBytes in pdfBytesList)
                        {
                            if (pdfBytes == null || pdfBytes.Length == 0)
                                continue;

                            using (MemoryStream sourceStream = new MemoryStream(pdfBytes))
                            {
                                using (PdfDocument sourceDoc = new PdfDocument(new PdfReader(sourceStream)))
                                {
                                    int numberOfPages = sourceDoc.GetNumberOfPages();
                                    sourceDoc.CopyPagesTo(1, numberOfPages, mergedDoc);
                                }
                            }
                        }
                    }
                }
                return mergedStream.ToArray();
            }
        }

        /// <summary>
        /// Renders a Razor view to HTML string - ✅ ASP.NET Core version
        /// </summary>
        private static string RenderViewToString(Controller controller, string viewName, object model)
        {
            if (controller == null)
                throw new ArgumentNullException(nameof(controller));

            var httpContext = controller.HttpContext;
            var actionContext = new ActionContext(
                httpContext,
                controller.RouteData,
                controller.ControllerContext.ActionDescriptor
            );

            var viewEngine = controller.HttpContext.RequestServices
                .GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;

            var viewResult = viewEngine.FindView(actionContext, viewName, false);

            if (!viewResult.Success)
                throw new Exception($"View '{viewName}' not found.");

            using (var sw = new StringWriter())
            {
                var viewDictionary = new ViewDataDictionary(
                    new Microsoft.AspNetCore.Mvc.ModelBinding.EmptyModelMetadataProvider(),
                    new Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary())
                {
                    Model = model
                };

                var viewContext = new ViewContext(
                    actionContext,
                    viewResult.View,
                    viewDictionary,
                    new TempDataDictionary(httpContext,
                        controller.HttpContext.RequestServices
                            .GetService(typeof(ITempDataProvider)) as ITempDataProvider),
                    sw,
                    new HtmlHelperOptions()
                );

                viewResult.View.RenderAsync(viewContext).GetAwaiter().GetResult();
                return sw.ToString();
            }
        }

        /// <summary>
        /// Generates PDF from Razor view and saves to folder
        /// </summary>
        public static string GeneratePdfFromView(
            Controller controller,
            string viewName,
            object model,
            string fileName,
            string savePath,
            IWebHostEnvironment env)  // ✅ Added env parameter
        {
            try
            {
                byte[] pdfBytes = GeneratePdfBytesFromView(controller, viewName, model, env);

                if (!Directory.Exists(savePath))
                    Directory.CreateDirectory(savePath);

                string filePath = Path.Combine(savePath, fileName);
                File.WriteAllBytes(filePath, pdfBytes);

                return filePath;
            }
            catch (Exception ex)
            {
                throw new Exception($"PDF Save Error: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Generates ticket number suffix (st, nd, rd, th)
        /// </summary>
        public static string GetDaySuffix(int day)
        {
            switch (day)
            {
                case 1: case 21: case 31: return "ST";
                case 2: case 22: return "ND";
                case 3: case 23: return "RD";
                default: return "TH";
            }
        }
    }
}