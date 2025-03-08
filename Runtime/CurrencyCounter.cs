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
        [SerializeField] Camera uiCamera;
        [SerializeField] Camera mainCamera;

        [Header("Animation Settings")]
        [SerializeField] private float counterAnimationDuration = 1.5f;
        [SerializeField] private float coinAnimationDuration = 1.0f;
        [SerializeField] private float coinSpawnDelay = 0.05f;
        [SerializeField] private float coinSpreadRadius = 50f;
        [SerializeField] private int maxCoinsToSpawn = 50;

        [Header("Pool Settings")]
        [SerializeField] private int defaultPoolCapacity = 50;
        [SerializeField] private int maxPoolSize = 100;

        private float previousCurrencyValue;
        private float currentCurrencyValue;
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
                textMesh.text = FormatCurrencyValue(currentCurrencyValue);
            }
        }

        private string FormatCurrencyValue(float value)
        {
            if (displayFormat == CurrencyDisplayFormat.Normal)
            {
                return useThousandsSeparator
                    ? string.Format("{0:N0}", value)
                    : value.ToString("F0");
            }

            return FormatAbbreviatedNumber(value);
        }

        private string FormatAbbreviatedNumber(float number)
        {
            if (number == 0) return "0";

            float abs = Mathf.Abs(number);
            string sign = number < 0 ? "-" : "";

            if (abs >= 1000000000000) // Trillion
            {
                return sign + (abs / 1000000000000f).ToString($"F{decimalPlaces}") + "T";
            }
            if (abs >= 1000000000) // Billion
            {
                return sign + (abs / 1000000000f).ToString($"F{decimalPlaces}") + "B";
            }
            if (abs >= 1000000) // Million
            {
                return sign + (abs / 1000000f).ToString($"F{decimalPlaces}") + "M";
            }
            if (abs >= 1000) // Thousand
            {
                return sign + (abs / 1000f).ToString($"F{decimalPlaces}") + "K";
            }

            return sign + abs.ToString("F0");
        }

        private IEnumerator AnimateCounterUpdate()
        {
            float elapsedTime = 0f;

            while (elapsedTime < counterAnimationDuration)
            {
                elapsedTime += Time.deltaTime;
                float progressRatio = elapsedTime / counterAnimationDuration;
                float displayValue = Mathf.Lerp(previousCurrencyValue, currentCurrencyValue, progressRatio);
                textMesh.text = FormatCurrencyValue(displayValue);
                yield return null;
            }

            UpdateCurrencyDisplay();
        }

        private IEnumerator AnimateCoinsCollection()
        {
            int coinsToSpawn = Mathf.Min(
                Mathf.RoundToInt(Mathf.Abs(currentCurrencyValue - previousCurrencyValue)),
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
            previousCurrencyValue = currentCurrencyValue;
            currentCurrencyValue = ((VirtualCurrency)sender).value;
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