using System;
using System.Collections.Generic;
using System.ComponentModel;
using THEBADDEST.DataManagement;
using UnityEngine;

namespace THEBADDEST.VirtualCurrencySystem
{
	public class VirtualCurrencyService : IDataElement
	{
		static Dictionary<CurrencyType, VirtualCurrency> virtualCurrencies;
		public string dataTag => "VCHandler";
		public static VirtualCurrencyService virtualCurrencyService;

		/// <summary>
		/// Automatically called by Unity Runtime when the application is loaded.
		/// Initializes the static <see cref="VirtualCurrencyService"/> instance and all virtual currencies by default value.
		/// </summary>
		[RuntimeInitializeOnLoadMethod]
		public static void Initialize()
		{
			virtualCurrencyService = new VirtualCurrencyService();
			var types = Enum.GetNames(typeof(CurrencyType));
			virtualCurrencies = new Dictionary<CurrencyType, VirtualCurrency>(types.Length);
			for (int i = 0; i < types.Length; i++)
			{
				if (Enum.TryParse(types[i], out CurrencyType currency))
				{
					var virtualCurrency = new VirtualCurrency(currency, BigNumber.Zero);
					virtualCurrencies.Add(currency, virtualCurrency);
				}
			}

			DataPersistor.Get(virtualCurrencyService);
		}

		/// <summary>
		/// Persists the current state of all virtual currencies to the default data storage location.
		/// </summary>
		public static void Save()
		{
			DataPersistor.Save(virtualCurrencyService);
		}

		/// <summary>
		/// Registers a <see cref="PropertyChangedEventHandler"/> to a virtual currency's value change event.
		/// </summary>
		/// <param name="currencyType">The type of currency to register the event handler for.</param>
		/// <param name="changeEvent">The event handler to register.</param>
		public static void OnValueChangeRegister(CurrencyType currencyType, PropertyChangedEventHandler changeEvent)
		{
			virtualCurrencies[currencyType].PropertyChanged += changeEvent;
			virtualCurrencies[currencyType].OnPropertyChanged();
		}

		/// <summary>
		/// Unregisters a <see cref="PropertyChangedEventHandler"/> from a virtual currency's value change event.
		/// </summary>
		/// <param name="currencyType">The type of currency to unregister the event handler from.</param>
		/// <param name="changeEvent">The event handler to unregister.</param>
		public static void OnValueChangeUnregister(CurrencyType currencyType, PropertyChangedEventHandler changeEvent)
		{
			virtualCurrencies[currencyType].PropertyChanged -= changeEvent;
		}

		/// <summary>
		/// Retrieves the current value of a virtual currency.
		/// </summary>
		/// <param name="currencyType">The type of currency to retrieve the value of.</param>
		/// <returns>The current value of the specified currency.</returns>
		public static BigNumber GetValue(CurrencyType currencyType)
		{
			return virtualCurrencies[currencyType].value;
		}

		/// <summary>
		/// Increases the value of a virtual currency by a specified amount.
		/// </summary>
		/// <param name="currencyType">The type of currency to increase.</param>
		/// <param name="value">The amount to add to the currency. Can be negative to decrease the currency.</param>
		/// <returns>The new value of the currency.</returns>
		public static BigNumber AddValue(CurrencyType currencyType, BigNumber value)
		{
			return virtualCurrencies[currencyType].value += value;
		}

		public static BigNumber AddValue(CurrencyType currencyType, float value)
		{
			return AddValue(currencyType, BigNumber.FromDouble(value));
		}

		public static BigNumber AddValue(CurrencyType currencyType, int value)
		{
			return AddValue(currencyType, BigNumber.FromDouble(value));
		}

		/// <summary>
		/// Sets the value of a virtual currency.
		/// </summary>
		/// <param name="currencyType">The type of currency to set.</param>
		/// <param name="value">The value to set the currency to.</param>
		public static void SetValue(CurrencyType currencyType, BigNumber value)
		{
			virtualCurrencies[currencyType].value = value;
		}

