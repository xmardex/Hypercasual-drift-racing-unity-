using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private ParticleSystem _collectFX;

    private CollisionDetector _collisionDetector;

    public Action<Coin> OnCollectCoin;

    private void Awake()
    {
        _collisionDetector = GetComponent<CollisionDetector>();
        _collisionDetector.OnTriggerE += CollectCoin;
    }

    private void CollectCoin()
    {
        OnCollectCoin?.Invoke(this);

        _collectFX.gameObject.transform.SetParent(null);
        _collectFX.gameObject.SetActive(true);
        _collectFX.Play();

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        _collisionDetector.OnTriggerE -= CollectCoin;
    }
}
