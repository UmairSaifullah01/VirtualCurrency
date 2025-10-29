using System;
using UnityEngine;

namespace THEBADDEST.VirtualCurrencySystem
{

	/// <summary>
	/// ScriptableObject-based purchasable item definition implementing <see cref="IPurchasableItem"/>.
	/// Create assets via the Create menu and reference them in your game content.
	/// </summary>
	[CreateAssetMenu(fileName = "PurchasableItem", menuName = "Virtual Currency/Purchasable Item", order = 0)]
	public class PurchasableItemSO : ScriptableObject, IPurchasableItem
	{
		[Header("Purchase Settings")]
		[SerializeField] private CurrencyType currencyType = CurrencyType.Coin;
		[SerializeField] private float price = 0f;

		[Header("Presentation (Optional)")]
		[SerializeField] private string displayName;
		[SerializeField] private Sprite icon;

		private bool purchased;
		private Action onSuccess, onFail;

		/// <summary>
		/// The currency type required for this purchase.
		/// </summary>
		public CurrencyType CurrencyType => currencyType;

		/// <summary>
		/// The price represented as a <see cref="BigNumber"/>.
		/// </summary>
		public BigNumber Price => BigNumber.FromDouble(price);

		/// <summary>
		/// Whether this item has been purchased in the current session.
		/// </summary>
		public bool IsPurchased
		{
			get => purchased;
			set => purchased = value;
		}

		/// <summary>
		/// Optional human-readable name for UI.
		/// </summary>
		public string DisplayName => displayName;

		/// <summary>
		/// Optional icon for UI.
		/// </summary>
		public Sprite Icon => icon;

		/// <summary>
		/// Register a callback to be invoked when a purchase succeeds.
		/// </summary>
		public void OnPurchaseSuccess(Action callback)
		{
			onSuccess += callback;
		}

		/// <summary>
		/// Register a callback to be invoked when a purchase fails.
		/// </summary>
		public void OnPurchasedFailed(Action callback)
		{
			onFail += callback;
		}

		/// <summary>
		/// Invoke success callbacks and clear them. Called by the purchase flow when successful.
		/// </summary>
		public void PurchaseSuccess()
		{
			onSuccess?.Invoke();
			onSuccess = null;
		}

		/// <summary>
		/// Invoke failure callbacks and clear them. Called by the purchase flow when failed.
		/// </summary>
		public void PurchasedFailed()
		{
			onFail?.Invoke();
			onFail = null;
		}
	}

}


