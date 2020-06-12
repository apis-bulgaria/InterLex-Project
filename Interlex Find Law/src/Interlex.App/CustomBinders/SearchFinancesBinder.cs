namespace Interlex.App.CustomBinders
{
    using System;
    using System.Web.Mvc;
    using Interlex.BusinessLayer.Models;

    public class SearchFinancesBinder : DefaultModelBinder
    {
        protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Type modelType)
        {
            int langId = ((BaseController)controllerContext.Controller).Language.Id;
            return new SearchFinances(langId);
        }
    }
}