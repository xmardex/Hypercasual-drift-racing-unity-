using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropsExplode : ExplodeBase
{
    [SerializeField] private float _damageNearValue;
    private DamagableEntity _damagableEntity;

    private bool _isVisible;

    private void Awake()
    {
        _damagableEntity = GetComponent<DamagableEntity>();
    }

    public void Explode()
    {
        base.Explode(_damagableEntity, out List<DamagableEntity> affectedEntities, out List<Rigidbody> explosionAffectedRBs);

        if(_isVisible)
            GameSoundAndHapticManager.Instance?.PlaySoundAndHaptic(SoundType.explode);

        foreach (var entity in affectedEntities)
            entity.Damage(_damageNearValue);

        StartCoroutine(ApplyExplosionIE(affectedEntities, explosionAffectedRBs));
    }

    IEnumerator ApplyExplosionIE(List<DamagableEntity> affectedEntities, List<Rigidbody> explosionAffectedRBs)
    {
        yield return new WaitForFixedUpdate();

        base.ApplyExplosionForces(explosionAffectedRBs);

        gameObject.SetActive(false);
    }

    private void OnBecameInvisible()
    {
        _isVisible = false;
    }

    private void OnBecameVisible()
    {
        _isVisible = true;
    }
}
