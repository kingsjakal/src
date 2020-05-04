﻿namespace Smi.Services.Payments
{
    /// <summary>
    /// Represents default values related to payment services
    /// </summary>
    public static partial class SmiPaymentDefaults
    {
        /// <summary>
        /// Gets a setting name to store countries in which a payment method is now allowed
        /// </summary>
        /// <remarks>
        /// {0} : payment method name
        /// </remarks>
        public static string RestrictedCountriesSettingName => "PaymentMethodRestictions.{0}";
    }
}