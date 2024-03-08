using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CarHealth))]
public class CarHealthDrain : MonoBehaviour
{
    [SerializeField] private float _damgePerHalfSecond;
    [SerializeField] private Vector3 _tweenScale;
    [SerializeField] private float _beforeDamageDelay;

    private ContinuousCollisionDetector _detector;
    private CarHealth _health;

    public Action OnHPDrainStart;
    public Action OnHPDrainEnd;

    private Coroutine _tweenHPBarAndDamageIE;

    private void Awake()
    {
        _health = GetComponent<CarHealth>();
        _detector = GetComponent<ContinuousCollisionDetector>();
        _detector.OnLongTermCollision += StartDrain;
        _detector.OnCollisionEnd += StopDrain;
    }

    private void StartDrain()
    {
        Debug.Log("Start");
        if (_tweenHPBarAndDamageIE == null)
            _tweenHPBarAndDamageIE = StartCoroutine(TweenHPBarAndDamageIE());
    }

    private void StopDrain()
    {
        Debug.Log("Stop");
        if (_tweenHPBarAndDamageIE != null)
        {
            StopCoroutine(_tweenHPBarAndDamageIE);
            _tweenHPBarAndDamageIE = null;
        }

        _health.HealthBar.HPSlider.transform.DOScale(Vector3.one, 0.24f).SetEase(Ease.Linear).OnComplete(() =>
        {
            OnHPDrainEnd?.Invoke();
        });
    }

    IEnumerator TweenHPBarAndDamageIE()
    {
        float t = 0;
        float interval = 0.5f;
        Transform hpBarTransform = _health.HealthBar.HPSlider.transform;
        yield return new WaitForSeconds(_beforeDamageDelay);
        OnHPDrainStart?.Invoke();
        while (true)
        {
            t += Time.deltaTime;
            if (t >= interval)
            {
                hpBarTransform.DOScale(_tweenScale, 0.24f).SetEase(Ease.Linear).OnComplete(() =>
                {
                    hpBarTransform.DOScale(Vector3.one, 0.24f).SetEase(Ease.Linear);
                });
                Debug.Log("DAMAGE DRAIN");
                _health.Damage(_damgePerHalfSecond);
                t = 0;
            }

            yield return null;
        }
    }
}
