using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using UnityEngine;


namespace THEBADDEST.VirtualCurrencySystem
{


	public class VCHandler
	{

		#region Static Fields

		static VCHandler instance;
		public static VCHandler Instance
		{
			get
			{
				if (instance == null)
					instance = new VCHandler();
				return instance;
			}
		}

		[RuntimeInitializeOnLoadMethod]
		static void Initialize()
		{
			if (Instance == null)
			{
				instance = new VCHandler();
			}
		}

		#endregion


		VirtualCurrency[] virtualCurrencies;


		// Constructor .....
		VCHandler()
		{
			//Get all Currencies values if saved
			var v = Enum.GetNames(typeof(Currency));
			virtualCurrencies = new VirtualCurrency[v.Length];
			StringBuilder builder = new StringBuilder("Initializing Virtual Currency");
			for (int i = 0; i < v.Length; i++)
			{
				if (Enum.TryParse(v[i], out Currency currency))
				{
					builder.Append(" ");
					virtualCurrencies[i] = new VirtualCurrency(currency, 0);
					builder.Append(currency);
				}
			}

			Debug.Log(builder.ToString());
		}

		public void OnValueChangeRegister(Currency currencyName, PropertyChangedEventHandler changeEvent)
		{
			foreach (VirtualCurrency vc in virtualCurrencies)
			{
				if (vc.Name == currencyName)
				{
					vc.PropertyChanged += changeEvent;
					changeEvent.Invoke(vc, null);
					return;
				}
			}
		}

		public void OnValueChangeUnregister(Currency currencyName, PropertyChangedEventHandler changeEvent)
		{
			foreach (VirtualCurrency vc in virtualCurrencies)
			{
				if (vc.Name == currencyName)
				{
					vc.PropertyChanged -= changeEvent;
					return;
				}
			}
		}


		public float GetValue(Currency currencyName)
		{
			return (from vc in virtualCurrencies where vc.Name == currencyName select vc.value).FirstOrDefault();
		}

		public float AddValue(Currency currencyName, float value)
		{
			foreach (VirtualCurrency vc in virtualCurrencies)
			{
				if (vc.Name == currencyName)
				{
					vc.value += value;
					return vc.value;
				}
			}

			return 0.0f;
		}

		public void SetValue(Currency currencyName, float value)
		{
			foreach (VirtualCurrency vc in virtualCurrencies)
			{
				if (vc.Name == currencyName)
				{
					vc.value = value;
					return;
				}
			}
		}

		public bool Buy(IPurchasable purchasable, Action OnPurchaseSuccess, Action OnPurchaseFailed)
		{
			for (int i = 0; i < virtualCurrencies.Length; i++)
			{
				if (virtualCurrencies[i].Name == purchasable.CurrencyName)
				{
					if (virtualCurrencies[i].value >= purchasable.Price)
					{
						virtualCurrencies[i].value -= purchasable.Price;
						purchasable.Purchased();
						OnPurchaseSuccess?.Invoke();
						return true;
					}
					else
					{
						OnPurchaseFailed?.Invoke();
						return false;
					}
				}
			}

			OnPurchaseFailed?.Invoke();
			return false;
		}

	}

	public enum Currency
	{

		Coin = 1 << 0,
		Cash = 1 << 1
		// Add New Name here like Gems = 1 << 2 

	}


}