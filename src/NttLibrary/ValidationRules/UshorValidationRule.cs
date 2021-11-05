using System;
using System.Globalization;
using System.Windows.Controls;

namespace NttLibrary.ValidationRules
{
    public class UshortValidationRule : ValidationRule
    {
        /// <summary>
        /// Should uses only after value was converted
        /// </summary>
        /// <exception cref="NotImplementedException"/>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (ValidationStep != ValidationStep.ConvertedProposedValue) throw new NotImplementedException("ValidationStep should be is ConvertedProposedValue");

            var canConvert = (value as ushort?) != null;
            return new ValidationResult(canConvert, "Not a valid Port");
        }
    }
}
