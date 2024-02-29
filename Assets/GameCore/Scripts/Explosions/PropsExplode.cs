using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropsExplode : ExplodeBase
{
    [SerializeField] private float _damageNearValue;
    private DamagableEntity _damagableEntity;

    private void Awake()
    {
        _damagableEntity = GetComponent<DamagableEntity>();
    }

    public void Explode()
    {
        base.Explode(_damagableEntity, out List<DamagableEntity> affectedEntities);

        foreach(var entity in affectedEntities)
        {
            entity.Damage(_damageNearValue);
        }
    }
}
