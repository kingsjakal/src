﻿using Smi.Core.Domain.Customers;
using Smi.Web.Models.Profile;

namespace Smi.Web.Factories
{
    /// <summary>
    /// Represents the interface of the profile model factory
    /// </summary>
    public partial interface IProfileModelFactory
    {
        /// <summary>
        /// Prepare the profile index model
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="page">Number of posts page; pass null to disable paging</param>
        /// <returns>Profile index model</returns>
        ProfileIndexModel PrepareProfileIndexModel(Customer customer, int? page);

        /// <summary>
        /// Prepare the profile info model
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <returns>Profile info model</returns>
        ProfileInfoModel PrepareProfileInfoModel(Customer customer);

        /// <summary>
        /// Prepare the profile posts model
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="page">Number of posts page</param>
        /// <returns>Profile posts model</returns>  
        ProfilePostsModel PrepareProfilePostsModel(Customer customer, int page);
    }
}
