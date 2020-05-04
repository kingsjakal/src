﻿using System.Collections.Generic;
using Smi.Core.Domain.Shipping;
using Smi.Web.Framework.Models;

namespace Smi.Web.Models.Checkout
{
    public partial class CheckoutShippingMethodModel : BaseSmiModel
    {
        public CheckoutShippingMethodModel()
        {
            ShippingMethods = new List<ShippingMethodModel>();
            Warnings = new List<string>();
        }

        public IList<ShippingMethodModel> ShippingMethods { get; set; }

        public bool NotifyCustomerAboutShippingFromMultipleLocations { get; set; }

        public IList<string> Warnings { get; set; }

        public bool DisplayPickupInStore { get; set; }
        public CheckoutPickupPointsModel PickupPointsModel { get; set; }

        #region Nested classes

        public partial class ShippingMethodModel : BaseSmiModel
        {
            public string ShippingRateComputationMethodSystemName { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string Fee { get; set; }
            public bool Selected { get; set; }

            public ShippingOption ShippingOption { get; set; }
        }

        #endregion
    }
}