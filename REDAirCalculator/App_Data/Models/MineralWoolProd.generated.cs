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
	/// <summary>Mineral Wool</summary>
	[PublishedModel("mineralWoolProd")]
	public partial class MineralWoolProd : PublishedContentModel, IProduct
	{
		// helpers
#pragma warning disable 0109 // new is redundant
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		public new const string ModelTypeAlias = "mineralWoolProd";
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		public new const PublishedItemType ModelItemType = PublishedItemType.Content;
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		public new static PublishedContentType GetModelContentType()
			=> PublishedModelUtility.GetModelContentType(ModelItemType, ModelTypeAlias);
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		public static PublishedPropertyType GetModelPropertyType<TValue>(Expression<Func<MineralWoolProd, TValue>> selector)
			=> PublishedModelUtility.GetModelPropertyType(GetModelContentType(), selector);
#pragma warning restore 0109

		// ctor
		public MineralWoolProd(IPublishedContent content)
			: base(content)
		{ }

		// properties

		///<summary>
		/// Displayed items: Choose item which will be displayed instead of this, if you need.
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("displayedItems")]
		public IEnumerable<IPublishedContent> DisplayedItems => this.Value<IEnumerable<IPublishedContent>>("displayedItems");

		///<summary>
		/// Insulation thickness
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("insulationThickness")]
		public IEnumerable<IPublishedContent> InsulationThickness => this.Value<IEnumerable<IPublishedContent>>("insulationThickness");

		///<summary>
		/// Length
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("length")]
		public decimal Length => this.Value<decimal>("length");

		///<summary>
		/// Volume
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("volume")]
		public decimal Volume => this.Value<decimal>("volume");

		///<summary>
		/// Width
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("width")]
		public decimal Width => this.Value<decimal>("width");

		///<summary>
		/// Delivery time
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("deliveryTime")]
		public int DeliveryTime => Product.GetDeliveryTime(this);

		///<summary>
		/// Elements per package
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("elementsPerPackage")]
		public int ElementsPerPackage => Product.GetElementsPerPackage(this);

		///<summary>
		/// Label
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("LabelDB")]
		public string LabelDB => Product.GetLabelDB(this);

		///<summary>
		/// Number
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("NumberDB")]
		public int NumberDB => Product.GetNumberDB(this);

		///<summary>
		/// Number SAP
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("numberSAP")]
		public int NumberSap => Product.GetNumberSap(this);

		///<summary>
		/// Units
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.0.4")]
		[ImplementPropertyType("Units")]
		public string Units => Product.GetUnits(this);
	}
}