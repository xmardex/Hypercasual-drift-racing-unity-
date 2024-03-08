using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropsExplode : ExplodeBase
{
    [SerializeField] private float _damageNearValue;
    private DamagableEntity _damagableEntity;

    private DamagableEntity _playerDE;

    private bool _isVisible;

    private void Start()
    {
        _damagableEntity = GetComponent<DamagableEntity>();
        _playerDE = GameObject.FindWithTag(Constants.PLAYER_CAR_TAG).GetComponent<DamagableEntity>();
    }

    public void Explode()
    {
        base.Explode(_damagableEntity, out List<DamagableEntity> affectedEntities, out List<Rigidbody> explosionAffectedRBs);

        if (_isVisible)
            GameSoundAndHapticManager.Instance?.PlaySoundAndHaptic(SoundType.explode, affectedEntities.Contains(_playerDE));

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
