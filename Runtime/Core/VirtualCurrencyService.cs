using System;
using System.Collections.Generic;
using THEBADDEST.DataManagement;
using UnityEngine;

namespace THEBADDEST.VirtualCurrencySystem
{
	public enum CurrencyValueType
	{
		Int,
		Float,
		BigNumber
	}

	[System.Serializable]
	public class CurrencyConfig
	{
		[Tooltip("The name of this currency (e.g., Coin, Gem, Gold)")]
		public string currencyName = CurrencyType.Coin;

		[Tooltip("The type of value this currency uses")]
		public CurrencyValueType valueType = CurrencyValueType.BigNumber;

		[Tooltip("Initial value when using Int type")]
		public int initialValueInt = 0;

		[Tooltip("Initial value when using Float type")]
		public float initialValueFloat = 0f;

		[Tooltip("Initial value when using BigNumber type (mantissa|exponent format)")]
		public string initialValueBigNumber = "0|0";

		/// <summary>
		/// Gets the initial value as a BigNumber based on the configured value type.
		/// </summary>
		public BigNumber GetInitialValue()
		{
			switch (valueType)
			{
				case CurrencyValueType.Int:
					return BigNumber.FromDouble(initialValueInt);
				case CurrencyValueType.Float:
					return BigNumber.FromDouble(initialValueFloat);
				case CurrencyValueType.BigNumber:
					return BigNumber.FromCompactString(initialValueBigNumber);
				default:
					return BigNumber.Zero;
			}
		}
	}
	[CreateAssetMenu(fileName = "VirtualCurrencyService", menuName = "THEBADDEST/Virtual Currency/Virtual Currency Service", order = 0)]
	public partial class VirtualCurrencyService : ScriptableObject, IDataElement
	{
		public string dataTag => "VCHandler";
		Dictionary<string, VirtualCurrency> virtualCurrencies;

		[Header("Currency Configuration")]
		[Tooltip("List of all currencies in the system. Configure name, initial value, and value type for each.")]
		[SerializeField] private List<CurrencyConfig> currencyConfigs = new List<CurrencyConfig>
		{
			new CurrencyConfig { currencyName = CurrencyType.Coin, valueType = CurrencyValueType.BigNumber, initialValueBigNumber = "0|0" },
			new CurrencyConfig { currencyName = CurrencyType.Gem, valueType = CurrencyValueType.BigNumber, initialValueBigNumber = "0|0" },
			new CurrencyConfig { currencyName = CurrencyType.Gold, valueType = CurrencyValueType.BigNumber, initialValueBigNumber = "0|0" }
		};
		

		/// <summary>
		/// Saves the current state of all virtual currencies deterministically using compact string format.
		/// </summary>
		/// <returns>A Data object containing the compact string values of all currencies in config order.</returns>
		public Data SaveData()
		{
			if (currencyConfigs == null || currencyConfigs.Count == 0) return new DataArray<string>(new string[0]);
			
			string[] values = new string[currencyConfigs.Count];
			for (int i = 0; i < currencyConfigs.Count; i++)
			{
				string currencyName = currencyConfigs[i].currencyName;
				if (virtualCurrencies != null && virtualCurrencies.ContainsKey(currencyName))
				{
					values[i] = virtualCurrencies[currencyName].value.ToCompactString();
				}
				else
				{
					values[i] = currencyConfigs[i].GetInitialValue().ToCompactString();
				}
			}

			return new DataArray<string>(values);
		}

		/// <summary>
		/// Loads currency values from a deterministic compact string array saved in config order.
		/// </summary>
		/// <param name="data">A Data object containing compact string values.</param>
		public void LoadData(Data data)
		{
			if (data == null || currencyConfigs == null) return;
			DataArray<string> dataArray = (DataArray<string>)data;
			int len = Mathf.Min(currencyConfigs.Count, dataArray.values.Length);
			for (int i = 0; i < len; i++)
			{
				string currencyName = currencyConfigs[i].currencyName;
				VirtualCurrencyService.SetValue(currencyName, BigNumber.FromCompactString(dataArray.values[i]));
			}
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
}