using System;
using System.Collections.Generic;
using THEBADDEST.DataManagement;
using UnityEngine;

namespace THEBADDEST.VirtualCurrencySystem
{
	public partial class VirtualCurrencyService : ScriptableObject, IDataElement
	{
		public string dataTag => "VCHandler";
		Dictionary<CurrencyType, VirtualCurrency> virtualCurrencies;

		private void OnEnable()
		{
			VirtualCurrencyService.Initialize(this);
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
				values[i] = VirtualCurrencyService.GetValue(currencyTypes[i]).ToCompactString();
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
                VirtualCurrencyService.SetValue(currencyTypes[i], BigNumber.FromCompactString(dataArray.values[i]));
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

	public enum CurrencyType
	{
		Coin = 0,
		Cash = 1,
		Gems = 2 // Add new currencies sequentially
	}
}