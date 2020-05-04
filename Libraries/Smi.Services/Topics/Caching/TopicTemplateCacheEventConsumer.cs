﻿using Smi.Core.Domain.Topics;
using Smi.Services.Caching;

namespace Smi.Services.Topics.Caching
{
    /// <summary>
    /// Represents a topic template cache event consumer
    /// </summary>
    public partial class TopicTemplateCacheEventConsumer : CacheEventConsumer<TopicTemplate>
    {
        /// <summary>
        /// entity
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="entityEventType">Entity event type</param>
        protected override void ClearCache(TopicTemplate entity, EntityEventType entityEventType)
        {
            Remove(SmiTopicDefaults.TopicTemplatesAllCacheKey);
        }
    }
}
