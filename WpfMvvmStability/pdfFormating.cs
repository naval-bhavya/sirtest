using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Win32;
using System.IO;
using System.Reflection;

namespace WpfMvvmStability
{
    class pdfFormating : iTextSharp.text.pdf.PdfPageEventHelper
    {
        protected Font footer
        {
            get
            {
                Color grey = new Color(128, 128, 128);
                Font font = FontFactory.GetFont("Arial", 9, Font.COURIER, grey);
                return font;
            }
        }

        public override void OnEndPage(PdfWriter writer, Document doc)
        {  
            // code for Rectangel border
            //.......................................
            base.OnEndPage(writer, doc);
            var content = writer.DirectContent;
            var pageBorderRect = new Rectangle(doc.PageSize);
            pageBorderRect.Left += doc.LeftMargin;
            pageBorderRect.Right -= doc.RightMargin;
            pageBorderRect.Top -= doc.TopMargin;
            pageBorderRect.Bottom += doc.BottomMargin;
            content.SetColorStroke(Color.BLACK);
            content.Rectangle(pageBorderRect.Left, pageBorderRect.Bottom, pageBorderRect.Width, pageBorderRect.Height);
            content.Stroke();
            //.....................................

            Color grey = new Color(128, 128, 128);
            iTextSharp.text.Font font = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, grey);
            //code for footer.....................
            PdfPTable footerTbl = new PdfPTable(1);
            footerTbl.TotalWidth = doc.PageSize.Width;
           
            Chunk myFooter = new Chunk("Page " + (doc.PageNumber), FontFactory.GetFont(FontFactory.HELVETICA_OBLIQUE, 8, grey));
            PdfPCell footer = new PdfPCell(new Phrase(myFooter));
            footer.Border = iTextSharp.text.Rectangle.NO_BORDER;
            footer.HorizontalAlignment = Element.ALIGN_CENTER;
            footerTbl.AddCell(footer);
            footerTbl.WriteSelectedRows(0, -1, 6, 39, writer.DirectContent);
            //...................................

            //code for watermark.................
            try
            {
                string imagePath = ResolveWatermarkPath();

                if (File.Exists(imagePath))
                {
                    iTextSharp.text.Image watermark = iTextSharp.text.Image.GetInstance(imagePath);
                    PdfContentByte canvas = writer.DirectContentUnder;
                    watermark.SetAbsolutePosition(doc.PageSize.Width / 2 - 190, doc.PageSize.Height / 2 - 140);
                    watermark.ScaleToFit(380, 380);
                    canvas.SaveState();
                    PdfGState state = new PdfGState();
                    state.FillOpacity = 0.13f;
                    canvas.SetGState(state);
                    canvas.AddImage(watermark);
                    canvas.RestoreState();
                }
            }
            catch { }
            //....................................

            Rectangle pageSize = doc.PageSize;

            string timeNew = "Time " + DateTime.Now.ToString("HH:mm:ss");
           

           // string date = "StabilityP15B Report P15B";
            Paragraph header = new Paragraph(timeNew, FontFactory.GetFont(FontFactory.TIMES, 8, iTextSharp.text.Font.BOLD, Color.GRAY));
            header.Alignment = Element.ALIGN_LEFT;
            PdfPTable headerTbl = new PdfPTable(1);
            headerTbl.TotalWidth = doc.PageSize.Width - 450;
            headerTbl.HorizontalAlignment = 100;
            PdfPCell cell1 = new PdfPCell(header);
            cell1.HorizontalAlignment = 100;
            cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell1.Border = 0;
            cell1.PaddingLeft = 10;
            cell1.BackgroundColor = Color.WHITE;
            headerTbl.AddCell(cell1);
          
         
            string date1 = DateTime.Now.ToString("dd-MM-yyyy");
            string Datenew = "Date " + date1;

            Paragraph header1 = new Paragraph(Datenew, FontFactory.GetFont(FontFactory.TIMES, 8, iTextSharp.text.Font.BOLD, Color.GRAY));
            header1.Alignment = Element.ALIGN_RIGHT;
            PdfPCell cell2 = new PdfPCell(header1);
            cell2.Border = 0;
            cell2.PaddingLeft = 10;
            cell2.VerticalAlignment = Element.ALIGN_TOP;
            cell2.HorizontalAlignment = 0;
            cell2.BackgroundColor = Color.WHITE;
            headerTbl.AddCell(cell2);
            headerTbl.WriteSelectedRows(0, 2, pageSize.GetLeft(45), pageSize.GetTop(31), writer.DirectContent);
            if (doc.PageNumber == 2) { doc.Add(new iTextSharp.text.Paragraph("  ")); doc.Add(new iTextSharp.text.Paragraph("  ")); }
           
         
        }

        private static string ResolveWatermarkPath()
        {
            string baseDir = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string[] candidates = new[]
            {
                System.IO.Path.Combine(baseDir, "Assets", "Images", "watermark_light.png"),
                System.IO.Path.Combine(baseDir, "Images", "watermark_light.png"),
                System.IO.Path.Combine(baseDir, "..", "Assets", "Images", "watermark_light.png"),
                System.IO.Path.Combine(baseDir, "..", "..", "Assets", "Images", "watermark_light.png"),
                System.IO.Path.Combine(baseDir, "..", "..", "..", "Assets", "Images", "watermark_light.png"),
                System.IO.Path.Combine(baseDir, "..", "..", "..", "..", "Assets", "Images", "watermark_light.png"),
                System.IO.Path.Combine(baseDir, "Images", "Watermark.png")
            };

            foreach (string candidate in candidates)
            {
                string fullPath = System.IO.Path.GetFullPath(candidate);
                if (File.Exists(fullPath))
                {
                    return fullPath;
                }
            }

            return string.Empty;
        }

       
    }

}
