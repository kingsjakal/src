﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Smi.Services.Events;
using Smi.Web.Framework.Events;
using Smi.Web.Framework.Models;

namespace Smi.Web.Framework.Mvc.Filters
{
    /// <summary>
    /// Represents filter attribute that publish ModelReceived event before the action executes, after model binding is complete
    /// and publish ModelPrepared event after the action executes, before the action result
    /// </summary>
    public sealed class PublishModelEventsAttribute : TypeFilterAttribute
    {
        #region Fields

        private readonly bool _ignoreFilter;

        #endregion

        #region Ctor

        /// <summary>
        /// Create instance of the filter attribute
        /// </summary>
        /// <param name="ignore">Whether to ignore the execution of filter actions</param>
        public PublishModelEventsAttribute(bool ignore = false) : base(typeof(PublishModelEventsFilter))
        {
            _ignoreFilter = ignore;
            Arguments = new object[] { ignore };
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether to ignore the execution of filter actions
        /// </summary>
        public bool IgnoreFilter => _ignoreFilter;

        #endregion

        #region Nested filter

        /// <summary>
        /// Represents filter that publish ModelReceived event before the action executes, after model binding is complete
        /// and publish ModelPrepared event after the action executes, before the action result
        /// </summary>
        private class PublishModelEventsFilter : IActionFilter
        {
            #region Fields

            private readonly bool _ignoreFilter;
            private readonly IEventPublisher _eventPublisher;

            #endregion

            #region Ctor

            public PublishModelEventsFilter(bool ignoreFilter,
                IEventPublisher eventPublisher)
            {
                _ignoreFilter = ignoreFilter;
                _eventPublisher = eventPublisher;
            }

            #endregion

            #region Methods

            /// <summary>
            /// Called before the action executes, after model binding is complete
            /// </summary>
            /// <param name="context">A context for action filters</param>
            public void OnActionExecuting(ActionExecutingContext context)
            {
                if (context == null)
                    throw new ArgumentNullException(nameof(context));

                //check whether this filter has been overridden for the Action
                var actionFilter = context.ActionDescriptor.FilterDescriptors
                    .Where(filterDescriptor => filterDescriptor.Scope == FilterScope.Action)
                    .Select(filterDescriptor => filterDescriptor.Filter).OfType<PublishModelEventsAttribute>().FirstOrDefault();

                //whether to ignore this filter
                if (actionFilter?.IgnoreFilter ?? _ignoreFilter)
                    return;

                if (context.HttpContext.Request == null)
                    return;

                //only in POST requests
                if (!context.HttpContext.Request.Method.Equals(WebRequestMethods.Http.Post, StringComparison.InvariantCultureIgnoreCase))
                    return;

                //model received event
                foreach (var model in context.ActionArguments.Values.OfType<BaseSmiModel>())
                {
                    //we publish the ModelReceived event for all models as the BaseSmiModel, 
                    //so you need to implement IConsumer<ModelReceived<BaseSmiModel>> interface to handle this event
                    _eventPublisher.ModelReceived(model, context.ModelState);
                }
            }

            /// <summary>
            /// Called after the action executes, before the action result
            /// </summary>
            /// <param name="context">A context for action filters</param>
            public void OnActionExecuted(ActionExecutedContext context)
            {
                if (context == null)
                    throw new ArgumentNullException(nameof(context));

                //check whether this filter has been overridden for the Action
                var actionFilter = context.ActionDescriptor.FilterDescriptors
                    .Where(filterDescriptor => filterDescriptor.Scope == FilterScope.Action)
                    .Select(filterDescriptor => filterDescriptor.Filter).OfType<PublishModelEventsAttribute>().FirstOrDefault();

                //whether to ignore this filter
                if (actionFilter?.IgnoreFilter ?? _ignoreFilter)
                    return;

                if (context.HttpContext.Request == null)
                    return;

                //model prepared event
                if (context.Controller is Controller controller)
                {
                    if (controller.ViewData.Model is BaseSmiModel model)
                    {
                        //we publish the ModelPrepared event for all models as the BaseSmiModel, 
                        //so you need to implement IConsumer<ModelPrepared<BaseSmiModel>> interface to handle this event
                        _eventPublisher.ModelPrepared(model);
                    }

                    if (controller.ViewData.Model is IEnumerable<BaseSmiModel> modelCollection)
                    {
                        //we publish the ModelPrepared event for collection as the IEnumerable<BaseSmiModel>, 
                        //so you need to implement IConsumer<ModelPrepared<IEnumerable<BaseSmiModel>>> interface to handle this event
                        _eventPublisher.ModelPrepared(modelCollection);
                    }
                }
            }

            #endregion
        }

        #endregion
    }
}