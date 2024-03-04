using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChasingStarsManager : MonoBehaviour
{
    [SerializeField] private GameObject[] _stars;
    private int _maxStars;
    private int _currentStarsCount;

    private List<CarAI> _allAICars;

    public void Initialize(List<CarAI> allAICars)
    {
        _maxStars = _stars.Length;
        _allAICars = allAICars;
        SetupEvents();

        _currentStarsCount = 0;
        UpdateStars();
    }

    public void AddStar()
    {
        _currentStarsCount = _currentStarsCount + 1 >= _maxStars ? _maxStars : _currentStarsCount + 1;
        UpdateStars();
    }
    public void RemoveStar()
    {
        _currentStarsCount = _currentStarsCount - 1 <= 0 ? 0 : _currentStarsCount - 1;
        UpdateStars();
    }

    private void UpdateStars()
    {
        foreach (var item in _stars)
            item.SetActive(false);
        for(int i = 0; i < _currentStarsCount; i++)
        {
            _stars[i].SetActive(true);
        }
    }

    private void SetupEvents()
    {
        if (_allAICars != null && _allAICars.Count > 0)
        {
            foreach (CarAI ai in _allAICars)
            {
                ai.OnAIStartChasing += AddStar;
                ai.OnAIDeactivated += RemoveStar;
            }
        }
    }

    private void RemoveEvents()
    {
        if (_allAICars != null && _allAICars.Count > 0)
        {
            foreach (CarAI ai in _allAICars)
            {
                ai.OnAIStartChasing -= AddStar;
                ai.OnAIDeactivated -= RemoveStar;
            }
        }
    }

    private void OnDestroy()
    {
        RemoveEvents();
    }

}
