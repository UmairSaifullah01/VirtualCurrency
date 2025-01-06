//
// using System.Collections;
//
// using System.ComponentModel;
//
// using THEBADDEST.MVVM;
// using THEBADDEST.UI;
// using TMPro;
// using UnityEngine;
// using UnityEngine.UI;
// using Random = UnityEngine.Random;
//
//
// namespace THEBADDEST.VirtualCurrencySystem
// {
//
//
// 	public enum CurrencyUpdateEffect
// 	{
//
// 		Sudden,
// 		CounterUpdate,
// 		MoveToIconPosition,
//
// 	}
//
// 	public class CurrencyCounter : ViewBase
// 	{
//
// 		[SerializeField] protected                  CurrencyType        currencyTypeName;
// 		[SerializeField, EditorAttributes.ReadOnly] TextMeshProUGUI textMesh;
// 		[SerializeField, EditorAttributes.ReadOnly] Image           icon;
// 		[SerializeField, EditorAttributes.ReadOnly] Camera          uiCamera;
// 		[SerializeField, EditorAttributes.ReadOnly] Camera          mainCamera;
// 		int                                                         oldValue;
// 		int                                                         currentValue;
// 		CurrencyUpdateEffect                                        effect;
// 		Vector3                                                     worldPosition;
//
// 		Coroutine              tweenCor;
// 		static CurrencyCounter Instance { get; set; }
//
// 		void SetValue()
// 		{
// 			Instance = this;
// 		}
//
// 		public static void SendCurrency(CurrencyType currencyType, int amount, CurrencyUpdateEffect effect = CurrencyUpdateEffect.Sudden, Vector3 worldPosition = default)
// 		{
// 			Instance.SendCurrencyInternal(currencyType, amount, effect, worldPosition);
// 		}
//
// 		public override void Init(IViewModel viewModel)
// 		{
// 			base.Init(viewModel);
// 			SetValue();
// 			if (textMesh == null) textMesh = GetComponentInChildren<TextMeshProUGUI>();
// 			if (icon     == null) icon     = transform.FindChildByRecursion<Image>("Icon");
// 		}
//
// 		void SendCurrencyInternal(CurrencyType currencyType, int amount, CurrencyUpdateEffect effect = CurrencyUpdateEffect.Sudden, Vector3 worldPosition = default)
// 		{
// 			this.effect        = effect;
// 			this.worldPosition = worldPosition;
// 			VCHandler.AddValue(currencyType, amount);
// 		}
//
//
// 		void Start()
// 		{
// 			VCHandler.OnValueChangeRegister(currencyTypeName, ChangeEffect);
// 			oldValue = VCHandler.GetValue(currencyTypeName).ToInt();
// 		}
//
// 		private void ChangeEffect(object sender, PropertyChangedEventArgs args)
// 		{
// 			float value = ((VirtualCurrency) sender).value;
// 			
// 			oldValue     = currentValue;
// 			currentValue = value.ToInt();
// 			switch (effect)
// 			{
// 				case CurrencyUpdateEffect.Sudden:
// 					string shortNumber = currentValue.ToShortNumber();
// 					textMesh.text = shortNumber;
// 					break;
//
// 				case CurrencyUpdateEffect.CounterUpdate:
// 		//			DOVirtual.Float(oldValue, currentValue, 1.5f, (updatedValue) => { textMesh.text = updatedValue.ToInt().ToShortNumber(); }).OnComplete(() => { oldValue = currentValue; });
// 					break;
//
// 				case CurrencyUpdateEffect.MoveToIconPosition:
// 					if (tweenCor != null)
// 					{
// 						StopCoroutine(tweenCor);
// 						//		FinalCurrencyValue(currentValue);
// 					}
//
// 					tweenCor = StartCoroutine(TweenCoins());
//
// 					// oldCurrencyValue = value;
// 					break;
// 			}
//
// 			effect = CurrencyUpdateEffect.Sudden;
// 		}
//
// 		IEnumerator TweenCoins()
// 		{
// 			float noOfCoins = Mathf.Abs(oldValue - currentValue);
// 			noOfCoins = noOfCoins > 50 ? 50 : noOfCoins;
// 			for (int i = 0; i < noOfCoins; i++)
// 			{
// 				RectTransform coin   = MasterObjectPooler.Instance.GetNew<RectTransform>("Coin");
// 				var           parent = transform.parent;
// 				coin.transform.parent = parent;
// 				coin.ResetLocalTransformation();
// 				Vector3       screenPoint = mainCamera.WorldToScreenPoint(worldPosition);
// 				RectTransform parentRect  = parent.GetComponent<RectTransform>();
// 				RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, screenPoint, uiCamera, out Vector2 localPoint);
// 				coin.anchoredPosition = localPoint;
// 				// var sequence = DOTween.Sequence();
// 				// sequence.Append(coin.DOMove(coin.position + Random.insideUnitSphere * 10f, .2f));
// 				// sequence.Append(coin.DOMove(icon.transform.position,                       0.5f).SetDelay(Random.Range(0.1f, 0.3f)));
// 				// sequence.OnComplete(() =>
// 				// {
// 				// 	oldValue++;
// 				// 	string shortNumber = oldValue.ToShortNumber();
// 				// 	textMesh.text = shortNumber;
// 				// 	MasterObjectPooler.Instance.Free("Coin", coin.gameObject);
// 				// });
// 			}
//
// 			yield return new WaitForSeconds(1f);
// 			FinalCurrencyValue(currentValue);
// 			tweenCor = null;
// 		}
//
// 		void FinalCurrencyValue(int value)
// 		{
// 			oldValue = value;
// 			string shortNumber = oldValue.ToShortNumber();
// 			textMesh.text = shortNumber;
// 		}
//
//
// 		private void OnDestroy()
// 		{
// 			VCHandler.OnValueChangeUnregister(currencyTypeName, ChangeEffect);
// 		}
//
// 		void OnValidate()
// 		{
// 			textMesh = GetComponentInChildren<TextMeshProUGUI>();
// 			icon     = transform.FindChildByRecursion<Image>("Icon");
// 			var cameras = FindObjectsOfType<Camera>();
// 			for (int i = 0; i < cameras.Length; i++)
// 			{
// 				if (cameras[i].gameObject.layer == LayerMask.NameToLayer("UI"))
// 				{
// 					uiCamera = cameras[i];
// 				}
//
// 				if (cameras[i].CompareTag("MainCamera"))
// 					mainCamera = cameras[i];
// 			}
// 		}
//
// 	}
//
//
// 	public static class VCExtensions
// 	{
//
// 		public static void AddValue(this VCHandler vcHandler, CurrencyType currencyType, int amount, CurrencyUpdateEffect effect = CurrencyUpdateEffect.Sudden, Vector3 worldPosition = default)
// 		{
// 			CurrencyCounter.SendCurrency(currencyType, amount, effect, worldPosition);
// 		}
//
// 	}
//
//
// }