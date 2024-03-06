using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nitro : MonoBehaviour
{
    [SerializeField] private float _magnetDuration;
    [SerializeField] private ParticleSystem _mainFX;
    [SerializeField] private ParticleSystem _childFX;

    private CollisionDetector _collisionDetector;

    public Action<Nitro> OnApplyNitro;

    private void Awake()
    {
        _collisionDetector = GetComponent<CollisionDetector>();
        _collisionDetector.OnTriggerE += ApplyNitro;
    }

    private void ApplyNitro(Collider triggerEnterCollider)
    {
        Vector3 magnetPosition = triggerEnterCollider.transform.position + triggerEnterCollider.transform.forward * 3f;
        transform.DOMove(magnetPosition, _magnetDuration).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            OnApplyNitro?.Invoke(this);

            _mainFX.transform.SetParent(null);
            _mainFX.transform.position = magnetPosition;
            ParticleSystem.MainModule mainModule = _mainFX.main;
            mainModule.loop = false;
            ParticleSystem.MainModule childMainModule = _childFX.main;
            childMainModule.loop = false;

            Destroy(gameObject);
        });
    }

    private void OnDestroy()
    {
        _collisionDetector.OnTriggerE -= ApplyNitro;
    }
}
