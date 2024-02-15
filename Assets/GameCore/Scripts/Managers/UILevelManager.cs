using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILevelManager : MonoBehaviour
{
    [SerializeField] private UICanvasContainer[] canvasesContainers;
    [SerializeField] private Button _playBtn;

    private Dictionary<UICanvasType, Canvas> canvases = new Dictionary<UICanvasType, Canvas>();

    public Action OnPlayBtnClick;

    public void Initialize()
    {
        InitCanvases();
        SetDefaulCanvas();
        SetListeners();
    }
    private void InitCanvases()
    {
        foreach (UICanvasContainer uiCanvasContainer in canvasesContainers)
        {
            canvases.Add(uiCanvasContainer.canvasType, uiCanvasContainer.canvas);
        }
    }

    private void DeactivateAll(List<Canvas> except = null)
    {
        foreach(Canvas canvas in canvases.Values) 
        {
            if (except != null && except.Contains(canvas)) 
                continue;
            canvas.gameObject.SetActive(false);
        }
    }

    private void SetDefaulCanvas()
    {
        foreach(UICanvasContainer canvasContainer in canvasesContainers)
        {
            canvasContainer.canvas.gameObject.SetActive(canvasContainer.isDefault);
        }
    }

    private void SetListeners()
    {
        _playBtn.onClick.AddListener(InvokePlayButtnClick);
    }

    public void ActivateCanvas(UICanvasType canvasType, bool activate)
    {
        DeactivateAll();
        if (canvases.TryGetValue(canvasType, out Canvas canvasToActivate))
        {
            canvasToActivate.gameObject.SetActive(activate);
        }
    }

    //Listeners
    private void InvokePlayButtnClick() => OnPlayBtnClick?.Invoke();


}

[Serializable]
public class UICanvasContainer
{
    public bool isDefault;
    public Canvas canvas;
    public UICanvasType canvasType;
}

public enum UICanvasType
{
    levelMenu = 0,
    inGameHud = 1,
}