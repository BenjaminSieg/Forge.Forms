﻿using System.Globalization;
using System.Windows.Data;
using Forge.Forms.DynamicExpressions;

namespace Forge.Forms.Validation
{
    public class EnforcedValidator : FieldValidator
    {
        public EnforcedValidator(
            ValidationPipe pipe,
            IErrorStringProvider errorProvider,
            IBoolProxy isEnforced,
            IValueConverter valueConverter,
            bool strictValidation,
            bool validatesOnTargetUpdated,
            bool ignoreNullOrEmpty)
            : base(
                pipe,
                errorProvider,
                isEnforced,
                valueConverter,
                strictValidation,
                validatesOnTargetUpdated,
                ignoreNullOrEmpty)
        {
        }

        protected override bool ValidateValue(object value, CultureInfo cultureInfo)
        {
            return false;
        }
    }
}
