﻿using FluentValidation;
using Smi.Plugin.Pickup.PickupInStore.Models;
using Smi.Services.Localization;
using Smi.Web.Framework.Validators;

namespace Smi.Plugin.Pickup.PickupInStore.Validators
{
    public partial class StorePickupPointValidator : BaseSmiValidator<StorePickupPointModel>
    {
        public StorePickupPointValidator(ILocalizationService localizationService)
        {
            // Latitude
            RuleFor(model => model.Latitude)
                .InclusiveBetween(-90, 90)
                .WithMessage(localizationService.GetResource("Plugins.Pickup.PickupInStore.Fields.Latitude.InvalidRange"))
                .When(model => model.Latitude.HasValue);
            RuleFor(model => model.Latitude)
                .Must(latitude => latitude.HasValue)
                .WithMessage(localizationService.GetResource("Plugins.Pickup.PickupInStore.Fields.Latitude.IsNullWhenLongitudeHasValue"))
                .When(model => model.Longitude.HasValue);
            RuleFor(model => model.Latitude)
                .ScalePrecision(8, 18)
                .WithMessage(localizationService.GetResource("Plugins.Pickup.PickupInStore.Fields.Latitude.InvalidPrecision"));

            // Longitude
            RuleFor(model => model.Longitude)
                .InclusiveBetween(-180, 180)
                .WithMessage(localizationService.GetResource("Plugins.Pickup.PickupInStore.Fields.Longitude.InvalidRange"))
                .When(model => model.Longitude.HasValue);
            RuleFor(model => model.Longitude)
                .Must(longitude => longitude.HasValue)
                .WithMessage(localizationService.GetResource("Plugins.Pickup.PickupInStore.Fields.Longitude.IsNullWhenLatitudeHasValue"))
                .When(model => model.Latitude.HasValue);
            RuleFor(model => model.Longitude)
                .ScalePrecision(8, 18)
                .WithMessage(localizationService.GetResource("Plugins.Pickup.PickupInStore.Fields.Longitude.InvalidPrecision"));
        }
    }
}
