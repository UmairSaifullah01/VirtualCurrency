using System;
using System.Collections.Generic;
using System.ComponentModel;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

namespace UMGS
{
    public class CurrencyManager : Singleton<CurrencyManager>
    {
        public List<string> virtualCurrenciesNames;
        private List<VirtualCurrency> _virtualCurrencies;

        protected override void Awake()
        {
            base.Awake();
            _virtualCurrencies = new List<VirtualCurrency>();
            for (var i = 0; i < virtualCurrenciesNames.Count; i++)
            {
                var v = new VirtualCurrency(virtualCurrenciesNames[i], i);
                _virtualCurrencies.Add(v);
            }

            if (!PlayerPrefs.HasKey("cash"))
            {
                SetValue("cash", 100);
                SetValue("gems", 0);
            }
        }

        public void OnValueChangeRegister(string currencyName, PropertyChangedEventHandler changeEvent)
        {
            var virtualCurrency = _virtualCurrencies.Find(x => x.displayName == currencyName);
            virtualCurrency.PropertyChanged += changeEvent;
            changeEvent(virtualCurrency, null);
        }

        public void OnValueChangeUnregister(string currencyName, PropertyChangedEventHandler changeEvent)
        {
            var v = _virtualCurrencies.Find(x => x.displayName == currencyName);
            if (v != null) v.PropertyChanged -= changeEvent;
        }

        public int GetValue(string currencyName)
        {
            return _virtualCurrencies.Find(x => x.displayName == currencyName).value;
        }

        public int GetValue(int id)
        {
            return _virtualCurrencies.Find(x => x.Id == id).value;
        }

        public void SetValue(string currencyName, int value)
        {
            _virtualCurrencies.Find(x => x.displayName == currencyName).value = value;
        }

        public void AddValue(string currencyName, int value)
        {
            var v = _virtualCurrencies.Find(x => x.displayName == currencyName);
            if (v != null) v.value += value;
        }

        public bool IsAvailable(string currencyName)
        {
            return _virtualCurrencies.Find(x => x.displayName == currencyName).IsAvailable;
        }

        public void SetValue(int id, int value)
        {
            _virtualCurrencies.Find(x => x.Id == id).value = value;
        }

        public string GetCurrencyName(int id)
        {
            return _virtualCurrencies[id].displayName;
        }

        private void OnValidate()
        {
            this.gameObject.name = "CurrencyManager";
        }
    }
}