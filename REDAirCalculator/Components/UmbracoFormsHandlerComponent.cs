using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;
using REDAirCalculator.DAL;
using REDAirCalculator.Models.DTO;
using REDAirCalculator.Utilities;
using Umbraco.Core.Composing;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Forms.Web.Controllers;
using Umbraco.Web;
using Umbraco.Web.PublishedModels;

namespace REDAirCalculator.Components
{
    public class UmbracoFormsHandlerComponent : IComponent
    {
        private readonly IFormRepository _formRepository;

        public UmbracoFormsHandlerComponent(IFormRepository formRepository)
        {
            _formRepository = formRepository;
        }


        public void Initialize()
        {
            UmbracoFormsController.FormPrePopulate += UmbracoFormsController_FormPrePopulate;
        }

        public void Terminate()
        {
        }

        private void UmbracoFormsController_FormPrePopulate(object sender, Umbraco.Forms.Core.FormEventArgs e)
        {
            UmbracoFormsController formSender = (UmbracoFormsController)sender;
            var currentEntryObjectGuid = formSender.TempData["Forms_Current_Record_id"];
          
            if (currentEntryObjectGuid == null) return;

            var projectsType = (int)formSender.TempData["projectsType"];

            if (projectsType == 1)
            {
                UpdateLinkData(e, currentEntryObjectGuid);
            }
            else if(projectsType == 0)
            {
                UpdateFlexMultiData(e, currentEntryObjectGuid);
            }

        }

        private void UpdateFlexMultiData(Umbraco.Forms.Core.FormEventArgs e, object currentEntryObjectGuid)
        {
            string _lang = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            FormDataDto currentEntry = _formRepository.GetFlexMultiSubmittedEntry(e.Form.Id, currentEntryObjectGuid);

            // updating of calculator form on resulter page
            // project data
            FormHelper.GetFormFieldByName(e.Form, "projectName").Values = new List<object>() { currentEntry.ProjectName.Replace("\\", "") };
            FormHelper.GetFormFieldByName(e.Form, "projectDescription").Values = new List<object>() { (currentEntry.ProjectDescription ?? "").Replace("\\", "") };
            FormHelper.GetFormFieldByName(e.Form, "customerName").Values = new List<object>() { (currentEntry.CustomerName ?? "").Replace("\\", "") };
            FormHelper.GetFormFieldByName(e.Form, "city").Values = new List<object>() { (currentEntry.City ?? "").Replace("\\", "") };
            FormHelper.GetFormFieldByName(e.Form, "address").Values = new List<object>() { (currentEntry.Address ?? "").Replace("\\", "") };
            FormHelper.GetFormFieldByName(e.Form, "postIndex").Values = new List<object>() { (currentEntry.PostIndex ?? "").Replace("\\", "") };

            if (currentEntry.WindSpeedArea != null)
            {
                FormHelper.GetFormFieldByName(e.Form, "windSpeedArea").Values = new List<object>() { currentEntry.WindSpeedArea };
            }

            // input data
            FormHelper.GetFormFieldByName(e.Form, "system").Values = new List<object>() { currentEntry.SystemType };
            FormHelper.GetFormFieldByName(e.Form, "showAllResults").Values = new List<object>() { currentEntry.ShowAllResults };
            FormHelper.GetFormFieldByName(e.Form, "baseRailSpacingParameter").Values = new List<object>() { currentEntry.BaseRailSpacing };
            if (currentEntry.SystemType == "FLEX" && currentEntry.InsulationThicknessMmFlex != null && _lang == "no")
            {
                FormHelper.GetFormFieldByName(e.Form, "insulationThickness").Values = new List<object>() { currentEntry.InsulationThicknessMmFlex };
                FormHelper.GetFormFieldByName(e.Form, "insulationThicknessMmFlex").Values = new List<object>() { currentEntry.InsulationThicknessMmFlex };
            }
            else
            {
                FormHelper.GetFormFieldByName(e.Form, "insulationThickness").Values = new List<object>() { currentEntry.InsulationThickness };
                if (_lang == "no" && currentEntry.InsulationThickness == "350")
                {
                    FormHelper.GetFormFieldByName(e.Form, "insulationThicknessMmFlex").Values = new List<object>() { "300" };
                }
                else
                {
                    FormHelper.GetFormFieldByName(e.Form, "insulationThicknessMmFlex").Values = new List<object>() { currentEntry.InsulationThickness };
                }
            }
            //FormHelper.GetFormFieldByName(e.Form, "insulationThickness").Values = new List<object>() { currentEntry.InsulationThickness }; ;
            FormHelper.GetFormFieldByName(e.Form, "weightOfTheCladding").Values = new List<object>() { currentEntry.CladdingWeight.ToString(new CultureInfo("da-DK")) }; ;
            FormHelper.GetFormFieldByName(e.Form, "anchorScrewDesignPulOutStrength").Values = new List<object>() { currentEntry.AnchorScrewDesign };
            FormHelper.GetFormFieldByName(e.Form, "anchorScrewPullOwnValue").Values = new List<object>() { currentEntry.AnchorScrewDesignOwnValue.ToString(new CultureInfo("da-DK")) };
            FormHelper.GetFormFieldByName(e.Form, "ownValueAnchorType").Values = new List<object>() { currentEntry.AnchorScrewDesignLCWType };
            FormHelper.GetFormFieldByName(e.Form, "terrainCategory").Values = new List<object>() { currentEntry.TerrainCategory };
            FormHelper.GetFormFieldByName(e.Form, "buildingHeight").Values = new List<object>() { currentEntry.Height.ToString(new CultureInfo("da-DK")) };
            FormHelper.GetFormFieldByName(e.Form, "windSpeed").Values = new List<object>() { currentEntry.Vbo.ToString(new CultureInfo("da-DK")) };
            FormHelper.GetFormFieldByName(e.Form, "area").Values = new List<object>() { currentEntry.Area.ToString(new CultureInfo("da-DK")) };
            FormHelper.GetFormFieldByName(e.Form, "consequenceClass").Values = new List<object>() { currentEntry.ConsequenceClass };
            FormHelper.GetFormFieldByName(e.Form, "advancedField").Values = new List<object>() { currentEntry.AdvancedField };
            FormHelper.GetFormFieldByName(e.Form, "frictionCoefficientOfTheBackWall").Values = new List<object>() { currentEntry.FrictionCoef };
            FormHelper.GetFormFieldByName(e.Form, "frictionCoefficientOwnValue").Values = new List<object>() { currentEntry.FrictionCoefOwnValue.ToString(new CultureInfo("da-DK")) }; ;
            FormHelper.GetFormFieldByName(e.Form, "lengthOfVerticalCorners").Values = new List<object>() { currentEntry.LengthCorners.ToString(new CultureInfo("da-DK")) };
            FormHelper.GetFormFieldByName(e.Form, "lengthOfVerticalSide").Values = new List<object>() { currentEntry.LengthDoorsWindowsSide.ToString(new CultureInfo("da-DK")) };
            FormHelper.GetFormFieldByName(e.Form, "numberOfLayers").Values = new List<object>() { currentEntry.NumberOfLayers };
        }

