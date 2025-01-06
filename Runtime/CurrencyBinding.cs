using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;


namespace THEBADDEST.VirtualCurrencySystem
{


	public abstract class CurrencyBinding : MonoBehaviour
	{

		[SerializeField] protected CurrencyType currencyTypeName;

		private void OnDestroy()
		{
			VCHandler.OnValueChangeUnregister(currencyTypeName, ChangeEffect);
		}

		protected abstract void ChangeEffect(object sender, PropertyChangedEventArgs args);

		private void Start()
		{
			VCHandler.OnValueChangeRegister(currencyTypeName, ChangeEffect);
		}

	}


}