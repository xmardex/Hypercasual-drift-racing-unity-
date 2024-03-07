using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private float _magnetSpeed;
    [SerializeField] private Vector3 _magnetOffset;
    [SerializeField] private ParticleSystem _collectFX;

    private CollisionDetector _collisionDetector;

    private Transform _target;

    public Action<Coin> OnCollectCoin;

    private void Awake()
    {
        _collisionDetector = GetComponent<CollisionDetector>();
        _collisionDetector.OnTriggerE += CollectCoin;
    }

    private void CollectCoin(Collider triggerEnterCollider)
    {
        if (_target == null)
        {
            _target = triggerEnterCollider.transform;
            StartCoroutine(LerpMagnetIE());
        }
    }

    IEnumerator LerpMagnetIE()
    {
        _collectFX.gameObject.transform.SetParent(null);
        _collectFX.gameObject.SetActive(true);
        _collectFX.Play();
        Vector3 startPos = transform.position;
        float t = 0;
        while(t <= 1)
        {
            t += Time.deltaTime * _magnetSpeed;
            transform.localScale = Vector3.Slerp(Vector3.one, Vector3.one/2, t);
            transform.position = Vector3.Slerp(startPos, _target.position+_target.transform.InverseTransformDirection(_magnetOffset), t);
            yield return null;
        }

        GameSoundAndHapticManager.Instance?.PlaySoundAndHaptic(SoundType.pickUp);

        OnCollectCoin?.Invoke(this);

        gameObject.SetActive(false);
        _target = null;
    }

    private void OnDestroy()
    {
        _collisionDetector.OnTriggerE -= CollectCoin;
    }
}
