﻿using System;
using System.Collections.Generic;
using Smi.Core;
using Smi.Core.Domain.Customers;
using Smi.Core.Domain.Forums;
using Smi.Core.Domain.Media;
using Smi.Services.Common;
using Smi.Services.Customers;
using Smi.Services.Directory;
using Smi.Services.Forums;
using Smi.Services.Helpers;
using Smi.Services.Localization;
using Smi.Services.Media;
using Smi.Web.Framework.Extensions;
using Smi.Web.Models.Common;
using Smi.Web.Models.Profile;

namespace Smi.Web.Factories
{
    /// <summary>
    /// Represents the profile model factory
    /// </summary>
    public partial class ProfileModelFactory : IProfileModelFactory
    {
        #region Fields

        private readonly CustomerSettings _customerSettings;
        private readonly ForumSettings _forumSettings;
        private readonly ICountryService _countryService;
        private readonly ICustomerService _customerService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IForumService _forumService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly IPictureService _pictureService;
        private readonly IWorkContext _workContext;
        private readonly MediaSettings _mediaSettings;

        #endregion

        #region Ctor

        public ProfileModelFactory(CustomerSettings customerSettings,
            ForumSettings forumSettings,
            ICountryService countryService,
            ICustomerService customerService,
            IDateTimeHelper dateTimeHelper,
            IForumService forumService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IPictureService pictureService,
            IWorkContext workContext,
            MediaSettings mediaSettings)
        {
            _customerSettings = customerSettings;
            _forumSettings = forumSettings;
            _countryService = countryService;
            _customerService = customerService;
            _dateTimeHelper = dateTimeHelper;
            _forumService = forumService;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _pictureService = pictureService;
            _workContext = workContext;
            _mediaSettings = mediaSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare the profile index model
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="page">Number of posts page; pass null to disable paging</param>
        /// <returns>Profile index model</returns>
        public virtual ProfileIndexModel PrepareProfileIndexModel(Customer customer, int? page)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            var pagingPosts = false;
            var postsPage = 0;

            if (page.HasValue)
            {
                postsPage = page.Value;
                pagingPosts = true;
            }

            var name = _customerService.FormatUsername(customer);
            var title = string.Format(_localizationService.GetResource("Profile.ProfileOf"), name);

            var model = new ProfileIndexModel
            {
                ProfileTitle = title,
                PostsPage = postsPage,
                PagingPosts = pagingPosts,
                CustomerProfileId = customer.Id,
                ForumsEnabled = _forumSettings.ForumsEnabled
            };
            return model;
        }

        /// <summary>
        /// Prepare the profile info model
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <returns>Profile info model</returns>
        public virtual ProfileInfoModel PrepareProfileInfoModel(Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            //avatar
            var avatarUrl = "";
            if (_customerSettings.AllowCustomersToUploadAvatars)
            {
                avatarUrl = _pictureService.GetPictureUrl(
                 _genericAttributeService.GetAttribute<int>(customer, SmiCustomerDefaults.AvatarPictureIdAttribute),
                 _mediaSettings.AvatarPictureSize,
                 _customerSettings.DefaultAvatarEnabled,
                 defaultPictureType: PictureType.Avatar);
            }

            //location
            var locationEnabled = false;
            var location = string.Empty;
            if (_customerSettings.ShowCustomersLocation)
            {
                locationEnabled = true;

                var countryId = _genericAttributeService.GetAttribute<int>(customer, SmiCustomerDefaults.CountryIdAttribute);
                var country = _countryService.GetCountryById(countryId);
                if (country != null)
                {
                    location = _localizationService.GetLocalized(country, x => x.Name);
                }
                else
                {
                    locationEnabled = false;
                }
            }

            //private message
            var pmEnabled = _forumSettings.AllowPrivateMessages && !_customerService.IsGuest(customer);

            //total forum posts
            var totalPostsEnabled = false;
            var totalPosts = 0;
            if (_forumSettings.ForumsEnabled && _forumSettings.ShowCustomersPostCount)
            {
                totalPostsEnabled = true;
                totalPosts = _genericAttributeService.GetAttribute<int>(customer, SmiCustomerDefaults.ForumPostCountAttribute);
            }

            //registration date
            var joinDateEnabled = false;
            var joinDate = string.Empty;

            if (_customerSettings.ShowCustomersJoinDate)
            {
                joinDateEnabled = true;
                joinDate = _dateTimeHelper.ConvertToUserTime(customer.CreatedOnUtc, DateTimeKind.Utc).ToString("f");
            }

            //birth date
            var dateOfBirthEnabled = false;
            var dateOfBirth = string.Empty;
            if (_customerSettings.DateOfBirthEnabled)
            {
                var dob = _genericAttributeService.GetAttribute<DateTime?>(customer, SmiCustomerDefaults.DateOfBirthAttribute);
                if (dob.HasValue)
                {
                    dateOfBirthEnabled = true;
                    dateOfBirth = dob.Value.ToString("D");
                }
            }

            var model = new ProfileInfoModel
            {
                CustomerProfileId = customer.Id,
                AvatarUrl = avatarUrl,
                LocationEnabled = locationEnabled,
                Location = location,
                PMEnabled = pmEnabled,
                TotalPostsEnabled = totalPostsEnabled,
                TotalPosts = totalPosts.ToString(),
                JoinDateEnabled = joinDateEnabled,
                JoinDate = joinDate,
                DateOfBirthEnabled = dateOfBirthEnabled,
                DateOfBirth = dateOfBirth,
            };

            return model;
        }

        /// <summary>
        /// Prepare the profile posts model
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="page">Number of posts page</param>
        /// <returns>Profile posts model</returns>
        public virtual ProfilePostsModel PrepareProfilePostsModel(Customer customer, int page)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            if (page > 0)
            {
                page -= 1;
            }

            var pageSize = _forumSettings.LatestCustomerPostsPageSize;

            var list = _forumService.GetAllPosts(0, customer.Id, string.Empty, false, page, pageSize);

            var latestPosts = new List<PostsModel>();

            foreach (var forumPost in list)
            {
                var posted = string.Empty;
                if (_forumSettings.RelativeDateTimeFormattingEnabled)
                {
                    var languageCode = _workContext.WorkingLanguage.LanguageCulture;
                    var postedAgo = forumPost.CreatedOnUtc.RelativeFormat(languageCode);
                    posted = string.Format(_localizationService.GetResource("Common.RelativeDateTime.Past"), postedAgo);
                }
                else
                {
                    posted = _dateTimeHelper.ConvertToUserTime(forumPost.CreatedOnUtc, DateTimeKind.Utc).ToString("f");
                }

                var topic = _forumService.GetTopicById(forumPost.TopicId);

                latestPosts.Add(new PostsModel
                {
                    ForumTopicId = topic.Id,
                    ForumTopicTitle = topic.Subject,
                    ForumTopicSlug = _forumService.GetTopicSeName(topic),
                    ForumPostText = _forumService.FormatPostText(forumPost),
                    Posted = posted
                });
            }

            var pagerModel = new PagerModel
            {
                PageSize = list.PageSize,
                TotalRecords = list.TotalCount,
                PageIndex = list.PageIndex,
                ShowTotalSummary = false,
                RouteActionName = "CustomerProfilePaged",
                UseRouteLinks = true,
                RouteValues = new RouteValues { pageNumber = page, id = customer.Id }
            };

            var model = new ProfilePostsModel
            {
                PagerModel = pagerModel,
                Posts = latestPosts,
            };

            return model;
        }

        #endregion
    }
}