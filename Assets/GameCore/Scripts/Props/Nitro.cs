using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nitro : MonoBehaviour
{
    [SerializeField] private float _magnetSpeed;
    [SerializeField] private Vector3 _magnetOffset;
    [SerializeField] private ParticleSystem _mainFX;
    [SerializeField] private ParticleSystem _childFX;

    private CollisionDetector _collisionDetector;

    public Action<Nitro> OnApplyNitro;

    private Transform _target;

    private void Awake()
    {
        _collisionDetector = GetComponent<CollisionDetector>();
        _collisionDetector.OnTriggerE += ApplyNitro;
    }

    private void ApplyNitro(Collider triggerEnterCollider)
    {
        if (_target == null)
        {
            _target = triggerEnterCollider.transform;
            StartCoroutine(LerpMagnetIE());
        }
    }

    IEnumerator LerpMagnetIE()
    {
        ParticleSystem.MainModule mainModule = _mainFX.main;
        mainModule.loop = false;
        ParticleSystem.MainModule childMainModule = _childFX.main;
        childMainModule.loop = false;

        Vector3 startPos = transform.position;

        float t = 0;
        while (t <= 1)
        {
            t += Time.deltaTime * _magnetSpeed;
            transform.localScale = Vector3.Slerp(Vector3.one, Vector3.one / 2, t);
            transform.position = Vector3.Slerp(startPos, _target.position + _target.transform.InverseTransformDirection(_magnetOffset), t);
            yield return null;
        }

        GameSoundAndHapticManager.Instance?.PlaySoundAndHaptic(SoundType.pickUp);

        OnApplyNitro?.Invoke(this);

        gameObject.SetActive(false);
        _target = null;
    }

    private void OnDestroy()
    {
        _collisionDetector.OnTriggerE -= ApplyNitro;
    }
}
