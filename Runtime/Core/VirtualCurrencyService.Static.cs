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
				// Try to load from Resources first
				var instance = Resources.Load<VirtualCurrencyService>("VirtualCurrencyService");
				
				// If not found in Resources, create a new instance
				if (instance == null)
				{
					instance = ScriptableObject.CreateInstance<VirtualCurrencyService>();
					Debug.LogWarning("VirtualCurrencyService not found in Resources. A new instance has been created. Please create a VirtualCurrencyService asset in a Resources folder.");
				}
				
				Initialize(instance);
			}
		}

		public static void Initialize(VirtualCurrencyService instance)
		{
			if (instance == null) return;
			virtualCurrencyService = instance;

			if (virtualCurrencyService.virtualCurrencies == null || virtualCurrencyService.virtualCurrencies.Count == 0)
			{
				if (virtualCurrencyService.currencyConfigs == null || virtualCurrencyService.currencyConfigs.Count == 0)
				{
					// Default currencies if config is empty
					virtualCurrencyService.virtualCurrencies = new Dictionary<string, VirtualCurrency>();
					virtualCurrencyService.virtualCurrencies[CurrencyType.Coin] = new VirtualCurrency(CurrencyType.Coin, BigNumber.Zero);
					virtualCurrencyService.virtualCurrencies[CurrencyType.Gem] = new VirtualCurrency(CurrencyType.Gem, BigNumber.Zero);
					virtualCurrencyService.virtualCurrencies[CurrencyType.Gold] = new VirtualCurrency(CurrencyType.Gold, BigNumber.Zero);
				}
				else
				{
					virtualCurrencyService.virtualCurrencies = new Dictionary<string, VirtualCurrency>(virtualCurrencyService.currencyConfigs.Count);
					for (int i = 0; i < virtualCurrencyService.currencyConfigs.Count; i++)
					{
						var config = virtualCurrencyService.currencyConfigs[i];
						if (!string.IsNullOrEmpty(config.currencyName))
						{
							virtualCurrencyService.virtualCurrencies[config.currencyName] = new VirtualCurrency(config.currencyName, config.GetInitialValue());
						}
					}
				}
			}

			DataPersistor.Get(virtualCurrencyService);
		}

		public static void Save()
		{
			EnsureInitialized();
			DataPersistor.Save(virtualCurrencyService);
		}

		public static void OnValueChangeRegister(string currencyName, PropertyChangedEventHandler changeEvent)
		{
			EnsureInitialized();
			if (virtualCurrencyService.virtualCurrencies.ContainsKey(currencyName))
			{
				virtualCurrencyService.virtualCurrencies[currencyName].PropertyChanged += changeEvent;
				virtualCurrencyService.virtualCurrencies[currencyName].OnPropertyChanged();
			}
		}

		public static void OnValueChangeUnregister(string currencyName, PropertyChangedEventHandler changeEvent)
		{
			EnsureInitialized();
			if (virtualCurrencyService.virtualCurrencies.ContainsKey(currencyName))
			{
				virtualCurrencyService.virtualCurrencies[currencyName].PropertyChanged -= changeEvent;
			}
		}

		/// <summary>
		/// Gets the current value of a currency as a BigNumber.
		/// </summary>
		/// <param name="currencyName">The name of the currency.</param>
		/// <returns>The current value as BigNumber, or BigNumber.Zero if currency not found.</returns>
		public static BigNumber GetValue(string currencyName)
		{
			EnsureInitialized();
			if (virtualCurrencyService.virtualCurrencies.ContainsKey(currencyName))
			{
				return virtualCurrencyService.virtualCurrencies[currencyName].value;
			}
			return BigNumber.Zero;
		}

		/// <summary>
		/// Gets the current value of a currency as an int.
		/// </summary>
		/// <param name="currencyName">The name of the currency.</param>
		/// <returns>The current value as int, or 0 if currency not found.</returns>
		public static int GetValueInt(string currencyName)
		{
			EnsureInitialized();
			if (virtualCurrencyService.virtualCurrencies.ContainsKey(currencyName))
			{
				return (int)virtualCurrencyService.virtualCurrencies[currencyName].value.ToDouble();
			}
			return 0;
		}

		/// <summary>
		/// Gets the current value of a currency as a float.
		/// </summary>
		/// <param name="currencyName">The name of the currency.</param>
		/// <returns>The current value as float, or 0f if currency not found.</returns>
		public static float GetValueFloat(string currencyName)
		{
			EnsureInitialized();
			if (virtualCurrencyService.virtualCurrencies.ContainsKey(currencyName))
			{
				return (float)virtualCurrencyService.virtualCurrencies[currencyName].value.ToDouble();
			}
			return 0f;
		}

		public static BigNumber AddValue(string currencyName, BigNumber value)
		{
			EnsureInitialized();
			if (virtualCurrencyService.virtualCurrencies.ContainsKey(currencyName))
			{
				return virtualCurrencyService.virtualCurrencies[currencyName].value += value;
			}
			return BigNumber.Zero;
		}

		public static BigNumber AddValue(string currencyName, float value)
		{
			EnsureInitialized();
			return AddValue(currencyName, BigNumber.FromDouble(value));
		}

		public static BigNumber AddValue(string currencyName, int value)
		{
			EnsureInitialized();
			return AddValue(currencyName, BigNumber.FromDouble(value));
		}

		/// <summary>
		/// Sets the value of a currency to a specific BigNumber.
		/// </summary>
		/// <param name="currencyName">The name of the currency.</param>
		/// <param name="value">The BigNumber value to set.</param>
		public static void SetValue(string currencyName, BigNumber value)
		{
			EnsureInitialized();
			if (virtualCurrencyService.virtualCurrencies.ContainsKey(currencyName))
			{
				virtualCurrencyService.virtualCurrencies[currencyName].value = value;
			}
		}

		/// <summary>
		/// Sets the value of a currency to a specific int.
		/// </summary>
		/// <param name="currencyName">The name of the currency.</param>
		/// <param name="value">The int value to set.</param>
		public static void SetValue(string currencyName, int value)
		{
			EnsureInitialized();
			if (virtualCurrencyService.virtualCurrencies.ContainsKey(currencyName))
			{
				virtualCurrencyService.virtualCurrencies[currencyName].value = BigNumber.FromDouble(value);
			}
		}

		/// <summary>
		/// Sets the value of a currency to a specific float.
		/// </summary>
		/// <param name="currencyName">The name of the currency.</param>
		/// <param name="value">The float value to set.</param>
		public static void SetValue(string currencyName, float value)
		{
			EnsureInitialized();
			if (virtualCurrencyService.virtualCurrencies.ContainsKey(currencyName))
			{
				virtualCurrencyService.virtualCurrencies[currencyName].value = BigNumber.FromDouble(value);
			}
		}

		/// <summary>
		/// Attempts to purchase using the specified currency with a float price.
		/// </summary>
		/// <param name="currencyName">The name of the currency to use for purchase.</param>
		/// <param name="purchasePrice">The price as a float.</param>
		/// <returns>True if purchase was successful, false if insufficient funds or currency not found.</returns>
		public static bool Purchase(string currencyName, float purchasePrice)
		{
			return Purchase(currencyName, BigNumber.FromDouble(purchasePrice));
		}

		/// <summary>
		/// Attempts to purchase using the specified currency with an int price.
		/// </summary>
		/// <param name="currencyName">The name of the currency to use for purchase.</param>
		/// <param name="purchasePrice">The price as an int.</param>
		/// <returns>True if purchase was successful, false if insufficient funds or currency not found.</returns>
		public static bool Purchase(string currencyName, int purchasePrice)
		{
			return Purchase(currencyName, BigNumber.FromDouble(purchasePrice));
		}

		/// <summary>
		/// Attempts to purchase using the specified currency with a BigNumber price.
		/// </summary>
		/// <param name="currencyName">The name of the currency to use for purchase.</param>
		/// <param name="purchasePrice">The price as a BigNumber.</param>
		/// <returns>True if purchase was successful, false if insufficient funds or currency not found.</returns>
		public static bool Purchase(string currencyName, BigNumber purchasePrice)
		{
			EnsureInitialized();
			if (virtualCurrencyService.virtualCurrencies.ContainsKey(currencyName) && 
			    virtualCurrencyService.virtualCurrencies[currencyName].value >= purchasePrice)
			{
				virtualCurrencyService.virtualCurrencies[currencyName].value -= purchasePrice;
				return true;
			}
			return false;
		}
		public static bool Purchase(IPurchasableItem purchasableItem)
		{
			EnsureInitialized();
			string currencyName = purchasableItem.CurrencyName;
			if (virtualCurrencyService.virtualCurrencies.ContainsKey(currencyName) && 
			    virtualCurrencyService.virtualCurrencies[currencyName].value >= purchasableItem.Price)
			{
				virtualCurrencyService.virtualCurrencies[currencyName].value -= purchasableItem.Price;
				purchasableItem.PurchaseSuccess();
				return true;
			}

			purchasableItem.PurchasedFailed();
			return false;
		}

		public static bool Purchase(IPurchasableItem purchasableItem, Action onSuccess , Action onFailed)
		{
			EnsureInitialized();
			string currencyName = purchasableItem.CurrencyName;
			if (virtualCurrencyService.virtualCurrencies.ContainsKey(currencyName) && 
			    virtualCurrencyService.virtualCurrencies[currencyName].value >= purchasableItem.Price)
			{
				virtualCurrencyService.virtualCurrencies[currencyName].value -= purchasableItem.Price;
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
			string currencyName = purchasableItem.CurrencyName;
			if (virtualCurrencyService.virtualCurrencies.ContainsKey(currencyName))
			{
				return virtualCurrencyService.virtualCurrencies[currencyName].value >= purchasableItem.Price;
			}
			return false;
		}

		public static string SaveAllToString()
		{
			EnsureInitialized();
			var dict = new Dictionary<string, string>();
			foreach(var kv in virtualCurrencyService.virtualCurrencies)
				dict[kv.Key] = kv.Value.value.ToCompactString();
			return JsonUtility.ToJson(new Serialization<string, string>(dict));
		}

		public static void LoadAllFromString(string json)
		{
			EnsureInitialized();
			if (string.IsNullOrEmpty(json)) return;
			var dict = JsonUtility.FromJson<Serialization<string, string>>(json).ToDictionary();
			foreach(var kv in dict)
			{
				if (virtualCurrencyService.virtualCurrencies.ContainsKey(kv.Key))
				{
					virtualCurrencyService.virtualCurrencies[kv.Key].value = BigNumber.FromCompactString(kv.Value);
				}
			}
		}
	}
}

