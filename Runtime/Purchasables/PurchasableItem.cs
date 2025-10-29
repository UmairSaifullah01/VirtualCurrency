using System;
using UnityEngine;

namespace THEBADDEST.VirtualCurrencySystem
{


	/// <summary>
	/// Serializable plain-data implementation of <see cref="IPurchasableItem"/>.
	/// Use this for runtime-created or serialized-in-scene purchasable definitions.
	/// For authoring via assets, consider <see cref="PurchasableItemSO"/>.
	/// </summary>
	[Serializable]
	public class PurchasableItem : IPurchasableItem
	{

		[SerializeField] private CurrencyType currencyType;
		[SerializeField] private float price;

		private bool purchased = false;

		private Action OnSuccess, OnFail;
		
		/// <summary>
		/// The currency type required for this purchase.
		/// </summary>
		public CurrencyType CurrencyType => currencyType;
		
		/// <summary>
		/// The price represented as a <see cref="BigNumber"/>.
		/// </summary>
		public BigNumber Price => BigNumber.FromDouble(price);
		
		/// <summary>
		/// Indicates whether the item has been purchased.
		/// This is a simple flag and does not persist automatically.
		/// </summary>
		public bool IsPurchased
		{
			get => purchased;
			set => purchased = value;
		}
		
		/// <summary>
		/// Register a callback to be invoked when a purchase succeeds.
		/// </summary>
		public void OnPurchaseSuccess(Action onSuccess)
		{
			this.OnSuccess += onSuccess;
		}

		/// <summary>
		/// Register a callback to be invoked when a purchase fails.
		/// </summary>
		public void OnPurchasedFailed(Action onFail)
		{
			this.OnFail += onFail;
		}

		/// <summary>
		/// Invoke success callbacks and clear them. Call from your purchase flow on success.
		/// </summary>
		public void PurchaseSuccess()
		{
			this.OnSuccess?.Invoke();
			this.OnSuccess = null;
		}

		/// <summary>
		/// Invoke failure callbacks and clear them. Call from your purchase flow on failure.
		/// </summary>
		public void PurchasedFailed()
		{
			this.OnFail?.Invoke();
			this.OnFail = null;
		}

	}


}