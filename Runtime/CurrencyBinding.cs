using System.ComponentModel;
using UnityEngine;


namespace THEBADDEST.VirtualCurrencySystem
{


	public abstract class CurrencyBinding : MonoBehaviour
	{

		[SerializeField] protected CurrencyType currencyTypeName = CurrencyType.Coin;

		protected virtual void OnDestroy()
		{
			VCHandler.OnValueChangeUnregister(currencyTypeName, ChangeEffect);
		}

		protected abstract void ChangeEffect(object sender, PropertyChangedEventArgs args);

		protected virtual void Start()
		{
			VCHandler.OnValueChangeRegister(currencyTypeName, ChangeEffect);
		}

	}


}