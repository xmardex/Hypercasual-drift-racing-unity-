using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private ToggleButton _soundToggle;
    [SerializeField] private ToggleButton _musicToggle;
    [SerializeField] private ToggleButton _vibroToggle;

    private void Awake()
    {
        _soundToggle.OnSwitch += SoundToggle;
        _musicToggle.OnSwitch += MusicToggle;
        _vibroToggle.OnSwitch += VibroToggle;
        SetStates();
    }

    private void SetStates()
    {
        int music = PlayerPrefs.GetInt(Constants.MUSIC_PREFS, 1);
        _musicToggle.ForceState(music == 1);

        float volume = PlayerPrefs.GetFloat(Constants.VOLUME_PREFS, 1);
        _soundToggle.ForceState(volume > 0);

        int vibro = PlayerPrefs.GetInt(Constants.VIBRO_PREFS, 1);
        _vibroToggle.ForceState(vibro == 1);
    }

    private void SoundToggle(bool isOn)
    {
        PlayerPrefs.SetInt(Constants.VOLUME_PREFS, isOn ? 1 : 0);
        GameSoundAndHapticManager.Instance?.ChangeVolume(isOn ? 1 : 0);
    }

    private void VibroToggle(bool isOn)
    {
        PlayerPrefs.SetInt(Constants.VIBRO_PREFS, isOn ? 1 : 0);
        GameSoundAndHapticManager.Instance?.VibroEnable(isOn);
    }

    private void MusicToggle(bool isOn)
    {
        PlayerPrefs.SetInt(Constants.MUSIC_PREFS, isOn ? 1 : 0);
        GameSoundAndHapticManager.Instance?.MusicEnable(isOn);
    }

    private void OnDestroy()
    {
        _soundToggle.OnSwitch -= SoundToggle;
        _musicToggle.OnSwitch -= MusicToggle;
        _vibroToggle.OnSwitch -= VibroToggle;
    }
}
