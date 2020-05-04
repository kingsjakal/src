﻿using System;
using System.Collections.Generic;
using System.Linq;
using Smi.Core;
using Smi.Core.Domain.Localization;
using Smi.Services.Localization;
using Smi.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Smi.Web.Areas.Admin.Models.Localization;
using Smi.Web.Framework.Factories;
using Smi.Web.Framework.Models.Extensions;

namespace Smi.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the language model factory implementation
    /// </summary>
    public partial class LanguageModelFactory : ILanguageModelFactory
    {
        #region Fields
        
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly IStoreMappingSupportedModelFactory _storeMappingSupportedModelFactory;

        #endregion

        #region Ctor

        public LanguageModelFactory(IBaseAdminModelFactory baseAdminModelFactory,
            ILanguageService languageService,
            ILocalizationService localizationService,
            IStoreMappingSupportedModelFactory storeMappingSupportedModelFactory)
        {
            _baseAdminModelFactory = baseAdminModelFactory;
            _languageService = languageService;
            _localizationService = localizationService;
            _storeMappingSupportedModelFactory = storeMappingSupportedModelFactory;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare locale resource search model
        /// </summary>
        /// <param name="searchModel">Locale resource search model</param>
        /// <param name="language">Language</param>
        /// <returns>Locale resource search model</returns>
        protected virtual LocaleResourceSearchModel PrepareLocaleResourceSearchModel(LocaleResourceSearchModel searchModel, Language language)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (language == null)
                throw new ArgumentNullException(nameof(language));

            searchModel.LanguageId = language.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }
        
        #endregion

        #region Methods

        /// <summary>
        /// Prepare language search model
        /// </summary>
        /// <param name="searchModel">Language search model</param>
        /// <returns>Language search model</returns>
        public virtual LanguageSearchModel PrepareLanguageSearchModel(LanguageSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged language list model
        /// </summary>
        /// <param name="searchModel">Language search model</param>
        /// <returns>Language list model</returns>
        public virtual LanguageListModel PrepareLanguageListModel(LanguageSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get languages
            var languages = _languageService.GetAllLanguages(showHidden: true).ToPagedList(searchModel);

            //prepare list model
            var model = new LanguageListModel().PrepareToGrid(searchModel, languages, () =>
            {
                return languages.Select(language =>
                {
                    return language.ToModel<LanguageModel>();
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare language model
        /// </summary>
        /// <param name="model">Language model</param>
        /// <param name="language">Language</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Language model</returns>
        public virtual LanguageModel PrepareLanguageModel(LanguageModel model, Language language, bool excludeProperties = false)
        {
            if (language != null)
            {
                //fill in model values from the entity
                model ??= language.ToModel<LanguageModel>();

                //prepare nested search model
                PrepareLocaleResourceSearchModel(model.LocaleResourceSearchModel, language);
            }

            //set default values for the new model
            if (language == null)
            {
                model.DisplayOrder = _languageService.GetAllLanguages().Max(l => l.DisplayOrder) + 1;
                model.Published = true;
            }

            //prepare available currencies
            //TODO: add locale resource for "---"
            _baseAdminModelFactory.PrepareCurrencies(model.AvailableCurrencies, defaultItemText: "---");

            //prepare available stores
            _storeMappingSupportedModelFactory.PrepareModelStores(model, language, excludeProperties);

            return model;
        }

        /// <summary>
        /// Prepare paged locale resource list model
        /// </summary>
        /// <param name="searchModel">Locale resource search model</param>
        /// <param name="language">Language</param>
        /// <returns>Locale resource list model</returns>
        public virtual LocaleResourceListModel PrepareLocaleResourceListModel(LocaleResourceSearchModel searchModel, Language language)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (language == null)
                throw new ArgumentNullException(nameof(language));

            //get locale resources
            var localeResources = _localizationService.GetAllResourceValues(language.Id, loadPublicLocales: null)
                .OrderBy(localeResource => localeResource.Key).AsQueryable();

            //filter locale resources
            //TODO: move filter to language service
            if (!string.IsNullOrEmpty(searchModel.SearchResourceName))
                localeResources = localeResources.Where(l => l.Key.ToLowerInvariant().Contains(searchModel.SearchResourceName.ToLowerInvariant()));
            if (!string.IsNullOrEmpty(searchModel.SearchResourceValue))
                localeResources = localeResources.Where(l => l.Value.Value.ToLowerInvariant().Contains(searchModel.SearchResourceValue.ToLowerInvariant()));

            var pagedLocaleResources = new PagedList<KeyValuePair<string, KeyValuePair<int, string>>>(localeResources,
                searchModel.Page - 1, searchModel.PageSize);

            //prepare list model
            var model = new LocaleResourceListModel().PrepareToGrid(searchModel, pagedLocaleResources, () =>
            {
                //fill in model values from the entity
                return pagedLocaleResources.Select(localeResource => new LocaleResourceModel
                {
                    LanguageId = language.Id,
                    Id = localeResource.Value.Key,
                    ResourceName = localeResource.Key,
                    ResourceValue = localeResource.Value.Value
                });
            });

            return model;
        }

        #endregion
    }
}