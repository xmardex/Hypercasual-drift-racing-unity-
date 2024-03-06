using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private RoadTrigger[] _brakeTriggers;
    [SerializeField] private RoadTrigger[] _gasTriggers;
    [SerializeField] private RoadTrigger _endTutorialTrigger;

    [SerializeField] private CanvasGroup _gasTutorialUI;
    [SerializeField] private CanvasGroup _brakeTutorialUI;

    [SerializeField] private float _showAndHideDuration;

    private void Awake()
    {
        foreach (var trigger in _brakeTriggers)
        {
            trigger.OnCarTriggerEnter += ShowBrakeTutrial;
        }
        foreach (var trigger in _gasTriggers)
        {
            trigger.OnCarTriggerEnter += ShowGasTutorial;
        }
        _endTutorialTrigger.OnCarTriggerEnter += HideBoth;

        _gasTutorialUI.alpha = 0;
        _brakeTutorialUI.alpha = 0;
        _gasTutorialUI.gameObject.SetActive(false);
        _brakeTutorialUI.gameObject.SetActive(false);
    }

    private void ShowGasTutorial()
    {
        _gasTutorialUI.gameObject.SetActive(true);
        _gasTutorialUI.DOFade(1, _showAndHideDuration);
        _brakeTutorialUI.DOFade(0, _showAndHideDuration)
            .OnComplete(() => _brakeTutorialUI.gameObject.SetActive(false));
    }

    private void ShowBrakeTutrial()
    {
        _brakeTutorialUI.gameObject.SetActive(true);
        _brakeTutorialUI.DOFade(1, _showAndHideDuration);
        _gasTutorialUI.DOFade(0, _showAndHideDuration)
            .OnComplete(() => _gasTutorialUI.gameObject.SetActive(false));
    }

    private void HideBoth()
    {
        _brakeTutorialUI.DOFade(0, _showAndHideDuration)
            .OnComplete(() => _brakeTutorialUI.gameObject.SetActive(false));
        _gasTutorialUI.DOFade(0, _showAndHideDuration)
            .OnComplete(() => _gasTutorialUI.gameObject.SetActive(false));
    }

    private void OnDestroy()
    {
        foreach (var trigger in _brakeTriggers)
        {
            trigger.OnCarTriggerEnter -= ShowBrakeTutrial;
        }
        foreach (var trigger in _gasTriggers)
        {
            trigger.OnCarTriggerEnter -= ShowGasTutorial;
        }
        _endTutorialTrigger.OnCarTriggerEnter -= HideBoth;
    }
}
