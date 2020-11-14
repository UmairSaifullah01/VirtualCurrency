using System.ComponentModel;
using TMPro;
using UMGS;
using UnityEngine;

public class CurrencyText : CurrencyBinding
{
    [SerializeField] private TextMeshProUGUI textContainer;
    
    protected override void ChangeEffect(object sender, PropertyChangedEventArgs args)
    {
        textContainer.text = (sender as VirtualCurrency)?.value.ToString();
    }
}