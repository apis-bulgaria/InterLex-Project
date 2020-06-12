using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Interlex.BusinessLayer.Models;

namespace Interlex.App.CustomBinders
{
    /// <summary>
    /// Custom model binder needed to pass langId parameter to the model constructor. 
    /// Model SearchBox does not have default constructor on purpose.
    /// </summary> 
    public class SearchBoxBinder : DefaultModelBinder
    {
        protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Type modelType)
        {
            int langId = ((BaseController)controllerContext.Controller).Language.Id;
            return new SearchBox(langId);
        }
    }    
}