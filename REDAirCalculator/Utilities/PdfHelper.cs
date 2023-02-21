using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using HtmlAgilityPack;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
using Umbraco.Web.PublishedModels;
using Image = MigraDoc.DocumentObjectModel.Shapes.Image;

namespace REDAirCalculator.Utilities
{
    public static class PdfHelper
    {
        public static Unit margins = "0.6cm";

        public static Unit cellPadding = "0.3cm";
        public static Unit leftTableIndent = "0.1cm";

        public static Color ligthGray = new Color(235, 235, 235);
        public static Color white = new Color(255, 255, 255);
        public static Color red = new Color(212, 0, 30);
        public static Color pantoneCoolGrey = new Color(99, 102, 106);

        // create pdf document and set settings
        public static Document GetDocument(string title, dynamic formData)
        {
            Document document = new Document();
            document.DefaultPageSetup.LeftMargin = margins;
            document.DefaultPageSetup.TopMargin = margins;
            document.DefaultPageSetup.BottomMargin = margins;
            document.DefaultPageSetup.RightMargin = margins;

            document.Info.Title = title;
            if (!string.IsNullOrEmpty(formData.ProjectDescription))
            {
                document.Info.Subject = formData.ProjectDescription;
            }
            if (!string.IsNullOrEmpty(formData.CustomerName))
            {
                document.Info.Author = formData.CustomerName;
            }

            return document;
        }

        public static void GetLogo(Section section, string iconPath)
        {
            Paragraph paragraph = section.AddParagraph();
            paragraph.Format.Alignment = ParagraphAlignment.Left;
            paragraph.Format.Shading.Color = red;

            // generate path for image file
            string logoName = "print-logo";
            string logoNameWithExt = logoName + ".png";
            string logoPath = Path.Combine(iconPath, logoNameWithExt);

            // imitate left indent of logo
            paragraph.AddFormattedText("-");
            paragraph.Format.Font.Color = red;

            Image logo = paragraph.AddImage(logoPath);
            logo.Width = Unit.FromCentimeter(3.5);
        }

        public static string[] GetCharsToRemove()
        {
            return new[] { "<p>", "</p>", "<span>", "</span>", "<em>", "</em>" };
        }

        public static string[] GetSentenceSelimiter()
        {
            return new[] { "<br>" };
        }

        public static List<Style> GetCommonStyles(Document document)
        {
            Style cellStyle = document.Styles.AddStyle("cellStyle", "Normal");
            cellStyle.ParagraphFormat.Borders.Distance = PdfHelper.cellPadding;
            cellStyle.ParagraphFormat.LeftIndent = PdfHelper.leftTableIndent;

            Style headerCellStyle = document.Styles.AddStyle("headerCellStyle", "Normal");
            headerCellStyle.ParagraphFormat.Borders.Distance = PdfHelper.cellPadding;
            headerCellStyle.ParagraphFormat.LeftIndent = PdfHelper.leftTableIndent;
            headerCellStyle.Font.Bold = true;
            headerCellStyle.Font.Color = PdfHelper.white;

            Style footerSectionStyle = document.Styles.AddStyle("footerSectionStyle", "Normal");
            footerSectionStyle.ParagraphFormat.Borders.Distance = PdfHelper.cellPadding;
            footerSectionStyle.ParagraphFormat.Font.Bold = false;

            Style amountStyle = document.Styles.AddStyle("amountStyle", "Normal");
            amountStyle.ParagraphFormat.Borders.Distance = PdfHelper.cellPadding;
            amountStyle.ParagraphFormat.LeftIndent = PdfHelper.leftTableIndent;
            amountStyle.ParagraphFormat.Font.Color = PdfHelper.red;

            Style sapNumberStyle = document.Styles.AddStyle("sapNumberStyle", "Normal");
            sapNumberStyle.ParagraphFormat.Borders.Distance = PdfHelper.cellPadding;
            sapNumberStyle.ParagraphFormat.LeftIndent = PdfHelper.leftTableIndent;
            sapNumberStyle.ParagraphFormat.Borders.DistanceFromBottom = Unit.FromCentimeter(0.1);

            Style dbNumberStyle = document.Styles.AddStyle("dbNumberStyle", "Normal");
            dbNumberStyle.ParagraphFormat.Borders.Distance = PdfHelper.cellPadding;
            dbNumberStyle.ParagraphFormat.LeftIndent = PdfHelper.leftTableIndent;
            dbNumberStyle.ParagraphFormat.Borders.DistanceFromTop = Unit.FromCentimeter(0.1);

            List<Style> styles = new List<Style>
            {
                cellStyle, 
                headerCellStyle, 
                footerSectionStyle,
                amountStyle,
                sapNumberStyle,
                dbNumberStyle
            };

            return styles;
        }

