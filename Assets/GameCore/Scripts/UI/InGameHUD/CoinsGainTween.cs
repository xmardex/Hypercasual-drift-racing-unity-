using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

using Random = UnityEngine.Random;
using TMPro;

public class CoinsGainTween : MonoBehaviour
{
    [SerializeField] private Image[] _coins;
    [SerializeField] private RectTransform coinsTweenTarget;
    [SerializeField] private float hOffsetMin, hOffsetMax,dOffsetMin,dOffesetMax,durationMin,durationMax,delayMin,delayMax;
    [SerializeField] private float scaleFactor = 0.5f;
    [SerializeField] private Ease easeMove = Ease.Linear;
    [SerializeField] private PathType pathType;

    [SerializeField] private TMP_Text _coinsCollectText;
    [SerializeField] private TMP_Text _coinsContainerText;

    Action OnCoinsGainComplete;

    private bool _coinsGained;

    public void DoCoinTween(int collectCoinsCount, int totalCoinsCount, Action OnCompleteTween)
    {
        OnCoinsGainComplete = OnCompleteTween;

        if (!_coinsGained)
        {
            int iterNums = collectCoinsCount >= _coins.Length ? _coins.Length : collectCoinsCount;
            Debug.Log(iterNums);
            float maxTextLerpDuration = delayMin * iterNums + durationMin;
            Debug.Log("maxTextLerpDuration " + maxTextLerpDuration);
            StartCoroutine(StartCoinsTextAnimationIE(maxTextLerpDuration, collectCoinsCount, totalCoinsCount));
            StartCoroutine(StartCoinFlyAnimationIE(collectCoinsCount));
            _coinsGained = true;
        }
    }
    
    private IEnumerator StartCoinsTextAnimationIE(float maxTextLerpDuration, int collectCoinsCount, int totalCoinsCount)
    {
        float stepT = maxTextLerpDuration / collectCoinsCount;
        float stepsCounter = 0;
        int collectCoinsCountCurrent = collectCoinsCount;
        int totalCoinsCountCurrent = totalCoinsCount;
        while (stepsCounter < maxTextLerpDuration)
        {
            if (collectCoinsCountCurrent <= 0)
                break;

            stepsCounter += stepT;
            _coinsCollectText.text = $"COLLECT {collectCoinsCountCurrent--}";
            _coinsContainerText.text = totalCoinsCountCurrent++.ToString();
            yield return new WaitForSeconds(stepT);
        }
        _coinsCollectText.text = $"COLLECT {0}";
        _coinsContainerText.text = (totalCoinsCount+collectCoinsCount).ToString();
    }

    private IEnumerator StartCoinFlyAnimationIE(int collectCoinsCount)
    {
        int iterCount = collectCoinsCount >= _coins.Length ? _coins.Length : collectCoinsCount;
        for (int i = 0; i < iterCount; i++)
        {
            Image coin = _coins[i];
            coin.gameObject.SetActive(true);
            float heightOffset = Random.Range(hOffsetMin, hOffsetMax);
            float deviation = Random.Range(dOffsetMin, dOffesetMax);
            float duration = Random.Range(durationMin, durationMax);
            float delay = Random.Range(delayMin, delayMax);

            Vector3 startPosition = coin.transform.position;
            Vector3 controlPoint1 = startPosition + Vector3.up * (heightOffset) + Vector3.left * deviation;
          
            Sequence sequence = DOTween.Sequence();
            sequence.Join(coin.transform
                .DOScale(Vector3.one * scaleFactor, duration));

            sequence.Join(coin.rectTransform.DOPath(new Vector3[] { startPosition, controlPoint1, coinsTweenTarget.position }, duration, pathType)
                .SetOptions(false)
                .SetEase(easeMove)
                .OnComplete(() =>
                {
                    coin.rectTransform.anchoredPosition = Vector3.zero;
                    coin.transform.localScale = Vector3.one;
                    coin.gameObject.SetActive(false);
                }
            ));

            yield return new WaitForSeconds(delay);
        }

        yield return new WaitForSeconds(durationMax);
        OnCoinsGainComplete?.Invoke();
    }
}
