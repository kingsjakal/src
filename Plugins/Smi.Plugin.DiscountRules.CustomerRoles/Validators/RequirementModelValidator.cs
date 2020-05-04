﻿using FluentValidation;
using Smi.Plugin.DiscountRules.CustomerRoles.Models;
using Smi.Services.Localization;
using Smi.Web.Framework.Validators;

namespace Smi.Plugin.DiscountRules.CustomerRoles.Validators
{
    /// <summary>
    /// Represents an <see cref="RequirementModel"/> validator.
    /// </summary>
    public class RequirementModelValidator : BaseSmiValidator<RequirementModel>
    {
        public RequirementModelValidator(ILocalizationService localizationService)
        {
            RuleFor(model => model.DiscountId)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Plugins.DiscountRules.CustomerRoles.Fields.DiscountId.Required"));
            RuleFor(model => model.CustomerRoleId)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Plugins.DiscountRules.CustomerRoles.Fields.CustomerRoleId.Required"));
        }
    }
}
