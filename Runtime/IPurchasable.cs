using System;
using UnityEngine;


namespace THEBADDEST.VirtualCurrencySystem
{

	public interface IPurchasable
	{

		CurrencyValue[] currencyValues { get; }
		bool            IsPurchased    { get; set; }

		void PurchaseSuccess ();
		void PurchasedFailed();

	}

	[Serializable]
	public struct CurrencyValue
	{

		public CurrencyType type;
		public float    price;

	}


}