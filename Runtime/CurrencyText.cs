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

        private string FormatCurrencyValue(float value)
        {
            if (displayFormat == CurrencyDisplayFormat.Normal)
            {
                return useThousandsSeparator
                    ? string.Format(CultureInfo.InvariantCulture, "{0:N0}", value)
                    : value.ToString("F0", CultureInfo.InvariantCulture);
            }

            return FormatAbbreviatedNumber(value);
        }

        private string FormatAbbreviatedNumber(float number)
        {
            if (number == 0) return "0";

            float abs = Mathf.Abs(number);
            string sign = number < 0 ? "-" : "";

            if (abs >= 1000000000000) // Trillion
            {
                return sign + (abs / 1000000000000f).ToString($"F{decimalPlaces}", CultureInfo.InvariantCulture) + "T";
            }
            if (abs >= 1000000000) // Billion
            {
                return sign + (abs / 1000000000f).ToString($"F{decimalPlaces}", CultureInfo.InvariantCulture) + "B";
            }
            if (abs >= 1000000) // Million
            {
                return sign + (abs / 1000000f).ToString($"F{decimalPlaces}", CultureInfo.InvariantCulture) + "M";
            }
            if (abs >= 1000) // Thousand
            {
                return sign + (abs / 1000f).ToString($"F{decimalPlaces}", CultureInfo.InvariantCulture) + "K";
            }

            return sign + abs.ToString("F0", CultureInfo.InvariantCulture);
        }
    }
}