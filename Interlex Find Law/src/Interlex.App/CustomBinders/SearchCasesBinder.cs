﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Interlex.BusinessLayer.Models;

namespace Interlex.App.CustomBinders
{
    public class SearchCasesBinder : DefaultModelBinder
    {
        protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Type modelType)
        {
            int langId = ((BaseController)controllerContext.Controller).Language.Id;
            return new SearchCases(langId);
        }
    }
}