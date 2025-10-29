using System;
using System.Globalization;
using System.ComponentModel;
using TMPro;
using UnityEngine;

namespace THEBADDEST.VirtualCurrencySystem
{
    public class CurrencyText : CurrencyBinding
    {
        [SerializeField] private TextMeshProUGUI textContainer;
        [SerializeField] private CurrencyDisplayFormat displayFormat;

        [Header("Display Settings")]
        [SerializeField] private bool useThousandsSeparator = true;
        [SerializeField] private int decimalPlaces = 1;

        protected override void ChangeEffect(object sender, PropertyChangedEventArgs args)
        {
            if (sender is VirtualCurrency currency)
            {
                textContainer.text = FormatCurrencyValue(currency.value);
            }
        }

        private string FormatCurrencyValue(BigNumber value)
        {
            switch (displayFormat)
            {
                case CurrencyDisplayFormat.Abbreviated:
                    return value.ToStringFormatted(decimalPlaces);
                case CurrencyDisplayFormat.Normal:
                    return value.ToString();
                default:
                    // Show as normal number, with or without separator
                    var val = value.ToDouble();
                    return useThousandsSeparator
                        ? val.ToString($"N{decimalPlaces}", System.Globalization.CultureInfo.InvariantCulture)
                        : val.ToString($"F{decimalPlaces}", System.Globalization.CultureInfo.InvariantCulture);
            }
        }
    }
}