		/// <summary>
		/// Purchase an item using the new IPurchasableWithCurrency interface
		/// </summary>
		/// <param name="purchasableItem">The item to purchase</param>
		/// <returns>True if the purchase was successful, false otherwise</returns>
		public static bool Purchase(IPurchasableItem purchasableItem)
		{
			if (virtualCurrencies[purchasableItem.CurrencyType].value >= purchasableItem.Price)
			{
				virtualCurrencies[purchasableItem.CurrencyType].value -= purchasableItem.Price;
				purchasableItem.PurchaseSuccess();
				return true;
			}

			purchasableItem.PurchasedFailed();
			return false;
		}

		/// <summary>
		/// Purchase an item using the new IPurchasableWithCurrency interface with success and failure callbacks
		/// </summary>
		public static bool Purchase(IPurchasableItem purchasableItem, Action onSuccess = null, Action onFailed = null)
		{
			if (virtualCurrencies[purchasableItem.CurrencyType].value >= purchasableItem.Price)
			{
				virtualCurrencies[purchasableItem.CurrencyType].value -= purchasableItem.Price;
				purchasableItem.PurchaseSuccess();
				onSuccess?.Invoke();
				return true;
			}

			purchasableItem.PurchasedFailed();
			onFailed?.Invoke();
			return false;
		}

		/// <summary>
		/// Check if there are sufficient funds to purchase an item
		/// </summary>
		public static bool CanPurchase(IPurchasableItem purchasableItem)
		{
			return virtualCurrencies[purchasableItem.CurrencyType].value >= purchasableItem.Price;
		}

		/// <summary>
		/// Saves the current state of all virtual currencies deterministically using compact string format.
		/// </summary>
		/// <returns>A Data object containing the compact string values of all currencies in enum order.</returns>
		public Data SaveData()
		{
			var currencyTypes = (CurrencyType[])Enum.GetValues(typeof(CurrencyType));
			string[] values = new string[currencyTypes.Length];
			for (int i = 0; i < currencyTypes.Length; i++)
			{
				values[i] = virtualCurrencies[currencyTypes[i]].value.ToCompactString();
			}

			return new DataArray<string>(values);
		}

		/// <summary>
		/// Loads currency values from a deterministic compact string array saved in enum order.
		/// </summary>
		/// <param name="data">A Data object containing compact string values.</param>
		public void LoadData(Data data)
		{
			if (data == null) return;
			DataArray<string> dataArray = (DataArray<string>)data;
			var currencyTypes = (CurrencyType[])Enum.GetValues(typeof(CurrencyType));
			int len = Mathf.Min(currencyTypes.Length, dataArray.values.Length);
			for (int i = 0; i < len; i++)
			{
				virtualCurrencies[currencyTypes[i]].value = BigNumber.FromCompactString(dataArray.values[i]);
			}
		}

		/// <summary>
		/// Save all virtual currencies as a string.
		/// </summary>
		public static string SaveAllToString()
		{
			var dict = new Dictionary<string, string>();
			foreach(var kv in virtualCurrencies)
				dict[((int)kv.Key).ToString()] = kv.Value.value.ToCompactString();
			return JsonUtility.ToJson(new Serialization<string, string>(dict));
		}

		/// <summary>
		/// Load all virtual currencies from a string.
		/// </summary>
		public static void LoadAllFromString(string json)
		{
			if (string.IsNullOrEmpty(json)) return;
			var dict = JsonUtility.FromJson<Serialization<string, string>>(json).ToDictionary();
			foreach(var kv in dict)
				if(int.TryParse(kv.Key, out int idx) && Enum.IsDefined(typeof(CurrencyType), idx))
					virtualCurrencies[(CurrencyType)idx].value = BigNumber.FromCompactString(kv.Value);
		}

		[System.Serializable]
		public class Serialization<TKey, TValue>
		{
			public List<TKey> keys = new List<TKey>();
			public List<TValue> values = new List<TValue>();

			public Serialization(Dictionary<TKey, TValue> dict)
			{
				keys = new List<TKey>(dict.Keys);
				values = new List<TValue>(dict.Values);
			}
			public Dictionary<TKey, TValue> ToDictionary()
			{
				var dict = new Dictionary<TKey, TValue>();
				for(int i = 0; i < keys.Count; i++)
					dict[keys[i]] = values[i];
				return dict;
			}
		}
	}

	public enum CurrencyType
	{
		Coin = 0,
		Cash = 1,
		Gems = 2 // Add new currencies sequentially
	}
}