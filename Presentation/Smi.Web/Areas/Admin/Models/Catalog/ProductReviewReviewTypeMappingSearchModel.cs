﻿using Smi.Web.Framework.Models;

namespace Smi.Web.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a product review and review type mapping search model
    /// </summary>
    public partial class ProductReviewReviewTypeMappingSearchModel : BaseSearchModel
    {
        #region Properties

        public int ProductReviewId { get; set; }

        public bool IsAnyReviewTypes { get; set; }

        #endregion
    }
}
