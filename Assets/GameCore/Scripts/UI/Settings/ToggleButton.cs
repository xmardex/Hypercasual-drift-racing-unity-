using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToggleButton : MonoBehaviour
{
    [SerializeField] private TMP_Text _toggleText;
    [SerializeField] private string _onText, _offText;
    [SerializeField] private Color _onColor, _offColor;

    private Button _button;

    public Action<bool> OnSwitch;

    private bool _isOn;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(Toggle);
    }

    private void Toggle()
    {
        _isOn = !_isOn;
        SwitchSate(_isOn);
    }

    public void ForceState(bool isOn)
    {
        _isOn = isOn;
        SwitchSate(isOn,false);
    }

    private void SwitchSate(bool isOn, bool withCallback = true)
    {
        _toggleText.text = isOn ? _onText : _offText;
        _toggleText.color = isOn ? _onColor : _offColor;
        OnSwitch?.Invoke(isOn);
    }
}
