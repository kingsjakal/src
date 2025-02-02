﻿using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Smi.Core.Domain.Common;
using Smi.Services.Configuration;
using Smi.Services.Localization;
using Smi.Services.Messages;
using Smi.Services.Security;
using Smi.Web.Areas.Admin.Factories;
using Smi.Web.Areas.Admin.Models.Common;
using Smi.Web.Areas.Admin.Models.Home;

namespace Smi.Web.Areas.Admin.Controllers
{
    public partial class HomeController : BaseAdminController
    {
        #region Fields

        private readonly AdminAreaSettings _adminAreaSettings;
        private readonly ICommonModelFactory _commonModelFactory;
        private readonly IHomeModelFactory _homeModelFactory;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;

        #endregion

        #region Ctor

        public HomeController(AdminAreaSettings adminAreaSettings,
            ICommonModelFactory commonModelFactory,
            IHomeModelFactory homeModelFactory,
            ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService,
            ISettingService settingService)
        {
            _adminAreaSettings = adminAreaSettings;
            _commonModelFactory = commonModelFactory;
            _homeModelFactory = homeModelFactory;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _settingService = settingService;
        }

        #endregion

        #region Methods

        public virtual IActionResult Index()
        {
            //display a warning to a store owner if there are some error
            if (_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
            {
                var warnings = _commonModelFactory.PrepareSystemWarningModels();
                if (warnings.Any(warning => warning.Level == SystemWarningLevel.Fail ||
                                            warning.Level == SystemWarningLevel.CopyrightRemovalKey ||
                                            warning.Level == SystemWarningLevel.Warning))
                    _notificationService.WarningNotification(
                        string.Format(_localizationService.GetResource("Admin.System.Warnings.Errors"),
                        Url.Action("Warnings", "Common")),
                        //do not encode URLs
                        false);
            }

            //prepare model
            var model = _homeModelFactory.PrepareDashboardModel(new DashboardModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult SmiMarketplaceNewsHideAdv()
        {
            _adminAreaSettings.HideAdvertisementsOnAdminArea = !_adminAreaSettings.HideAdvertisementsOnAdminArea;
            _settingService.SaveSetting(_adminAreaSettings);

            return Content("Setting changed");
        }

        #endregion
    }
}