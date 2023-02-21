using System;
using System.Collections.Generic;
using System.Globalization;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using System.Linq;
using System.Text.RegularExpressions;
using REDAirCalculator.Models.DTO;
using Umbraco.Forms.Core.Models;
using REDAirCalculator.Models.Pdf;
using REDAirCalculator.Models.ResultModels;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
using Umbraco.Web.PublishedModels;
using REDAirCalculator.Models.ResultViewModels;
using HtmlAgilityPack;

namespace REDAirCalculator.Utilities
{
    public class LinkPdfGenerator
    {
        private readonly LinkCalculator _linkModel;
        private readonly FlexMultiCalculator _model;
        private readonly string _formGuid;
        private readonly string _entryGuid;
        private readonly LinkFormDataDto _formData;
        private readonly LinkResultsViewModel _calculatedData;
        private readonly string _logoPath;
        private readonly string _downloadPath;
        private readonly string _title;
        private readonly DateTime _languageDateTime;
        private readonly string _language;

        public LinkPdfGenerator(
            LinkCalculator linkModel,
            FlexMultiCalculator model,
            string formGuid,
            string entryGuid,
            LinkFormDataDto formData,
            LinkResultsViewModel calculatedData,
            string logoPath,
            string downloadPath,
            string title,
            DateTime languageDateTime,
            string language)
        {
            _linkModel = linkModel;
            _model = model;
            _formGuid = formGuid;
            _entryGuid = entryGuid;
            _formData = formData;
            _calculatedData = calculatedData;
            _logoPath = logoPath;
            _downloadPath = downloadPath;
            _title = title;
            _languageDateTime = languageDateTime;
            _language = language;
        }

