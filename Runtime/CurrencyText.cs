using System.ComponentModel;
using System.Globalization;
using TMPro;
using UnityEngine;

namespace THEBADDEST.VirtualCurrencySystem
{


    public class CurrencyText : CurrencyBinding
    {
        [SerializeField] private TextMeshProUGUI textContainer;
    
        protected override void ChangeEffect(object sender, PropertyChangedEventArgs args)
        {
            textContainer.text = (sender as VirtualCurrency)?.value.ToString(CultureInfo.InvariantCulture);
        }
    }


}