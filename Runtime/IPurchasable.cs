using System;
using UnityEngine;

namespace THEBADDEST.VirtualCurrencySystem
{
	[Obsolete("IPurchasable is deprecated. Please use IPurchasableWithCurrency instead for better currency management.")]
	public interface IPurchasable
	{
		CurrencyValue[] currencyValues { get; }
		bool IsPurchased { get; set; }

		void PurchaseSuccess();
		void PurchasedFailed();
	}

	/// <summary>
	/// Interface for implementing purchasable items with a single currency type.
	/// This is the recommended interface for implementing purchasable items.
	/// </summary>
	public interface IPurchasableWithCurrency
	{
		/// <summary>
		/// The type of currency required for the purchase
		/// </summary>
		CurrencyType CurrencyType { get; }

		/// <summary>
		/// The price of the item in the specified currency
		/// </summary>
		float Price { get; }

		/// <summary>
		/// Whether the item has been purchased
		/// </summary>
		bool IsPurchased { get; set; }

		/// <summary>
		/// Called when the purchase is successful
		/// </summary>
		void PurchaseSuccess();

		/// <summary>
		/// Called when the purchase fails
		/// </summary>
		void PurchasedFailed();
	}

	[Serializable]
	public struct CurrencyValue
	{
		[Tooltip("Type of currency")]
		public CurrencyType type;

		[Tooltip("Amount of currency required")]
		public float price;
	}
}