        private void UpdateLinkData(Umbraco.Forms.Core.FormEventArgs e, object currentEntryObjectGuid)
        {
            LinkFormDataDto currentEntry = _formRepository.GetLinkSubmittedEntry(e.Form.Id, currentEntryObjectGuid);

            // updating of calculator form on resulter page
            // project data
            FormHelper.GetFormFieldByName(e.Form, "projectName").Values = new List<object>() { currentEntry.ProjectName.Replace("\\", "") };
            FormHelper.GetFormFieldByName(e.Form, "projectDescription").Values = new List<object>() { (currentEntry.ProjectDescription ?? "").Replace("\\", "") };
            FormHelper.GetFormFieldByName(e.Form, "customerName").Values = new List<object>() { (currentEntry.CustomerName ?? "").Replace("\\", "") };
            FormHelper.GetFormFieldByName(e.Form, "city").Values = new List<object>() { (currentEntry.City ?? "").Replace("\\", "") };
            FormHelper.GetFormFieldByName(e.Form, "address").Values = new List<object>() { (currentEntry.Address ?? "").Replace("\\", "") };
            FormHelper.GetFormFieldByName(e.Form, "postIndex").Values = new List<object>() { (currentEntry.PostIndex ?? "").Replace("\\", "") };
            FormHelper.GetFormFieldByName(e.Form, "windSpeedArea").Values = new List<object>() { (currentEntry.WindSpeedArea ?? "").Replace("\\", "") };

            //project input
            FormHelper.GetFormFieldByName(e.Form, "plankDepth").Values = new List<object>() { currentEntry.PlankDepth.ToString(new CultureInfo("da-DK")) };
            FormHelper.GetFormFieldByName(e.Form, "precutPlanks").Values = new List<object>() { currentEntry.PrecutPlanks };
            FormHelper.GetFormFieldByName(e.Form, "openingTypes").Values = new List<object>() { currentEntry.OpeningTypes };
        }
    }
}