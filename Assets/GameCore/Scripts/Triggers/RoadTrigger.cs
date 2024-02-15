using UnityEngine;
using System;

[RequireComponent(typeof(Collider))]
public class RoadTrigger : MonoBehaviour
{
    private Collider _collider;
    [SerializeField] private string[] _tagsForTrigger;
    public Action OnCarTriggerEnter;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        _collider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (ComparerTags(other))
        {
            OnCarTriggerEnter?.Invoke();
        }
    }

    private bool ComparerTags(Collider other)
    {
        foreach (string tag in _tagsForTrigger)
        {
            if (tag == other.tag)
                return true;
            else
                continue;
        }
        return false;
    }
}
