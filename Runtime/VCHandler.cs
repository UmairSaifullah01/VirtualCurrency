using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using THEBADDEST.DataManagement;
using UnityEngine;

namespace THEBADDEST.VirtualCurrencySystem
{
	public class VCHandler : IDataElement
	{
		static Dictionary<CurrencyType, VirtualCurrency> virtualCurrencies;
		public string dataTag => "VCHandler";
		public static VCHandler vcHandler;

		/// <summary>
		/// Automatically called by Unity Runtime when the application is loaded.
		/// Initializes the static <see cref="VCHandler"/> instance and all virtual currencies by default value.
		/// </summary>
		[RuntimeInitializeOnLoadMethod]
		public static void Initialize()
		{
			vcHandler = new VCHandler();
			var types = Enum.GetNames(typeof(CurrencyType));
			virtualCurrencies = new Dictionary<CurrencyType, VirtualCurrency>(types.Length);
			for (int i = 0; i < types.Length; i++)
			{
				if (Enum.TryParse(types[i], out CurrencyType currency))
				{
					var virtualCurrency = new VirtualCurrency(currency, 0);
					virtualCurrencies.Add(currency, virtualCurrency);
				}
			}

			DataPersistor.Get(vcHandler);
		}

		/// <summary>
		/// Persists the current state of all virtual currencies to the default data storage location.
		/// </summary>
		public static void Save()
		{
			DataPersistor.Save(vcHandler);
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
		public static float GetValue(CurrencyType currencyType)
		{
			return virtualCurrencies[currencyType].value;
		}

		/// <summary>
		/// Increases the value of a virtual currency by a specified amount.
		/// </summary>
		/// <param name="currencyType">The type of currency to increase.</param>
		/// <param name="value">The amount to add to the currency. Can be negative to decrease the currency.</param>
		/// <returns>The new value of the currency.</returns>
		public static float AddValue(CurrencyType currencyType, float value)
		{
			return virtualCurrencies[currencyType].value += value;
		}

		/// <summary>
		/// Sets the value of a virtual currency.
		/// </summary>
		/// <param name="currencyType">The type of currency to set.</param>
		/// <param name="value">The value to set the currency to.</param>
		public static void SetValue(CurrencyType currencyType, float value)
		{
			virtualCurrencies[currencyType].value = value;
		}

		/// <summary>
		/// Purchase an item using the new IPurchasableWithCurrency interface
		/// </summary>
		/// <param name="purchasable">The item to purchase</param>
		/// <returns>True if the purchase was successful, false otherwise</returns>
		public static bool Purchase(IPurchasableWithCurrency purchasable)
		{
			if (virtualCurrencies[purchasable.CurrencyType].value >= purchasable.Price)
			{
				virtualCurrencies[purchasable.CurrencyType].value -= purchasable.Price;
				purchasable.PurchaseSuccess();
				return true;
			}

			purchasable.PurchasedFailed();
			return false;
		}

		/// <summary>
		/// Purchase an item using the new IPurchasableWithCurrency interface with success and failure callbacks
		/// </summary>
		public static bool Purchase(IPurchasableWithCurrency purchasable, Action onSuccess = null, Action onFailed = null)
		{
			if (virtualCurrencies[purchasable.CurrencyType].value >= purchasable.Price)
			{
				virtualCurrencies[purchasable.CurrencyType].value -= purchasable.Price;
				purchasable.PurchaseSuccess();
				onSuccess?.Invoke();
				return true;
			}

			purchasable.PurchasedFailed();
			onFailed?.Invoke();
			return false;
		}

		/// <summary>
		/// Check if there are sufficient funds to purchase an item
		/// </summary>
		public static bool CanPurchase(IPurchasableWithCurrency purchasable)
		{
			return virtualCurrencies[purchasable.CurrencyType].value >= purchasable.Price;
		}

		/// <summary>
		/// Saves the current state of all virtual currencies in the VCHandler as a Data object.
		/// </summary>
		/// <returns>A Data object containing the current values of all virtual currencies.</returns>
		public Data SaveData()
		{
			float[] values = new float[virtualCurrencies.Count];
			int i = 0;
			foreach (var currency in virtualCurrencies)
			{
				values[i] = currency.Value.value;
				i++;
			}

			return new DataArray<float>(values);
		}

		/// <summary>
		/// Loads the given Data object into the VCHandler, replacing the current values of all virtual currencies.
		/// </summary>
		/// <param name="data">A Data object containing the values to load.</param>
		public void LoadData(Data data)
		{
			if (data == null) return;
			DataArray<float> dataArray = (DataArray<float>)data;
			int i = 0;
			foreach (var currency in virtualCurrencies)
			{
				currency.Value.value = dataArray.values[i];
				i++;
			}
		}

		#region Deprecated Purchase Methods

		/// <summary>
		/// Buy an item using virtual currency.
		/// </summary>
		/// <param name="purchasable">The item to purchase.</param>
		/// <returns>True if the item was purchased successfully, false otherwise.</returns>
		[Obsolete("This method is deprecated. Please use Purchase(IPurchasableWithCurrency) instead for better currency management.")]
		public static bool Buy(IPurchasable purchasable)
		{
			foreach (var currencyValue in purchasable.currencyValues)
			{
				if (virtualCurrencies[currencyValue.type].value >= currencyValue.price)
				{
					virtualCurrencies[currencyValue.type].value -= currencyValue.price;
					purchasable.PurchaseSuccess();
					return true;
				}
			}

			purchasable.PurchasedFailed();
			return false;
		}

		/// <summary>
		/// Buy with Specific Currency
		/// </summary>
		[Obsolete("This method is deprecated. Please use Purchase(IPurchasableWithCurrency) instead for better currency management.")]
		public static bool Buy(IPurchasable purchasable, CurrencyType currencyType)
		{
			CurrencyValue value = purchasable.currencyValues.First(c => c.type == currencyType);
			if (virtualCurrencies[currencyType].value >= value.price)
			{
				virtualCurrencies[currencyType].value -= value.price;
				purchasable.PurchaseSuccess();
				return true;
			}

			purchasable.PurchasedFailed();
			return false;
		}

		/// <summary>
		/// Buy with Possible Currency with callbacks
		/// </summary>
		[Obsolete("This method is deprecated. Please use Purchase(IPurchasableWithCurrency, Action, Action) instead for better currency management.")]
		public static bool Buy(IPurchasable purchasable, Action onPurchaseSuccess, Action onPurchaseFailed)
		{
			foreach (var currencyValue in purchasable.currencyValues)
			{
				if (virtualCurrencies[currencyValue.type].value >= currencyValue.price)
				{
					virtualCurrencies[currencyValue.type].value -= currencyValue.price;
					purchasable.PurchaseSuccess();
					onPurchaseSuccess?.Invoke();
					return true;
				}
			}

			purchasable.PurchasedFailed();
			onPurchaseFailed?.Invoke();
			return false;
		}

		#endregion
	}

	public enum CurrencyType
	{
		Coin = 1 << 0,
		Cash = 1 << 1
		// Add New Name here like Gems = 1 << 2 
	}
}