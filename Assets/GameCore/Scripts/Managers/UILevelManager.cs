using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UILevelManager : MonoBehaviour
{
    [SerializeField] private UICanvasContainer[] canvasesContainers;
    [SerializeField] private Button _playBtn;
    [SerializeField] private Button _collectBtn;
    [SerializeField] private Button _retryBtn;
    [SerializeField] private Button _openSettingsBtn;
    [SerializeField] private Button _closeSettingsBtn;

    [SerializeField] private TMP_Text _levelNumInGame, _levelNumInMenu, _coinsCountText, _coinsCollectedText;
    [SerializeField] private Slider _levelProgressSlider;

    private Dictionary<UICanvasType, UICanvasContainer> _canvases = new Dictionary<UICanvasType, UICanvasContainer>();

    public Action OnPlayBtnClick, OnRetryBtnClick, OnCollectBtnClick, OnSettingsOpenBtnClick, OnSettingsCloseBtnClick;

    public void Initialize()
    {
        InitCanvases();
        SetDefaultCanvas();
        SetListeners();
    }
    private void InitCanvases()
    {
        foreach (UICanvasContainer uiCanvasContainer in canvasesContainers)
        {
            _canvases.Add(uiCanvasContainer.canvasType, uiCanvasContainer);
        }
    }

    private void DeactivateAll(List<Canvas> except = null)
    {
        foreach(UICanvasContainer canvasContainer in _canvases.Values) 
        {
            if (except != null && except.Contains(canvasContainer.canvas)) 
                continue;
            canvasContainer.canvas.gameObject.SetActive(false);
        }
    }

    private void SetDefaultCanvas()
    {
        foreach(UICanvasContainer canvasContainer in canvasesContainers)
        {
            canvasContainer.canvas.gameObject.SetActive(canvasContainer.isDefault);
        }
    }

    private void SetListeners()
    {
        _playBtn.onClick.AddListener(InvokePlayButtonClick);
        _retryBtn.onClick.AddListener(InvokeRetryButtonClick);
        _collectBtn.onClick.AddListener(InvokeCollectButtonClick);

        _openSettingsBtn.onClick.AddListener(InvokeSettingsOpenButtonClick);
        _closeSettingsBtn.onClick.AddListener(InvokeSettingsCloseButtonClick);
    }

    public void ActivateCanvas(UICanvasType canvasType, bool activate)
    {
        if (_canvases.TryGetValue(canvasType, out UICanvasContainer canvasToActivate))
        {
            if(!canvasToActivate.isOverlay)
                DeactivateAll();
            canvasToActivate.canvas.gameObject.SetActive(activate);
        }
    }

    //Listeners
    private void InvokePlayButtonClick() => OnPlayBtnClick?.Invoke();
    private void InvokeRetryButtonClick() => OnRetryBtnClick?.Invoke();
    private void InvokeCollectButtonClick() => OnCollectBtnClick?.Invoke();

    private void InvokeSettingsOpenButtonClick() => OnSettingsOpenBtnClick?.Invoke();
    private void InvokeSettingsCloseButtonClick() => OnSettingsCloseBtnClick?.Invoke();

    public void ApplyLevelSO(LevelSO levelSO)
    {
        string levelNumStr = levelSO.LevelNum.ToString();
        _levelNumInMenu.text = $"Level: {levelNumStr}";
        _levelNumInGame.text = levelNumStr;
    }

    public void UpdateLevelProgerssSlider(float newValue)
    {
        _levelProgressSlider.value = newValue;
    }

    public void UpdateCollectButton(int collectedCoinsCount)
    {
        _coinsCollectedText.text = $"COLLECT {collectedCoinsCount}";
    }

    public void UpdateCoinsCount(int coinsCount)
    {
        //TODA: may be add cool tween
        _coinsCountText.text = coinsCount.ToString();
    }

}

[Serializable]
public class UICanvasContainer
{
    public bool isDefault;
    public bool isOverlay;
    public Canvas canvas;
    public UICanvasType canvasType;
}

public enum UICanvasType
{
    mainMenu = 0,
    settings = 1,
    win = 2,
    lose = 3,
    inGameHud = 4,
}