        public static void AddFooterData(
            FlexMultiCalculator model,
            Document document,
            Section section,
            Unit contentWidth,
            Style footerSectionStyle,
            DateTime languageDateTime
            )
        {
            document.DefaultPageSetup.FooterDistance = Unit.FromCentimeter(0);

            Unit footerTableColumn1Width = Unit.FromCentimeter(contentWidth.Centimeter * 0.5);
            Unit footer2TableColumn2Width = Unit.FromCentimeter(contentWidth.Centimeter * 0.5);

            var rightFooterPagePar = new Paragraph
            {
                Format =
                {
                    Alignment = ParagraphAlignment.Right
                },
                Style = footerSectionStyle.Name
            };

            string pageFieldValue = model
                .Value("pageField", fallback: Fallback.ToLanguage).ToString();

            string[] pageValueDelimiter = { "{0}" };
            string[] values = pageFieldValue.Split(pageValueDelimiter, StringSplitOptions.None);

            rightFooterPagePar.AddText(values[0]);
            rightFooterPagePar.AddPageField();
            rightFooterPagePar.AddText(values[1]);
            rightFooterPagePar.AddNumPagesField();

            var date = languageDateTime.ToString("d'.'M'.'yyyy kl HH:mm");
            var leftDateSection = new Paragraph
            {
                Format = { Alignment = ParagraphAlignment.Left },
                Style = footerSectionStyle.Name,
            };

            var calculatedOnText = model
                .Value("calculatedOnText", fallback: Fallback.ToLanguage).ToString();

            leftDateSection.AddFormattedText(calculatedOnText + " ");
            leftDateSection.AddFormattedText(date);

            Table footerTable = section.Footers.Primary.AddTable();
            footerTable.AddColumn(footerTableColumn1Width);
            footerTable.AddColumn(footer2TableColumn2Width);
            footerTable.Rows.Alignment = RowAlignment.Center;

            var row = footerTable.AddRow();
            row.Cells[0].Add(leftDateSection);
            row.Cells[1].Add(rightFooterPagePar);
        }

        public static void GetTableSpace(Section section)
        {
            Paragraph tableSpace = section.AddParagraph();
            tableSpace.Format.LineSpacingRule = LineSpacingRule.Exactly;
            tableSpace.Format.LineSpacing = Unit.FromCentimeter(0);
            tableSpace.Format.SpaceBefore = margins;
        }

        public static void GetTableHeadline(Section section, string text)
        {
            Paragraph headline = section.AddParagraph();
            headline.AddFormattedText(text, TextFormat.Bold);
            headline.Format.SpaceAfter = margins / 3;
        }

        public static void GetDescription(
            Section section,
            IHtmlString descriptionHtmlString
            )
        {
            Paragraph description = section.AddParagraph();

            var html = descriptionHtmlString.ToHtmlString();

            description.AddFormattedText("\n");
            HtmlPdfHelper.HtmlToParagraph(description,html);
       
        }

        public static void GetEndText(
            Section section,
            HtmlDocument htmlDocument,
            IHtmlString endTextHtmlString
        )
        {
            // rendering
            Paragraph endTextSection = section.AddParagraph();

            htmlDocument.LoadHtml(endTextHtmlString.ToHtmlString());
            HtmlNode endTextNode = htmlDocument.DocumentNode;
            string endText = endTextNode.InnerHtml;

            foreach (var charToRemove in GetCharsToRemove())
            {
                endText = endText.Replace(charToRemove, string.Empty);
            }

            string[] endTextArray = endText.Split(GetSentenceSelimiter(), StringSplitOptions.None);

            foreach (string endTextItem in endTextArray)
            {
                endTextSection.AddLineBreak();
                endTextSection.AddFormattedText(endTextItem);
            }
        }

        public static string SaveDocument(Document document, string downloadPath, string entryGuid)
        {
            // rendering
            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(true)
            {
                Document = document,
            };
            pdfRenderer.RenderDocument();

            // creates directory for downloaded pdfs if not exist
            Directory.CreateDirectory(downloadPath);

            // generate path for pdf file
            string fileName = entryGuid;
            string fileNameWithExt = fileName + ".pdf";
            string filePath = Path.Combine(downloadPath, fileNameWithExt);

            // save pdf
            pdfRenderer.PdfDocument.Save(filePath);

            return fileNameWithExt;
        }
    }
}