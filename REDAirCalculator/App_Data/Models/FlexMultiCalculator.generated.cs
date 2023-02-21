//------------------------------------------------------------------------------
// <auto-generated>
//   This code was generated by a tool.
//
//    Umbraco.ModelsBuilder v8.0.4
//
//   Changes to this file will be lost if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
using Umbraco.ModelsBuilder;
using Umbraco.ModelsBuilder.Umbraco;

namespace Umbraco.Web.PublishedModels
{
	/// <summary>Calculator</summary>
	[PublishedModel("flexMultiCalculator")]
	public partial class FlexMultiCalculator : Common
	{
		// helpers
#pragma warning disable 0109 // new is redundant
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		public new const string ModelTypeAlias = "flexMultiCalculator";
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		public new const PublishedItemType ModelItemType = PublishedItemType.Content;
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		public new static PublishedContentType GetModelContentType()
			=> PublishedModelUtility.GetModelContentType(ModelItemType, ModelTypeAlias);
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		public static PublishedPropertyType GetModelPropertyType<TValue>(Expression<Func<FlexMultiCalculator, TValue>> selector)
			=> PublishedModelUtility.GetModelPropertyType(GetModelContentType(), selector);
#pragma warning restore 0109

		// ctor
		public FlexMultiCalculator(IPublishedContent content)
			: base(content)
		{ }

		// properties

		///<summary>
		/// Accept conditions text
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("acceptConditionsText")]
		public IHtmlString AcceptConditionsText => this.Value<IHtmlString>("acceptConditionsText");

