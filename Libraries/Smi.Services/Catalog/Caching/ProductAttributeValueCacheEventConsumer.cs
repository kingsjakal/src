﻿using Smi.Core.Domain.Catalog;
using Smi.Services.Caching;

namespace Smi.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a product attribute value cache event consumer
    /// </summary>
    public partial class ProductAttributeValueCacheEventConsumer : CacheEventConsumer<ProductAttributeValue>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(ProductAttributeValue entity)
        {
            RemoveByPrefix(SmiCatalogDefaults.ProductAttributeValuesAllPrefixCacheKey);
        }
    }
}
