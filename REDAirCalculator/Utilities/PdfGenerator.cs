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
    public class PdfGenerator
    {
        private readonly FlexMultiCalculator _model;
        private readonly string _formGuid;
        private readonly string _entryGuid;
        private readonly FormDataDto _formData;
        private readonly CalculatedDataDto _calculatedData;
        private readonly string _logoPath;
        private readonly string _downloadPath;
        private readonly string _title;
        private readonly DateTime _languageDateTime;
        private readonly string _lang;
        private readonly IUmbracoContextFactory _context;

        public PdfGenerator(
            FlexMultiCalculator model,
            string formGuid,
            string entryGuid,
            FormDataDto formData,
            CalculatedDataDto calculatedData,
            string logoPath,
            string downloadPath,
            string title,
            DateTime languageDateTime,
            string lang,
            IUmbracoContextFactory context)
        {
            _model = model;
            _formGuid = formGuid;
            _entryGuid = entryGuid;
            _formData = formData;
            _calculatedData = calculatedData;
            _logoPath = logoPath;
            _downloadPath = downloadPath;
            _title = title;
            _languageDateTime = languageDateTime;
            _lang = lang;
            _context = context;
        }

        public string Generate()
        {
            #region project and input data
            // data
            FormDataDto formData = _formData;

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

            // styles for products 
            Style firstProductLineStyle = document.Styles.AddStyle("firstProductLineStyle", "Normal");
            firstProductLineStyle.ParagraphFormat.Borders.Distance = PdfHelper.cellPadding;
            firstProductLineStyle.ParagraphFormat.Borders.DistanceFromBottom = Unit.FromCentimeter(0);

            Style middleProductLineStyle = document.Styles.AddStyle("middleProductLineStyle", "Normal");
            middleProductLineStyle.ParagraphFormat.Borders.Distance = PdfHelper.cellPadding;
            middleProductLineStyle.ParagraphFormat.Borders.DistanceFromTop = Unit.FromCentimeter(0.1);
            middleProductLineStyle.ParagraphFormat.Borders.DistanceFromBottom = Unit.FromCentimeter(0.1);

            Style lastProductLineStyle = document.Styles.AddStyle("lastProductLineStyle", "Normal");
            lastProductLineStyle.ParagraphFormat.Borders.Distance = PdfHelper.cellPadding;
            lastProductLineStyle.ParagraphFormat.Borders.DistanceFromTop = Unit.FromCentimeter(0);
            //
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
            Field windSpeedArea = FormHelper.GetFormFieldByName(form, "windSpeedArea");
            Field city = FormHelper.GetFormFieldByName(form, "city");

            // data values
            string projectNameValue = (formData.ProjectName ?? "").Replace("\\", "");
            string projectDescriptionValue = (formData.ProjectDescription ?? "").Replace("\\", "");
            string customerNameValue = (formData.CustomerName ?? "").Replace("\\", "");
            string addressValue = (formData.Address ?? "").Replace("\\", "");
            string postIndexValue = (formData.PostIndex ?? "").Replace("\\", "");
            string windSpeedAreaValue = formData.WindSpeedArea;
            string cityValue = (formData.City ?? "").Replace("\\", "");

            List<ProjectPdfData> projectPdfData = new List<ProjectPdfData>
            {
                new ProjectPdfData(projectName, projectNameValue),
                new ProjectPdfData(projectDescription, projectDescriptionValue),
                new ProjectPdfData(customerName, customerNameValue),
                new ProjectPdfData(address, addressValue),
                new ProjectPdfData(postIndex, postIndexValue),
                new ProjectPdfData(windSpeedArea, windSpeedAreaValue),
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

            #region products table 
            // data
            QuantityCalculationsModel products = _calculatedData.Products;

            Unit productsTableColumn1Width = Unit.FromCentimeter(contentWidth.Centimeter * 0.25);
            Unit productsTableColumn2Width = Unit.FromCentimeter(contentWidth.Centimeter * 0.25);
            Unit productsTableColumn3Width = Unit.FromCentimeter(contentWidth.Centimeter * 0.50);

            table = section.AddTable();
            table.AddColumn(productsTableColumn1Width);
            table.AddColumn(productsTableColumn2Width);
            table.AddColumn(productsTableColumn3Width);
            table.Rows.Alignment = RowAlignment.Center;

            row = table.AddRow();
            row.HeadingFormat = true;
            row.Shading.Color = PdfHelper.pantoneCoolGrey;
            row.Cells[0].AddParagraph(_model.ItemProductNumberColText).Style = headerCellStyle.Name;
            row.Cells[1].AddParagraph(_model.ItemsCountColText).Style = headerCellStyle.Name;
            row.Cells[2].AddParagraph(_model.ItemDescriptionColText).Style = headerCellStyle.Name;

            shouldBeGray = true;

            // mineral wool
            foreach (var item in products.MineralWool)
            {
                row = table.AddRow();
                row.Shading.Color = shouldBeGray ? PdfHelper.ligthGray : PdfHelper.white;
                AddProductRow(
                    item,
                    row,
                    sapNumberStyle,
                    dbNumberStyle,
                    amountStyle,
                    firstProductLineStyle,
                    middleProductLineStyle,
                    lastProductLineStyle);
                shouldBeGray = !shouldBeGray;
            }
            // screws
            if (products.Screws != null)
            {
                ProductDto screws = products.Screws;

                row = table.AddRow();
                row.Shading.Color = shouldBeGray ? PdfHelper.ligthGray : PdfHelper.white;
                AddProductRow(
                    screws,
                    row,
                    sapNumberStyle,
                    dbNumberStyle,
                    amountStyle,
                    firstProductLineStyle,
                    middleProductLineStyle,
                    lastProductLineStyle);
                shouldBeGray = !shouldBeGray;
            }

            // rail
            ProductDto rail = products.Rail;
            bool ExclusiveNo = false;
            if (_lang == "no" && formData.SystemType == "FLEX") {
                ExclusiveNo = true;
            }

            row = table.AddRow();
            row.Shading.Color = shouldBeGray ? PdfHelper.ligthGray : PdfHelper.white;
            AddProductRow(
                rail,
                row,
                sapNumberStyle,
                dbNumberStyle,
                amountStyle,
                firstProductLineStyle,
                middleProductLineStyle,
                lastProductLineStyle,
                ExclusiveNo);
            shouldBeGray = !shouldBeGray;

            foreach (var item in products.Accessories)
            {
                row = table.AddRow();
                row.Shading.Color = shouldBeGray ? PdfHelper.ligthGray : PdfHelper.white;
                AddProductRow(
                    item,
                    row,
                    sapNumberStyle,
                    dbNumberStyle,
                    amountStyle,
                    firstProductLineStyle,
                    middleProductLineStyle,
                    lastProductLineStyle);
                shouldBeGray = !shouldBeGray;
            }
            #endregion

            #region tables space 
            PdfHelper.GetTableSpace(section);
            #endregion

            #region flex/multi tables check
            bool isMulti = formData.SystemType == "MULTI";
            bool showAll = formData.ShowAllResults;
            ForMultiViewModel forMultiModel = new ForMultiViewModel();
            #endregion

            #region insulation settings table headline 
            PdfHelper.GetTableHeadline(section, _model.InsulationSettingsText);
            #endregion

            #region insulation settings table 
            // data
            DescriptionModel insulationSettingsModel = _calculatedData.DescriptionModel;

            Unit insulationSettingsTableColumn1Width = Unit.FromCentimeter(contentWidth.Centimeter * 0.70);
            Unit insulationSettingsTableColumn2Width = Unit.FromCentimeter(contentWidth.Centimeter * 0.15);
            Unit insulationSettingsTableColumn3Width = Unit.FromCentimeter(contentWidth.Centimeter * 0.15);

            table = section.AddTable();
            table.AddColumn(insulationSettingsTableColumn1Width);
            table.AddColumn(insulationSettingsTableColumn2Width);
            table.AddColumn(insulationSettingsTableColumn3Width);
            table.Rows.Alignment = RowAlignment.Center;

            row = table.AddRow();
            row.HeadingFormat = true;
            row.Shading.Color = PdfHelper.pantoneCoolGrey;
            row.Cells[0].AddParagraph(_model.ItemNameColText).Style = headerCellStyle.Name;
            row.Cells[1].AddParagraph(_model.ItemUnitsOfMeasurementColText).Style = headerCellStyle.Name;
            row.Cells[2].AddParagraph(_model.ItemNumberColText).Style = headerCellStyle.Name;

            List<DescriptionPdfData> insulationsPdfData = new List<DescriptionPdfData>
            {
                new DescriptionPdfData(
                    _model.Value("baseRailSpacing", fallback: Fallback.ToLanguage).ToString(),
                    _model.Value("unit6", fallback: Fallback.ToDefaultValue).ToString(),
                    insulationSettingsModel.BaseRailSpacing),
                new DescriptionPdfData(
                    _model.Value("anchorScrewDistance", fallback: Fallback.ToLanguage).ToString(),
                    _model.Value("unit6", fallback: Fallback.ToDefaultValue).ToString(),
                    insulationSettingsModel.AnchorScrewDistance),
                new DescriptionPdfData(
                    !isMulti ?
                        _model.Value("compressionDepthFlex", fallback: Fallback.ToLanguage).ToString() :
                        _model.Value("compressionDepthMulti", fallback: Fallback.ToLanguage).ToString(),
                    _model.Value("unit6", fallback: Fallback.ToDefaultValue).ToString(),
                    insulationSettingsModel.CompressionDepth),
                new DescriptionPdfData(
                    _model.Value("maxDistanceBetweenBrackets", fallback: Fallback.ToLanguage).ToString(),
                    _model.Value("unit6", fallback: Fallback.ToDefaultValue).ToString(),
                    insulationSettingsModel.MaxDistanceBetweenBrackets,
                    true,
                    formData.ShowAllResults)
            };

            if (!isMulti)
            {
                insulationsPdfData = insulationsPdfData.Where(dataItem => dataItem.IsMulti == false).ToList();
            }

            //if (insulationSettingsModel.SelectedAll)
            //{
            //    insulationsPdfData = insulationsPdfData.Where(dataItem => dataItem.DictionaryName != "Base Rail Spacing").ToList();
            //}

            insulationsPdfData = insulationsPdfData.Where(dataItem => dataItem.IsVisible == true).ToList();

            foreach (DescriptionPdfData insulationsDataItem in insulationsPdfData)
            {
                row = table.AddRow();
                row.Shading.Color = shouldBeGray ? PdfHelper.ligthGray : PdfHelper.white;
                row.Cells[0].AddParagraph(insulationsDataItem.DictionaryName).Style = cellStyle.Name;
                row.Cells[1].AddParagraph(insulationsDataItem.DictionaryUnit).Style = cellStyle.Name;
                row.Cells[2].AddParagraph(insulationsDataItem.Value.ToString()).Style = cellStyle.Name;
                shouldBeGray = !shouldBeGray;
            }
            #endregion

            #region insulation settings description
            PdfHelper.GetDescription(section, _model.DescriptionText);
            #endregion

            #region tables space 
            PdfHelper.GetTableSpace(section);
            #endregion

            #region load calculations table headline 
            PdfHelper.GetTableHeadline(section, _model.LoadCalculationsText);
            #endregion

            #region load calculations table 
            // data
            LoadCalculationsViewModel loadCalculationsModel = _calculatedData.LoadCalculations;

            Unit loadCalculationsTableColumn1Width = Unit.FromCentimeter(contentWidth.Centimeter * 0.70);
            Unit loadCalculationsTableColumn2Width = Unit.FromCentimeter(contentWidth.Centimeter * 0.15);
            Unit loadCalculationsTableColumn3Width = Unit.FromCentimeter(contentWidth.Centimeter * 0.15);

            table = section.AddTable();
            table.AddColumn(loadCalculationsTableColumn1Width);
            table.AddColumn(loadCalculationsTableColumn2Width);
            table.AddColumn(loadCalculationsTableColumn3Width);
            table.Rows.Alignment = RowAlignment.Center;

            row = table.AddRow();
            row.HeadingFormat = true;
            row.Shading.Color = PdfHelper.pantoneCoolGrey;
            row.Cells[0].AddParagraph(_model.ItemNameColText).Style = headerCellStyle.Name;
            row.Cells[1].AddParagraph(_model.ItemUnitsOfMeasurementColText).Style = headerCellStyle.Name;
            row.Cells[2].AddParagraph(_model.ItemNumberColText).Style = headerCellStyle.Name;

            List<LoadCalculationsPdfData> loadCalculationsPdfData = new List<LoadCalculationsPdfData>
            {
                new LoadCalculationsPdfData(
                    _model.Value("totalSelfweightOfFacadeText", fallback: Fallback.ToLanguage).ToString(),
                    _model.Value("unit5", fallback: Fallback.ToDefaultValue).ToString(),
                    loadCalculationsModel.TotalSelfweight),
                new LoadCalculationsPdfData(
                    _model.Value("maxForceInAnchorScrewFromWindText", fallback: Fallback.ToLanguage).ToString(),
                    _model.Value("unit1", fallback: Fallback.ToDefaultValue).ToString(),
                    loadCalculationsModel.MaxForceWind),
                new LoadCalculationsPdfData(
                    _model.Value("maxForceInAnchorScrewFromPrestressText", fallback: Fallback.ToLanguage).ToString(),
                    _model.Value("unit1", fallback: Fallback.ToDefaultValue).ToString(),
                    loadCalculationsModel.MaxForcePrestress),
                new LoadCalculationsPdfData(
                    _model.Value("maxForceInAnchorScrewFromSelfweightText", fallback: Fallback.ToLanguage).ToString(),
                    _model.Value("unit1", fallback: Fallback.ToDefaultValue).ToString(),
                    loadCalculationsModel.MaxForceSelfweight,
                    true),
                new LoadCalculationsPdfData(
                    _model.Value("totalMaxForceInAnchorScrewText", fallback: Fallback.ToLanguage).ToString(),
                    _model.Value("unit1", fallback: Fallback.ToDefaultValue).ToString(),
                    loadCalculationsModel.TotalMaxForce),
                new LoadCalculationsPdfData(
                    _model.Value("anchorScrewPullOutStrengthText", fallback: Fallback.ToLanguage).ToString(),
                    _model.Value("unit1", fallback: Fallback.ToDefaultValue).ToString(),
                    loadCalculationsModel.AnchorScrewPull),
                new LoadCalculationsPdfData(
                    _model.Value("necessaryPrestressText", fallback: Fallback.ToLanguage).ToString(),
                    _model.Value("unit4", fallback: Fallback.ToDefaultValue).ToString(),
                    loadCalculationsModel.NecessaryPrestress),
                new LoadCalculationsPdfData(
                    _model.Value("minPrestressForcePrMBaseRailText", fallback: Fallback.ToLanguage).ToString(),
                    _model.Value("unit4", fallback: Fallback.ToDefaultValue).ToString(),
                    loadCalculationsModel.MinPrestressForce),
                new LoadCalculationsPdfData(
                    _model.Value("windPeakVelocityPressureText", fallback: Fallback.ToLanguage).ToString(),
                    _model.Value("unit3", "en", fallback: Fallback.ToLanguage).ToString(),
                    loadCalculationsModel.WindPeakVelocity)
            };

            if (!isMulti)
            {
                loadCalculationsPdfData = loadCalculationsPdfData.Where(dataItem => dataItem.IsMulti == false).ToList();
            }

            loadCalculationsPdfData = loadCalculationsPdfData.Where(dataItem => !string.IsNullOrEmpty(dataItem.Value)).ToList();

            foreach (LoadCalculationsPdfData loadCalculationsDataItem in loadCalculationsPdfData)
            {
                row = table.AddRow();
                row.Shading.Color = shouldBeGray ? PdfHelper.ligthGray : PdfHelper.white;
                row.Cells[0].AddParagraph(loadCalculationsDataItem.DictionaryName).Style = cellStyle.Name;

                // show superscript if needed
                bool isDigitInDictionaryUnit = loadCalculationsDataItem.DictionaryUnit.Any(char.IsDigit);
                if (isDigitInDictionaryUnit)
                {
                    Paragraph loadCalculationsUnit = row.Cells[1].AddParagraph();
                    string unitDigit = Regex.Match(loadCalculationsDataItem.DictionaryUnit, @"\d+").Value;
                    FormattedText loadCalculationsUnitFormattedText = loadCalculationsUnit.AddFormattedText(loadCalculationsDataItem.DictionaryUnit.Replace(unitDigit + ")", ""));
                    loadCalculationsUnitFormattedText = loadCalculationsUnit.AddFormattedText(unitDigit);
                    loadCalculationsUnitFormattedText.Superscript = true;
                    loadCalculationsUnit.AddFormattedText(")");
                    loadCalculationsUnit.Style = cellStyle.Name;
                }
                else
                {
                    row.Cells[1].AddParagraph(loadCalculationsDataItem.DictionaryUnit).Style = cellStyle.Name;
                }

                row.Cells[2].AddParagraph(loadCalculationsDataItem.Value).Style = cellStyle.Name;
                shouldBeGray = !shouldBeGray;
            }
            #endregion

            #region tables space 
            PdfHelper.GetTableSpace(section);
            #endregion

            if (isMulti && showAll)
            {
                #region for multi only table headline 
                Paragraph forMultiOnlyTableHeadline = section.AddParagraph();
                forMultiOnlyTableHeadline.AddFormattedText(_model.ForMultiOnlyText, TextFormat.Bold);
                forMultiOnlyTableHeadline.Format.SpaceAfter = PdfHelper.margins / 3;
                #endregion

                #region for multi only table
                // data
                forMultiModel = _calculatedData.ForMultiModel;

                Unit forMultiOnlyTableColumn1Width = Unit.FromCentimeter(contentWidth.Centimeter * 0.70);
                Unit forMultiOnlyTableColumn2Width = Unit.FromCentimeter(contentWidth.Centimeter * 0.15);
                Unit forMultiOnlyTableColumn3Width = Unit.FromCentimeter(contentWidth.Centimeter * 0.15);

                table = section.AddTable();
                table.AddColumn(forMultiOnlyTableColumn1Width);
                table.AddColumn(forMultiOnlyTableColumn2Width);
                table.AddColumn(forMultiOnlyTableColumn3Width);
                table.Rows.Alignment = RowAlignment.Center;

                row = table.AddRow();
                row.HeadingFormat = true;
                row.Shading.Color = PdfHelper.pantoneCoolGrey;
                row.Cells[0].AddParagraph(_model.ItemNameColText).Style = headerCellStyle.Name;
                row.Cells[1].AddParagraph(_model.ItemUnitsOfMeasurementColText).Style = headerCellStyle.Name;
                row.Cells[2].AddParagraph(_model.ItemNumberColText).Style = headerCellStyle.Name;

                List<ForMultiPdfData> forMultiPdfData = new List<ForMultiPdfData>
                {
                    new ForMultiPdfData(
                        _model.Value("maxHorizontalForceInFixedBracketText", fallback: Fallback.ToLanguage).ToString(),
                        _model.Value("unit1", fallback: Fallback.ToDefaultValue).ToString(),
                        forMultiModel.MaxForceFixedBrackets),
                    new ForMultiPdfData(
                        _model.Value("strengthHorizontalForceInFixedBracketText", fallback: Fallback.ToLanguage).ToString(),
                        _model.Value("unit1", fallback: Fallback.ToDefaultValue).ToString(),
                        forMultiModel.StrengthFixedBrackets),
                    new ForMultiPdfData(
                        _model.Value("maxHorizontalForceInSlidingBracketText", fallback: Fallback.ToLanguage).ToString(),
                        _model.Value("unit1", fallback: Fallback.ToLanguage).ToString(),
                        forMultiModel.MaxForceSlidingBrackets),
                    new ForMultiPdfData(
                        _model.Value("strengthHorizontalForceInSlidingBracketText", fallback: Fallback.ToLanguage).ToString()
                            .Replace("{numberOfScrews}", forMultiModel.NumberOfScrews.ToString()),
                        _model.Value("unit1", fallback: Fallback.ToLanguage).ToString(),
                        forMultiModel.StrengthSlidingBrackets),
                    new ForMultiPdfData(
                        _model.Value("bendingMomentInTProfileText", fallback: Fallback.ToLanguage).ToString()
                            .Replace("{numberOfScrews}", forMultiModel.NumberOfScrews.ToString()),
                        _model.Value("unit2", fallback: Fallback.ToDefaultValue).ToString(),
                        forMultiModel.BendingMomentTProfile),
                    new ForMultiPdfData(
                        _model.Value("strengthBendingMomentInTProfileText", fallback: Fallback.ToLanguage).ToString(),
                        _model.Value("unit2", fallback: Fallback.ToLanguage).ToString(),
                        forMultiModel.StrengthTProfile),
                };

                foreach (ForMultiPdfData forMultiPdfDataItem in forMultiPdfData)
                {
                    row = table.AddRow();
                    row.Shading.Color = shouldBeGray ? PdfHelper.ligthGray : PdfHelper.white;
                    row.Cells[0].AddParagraph(forMultiPdfDataItem.DictionaryName).Style = cellStyle.Name;
                    row.Cells[1].AddParagraph(forMultiPdfDataItem.DictionaryUnit).Style = cellStyle.Name;
                    row.Cells[2].AddParagraph(forMultiPdfDataItem.Value.ToString()).Style =
                        cellStyle.Name;
                    shouldBeGray = !shouldBeGray;
                }
                #endregion

                #region tables space 
                Paragraph tableSpace5 = section.AddParagraph();
                tableSpace5.Format.LineSpacingRule = LineSpacingRule.Exactly;
                tableSpace5.Format.LineSpacing = Unit.FromCentimeter(0);
                tableSpace5.Format.SpaceBefore = PdfHelper.margins;
                #endregion
            }

            #region design check table headline 
            Paragraph designCheckTableHeadline = section.AddParagraph();
            designCheckTableHeadline.AddFormattedText(_model.DesignCheckText, TextFormat.Bold);
            designCheckTableHeadline.Format.SpaceAfter = PdfHelper.margins / 3;
            #endregion

            #region design check table
            // data
            DesignCheckViewModel designCheck = _calculatedData.DesignCheckModel;

            // design check present values (from load calculations)
            string totalMaxForce = loadCalculationsModel.TotalMaxForce;
            string necessaryPrestress = @loadCalculationsModel.NecessaryPrestress;

            // design check guaranteed values (from load calculations)
            string anchorScrewPull = loadCalculationsModel.AnchorScrewPull;
            string minPrestressForce = loadCalculationsModel.MinPrestressForce;

            // design check present values (from for multi model)
            string maxForceFixedBrackets = forMultiModel.MaxForceFixedBrackets;
            string maxForceSlidingBrackets = forMultiModel.MaxForceSlidingBrackets;
            string bendingMomentTProfile = forMultiModel.BendingMomentTProfile;

            // design check guaranteed values (from for multi model)
            string strengthFixedBrackets = forMultiModel.StrengthFixedBrackets;
            string strengthSlidingBrackets = forMultiModel.StrengthSlidingBrackets;
            string strengthTProfile = forMultiModel.StrengthTProfile;

            Unit designCheckTableColumn1Width = Unit.FromCentimeter(contentWidth.Centimeter * 0.50);
            Unit designCheckTableColumn2Width = Unit.FromCentimeter(contentWidth.Centimeter * 0.125);
            Unit designCheckTableColumn3Width = Unit.FromCentimeter(contentWidth.Centimeter * 0.125);
            Unit designCheckTableColumn4Width = Unit.FromCentimeter(contentWidth.Centimeter * 0.125);
            Unit designCheckTableColumn5Width = Unit.FromCentimeter(contentWidth.Centimeter * 0.125);

            table = section.AddTable();
            table.AddColumn(designCheckTableColumn1Width);
            table.AddColumn(designCheckTableColumn2Width);
            table.AddColumn(designCheckTableColumn3Width);
            table.AddColumn(designCheckTableColumn4Width);
            table.AddColumn(designCheckTableColumn5Width);
            table.Rows.Alignment = RowAlignment.Center;

            row = table.AddRow();
            row.HeadingFormat = true;
            row.Shading.Color = PdfHelper.pantoneCoolGrey;
            row.Cells[0].AddParagraph(_model.ItemNameColText).Style = headerCellStyle.Name;
            row.Cells[1].AddParagraph(_model.ItemUnitsOfMeasurementColText).Style = headerCellStyle.Name;
            row.Cells[2].AddParagraph(_model.ItemPresentColText).Style = headerCellStyle.Name;
            row.Cells[3].AddParagraph(_model.ItemGuaranteedValueColText).Style = headerCellStyle.Name;
            row.Cells[4].AddParagraph(_model.ItemUtilisationColText).Style = headerCellStyle.Name;

            List<DesignCheckPdfData> designCheckPdfData = new List<DesignCheckPdfData>
            {
                new DesignCheckPdfData(
                    _model.Value("anchorScrewForceText", fallback: Fallback.ToLanguage).ToString(),
                    _model.Value("unit1", fallback: Fallback.ToDefaultValue).ToString(),
                    totalMaxForce,
                    anchorScrewPull,
                    designCheck.AnchorScrewForce),
                new DesignCheckPdfData(
                    _model.Value("necessaryPrestressForceText", fallback: Fallback.ToLanguage).ToString(),
                    _model.Value("unit4", fallback: Fallback.ToDefaultValue).ToString(),
                    necessaryPrestress,
                    minPrestressForce,
                    designCheck.NecessaryPrestress),
                new DesignCheckPdfData(_model.Value("forceInFixedBracketText", fallback: Fallback.ToLanguage).ToString(),
                    _model.Value("unit1", fallback: Fallback.ToDefaultValue).ToString(),
                    maxForceFixedBrackets,
                    strengthFixedBrackets,
                    designCheck.ForceInFixedBrackets,
                    true),
                new DesignCheckPdfData(
                    _model.Value("forceInSlidingBracketText", fallback: Fallback.ToLanguage).ToString(),
                    _model.Value("unit1", fallback: Fallback.ToDefaultValue).ToString(),
                    maxForceSlidingBrackets,
                    strengthSlidingBrackets,
                    designCheck.ForceInSlidingBrackets,
                    true),
                new DesignCheckPdfData(
                    _model.Value("bendingInTProfileText", fallback: Fallback.ToLanguage).ToString(),
                    _model.Value("unit2", fallback: Fallback.ToDefaultValue).ToString(),
                    bendingMomentTProfile,
                    strengthTProfile,
                    designCheck.BendingTProfile,
                    true),
            };

            if (!isMulti)
            {
                designCheckPdfData = designCheckPdfData.Where(dataItem => dataItem.IsMulti == false).ToList();
            }

            designCheckPdfData = designCheckPdfData.Where(dataItem => !string.IsNullOrEmpty(dataItem.Value)).ToList();

            foreach (DesignCheckPdfData designCheckDataItem in designCheckPdfData)
            {
                row = table.AddRow();
                row.Shading.Color = shouldBeGray ? PdfHelper.ligthGray : PdfHelper.white;
                row.Cells[0].AddParagraph(designCheckDataItem.DictionaryName).Style = cellStyle.Name;
                row.Cells[1].AddParagraph(designCheckDataItem.DictionaryUnit).Style = cellStyle.Name;
                row.Cells[2].AddParagraph(designCheckDataItem.Presented.ToString(CultureInfo.InvariantCulture)).Style = cellStyle.Name;
                row.Cells[3].AddParagraph(designCheckDataItem.Guaranted.ToString(CultureInfo.InvariantCulture)).Style = cellStyle.Name;
                row.Cells[4].AddParagraph(designCheckDataItem.Value.ToString(CultureInfo.InvariantCulture)).Style = cellStyle.Name;
                shouldBeGray = !shouldBeGray;
            }
            #endregion

            #region tables space 
            PdfHelper.GetTableSpace(section);
            #endregion

            #region input data table
            // data keys
            Field system = FormHelper.GetFormFieldByName(form, "system");
            Field showAllResults = FormHelper.GetFormFieldByName(form, "showAllResults");
            Field weightOfTheCladding = FormHelper.GetFormFieldByName(form, "weightOfTheCladding");
            Field anchorScrewDesignPulOutStrength = FormHelper.GetFormFieldByName(form, "anchorScrewDesignPulOutStrength");
            Field anchorScrewPullOwnValue = FormHelper.GetFormFieldByName(form, "anchorScrewPullOwnValue");
            //Field ownValueAnchorType = FormHelper.GetFormFieldByName(form, "ownValueAnchorType");
            Field frictionCoefficientOfTheBackWall = FormHelper.GetFormFieldByName(form, "frictionCoefficientOfTheBackWall");
            Field frictionCoefficientOwnValue = FormHelper.GetFormFieldByName(form, "frictionCoefficientOwnValue");
            Field terrainCategory = FormHelper.GetFormFieldByName(form, "terrainCategory");
            Field buildingHeight = FormHelper.GetFormFieldByName(form, "buildingHeight");
            Field windSpeed = FormHelper.GetFormFieldByName(form, "windSpeed");
            Field insulationThickness = FormHelper.GetFormFieldByName(form, "insulationThickness");
            Field area = FormHelper.GetFormFieldByName(form, "area");
            Field lengthOfVerticalCorners = FormHelper.GetFormFieldByName(form, "lengthOfVerticalCorners");
            Field lengthOfVerticalSide = FormHelper.GetFormFieldByName(form, "lengthOfVerticalSide");
            Field consequenceClass = FormHelper.GetFormFieldByName(form, "consequenceClass");
            Field baseRailSpacingParameter = FormHelper.GetFormFieldByName(form, "baseRailSpacingParameter");

            Unit inputDataTableColumn1Width = Unit.FromCentimeter(contentWidth.Centimeter * 0.50);
            Unit inputDataTableColumn2Width = Unit.FromCentimeter(contentWidth.Centimeter * 0.25);
            Unit inputDataTableColumn3Width = Unit.FromCentimeter(contentWidth.Centimeter * 0.25);

            bool showAnchorScrewOwnValue =
                formData.AnchorScrewDesign == anchorScrewPullOwnValue.Condition.Rules.Single().Value;

            bool showFrictionCoefficientOwnValue =
                frictionCoefficientOwnValue.Condition.Rules.Any(condition => condition.Value == formData.FrictionCoef);

            string showAllResultsValue = formData.ShowAllResults ? _model.Value("includeVerticalAdjustmentComponentsTextYes", fallback: Fallback.ToLanguage).ToString() :
                _model.Value("includeVerticalAdjustmentComponentsTextNo", fallback: Fallback.ToLanguage).ToString();

            List<InputPdfData> inputPdfData = new List<InputPdfData>
            {
                new InputPdfData(
                    system,
                    "",
                    formData.SystemType),
                new InputPdfData(
                    showAllResults,
                    "",
                    formData.ShowAllResults,
                    "",
                    showAllResultsValue,
                    isMulti),
                new InputPdfData(
                    weightOfTheCladding,
                    _model.Value("unit5", fallback: Fallback.ToLanguage).ToString(),
                    formData.CladdingWeight),
                new InputPdfData(
                    anchorScrewDesignPulOutStrength,
                    "",
                    formData.AnchorScrewDesign),
                new InputPdfData(
                    anchorScrewDesignPulOutStrength,
                    _model.Value("unit1", fallback: Fallback.ToLanguage).ToString(),
                    formData.AnchorScrewDesignOwnValue,
                    $" ({formData.AnchorScrewDesign.ToLower()})",
                    "",
                    showAnchorScrewOwnValue),
                new InputPdfData(anchorScrewDesignPulOutStrength,
                    _model.Value("unit1", fallback: Fallback.ToLanguage).ToString(),
                    formData.AnchorScrewDesignLCWType,
                    $" ({formData.AnchorScrewDesign.ToLower()} - type)",
                    "",
                    showAnchorScrewOwnValue),
                new InputPdfData(
                    frictionCoefficientOfTheBackWall,
                    "(-)",
                    formData.FrictionCoef),
                new InputPdfData(
                    frictionCoefficientOfTheBackWall,
                    "(-)",
                    formData.FrictionCoefOwnValue,
                    $" ({formData.FrictionCoef.ToLower()})",
                    "",
                    showFrictionCoefficientOwnValue),
                new InputPdfData(
                    terrainCategory,
                    "",
                    formData.TerrainCategory),
                new InputPdfData(
                    buildingHeight,
                    $"({_model.Value("unitM", fallback: Fallback.ToLanguage)})",
                    formData.Height),
                new InputPdfData(
                    windSpeed,
                    "(m/s)",
                    _calculatedData.WindSpeed),
                new InputPdfData(
                    insulationThickness,
                    "(mm)",
                    formData.InsulationThickness),
                new InputPdfData(
                    area,
                    "(m2)",
                    formData.Area),
                new InputPdfData(lengthOfVerticalCorners,
                    $"({_model.Value("unitM", fallback: Fallback.ToLanguage)})",
                    formData.LengthCorners),
                new InputPdfData(
                    lengthOfVerticalSide,
                    $"({_model.Value("unitM", fallback: Fallback.ToLanguage)})",
                    formData.LengthDoorsWindowsSide),
                new InputPdfData(
                    consequenceClass,
                    "",
                    formData.ConsequenceClass),
                new InputPdfData(
                    baseRailSpacingParameter,
                    "(mm)",
                    formData.BaseRailSpacing)
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

            #region end text
            PdfHelper.GetEndText(section, htmlDocument, _model.EndText);
            #endregion

            #region document save
            string fileNameWithExt = PdfHelper.SaveDocument(document, _downloadPath, _entryGuid);
            #endregion

            return fileNameWithExt;
        }

        private void AddProductRow(
            ProductDto item,
            Row row,
            Style sapNumberStyle,
            Style dbNumberStyle,
            Style amountStyle,
            Style firstProductLineStyle,
            Style middleProductLineStyle,
            Style lastProductLineStyle,
            bool IsExclusiveFlexNo = false)
        {
            IEnumerable<IPublishedContent> rootNodes = null;
            IPublishedContent calculatorPage = null;
            string norwegianUnitName = string.Empty;
            string norwegianComponentName = string.Empty;
            string norwegianQuantityDelivered = string.Empty;

            if (IsExclusiveFlexNo)
            {
                rootNodes = _context.EnsureUmbracoContext().UmbracoContext.ContentCache.GetAtRoot().First().Children();
                calculatorPage = ContentFinder.GetCurrentCalculatorPage(0, rootNodes);
                norwegianUnitName = calculatorPage.Value("unitName").ToString();
                norwegianComponentName = calculatorPage.Value("componentName").ToString();
                norwegianQuantityDelivered = calculatorPage.Value("quantityDeliveredMessage").ToString();
            }

            if (IsExclusiveFlexNo)
            {
                row.Cells[0].AddParagraph("").Style = sapNumberStyle.Name;
                row.Cells[1].AddParagraph(item.Nessasary.Value.ToString()).Style = amountStyle.Name;
            }
            else
            {
                row.Cells[0].AddParagraph(item.SAPNumber.ToString()).Style = sapNumberStyle.Name;
                if (item.HasDbNumber)
                {
                    row.Cells[0].AddParagraph(item.DBLabel + ": " + item.DBNumber).Style = dbNumberStyle.Name;
                }
                row.Cells[1].AddParagraph(item.Amount.ToString()).Style = amountStyle.Name;
            }

            // first product line
            bool isDigitPresentInUnitsPerPackage = item.UnitsPerPackage.Name.Any(char.IsDigit);
            if (isDigitPresentInUnitsPerPackage)
            {
                var unitsForAmount = item.Amount == 1 ? item.UnitInSingular : item.Units;
                Paragraph firstProductLine = row.Cells[2].AddParagraph();
                FormattedText firstProductLineFormattedText = firstProductLine.AddFormattedText(
                    "(" + unitsForAmount + ")" +
                    " " + item.Name +
                    " (" + item.UnitsPerPackage.Value + " ");
                string unitDigit = Regex.Match(item.UnitsPerPackage.Name, @"\d+").Value;
                firstProductLine.AddFormattedText(item.UnitsPerPackage.Name.Replace(unitDigit, ""));
                firstProductLineFormattedText = firstProductLine.AddFormattedText(unitDigit);
                firstProductLineFormattedText.Superscript = true;
                firstProductLine.AddFormattedText(" / " + item.UnitInSingular + ")");
                firstProductLine.Style = firstProductLineStyle.Name;
            }
            else
            {
                var unitsForAmount = item.Amount == 1 ? item.UnitInSingular : item.Units;
                if (IsExclusiveFlexNo)
                {
                    row.Cells[2].AddParagraph(
                    "(" + norwegianUnitName + ")" +
                    " " + norwegianComponentName).Style = firstProductLineStyle.Name;
                }
                else
                {
                    row.Cells[2].AddParagraph(
                    "(" + unitsForAmount + ")" +
                    " " + item.Name +
                    " (" + item.UnitsPerPackage.Value + " " + item.UnitsPerPackage.Name + " / " + item.UnitInSingular + ")").Style = firstProductLineStyle.Name;
                }
            }

            // middle product line
            bool isDigitPresentInNessasaryName = item.Nessasary.Name.Any(char.IsDigit);
            if (isDigitPresentInNessasaryName)
            {
                Paragraph middleProductLine = row.Cells[2].AddParagraph();
                FormattedText middleProductLineFormattedText = middleProductLine.AddFormattedText(
                    _model.Value("estimatedAmount", fallback: Fallback.ToLanguage) +
                    ": " + item.Nessasary.Value +
                    " ");
                string unitDigit = Regex.Match(item.Nessasary.Name, @"\d+").Value;
                middleProductLine.AddFormattedText(item.Nessasary.Name.Replace(unitDigit, ""));
                middleProductLineFormattedText = middleProductLine.AddFormattedText(unitDigit);
                middleProductLineFormattedText.Superscript = true;
                middleProductLine.Style = middleProductLineStyle.Name;
            }
            else
            {
                row.Cells[2].AddParagraph(
                    _model.Value("estimatedAmount", fallback: Fallback.ToLanguage) +
                    ": " + item.Nessasary.Value +
                    " " + item.Nessasary.Name).Style = middleProductLineStyle.Name;
            }

            // last product line
            bool isDigitPresentInPackagingName = item.InPackaging.Name.Any(char.IsDigit);
            if (isDigitPresentInPackagingName)
            {
                Paragraph lastProductLine = row.Cells[2].AddParagraph();
                FormattedText lastProductLineFormattedText = lastProductLine.AddFormattedText(
                    _model.Value("quantityDelivered", fallback: Fallback.ToLanguage) +
                    ": " + item.InPackaging.Value +
                    " ");
                string unitDigit = Regex.Match(item.InPackaging.Name, @"\d+").Value;
                lastProductLine.AddFormattedText(item.InPackaging.Name.Replace(unitDigit, ""));
                lastProductLineFormattedText = lastProductLine.AddFormattedText(unitDigit);
                lastProductLineFormattedText.Superscript = true;
                lastProductLine.Style = lastProductLineStyle.Name;
            }
            else
            {
                if (IsExclusiveFlexNo)
                {
                    row.Cells[2].AddParagraph(
                    _model.Value("quantityDelivered", fallback: Fallback.ToLanguage) +
                    ": " + norwegianQuantityDelivered).Style = lastProductLineStyle.Name;
                }
                else
                {
                    row.Cells[2].AddParagraph(
                    _model.Value("quantityDelivered", fallback: Fallback.ToLanguage) +
                    ": " + item.InPackaging.Value +
                    " " + item.InPackaging.Name).Style = lastProductLineStyle.Name;
                }
            }
        }
    }
}