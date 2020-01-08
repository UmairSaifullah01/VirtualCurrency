using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

namespace UMGS
{
    public abstract class CurrencyBinding : MonoBehaviour
    {
        [SerializeField] protected string currencyName;
        private void OnDisable()
        {
            if (CurrencyManager.Instance != null)
                CurrencyManager.Instance.OnValueChangeUnregister(currencyName, ChangeEffect);
            
        }

        protected abstract void ChangeEffect(object sender,PropertyChangedEventArgs args);
        
        private void OnEnable()
        {
            if (CurrencyManager.Instance != null)
                CurrencyManager.Instance.OnValueChangeRegister(currencyName, ChangeEffect);
            
        }
        
    }
}
