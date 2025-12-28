using System.ComponentModel;
using UnityEngine;


namespace THEBADDEST.VirtualCurrencySystem
{


	public abstract class CurrencyBinding : MonoBehaviour
	{

		[SerializeField] protected string currencyName = CurrencyType.Coin;

		protected virtual void OnDestroy()
		{
			VirtualCurrencyService.OnValueChangeUnregister(currencyName, ChangeEffect);
		}

		protected abstract void ChangeEffect(object sender, PropertyChangedEventArgs args);

		protected virtual void Start()
		{
			VirtualCurrencyService.OnValueChangeRegister(currencyName, ChangeEffect);
		}

	}


}