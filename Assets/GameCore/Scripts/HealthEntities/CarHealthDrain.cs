using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CarHealth))]
public class CarHealthDrain : MonoBehaviour
{
    [SerializeField] private float _damgePerHalfSecond;
    [SerializeField] private Vector3 _tweenScale;

    private ContinuousCollisionDetector _detector;
    private CarHealth _health;

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
        Debug.Log("START DRAIN");
        if (_tweenHPBarAndDamageIE == null)
            _tweenHPBarAndDamageIE = StartCoroutine(TweenHPBarAndDamageIE());
    }

    private void StopDrain()
    {
        Debug.Log("STOP DRAIN");
        if (_tweenHPBarAndDamageIE != null)
            StopCoroutine(_tweenHPBarAndDamageIE);

        _health.HealthBar.HPSlider.transform.DOScale(Vector3.one, 0.24f).SetEase(Ease.Linear);
    }

    IEnumerator TweenHPBarAndDamageIE()
    {
        float t = 0;
        float interval = 0.5f;
        Transform hpBarTransform = _health.HealthBar.HPSlider.transform;

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