        public string Generate()
        {
            #region project and input data
            // data
            LinkFormDataDto formData = _formData;

            string guid = _formGuid;
            Form form = FormHelper.GetForm(new Guid(guid));
            #endregion

            #region document init
            Document document = PdfHelper.GetDocument(_title, formData);

            Unit pageWidth = document.DefaultPageSetup.PageWidth;
            Unit contentWidth = Unit.FromCentimeter(pageWidth.Centimeter - (2 * PdfHelper.margins.Centimeter));

            Section section = document.AddSection();
            HtmlDocument htmlDocument = new HtmlDocument();
            #endregion

            #region styles
            List<Style> styles = PdfHelper.GetCommonStyles(document);
            Style cellStyle = styles[0];
            Style headerCellStyle = styles[1];
            Style footerSectionStyle = styles[2];
            Style amountStyle = styles[3];
            Style sapNumberStyle = styles[4];
            Style dbNumberStyle = styles[5];

            Style multipleRowStyle = document.Styles.AddStyle("multipleRowStyle", "Normal");
            multipleRowStyle.ParagraphFormat.Borders.Distance = PdfHelper.cellPadding;
            multipleRowStyle.ParagraphFormat.Borders.DistanceFromTop = Unit.FromCentimeter(0.1);
            multipleRowStyle.ParagraphFormat.Borders.DistanceFromBottom = Unit.FromCentimeter(0);
            #endregion

            #region logo
            PdfHelper.GetLogo(section, _logoPath);
            #endregion

            #region footer
            PdfHelper.AddFooterData(_model, document, section, contentWidth, footerSectionStyle, _languageDateTime);
            #endregion

            #region project data table
            // data keys
            Field projectName = FormHelper.GetFormFieldByName(form, "projectName");
            Field projectDescription = FormHelper.GetFormFieldByName(form, "projectDescription");
            Field customerName = FormHelper.GetFormFieldByName(form, "customerName");
            Field address = FormHelper.GetFormFieldByName(form, "address");
            Field postIndex = FormHelper.GetFormFieldByName(form, "postIndex");
            Field city = FormHelper.GetFormFieldByName(form, "city");

            // data values
            string projectNameValue = (formData.ProjectName ?? "").Replace("\\", "");
            string projectDescriptionValue = (formData.ProjectDescription ?? "").Replace("\\", "");
            string customerNameValue = (formData.CustomerName ?? "").Replace("\\", "");
            string addressValue = (formData.Address ?? "").Replace("\\", "");
            string postIndexValue = (formData.PostIndex ?? "").Replace("\\", "");
            string cityValue = (formData.City ?? "").Replace("\\", "");

            List<ProjectPdfData> projectPdfData = new List<ProjectPdfData>
            {
                new ProjectPdfData(projectName, projectNameValue),
                new ProjectPdfData(projectDescription, projectDescriptionValue),
                new ProjectPdfData(customerName, customerNameValue),
                new ProjectPdfData(address, addressValue),
                new ProjectPdfData(postIndex, postIndexValue),
                new ProjectPdfData(city, cityValue),
            };

            // exclude empty lines
            projectPdfData = projectPdfData.Where(dataItem => !string.IsNullOrEmpty(dataItem.Value)).ToList();

            Unit projectDataTableColumn1Width = Unit.FromCentimeter(contentWidth.Centimeter * 0.25);
            Unit projectDataTableColumn2Width = Unit.FromCentimeter(contentWidth.Centimeter * 0.75);

            Table table = section.AddTable();

            table.AddColumn(projectDataTableColumn1Width);
            table.AddColumn(projectDataTableColumn2Width);
            table.Rows.Alignment = RowAlignment.Center;

            Row row = table.AddRow();
            row.HeadingFormat = true;
            row.Shading.Color = PdfHelper.pantoneCoolGrey;
            row.Cells[0].AddParagraph(_model.Value("projectDataText", fallback: Fallback.ToLanguage).ToString()).Style = headerCellStyle.Name;

            bool shouldBeGray = true;
            foreach (ProjectPdfData projectDataItem in projectPdfData)
            {
                row = table.AddRow();
                row.Shading.Color = shouldBeGray ? PdfHelper.ligthGray : PdfHelper.white;
                row.Cells[0].AddParagraph(projectDataItem.Field.Caption).Style = cellStyle.Name;
                row.Cells[1].AddParagraph(projectDataItem.Value).Style = cellStyle.Name;
                shouldBeGray = !shouldBeGray;
            }
            #endregion

            #region tables space
            PdfHelper.GetTableSpace(section);
            #endregion

            #region link results table headline 
            PdfHelper.GetTableHeadline(section, _linkModel.HeadingProducts);
            #endregion

            #region link results table 
            // data
            List<LinkResultsItemModel> linkResults = _calculatedData.LinkResultsPart;

            Unit linkResultsTableColumn1Width = Unit.FromCentimeter(contentWidth.Centimeter * 0.25);
            Unit linkResultsTableColumn2Width = Unit.FromCentimeter(contentWidth.Centimeter * 0.25);
            Unit linkResultsTableColumn3Width = Unit.FromCentimeter(contentWidth.Centimeter * 0.50);

            table = section.AddTable();
            table.AddColumn(linkResultsTableColumn1Width);
            table.AddColumn(linkResultsTableColumn2Width);
            table.AddColumn(linkResultsTableColumn3Width);
            table.Rows.Alignment = RowAlignment.Center;

            row = table.AddRow();
            row.HeadingFormat = true;
            row.Shading.Color = PdfHelper.pantoneCoolGrey;
            row.Cells[0].AddParagraph(_model.ItemProductNumberColText).Style = headerCellStyle.Name;
            row.Cells[1].AddParagraph(_model.ItemsCountColText).Style = headerCellStyle.Name;
            row.Cells[2].AddParagraph(_model.ItemDescriptionColText).Style = headerCellStyle.Name;

            shouldBeGray = true;
            foreach (LinkResultsItemModel linkResult in linkResults)
            {
                if (linkResult.Units != 0)
                {
                    row = table.AddRow();
                    row.Shading.Color = shouldBeGray ? PdfHelper.ligthGray : PdfHelper.white;

                    if (linkResult.SAPNumber != 0)
                    {
                        row.Cells[0].AddParagraph(linkResult.SAPNumber.ToString()).Style = sapNumberStyle.Name;
                    }

                    if (CalculatorHelper.HasDbNumber(linkResult.DBNumber, _language))
                    {
                        row.Cells[0].AddParagraph(linkResult.DBLabel + ": " + linkResult.DBNumber).Style = dbNumberStyle.Name;
                    }

                    row.Cells[1].AddParagraph($"{linkResult.Units}").Style = amountStyle.Name;

                    // cell 2
                    var unitsForAmount = linkResult.Units == 1 ? linkResult.UnitsInSingular : linkResult.UnitsInPlural;
                    row.Cells[2].VerticalAlignment = VerticalAlignment.Center;
                    row.Cells[2].AddParagraph($" ({unitsForAmount}) {linkResult.Description}").Style = multipleRowStyle.Name;

                    if (linkResult.DeliveredLBM != 0)
                    {
                        row.Cells[2].AddParagraph($"{_linkModel.DeliveredLbm}: {linkResult.DeliveredLBM}").Style = multipleRowStyle.Name;
                    }

                    if (linkResult.UsedBrackets != 0)
                    {
                        row.Cells[2].AddParagraph($"{_linkModel.UsedBrackets}: {linkResult.UsedBrackets}").Style = multipleRowStyle.Name;
                    }

                    if (linkResult.UsedScrews != 0)
                    {
                        row.Cells[2].AddParagraph($"{_linkModel.UsedScrews}: {linkResult.UsedScrews}").Style = multipleRowStyle.Name;
                    }

                    if (linkResult.DeliveredBrackets != 0)
                    {
                        row.Cells[2].AddParagraph($"{_linkModel.DeliveredBrackets}: {linkResult.DeliveredBrackets}").Style = multipleRowStyle.Name;
                    }

                    if (linkResult.DeliveredBrackets == 0 && linkResult.HasEmptyMessage)
                    {
                        row.Cells[2].AddParagraph($"{_linkModel.DeliveredBrackets}: {_linkModel.NoDeliveredBracketsMessage}").Style = multipleRowStyle.Name;
                    }

                    if (linkResult.DeliveredScrews != 0)
                    {
                        row.Cells[2].AddParagraph($"{_linkModel.DeliveredScrews}: {linkResult.DeliveredScrews}").Style = multipleRowStyle.Name;
                    }

                    if (linkResult.PlankDepth != 0)
                    {
                        row.Cells[2].AddParagraph($"{_linkModel.PlankDepth}: {linkResult.PlankDepth}").Style = multipleRowStyle.Name;
                    }

                    if (linkResult.DeliveredM2 != 0)
                    {
                        row.Cells[2].AddParagraph($"{_linkModel.Delivered}: {linkResult.DeliveredM2}").Style = multipleRowStyle.Name;
                    }

                (row.Cells[2].Elements.First as Paragraph).Format.Borders.DistanceFromTop = PdfHelper.cellPadding;
                    (row.Cells[2].Elements.LastObject as Paragraph).Format.Borders.DistanceFromBottom = PdfHelper.cellPadding;

                    shouldBeGray = !shouldBeGray;
                }
            }
            #endregion

            #region tables space 
            PdfHelper.GetTableSpace(section);
            #endregion

            #region installation instructions table headline 
            PdfHelper.GetTableHeadline(section, _linkModel.HeadingInstallation);
            #endregion

            #region installation instructions table 
            // data
            List<OpeningTypeDto> installationInstructionsModel = _calculatedData.InstalationInstractions;

            Unit installationInstructionsTableColumn1Width = Unit.FromCentimeter(contentWidth.Centimeter * 0.20);
            Unit installationInstructionsTableColumn2Width = Unit.FromCentimeter(contentWidth.Centimeter * 0.10);
            Unit installationInstructionsTableColumn3Width = Unit.FromCentimeter(contentWidth.Centimeter * 0.10);
            Unit installationInstructionsTableColumn4Width = Unit.FromCentimeter(contentWidth.Centimeter * 0.10);
            Unit installationInstructionsTableColumn5Width = Unit.FromCentimeter(contentWidth.Centimeter * 0.10);
            Unit installationInstructionsTableColumn6Width = Unit.FromCentimeter(contentWidth.Centimeter * 0.20);
            Unit installationInstructionsTableColumn7Width = Unit.FromCentimeter(contentWidth.Centimeter * 0.20);

            table = section.AddTable();
            table.AddColumn(installationInstructionsTableColumn1Width);
            table.AddColumn(installationInstructionsTableColumn2Width);
            table.AddColumn(installationInstructionsTableColumn3Width);
            table.AddColumn(installationInstructionsTableColumn4Width);
            table.AddColumn(installationInstructionsTableColumn5Width);
            table.AddColumn(installationInstructionsTableColumn6Width);
            table.AddColumn(installationInstructionsTableColumn7Width);
            table.Rows.Alignment = RowAlignment.Center;

            row = table.AddRow();
            row.HeadingFormat = true;
            row.Shading.Color = PdfHelper.pantoneCoolGrey;
            row.Cells[0].AddParagraph(_linkModel.HeadingOpeningType).Style = headerCellStyle.Name;
            row.Cells[1].AddParagraph(_linkModel.HeadingWindowDoor).Style = headerCellStyle.Name;
            row.Cells[2].AddParagraph(_linkModel.HeadingWidth).Style = headerCellStyle.Name;
            row.Cells[3].AddParagraph(_linkModel.HeadingHeight).Style = headerCellStyle.Name;
            row.Cells[4].AddParagraph(_linkModel.HeadingAmount).Style = headerCellStyle.Name;
            row.Cells[5].AddParagraph(_linkModel.HeadingBrackets).Style = headerCellStyle.Name;
            row.Cells[6].AddParagraph(_linkModel.HeadingInformation).Style = headerCellStyle.Name;

            shouldBeGray = true;

            int typeCounter = 0;
            foreach (OpeningTypeDto openingTypeDto in installationInstructionsModel)
            {
                row = table.AddRow();
                row.Shading.Color = shouldBeGray ? PdfHelper.ligthGray : PdfHelper.white;
                row.Cells[0].AddParagraph($"{openingTypeDto.Type} {++typeCounter}").Style = cellStyle.Name;
                row.Cells[1].AddParagraph(openingTypeDto.Type).Style = cellStyle.Name;
                row.Cells[2].AddParagraph(openingTypeDto.Width.ToString(CultureInfo.InvariantCulture)).Style = cellStyle.Name;
                row.Cells[3].AddParagraph(openingTypeDto.Height.ToString(CultureInfo.InvariantCulture)).Style = cellStyle.Name;
                row.Cells[4].AddParagraph(openingTypeDto.Amount.ToString(CultureInfo.InvariantCulture)).Style = cellStyle.Name;

                // cell 5
                row.Cells[5].VerticalAlignment = VerticalAlignment.Center;

                row.Cells[5].AddParagraph($"{_linkModel.AngleBrackets}: {openingTypeDto.PerItemAngleBrackets}").Style = multipleRowStyle.Name;
                row.Cells[5].AddParagraph($"{_linkModel.JoinerBrackets}: {openingTypeDto.PerItemJoiner}").Style = multipleRowStyle.Name;
                row.Cells[5].AddParagraph($"{_linkModel.CornerBrackets}: {openingTypeDto.PerItemCorner}").Style = multipleRowStyle.Name;

                (row.Cells[5].Elements.First as Paragraph).Format.Borders.DistanceFromTop = PdfHelper.cellPadding;
                (row.Cells[5].Elements.LastObject as Paragraph).Format.Borders.DistanceFromBottom = PdfHelper.cellPadding;

                // cell 6
                row.Cells[6].VerticalAlignment = VerticalAlignment.Center;

                if (openingTypeDto.PerItemLBM != 0)
                {
                    row.Cells[6].AddParagraph($"{_linkModel.BoardLbm}: {openingTypeDto.PerItemLBM}").Style = multipleRowStyle.Name;
                }

                if (openingTypeDto.SelfWeight != 0)
                {
                    row.Cells[6].AddParagraph($"{_linkModel.SelfWeight}: {openingTypeDto.SelfWeight} (kg)").Style = multipleRowStyle.Name;
                }

                (row.Cells[6].Elements.First as Paragraph).Format.Borders.DistanceFromTop = PdfHelper.cellPadding;
                (row.Cells[6].Elements.LastObject as Paragraph).Format.Borders.DistanceFromBottom = PdfHelper.cellPadding;

                shouldBeGray = !shouldBeGray;
            }
            #endregion

            #region installation instruction description
            PdfHelper.GetDescription(section, _linkModel.OpeningTypesDescriptionText);
            #endregion

            #region tables space 
            PdfHelper.GetTableSpace(section);
            #endregion

            #region calculations table 
            // data
            LinkCalculationsModel calculationsModel = _calculatedData.LinkCalculations;

            Unit calculationsTableColumn1Width = Unit.FromCentimeter(contentWidth.Centimeter * 0.26);
            Unit calculationsTableColumn2Width = Unit.FromCentimeter(contentWidth.Centimeter * 0.60);
            Unit calculationsTableColumn3Width = Unit.FromCentimeter(contentWidth.Centimeter * 0.14);

            table = section.AddTable();
            table.AddColumn(calculationsTableColumn1Width);
            table.AddColumn(calculationsTableColumn2Width);
            table.AddColumn(calculationsTableColumn3Width);
            table.Rows.Alignment = RowAlignment.Center;

            row = table.AddRow();
            row.HeadingFormat = true;
            row.Shading.Color = PdfHelper.pantoneCoolGrey;
            row.Cells[0].AddParagraph(_linkModel.SumOfNeededLbm).Style = headerCellStyle.Name;
            row.Cells[1].AddParagraph().Style = headerCellStyle.Name;
            row.Cells[2].AddParagraph(calculationsModel.SumOfNeededLBM.ToString(CultureInfo.InvariantCulture)).Style = headerCellStyle.Name;
            #endregion

            #region tables space 
            PdfHelper.GetTableSpace(section);
            #endregion

            #region input data table
            // data keys
            Field plankDepth = FormHelper.GetFormFieldByName(form, "plankDepth");
            Field precutBlanks = FormHelper.GetFormFieldByName(form, "precutPlanks");

            Unit inputDataTableColumn1Width = Unit.FromCentimeter(contentWidth.Centimeter * 0.50);
            Unit inputDataTableColumn2Width = Unit.FromCentimeter(contentWidth.Centimeter * 0.25);
            Unit inputDataTableColumn3Width = Unit.FromCentimeter(contentWidth.Centimeter * 0.25);

            string precutPlanksValue = formData.PrecutPlanks ? _linkModel.Value("precutPlanksTextYes", fallback: Fallback.ToLanguage).ToString() :
                _linkModel.Value("precutPlanksTextNo", fallback: Fallback.ToLanguage).ToString();

            List<InputPdfData> inputPdfData = new List<InputPdfData>
            {
                new InputPdfData(
                    plankDepth,
                    "mm",
                    formData.PlankDepth),
                new InputPdfData(
                    precutBlanks,
                    "",
                    precutPlanksValue)
            };

            table = section.AddTable();
            table.AddColumn(inputDataTableColumn1Width);
            table.AddColumn(inputDataTableColumn2Width);
            table.AddColumn(inputDataTableColumn3Width);
            table.Rows.Alignment = RowAlignment.Center;

            row = table.AddRow();
            row.HeadingFormat = true;
            row.Shading.Color = PdfHelper.pantoneCoolGrey;
            row.Cells[0].AddParagraph(_model.Value("inputText", fallback: Fallback.ToLanguage).ToString()).Style = headerCellStyle.Name;

            inputPdfData = inputPdfData.Where(dataItem => dataItem.IsVisible == true).ToList();

            shouldBeGray = true;
            foreach (InputPdfData inputDataItem in inputPdfData)
            {
                row = table.AddRow();
                row.Shading.Color = shouldBeGray ? PdfHelper.ligthGray : PdfHelper.white;
                row.Cells[0].AddParagraph(inputDataItem.Field.Caption + inputDataItem.ExtraText).Style = cellStyle.Name;

                // show superscript if needed
                bool isDigitInUnit = inputDataItem.Unit.Any(char.IsDigit);
                if (isDigitInUnit)
                {
                    Paragraph inputUnit = row.Cells[1].AddParagraph();
                    string unitDigit = Regex.Match(inputDataItem.Unit, @"\d+").Value;
                    FormattedText inputUnitFormattedText = inputUnit.AddFormattedText(inputDataItem.Unit.Replace(unitDigit + ")", ""));
                    inputUnitFormattedText = inputUnit.AddFormattedText(unitDigit);
                    inputUnitFormattedText.Superscript = true;
                    inputUnit.AddFormattedText(")");
                    inputUnit.Style = cellStyle.Name;
                }
                else
                {
                    row.Cells[1].AddParagraph(inputDataItem.Unit).Style = cellStyle.Name;
                }

                row.Cells[2].AddParagraph(string.IsNullOrEmpty(inputDataItem.ExtraValue)
                    ? (string)inputDataItem.Value.ToString()
                    : inputDataItem.ExtraValue).Style = cellStyle.Name;
                shouldBeGray = !shouldBeGray;
            }
            #endregion

            #region tables space 
            PdfHelper.GetTableSpace(section);
            #endregion

            #region end text
            PdfHelper.GetEndText(section, htmlDocument, _model.EndText);
            #endregion

            #region document save
            string fileNameWithExt = PdfHelper.SaveDocument(document, _downloadPath, _entryGuid);
            #endregion

            return fileNameWithExt;
        }
    }
}