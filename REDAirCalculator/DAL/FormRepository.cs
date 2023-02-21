using System;
using REDAirCalculator.Models.DTO;
using REDAirCalculator.Utilities;
using Serilog;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Persistence.Dtos;

namespace REDAirCalculator.DAL
{
    public interface IFormRepository
    {
        FormDataDto GetFlexMultiSubmittedEntry(Guid formGuid, object entryObjectGuid);
        LinkFormDataDto GetLinkSubmittedEntry(Guid formGuid, object entryObjectGuid);
    }

    public class FormRepository : IFormRepository
    {
        public FormDataDto GetFlexMultiSubmittedEntry(Guid formGuid, object entryObjectGuid)
        {
            Guid entryGuid = FormHelper.GetSubmittedEntryGuid(entryObjectGuid);
            Form form = FormHelper.GetForm(formGuid);
            Record entry = FormHelper.GetEntry(entryGuid, form);

            if (entry == null)
            {
                Log.Information("no entry rendered");
            }

            FormDataDto formDataDto = new FormDataDto();
            {
                // project data
                formDataDto.ProjectName = FormHelper.GetEntryFieldByName(entry, "projectName");
                formDataDto.ProjectDescription = FormHelper.GetEntryFieldByName(entry, "projectDescription");
                formDataDto.CustomerName = FormHelper.GetEntryFieldByName(entry, "customerName");
                formDataDto.City = FormHelper.GetEntryFieldByName(entry, "city");
                formDataDto.Address = FormHelper.GetEntryFieldByName(entry, "address");
                formDataDto.PostIndex = FormHelper.GetEntryFieldByName(entry, "postIndex");

                formDataDto.WindSpeedArea = FormHelper.GetEntryFieldByName(entry, "windSpeedArea");

                // input data
                formDataDto.SystemType = FormHelper.GetEntryFieldByName(entry, "system");
                formDataDto.ShowAllResults = Convert.ToBoolean(FormHelper.GetEntryFieldByName(entry, "showAllResults"));
                formDataDto.InsulationThickness = FormHelper.GetEntryFieldByName(entry, "insulationThickness");
                formDataDto.InsulationThicknessMmFlex = FormHelper.GetEntryFieldByName(entry, "insulationThicknessMmFlex");
                formDataDto.CladdingWeight = FormHelper.GetEntryFieldByName(entry, "weightOfTheCladding", true);
                formDataDto.AnchorScrewDesign = entry.GetRecordFieldByAlias("anchorScrewDesignPulOutStrength").Values[0].ToString(); // for fixing issue with converting "/" to "V"
                formDataDto.AnchorScrewDesignOwnValue = FormHelper.GetEntryFieldByName(entry, "anchorScrewPullOwnValue", true) ?? 0;
                formDataDto.AnchorScrewDesignLCWType = FormHelper.GetEntryFieldByName(entry, "ownValueAnchorType");
                formDataDto.TerrainCategory = FormHelper.GetEntryFieldByName(entry, "terrainCategory");
                formDataDto.Height = FormHelper.GetEntryFieldByName(entry, "buildingHeight", true);
                formDataDto.Vbo = FormHelper.GetEntryFieldByName(entry, "windSpeed", true) ?? 0;
                formDataDto.Area = FormHelper.GetEntryFieldByName(entry, "area", true);
                formDataDto.ConsequenceClass = FormHelper.GetEntryFieldByName(entry, "consequenceClass");
                formDataDto.BaseRailSpacing = FormHelper.GetEntryFieldByName(entry, "baseRailSpacingParameter");
                formDataDto.AdvancedField = Convert.ToBoolean(FormHelper.GetEntryFieldByName(entry, "advancedField"));
                formDataDto.FrictionCoef = FormHelper.GetEntryFieldByName(entry, "frictionCoefficientOfTheBackWall");
                formDataDto.FrictionCoefOwnValue = FormHelper.GetEntryFieldByName(entry, "frictionCoefficientOwnValue", true) ?? 0;
                formDataDto.LengthCorners = FormHelper.GetEntryFieldByName(entry, "lengthOfVerticalCorners", true);
                formDataDto.LengthDoorsWindowsSide = FormHelper.GetEntryFieldByName(entry, "lengthOfVerticalSide", true);
                formDataDto.NumberOfLayers = FormHelper.GetEntryFieldByName(entry, "numberOfLayers");
            }

            return formDataDto;
        }

        public LinkFormDataDto GetLinkSubmittedEntry(Guid formGuid, object entryObjectGuid)
        {
            Guid entryGuid = FormHelper.GetSubmittedEntryGuid(entryObjectGuid);
            Form form = FormHelper.GetForm(formGuid);
            Record entry = FormHelper.GetEntry(entryGuid, form);

            if (entry == null)
            {
                Log.Information("no entry rendered");
            }

            string windSpeedAreaValue = "";

            try{
                windSpeedAreaValue = FormHelper.GetEntryFieldByName(entry, "windSpeedArea");
            }
            catch{}

            LinkFormDataDto formData = new LinkFormDataDto
            {
                ProjectName = FormHelper.GetEntryFieldByName(entry, "projectName"),
                ProjectDescription = FormHelper.GetEntryFieldByName(entry, "projectDescription"),
                CustomerName = FormHelper.GetEntryFieldByName(entry, "customerName"),
                City = FormHelper.GetEntryFieldByName(entry, "city"),
                Address = FormHelper.GetEntryFieldByName(entry, "address"),
                PostIndex = FormHelper.GetEntryFieldByName(entry, "postIndex"),
                WindSpeedArea = windSpeedAreaValue,

                PlankDepth = FormHelper.GetEntryFieldByName(entry, "plankDepth", true),
                PrecutPlanks = Convert.ToBoolean(FormHelper.GetEntryFieldByName(entry, "precutPlanks")),
                OpeningTypes = FormHelper.GetOpeningTypes(entry)
            };

            return formData;
        }
    }

}