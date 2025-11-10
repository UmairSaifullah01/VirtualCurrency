using System;
using System.Collections.Generic;
using System.ComponentModel;
using THEBADDEST.DataManagement;
using UnityEngine;

namespace THEBADDEST.VirtualCurrencySystem
{
	public partial class VirtualCurrencyService
	{
		public static VirtualCurrencyService virtualCurrencyService;

		static void EnsureInitialized()
		{
			if (virtualCurrencyService == null)
			{
				var instance = ScriptableObject.CreateInstance<VirtualCurrencyService>();
				Initialize(instance);
			}
		}

		public static void Initialize(VirtualCurrencyService instance)
		{
			if (instance == null) return;
			virtualCurrencyService = instance;

			if (virtualCurrencyService.virtualCurrencies == null || virtualCurrencyService.virtualCurrencies.Count == 0)
			{
				var types = (CurrencyType[])Enum.GetValues(typeof(CurrencyType));
				virtualCurrencyService.virtualCurrencies = new Dictionary<CurrencyType, VirtualCurrency>(types.Length);
				for (int i = 0; i < types.Length; i++)
				{
					var currency = types[i];
					virtualCurrencyService.virtualCurrencies[currency] = new VirtualCurrency(currency, BigNumber.Zero);
				}
			}

			DataPersistor.Get(virtualCurrencyService);
		}

		public static void Save()
		{
			EnsureInitialized();
			DataPersistor.Save(virtualCurrencyService);
		}

		public static void OnValueChangeRegister(CurrencyType currencyType, PropertyChangedEventHandler changeEvent)
		{
			EnsureInitialized();
			virtualCurrencyService.virtualCurrencies[currencyType].PropertyChanged += changeEvent;
			virtualCurrencyService.virtualCurrencies[currencyType].OnPropertyChanged();
		}

		public static void OnValueChangeUnregister(CurrencyType currencyType, PropertyChangedEventHandler changeEvent)
		{
			EnsureInitialized();
			virtualCurrencyService.virtualCurrencies[currencyType].PropertyChanged -= changeEvent;
		}

		public static BigNumber GetValue(CurrencyType currencyType)
		{
			EnsureInitialized();
			return virtualCurrencyService.virtualCurrencies[currencyType].value;
		}

		public static BigNumber AddValue(CurrencyType currencyType, BigNumber value)
		{
			EnsureInitialized();
			return virtualCurrencyService.virtualCurrencies[currencyType].value += value;
		}

		public static BigNumber AddValue(CurrencyType currencyType, float value)
		{
			EnsureInitialized();
			return AddValue(currencyType, BigNumber.FromDouble(value));
		}

		public static BigNumber AddValue(CurrencyType currencyType, int value)
		{
			EnsureInitialized();
			return AddValue(currencyType, BigNumber.FromDouble(value));
		}

		public static void SetValue(CurrencyType currencyType, BigNumber value)
		{
			EnsureInitialized();
			virtualCurrencyService.virtualCurrencies[currencyType].value = value;
		}

		public static bool Purchase(IPurchasableItem purchasableItem)
		{
			EnsureInitialized();
			if (virtualCurrencyService.virtualCurrencies[purchasableItem.CurrencyType].value >= purchasableItem.Price)
			{
				virtualCurrencyService.virtualCurrencies[purchasableItem.CurrencyType].value -= purchasableItem.Price;
				purchasableItem.PurchaseSuccess();
				return true;
			}

			purchasableItem.PurchasedFailed();
			return false;
		}

		public static bool Purchase(IPurchasableItem purchasableItem, Action onSuccess , Action onFailed)
		{
			EnsureInitialized();
			if (virtualCurrencyService.virtualCurrencies[purchasableItem.CurrencyType].value >= purchasableItem.Price)
			{
				virtualCurrencyService.virtualCurrencies[purchasableItem.CurrencyType].value -= purchasableItem.Price;
				purchasableItem.PurchaseSuccess();
				onSuccess?.Invoke();
				return true;
			}

			purchasableItem.PurchasedFailed();
			onFailed?.Invoke();
			return false;
		}

		public static bool CanPurchase(IPurchasableItem purchasableItem)
		{
			EnsureInitialized();
			return virtualCurrencyService.virtualCurrencies[purchasableItem.CurrencyType].value >= purchasableItem.Price;
		}

		public static string SaveAllToString()
		{
			EnsureInitialized();
			var dict = new Dictionary<string, string>();
			foreach(var kv in virtualCurrencyService.virtualCurrencies)
				dict[((int)kv.Key).ToString()] = kv.Value.value.ToCompactString();
			return JsonUtility.ToJson(new Serialization<string, string>(dict));
		}

		public static void LoadAllFromString(string json)
		{
			EnsureInitialized();
			if (string.IsNullOrEmpty(json)) return;
			var dict = JsonUtility.FromJson<Serialization<string, string>>(json).ToDictionary();
			foreach(var kv in dict)
				if(int.TryParse(kv.Key, out int idx) && Enum.IsDefined(typeof(CurrencyType), idx))
					virtualCurrencyService.virtualCurrencies[(CurrencyType)idx].value = BigNumber.FromCompactString(kv.Value);
		}
	}
}

