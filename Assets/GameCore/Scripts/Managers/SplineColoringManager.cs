using System;
using UnityEngine;

[ExecuteInEditMode]
public class SplineColoringManager : MonoBehaviour
{
    private int _maxBlocks = 20;

    [SerializeField] private GameObject[] _spheres;
    [SerializeField] private Renderer _roadRenderer;
    [SerializeField] private bool _executeInEditMode = false;

    MaterialPropertyBlock propBlock;
    public void Initialize()
    {
        InitColors();
        if (ValidateParams(_spheres))
        {
            
        }
    }

    private void Update()
    {
        if (_executeInEditMode)
        {
            if (ValidateParams(_spheres))
            {
                InitColors();
            }
        }
    }

    private void InitColors()
    {
        propBlock = new MaterialPropertyBlock();
        for (int i = 0; i < _spheres.Length; i++)
        {
            GameObject sphere = _spheres[i];
            if(sphere != null)
            {
                Vector3 spherePos = sphere.transform.position;
                float sphereRadius = sphere.GetComponent<SphereCollider>().radius;
                propBlock.SetVector($"_SpherePos{i + 1}", spherePos);
                propBlock.SetFloat($"_SphereRadius{i + 1}", sphereRadius);
            }
        }
        _roadRenderer.SetPropertyBlock(propBlock);
    }

    public bool ValidateParams(GameObject[] _spheres)
    {
        return _spheres.Length <= _maxBlocks;
    }
}