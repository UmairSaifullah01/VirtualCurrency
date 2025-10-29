using System;
using System.Collections;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

namespace THEBADDEST.VirtualCurrencySystem
{
    public enum CurrencyUpdateEffect
    {
        Sudden,
        CounterUpdate,
        MoveToIconPosition,
    }

    public enum CurrencyDisplayFormat
    {
        Normal,
        Abbreviated,
    }

    public class CurrencyCounter : CurrencyBinding
    {
        [Header("UI References")]
        [SerializeField] TextMeshProUGUI textMesh;
        [SerializeField] private CurrencyUpdateEffect currencyAnimationType;
        [SerializeField] private CurrencyDisplayFormat displayFormat;
        [SerializeField] Transform currencyIconTarget;
        [SerializeField] Transform coinPrefab;

        [Header("Display Settings")]
        [SerializeField] private bool useThousandsSeparator = true;
        [SerializeField] private int decimalPlaces = 1;

        [Header("Camera References")]
        [SerializeField] public Camera uiCamera;
        [SerializeField] public Camera mainCamera;

        [Header("Animation Settings")]
        [SerializeField] private float counterAnimationDuration = 1.5f;
        [SerializeField] private float coinAnimationDuration = 1.0f;
        [SerializeField] private float coinSpawnDelay = 0.05f;
        [SerializeField] private float coinSpreadRadius = 50f;
        [SerializeField] private int maxCoinsToSpawn = 50;

        [Header("Pool Settings")]
        [SerializeField] private int defaultPoolCapacity = 50;
        [SerializeField] private int maxPoolSize = 100;

        private BigNumber previousNumberValue;
        private BigNumber currentNumberValue;
        private Vector3 coinSpawnWorldPosition;
        private Coroutine activeAnimationCoroutine;

        private IObjectPool<RectTransform> coinPool;

        private void Awake()
        {
            InitializeCoinPool();
            
        }

        private void InitializeCoinPool()
        {
            coinPool = new ObjectPool<RectTransform>(
                createFunc: CreateCoin,
                actionOnGet: OnGetCoinFromPool,
                actionOnRelease: OnReleaseCoinToPool,
                actionOnDestroy: OnDestroyCoin,
                collectionCheck: true,
                defaultCapacity: defaultPoolCapacity,
                maxSize: maxPoolSize
            );
        }

        private RectTransform CreateCoin()
        {
            var coin = Instantiate(coinPrefab, transform.parent).GetComponent<RectTransform>();
            coin.gameObject.SetActive(false);
            return coin;
        }

        private void OnGetCoinFromPool(RectTransform coin)
        {
            coin.gameObject.SetActive(true);
        }

        private void OnReleaseCoinToPool(RectTransform coin)
        {
            coin.gameObject.SetActive(false);
        }

        private void OnDestroyCoin(RectTransform coin)
        {
            Destroy(coin.gameObject);
        }

        private void ApplyCurrencyUpdateEffect()
        {
            switch (currencyAnimationType)
            {
                case CurrencyUpdateEffect.Sudden:
                    UpdateCurrencyDisplay();
                    break;

                case CurrencyUpdateEffect.CounterUpdate:
                    StartCoroutine(AnimateCounterUpdate());
                    break;

                case CurrencyUpdateEffect.MoveToIconPosition:
                    if (activeAnimationCoroutine != null)
                    {
                        StopCoroutine(activeAnimationCoroutine);
                    }

                    activeAnimationCoroutine = StartCoroutine(AnimateCoinsCollection());
                    break;
            }
        }

        private void UpdateCurrencyDisplay()
        {
            if (textMesh != null)
            {
                textMesh.text = FormatCurrencyValue(currentNumberValue);
            }
        }

        private string FormatCurrencyValue(BigNumber value)
        {
            switch (displayFormat)
            {
                case CurrencyDisplayFormat.Abbreviated:
                    return value.ToStringFormatted(decimalPlaces);
                case CurrencyDisplayFormat.Normal:
                default:
                    var val = value.ToDouble();
                    return useThousandsSeparator
                        ? val.ToString($"N{decimalPlaces}", System.Globalization.CultureInfo.InvariantCulture)
                        : val.ToString($"F{decimalPlaces}", System.Globalization.CultureInfo.InvariantCulture);
            }
        }

        private IEnumerator AnimateCounterUpdate()
        {
            float elapsedTime = 0f;
            double start = previousNumberValue.ToDouble();
            double end = currentNumberValue.ToDouble();
            while (elapsedTime < counterAnimationDuration)
            {
                elapsedTime += Time.deltaTime;
                float progressRatio = elapsedTime / counterAnimationDuration;
                double displayValue = Mathf.Lerp((float)start, (float)end, progressRatio);
                var displayCurrency = BigNumber.FromDouble(displayValue);
                textMesh.text = FormatCurrencyValue(displayCurrency);
                yield return null;
            }
            UpdateCurrencyDisplay();
        }

        private IEnumerator AnimateCoinsCollection()
        {
            int coinsToSpawn = Mathf.Min(
                Mathf.RoundToInt(Mathf.Abs((float)currentNumberValue.ToDouble() - (float)previousNumberValue.ToDouble())),
                maxCoinsToSpawn
            );

            for (int i = 0; i < coinsToSpawn; i++)
            {
                SpawnAndAnimateCoin();
                yield return new WaitForSeconds(coinSpawnDelay);
            }

            yield return new WaitForSeconds(coinAnimationDuration);
            UpdateCurrencyDisplay();
            activeAnimationCoroutine = null;
        }

        private void SpawnAndAnimateCoin()
        {
            var parentRect = transform.parent.GetComponent<RectTransform>();
            Vector3 screenPoint = mainCamera.WorldToScreenPoint(coinSpawnWorldPosition);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRect,
                screenPoint,
                uiCamera,
                out Vector2 localPoint
            );

            RectTransform coin = coinPool.Get();
            coin.SetParent(parentRect);
            coin.anchoredPosition = localPoint;

            StartCoroutine(AnimateCoinMovement(coin));
        }

        private IEnumerator AnimateCoinMovement(RectTransform coin)
        {
            Vector3 startPosition = coin.position;
            Vector3 randomOffset = Random.insideUnitSphere * coinSpreadRadius;
            Vector3 midPosition = startPosition + randomOffset;
            Vector3 targetPosition = currencyIconTarget.position;

            float elapsedTime = 0f;

            while (elapsedTime < coinAnimationDuration)
            {
                elapsedTime += Time.deltaTime;
                float progressRatio = elapsedTime / coinAnimationDuration;

                if (progressRatio < 0.5f)
                {
                    float firstHalfProgress = progressRatio * 2f;
                    coin.position = Vector3.Lerp(startPosition, midPosition, firstHalfProgress);
                }
                else
                {
                    float secondHalfProgress = (progressRatio - 0.5f) * 2f;
                    coin.position = Vector3.Lerp(midPosition, targetPosition, secondHalfProgress);
                }

                yield return null;
            }

            coinPool.Release(coin);
        }

        public void SetCoinSpawnWorldPosition(Vector3 worldPosition)
        {
            coinSpawnWorldPosition = worldPosition;
        }

        protected override void ChangeEffect(object sender, PropertyChangedEventArgs args)
        {
            previousNumberValue = currentNumberValue;
            currentNumberValue = ((VirtualCurrency)sender).value;
            ApplyCurrencyUpdateEffect();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (coinPool is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}