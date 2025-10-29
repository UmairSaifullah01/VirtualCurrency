using UnityEngine;

namespace THEBADDEST.VirtualCurrencySystem
{


	/// <summary>
	/// Interface for implementing purchasable items with a single currency type.
	/// This is the recommended interface for implementing purchasable items.
	/// </summary>
	public interface IPurchasableItem
	{
		/// <summary>
		/// The type of currency required for the purchase
		/// </summary>
		CurrencyType CurrencyType { get; }

		/// <summary>
		/// The price of the item in the specified currency
		/// </summary>
		BigNumber Price { get; }

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
}