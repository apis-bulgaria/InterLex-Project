using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Interlex.BusinessLayer.Models;
using System.Linq.Expressions;

namespace Interlex.App.Helpers
{
    public static class CheckTreeViewExtensions
    {
        public static MvcHtmlString CheckTreeViewFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            ModelMetadata modelMetadata = ModelMetadata.FromLambdaExpression<TModel, TProperty>(expression, htmlHelper.ViewData);
            string fieldExpression = ExpressionHelper.GetExpressionText(expression);
            object formattedModelValue = modelMetadata.Model;
            if (modelMetadata.Model == null)
            {
                formattedModelValue = modelMetadata.NullDisplayText;
            }
            string text = modelMetadata.EditFormatString;
            if (modelMetadata.Model != null && !string.IsNullOrEmpty(text))
            {
                formattedModelValue = string.Format(System.Globalization.CultureInfo.CurrentCulture, text, new object[]
                {
                    modelMetadata.Model
                });
            }
            ViewDataDictionary viewDataDictionary = new ViewDataDictionary(htmlHelper.ViewDataContainer.ViewData)
            {
                Model = modelMetadata.Model,
                ModelMetadata = modelMetadata,
                TemplateInfo = new TemplateInfo
                {
                    FormattedModelValue = formattedModelValue,
                    HtmlFieldPrefix = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(fieldExpression),
                }
            };
            return htmlHelper.Partial("~/Views/Shared/_CheckTreeView.cshtml", modelMetadata.Model, viewDataDictionary);
        }
    }
}