		///<summary>
		/// Address tooltip
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("addressTooltip")]
		public IHtmlString AddressTooltip => this.Value<IHtmlString>("addressTooltip");

		///<summary>
		/// Anchor screw design pul out strength tooltip
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("anchorScrewDesignPulOutStrengthTooltip")]
		public IHtmlString AnchorScrewDesignPulOutStrengthTooltip => this.Value<IHtmlString>("anchorScrewDesignPulOutStrengthTooltip");

		///<summary>
		/// Anchor Screw Distance
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("anchorScrewDistance")]
		public string AnchorScrewDistance => this.Value<string>("anchorScrewDistance");

		///<summary>
		/// Anchor screw force text
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("anchorScrewForceText")]
		public string AnchorScrewForceText => this.Value<string>("anchorScrewForceText");

		///<summary>
		/// Anchor screw pull out strength text
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("anchorScrewPullOutStrengthText")]
		public string AnchorScrewPullOutStrengthText => this.Value<string>("anchorScrewPullOutStrengthText");

		///<summary>
		/// Anchor screw pull own value tooltip
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("anchorScrewPullOwnValueTooltip")]
		public IHtmlString AnchorScrewPullOwnValueTooltip => this.Value<IHtmlString>("anchorScrewPullOwnValueTooltip");

		///<summary>
		/// Area tooltip
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("areaTooltip")]
		public IHtmlString AreaTooltip => this.Value<IHtmlString>("areaTooltip");

		///<summary>
		/// Bag
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("bag")]
		public string Bag => this.Value<string>("bag");

		///<summary>
		/// Bags
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("bags")]
		public string Bags => this.Value<string>("bags");

		///<summary>
		/// Base Rail Spacing
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("baseRailSpacing")]
		public string BaseRailSpacing => this.Value<string>("baseRailSpacing");

		///<summary>
		/// Base rail spacing parameter tooltip
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("baseRailSpacingParameterTooltip")]
		public IHtmlString BaseRailSpacingParameterTooltip => this.Value<IHtmlString>("baseRailSpacingParameterTooltip");

		///<summary>
		/// Bending in T-profile text
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("bendingInTProfileText")]
		public string BendingInTprofileText => this.Value<string>("bendingInTProfileText");

		///<summary>
		/// Bending moment in T-profile text
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("bendingMomentInTProfileText")]
		public string BendingMomentInTprofileText => this.Value<string>("bendingMomentInTProfileText");

		///<summary>
		/// Box
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("box")]
		public string Box => this.Value<string>("box");

		///<summary>
		/// Boxes
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("boxes")]
		public string Boxes => this.Value<string>("boxes");

		///<summary>
		/// Building height tooltip
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("buildingHeightTooltip")]
		public IHtmlString BuildingHeightTooltip => this.Value<IHtmlString>("buildingHeightTooltip");

		///<summary>
		/// Bundle
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("bundle")]
		public string Bundle => this.Value<string>("bundle");

		///<summary>
		/// Bundles
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("bundles")]
		public string Bundles => this.Value<string>("bundles");

		///<summary>
		/// Button text
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("calculateButtonText")]
		public string CalculateButtonText => this.Value<string>("calculateButtonText");

		///<summary>
		/// Calculated on text
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("calculatedOnText")]
		public string CalculatedOnText => this.Value<string>("calculatedOnText");

		///<summary>
		/// Tilte
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("calculatorPageTilte")]
		public string CalculatorPageTilte => this.Value<string>("calculatorPageTilte");

		///<summary>
		/// City tooltip
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("cityTooltip")]
		public IHtmlString CityTooltip => this.Value<IHtmlString>("cityTooltip");

		///<summary>
		/// ComponentName
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("componentName")]
		public string ComponentName => this.Value<string>("componentName");

		///<summary>
		/// Compression depth (flex)
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("compressionDepthFlex")]
		public string CompressionDepthFlex => this.Value<string>("compressionDepthFlex");

		///<summary>
		/// Compression depth (multi)
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("compressionDepthMulti")]
		public string CompressionDepthMulti => this.Value<string>("compressionDepthMulti");

		///<summary>
		/// Consequence class tooltip
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("consequenceClassTooltip")]
		public IHtmlString ConsequenceClassTooltip => this.Value<IHtmlString>("consequenceClassTooltip");

		///<summary>
		/// Customer name tooltip
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("customerNameTooltip")]
		public IHtmlString CustomerNameTooltip => this.Value<IHtmlString>("customerNameTooltip");

		///<summary>
		/// Date text
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("dateText")]
		public string DateText => this.Value<string>("dateText");

		///<summary>
		/// Description Text
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("descriptionText")]
		public IHtmlString DescriptionText => this.Value<IHtmlString>("descriptionText");

		///<summary>
		/// Design check text
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("designCheckText")]
		public string DesignCheckText => this.Value<string>("designCheckText");

		///<summary>
		/// End text
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("endText")]
		public IHtmlString EndText => this.Value<IHtmlString>("endText");

		///<summary>
		/// Estimated amount
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("estimatedAmount")]
		public string EstimatedAmount => this.Value<string>("estimatedAmount");

		///<summary>
		/// Force in fixed bracket text
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("forceInFixedBracketText")]
		public string ForceInFixedBracketText => this.Value<string>("forceInFixedBracketText");

		///<summary>
		/// Force in sliding bracket text
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("forceInSlidingBracketText")]
		public string ForceInSlidingBracketText => this.Value<string>("forceInSlidingBracketText");

		///<summary>
		/// For multi only text
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("forMultiOnlyText")]
		public string ForMultiOnlyText => this.Value<string>("forMultiOnlyText");

		///<summary>
		/// Friction coefficient of the back wall tooltip
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("frictionCoefficientOfTheBackWallTooltip")]
		public IHtmlString FrictionCoefficientOfTheBackWallTooltip => this.Value<IHtmlString>("frictionCoefficientOfTheBackWallTooltip");

		///<summary>
		/// Friction coefficient own value tooltip
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("frictionCoefficientOwnValueTooltip")]
		public IHtmlString FrictionCoefficientOwnValueTooltip => this.Value<IHtmlString>("frictionCoefficientOwnValueTooltip");

		///<summary>
		/// Include vertical adjustment components text "no"
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("includeVerticalAdjustmentComponentsTextNo")]
		public string IncludeVerticalAdjustmentComponentsTextNo => this.Value<string>("includeVerticalAdjustmentComponentsTextNo");

		///<summary>
		/// Include vertical adjustment components text "yes"
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("includeVerticalAdjustmentComponentsTextYes")]
		public string IncludeVerticalAdjustmentComponentsTextYes => this.Value<string>("includeVerticalAdjustmentComponentsTextYes");

		///<summary>
		/// Input text
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("inputText")]
		public string InputText => this.Value<string>("inputText");

		///<summary>
		/// Insulation Settings text
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("insulationSettingsText")]
		public string InsulationSettingsText => this.Value<string>("insulationSettingsText");

		///<summary>
		/// Insulation thickness tooltip
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("insulationThicknessTooltip")]
		public IHtmlString InsulationThicknessTooltip => this.Value<IHtmlString>("insulationThicknessTooltip");

		///<summary>
		/// Item description col text
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("itemDescriptionColText")]
		public string ItemDescriptionColText => this.Value<string>("itemDescriptionColText");

		///<summary>
		/// Item guaranteed value col text
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("itemGuaranteedValueColText")]
		public string ItemGuaranteedValueColText => this.Value<string>("itemGuaranteedValueColText");

		///<summary>
		/// Item name col text
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("itemNameColText")]
		public string ItemNameColText => this.Value<string>("itemNameColText");

		///<summary>
		/// Item number col text
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("itemNumberColText")]
		public string ItemNumberColText => this.Value<string>("itemNumberColText");

		///<summary>
		/// Item present col text
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("itemPresentColText")]
		public string ItemPresentColText => this.Value<string>("itemPresentColText");

		///<summary>
		/// Item product number col text
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("itemProductNumberColText")]
		public string ItemProductNumberColText => this.Value<string>("itemProductNumberColText");

		///<summary>
		/// Items count col text
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("itemsCountColText")]
		public string ItemsCountColText => this.Value<string>("itemsCountColText");

		///<summary>
		/// Item units of measurement col text
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("itemUnitsOfMeasurementColText")]
		public string ItemUnitsOfMeasurementColText => this.Value<string>("itemUnitsOfMeasurementColText");

		///<summary>
		/// Item utilisation col text
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("itemUtilisationColText")]
		public string ItemUtilisationColText => this.Value<string>("itemUtilisationColText");

		///<summary>
		/// Length of vertical corners tooltip
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("lengthOfVerticalCornersTooltip")]
		public IHtmlString LengthOfVerticalCornersTooltip => this.Value<IHtmlString>("lengthOfVerticalCornersTooltip");

		///<summary>
		/// Length of vertical side tooltip
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("lengthOfVerticalSideTooltip")]
		public IHtmlString LengthOfVerticalSideTooltip => this.Value<IHtmlString>("lengthOfVerticalSideTooltip");

		///<summary>
		/// Load calculations text
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("loadCalculationsText")]
		public string LoadCalculationsText => this.Value<string>("loadCalculationsText");

		///<summary>
		/// Max distance between brackets
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("maxDistanceBetweenBrackets")]
		public string MaxDistanceBetweenBrackets => this.Value<string>("maxDistanceBetweenBrackets");

		///<summary>
		/// Max force in anchor screw from prestress text
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("maxForceInAnchorScrewFromPrestressText")]
		public string MaxForceInAnchorScrewFromPrestressText => this.Value<string>("maxForceInAnchorScrewFromPrestressText");

		///<summary>
		/// Max force in anchor screw from selfweight text
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("maxForceInAnchorScrewFromSelfweightText")]
		public string MaxForceInAnchorScrewFromSelfweightText => this.Value<string>("maxForceInAnchorScrewFromSelfweightText");

		///<summary>
		/// Max force in anchor screw from wind text
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("maxForceInAnchorScrewFromWindText")]
		public string MaxForceInAnchorScrewFromWindText => this.Value<string>("maxForceInAnchorScrewFromWindText");

		///<summary>
		/// Max horizontal force in fixed bracket text
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("maxHorizontalForceInFixedBracketText")]
		public string MaxHorizontalForceInFixedBracketText => this.Value<string>("maxHorizontalForceInFixedBracketText");

		///<summary>
		/// Max horizontal force in sliding bracket text
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("maxHorizontalForceInSlidingBracketText")]
		public string MaxHorizontalForceInSlidingBracketText => this.Value<string>("maxHorizontalForceInSlidingBracketText");

		///<summary>
		/// Min. prestress force pr. m base rail text
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("minPrestressForcePrMBaseRailText")]
		public string MinPrestressForcePrMbaseRailText => this.Value<string>("minPrestressForcePrMBaseRailText");

		///<summary>
		/// Necessary prestress force text
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("necessaryPrestressForceText")]
		public string NecessaryPrestressForceText => this.Value<string>("necessaryPrestressForceText");

		///<summary>
		/// Necessary prestress text
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("necessaryPrestressText")]
		public string NecessaryPrestressText => this.Value<string>("necessaryPrestressText");

		///<summary>
		/// Own value anchor type tooltip
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("ownValueAnchorTypeTooltip")]
		public IHtmlString OwnValueAnchorTypeTooltip => this.Value<IHtmlString>("ownValueAnchorTypeTooltip");

		///<summary>
		/// Package type
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("packageType")]
		public string PackageType => this.Value<string>("packageType");

		///<summary>
		/// Page field
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("pageField")]
		public string PageField => this.Value<string>("pageField");

		///<summary>
		/// Pallet
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("pallet")]
		public string Pallet => this.Value<string>("pallet");

		///<summary>
		/// Pallets
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("pallets")]
		public string Pallets => this.Value<string>("pallets");

		///<summary>
		/// Piece
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("piece")]
		public string Piece => this.Value<string>("piece");

		///<summary>
		/// Pieces
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("pieces")]
		public string Pieces => this.Value<string>("pieces");

		///<summary>
		/// Post index tooltip
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("postIndexTooltip")]
		public IHtmlString PostIndexTooltip => this.Value<IHtmlString>("postIndexTooltip");

		///<summary>
		/// Project data text
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("projectDataText")]
		public string ProjectDataText => this.Value<string>("projectDataText");

		///<summary>
		/// Project description tooltip
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("projectDescriptionTooltip")]
		public IHtmlString ProjectDescriptionTooltip => this.Value<IHtmlString>("projectDescriptionTooltip");

		///<summary>
		/// Project name tooltip
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("projectNameTooltip")]
		public IHtmlString ProjectNameTooltip => this.Value<IHtmlString>("projectNameTooltip");

		///<summary>
		/// Quantity delivered
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("quantityDelivered")]
		public string QuantityDelivered => this.Value<string>("quantityDelivered");

		///<summary>
		/// QuantityDeliveredMessage
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("quantityDeliveredMessage")]
		public string QuantityDeliveredMessage => this.Value<string>("quantityDeliveredMessage");

		///<summary>
		/// Tilte
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("resulterPageTilte")]
		public string ResulterPageTilte => this.Value<string>("resulterPageTilte");

		///<summary>
		/// Select Form
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("selectForm")]
		public object SelectForm => this.Value("selectForm");

		///<summary>
		/// Show all results tooltip
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("showAllResultsTooltip")]
		public IHtmlString ShowAllResultsTooltip => this.Value<IHtmlString>("showAllResultsTooltip");

		///<summary>
		/// Strength, bending moment in T-profile text
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("strengthBendingMomentInTProfileText")]
		public string StrengthBendingMomentInTprofileText => this.Value<string>("strengthBendingMomentInTProfileText");

		///<summary>
		/// Strength, horizontal force in fixed bracket text
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("strengthHorizontalForceInFixedBracketText")]
		public string StrengthHorizontalForceInFixedBracketText => this.Value<string>("strengthHorizontalForceInFixedBracketText");

		///<summary>
		/// Strength, horizontal force in sliding bracket text
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("strengthHorizontalForceInSlidingBracketText")]
		public string StrengthHorizontalForceInSlidingBracketText => this.Value<string>("strengthHorizontalForceInSlidingBracketText");

		///<summary>
		/// System tooltip
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("systemTooltip")]
		public IHtmlString SystemTooltip => this.Value<IHtmlString>("systemTooltip");

		///<summary>
		/// Terrain Category tooltip
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("terrainCategoryTooltip")]
		public IHtmlString TerrainCategoryTooltip => this.Value<IHtmlString>("terrainCategoryTooltip");

		///<summary>
		/// Total max force in anchor screw text
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("totalMaxForceInAnchorScrewText")]
		public string TotalMaxForceInAnchorScrewText => this.Value<string>("totalMaxForceInAnchorScrewText");

		///<summary>
		/// Total selfweight of facade text
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("totalSelfweightOfFacadeText")]
		public string TotalSelfweightOfFacadeText => this.Value<string>("totalSelfweightOfFacadeText");

		///<summary>
		/// Unit 1
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("unit1")]
		public string Unit1 => this.Value<string>("unit1");

		///<summary>
		/// Unit 2
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("unit2")]
		public string Unit2 => this.Value<string>("unit2");

		///<summary>
		/// Unit 3
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("unit3")]
		public string Unit3 => this.Value<string>("unit3");

		///<summary>
		/// Unit 3 ( ibm )
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("unit3Ibm")]
		public string Unit3Ibm => this.Value<string>("unit3Ibm");

		///<summary>
		/// Unit 4
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("unit4")]
		public string Unit4 => this.Value<string>("unit4");

		///<summary>
		/// Unit 5
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("unit5")]
		public string Unit5 => this.Value<string>("unit5");

		///<summary>
		/// Unit 6
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("unit6")]
		public string Unit6 => this.Value<string>("unit6");

		///<summary>
		/// Unit 2 ( m )
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("unitM")]
		public string UnitM => this.Value<string>("unitM");

		///<summary>
		/// UnitName
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("unitName")]
		public string UnitName => this.Value<string>("unitName");

		///<summary>
		/// Unit 1 ( stk )
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("unitStk")]
		public string UnitStk => this.Value<string>("unitStk");

		///<summary>
		/// Weight of the cladding tooltip
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("weightOfTheCladdingTooltip")]
		public IHtmlString WeightOfTheCladdingTooltip => this.Value<IHtmlString>("weightOfTheCladdingTooltip");

		///<summary>
		/// Wind peak velocity pressure text
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("windPeakVelocityPressureText")]
		public string WindPeakVelocityPressureText => this.Value<string>("windPeakVelocityPressureText");

		///<summary>
		/// Wind speed area tooltip
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("windSpeedAreaTooltip")]
		public IHtmlString WindSpeedAreaTooltip => this.Value<IHtmlString>("windSpeedAreaTooltip");

		///<summary>
		/// Wind speed tooltip
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("windSpeedTooltip")]
		public IHtmlString WindSpeedTooltip => this.Value<IHtmlString>("windSpeedTooltip");
	}
}