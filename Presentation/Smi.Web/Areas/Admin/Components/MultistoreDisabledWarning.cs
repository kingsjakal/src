﻿using Microsoft.AspNetCore.Mvc;
using Smi.Core.Domain.Catalog;
using Smi.Services.Configuration;
using Smi.Services.Stores;
using Smi.Web.Framework.Components;

namespace Smi.Web.Areas.Admin.Components
{
    public class MultistoreDisabledWarningViewComponent : SmiViewComponent
    {
        private readonly CatalogSettings _catalogSettings;
        private readonly ISettingService _settingService;
        private readonly IStoreService _storeService;

        public MultistoreDisabledWarningViewComponent(CatalogSettings catalogSettings,
            ISettingService settingService,
            IStoreService storeService)
        {
            _catalogSettings = catalogSettings;
            _settingService = settingService;
            _storeService = storeService;
        }

        public IViewComponentResult Invoke()
        {

            //action displaying notification (warning) to a store owner that "limit per store" feature is ignored

            //default setting
            var enabled = _catalogSettings.IgnoreStoreLimitations;
            if (!enabled)
            {
                //overridden settings
                var stores = _storeService.GetAllStores();
                foreach (var store in stores)
                {
                    var catalogSettings = _settingService.LoadSetting<CatalogSettings>(store.Id);
                    enabled = catalogSettings.IgnoreStoreLimitations;

                    if (enabled)
                        break;
                }
            }

            //This setting is disabled. No warnings.
            if (!enabled)
                return Content("");

            return View();
        }
    }
}
