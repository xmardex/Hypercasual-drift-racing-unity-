using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Coin : MonoBehaviour
{
    [SerializeField] private float _magnetDuration;
    [SerializeField] private ParticleSystem _collectFX;

    private CollisionDetector _collisionDetector;

    public Action<Coin> OnCollectCoin;

    private void Awake()
    {
        _collisionDetector = GetComponent<CollisionDetector>();
        _collisionDetector.OnTriggerE += CollectCoin;
    }

    private void CollectCoin(Collider triggerEnterCollider)
    {
        Vector3 magnetPosition = triggerEnterCollider.transform.position + triggerEnterCollider.transform.forward * 3f;
        transform.DOMove(magnetPosition, _magnetDuration).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            OnCollectCoin?.Invoke(this);
            _collectFX.gameObject.transform.SetParent(null);
            _collectFX.transform.position = magnetPosition;
            _collectFX.gameObject.SetActive(true);
            _collectFX.Play();

            Destroy(gameObject);
        });
    }

    private void OnDestroy()
    {
        _collisionDetector.OnTriggerE -= CollectCoin;
    }
}
