﻿using Smi.Core.Caching;
using Smi.Core.Domain.Configuration;
using Smi.Core.Events;
using Smi.Services.Events;

namespace Smi.Plugin.Widgets.NivoSlider.Infrastructure.Cache
{
    /// <summary>
    /// Model cache event consumer (used for caching of presentation layer models)
    /// </summary>
    public partial class ModelCacheEventConsumer :
        IConsumer<EntityInsertedEvent<Setting>>,
        IConsumer<EntityUpdatedEvent<Setting>>,
        IConsumer<EntityDeletedEvent<Setting>>
    {
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : picture id
        /// {1} : connection type (http/https)
        /// </remarks>
        public static CacheKey PICTURE_URL_MODEL_KEY = new CacheKey("Smi.plugins.widgets.nivoslider.pictureurl-{0}-{1}", PICTURE_URL_PATTERN_KEY);
        public const string PICTURE_URL_PATTERN_KEY = "Smi.plugins.widgets.nivoslider";

        private readonly IStaticCacheManager _staticCacheManager;

        public ModelCacheEventConsumer(IStaticCacheManager staticCacheManager)
        {
            _staticCacheManager = staticCacheManager;
        }

        public void HandleEvent(EntityInsertedEvent<Setting> eventMessage)
        {
            _staticCacheManager.RemoveByPrefix(PICTURE_URL_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdatedEvent<Setting> eventMessage)
        {
            _staticCacheManager.RemoveByPrefix(PICTURE_URL_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeletedEvent<Setting> eventMessage)
        {
            _staticCacheManager.RemoveByPrefix(PICTURE_URL_PATTERN_KEY);
        }
    }
}