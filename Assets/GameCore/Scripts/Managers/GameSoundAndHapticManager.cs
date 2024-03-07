using MoreMountains.NiceVibrations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSoundAndHapticManager : MonoBehaviour
{
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioSource[] _uiSources;
    
    [SerializeField] private AudioClip _defaultMusic;
    [SerializeField] private List<SoundPreset> _soundsPresets;

    private Dictionary<SoundType,SoundPreset> _sounds;

    public static GameSoundAndHapticManager Instance;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        Initialize();
    }

    private void Initialize()
    {
        _sounds ??= new Dictionary<SoundType, SoundPreset> ();
        foreach(var sound in _soundsPresets) 
        {
            _sounds.Add(sound.soundType, sound);
        }
        MMNViOSCoreHaptics.CreateEngine();
    }

    private void Start()
    {
        _musicSource.clip = _defaultMusic;
        _musicSource.Play();

        float volume = PlayerPrefs.GetFloat(Constants.VOLUME_PREFS, 1);
        int music = PlayerPrefs.GetInt(Constants.MUSIC_PREFS, 1);
        int vibro = PlayerPrefs.GetInt(Constants.VIBRO_PREFS, 1);
        ChangeVolume(volume);
        MusicEnable(music == 1);
        VibroEnable(vibro == 1);
    }

    public void ChangeVolume(float volume)
    {
        AudioListener.volume = volume; 
    }

    public void MusicEnable(bool enable)
    {
        _musicSource.volume = enable ? 1 : 0;
    }

    public void VibroEnable(bool enable)
    {
        MMVibrationManager.SetHapticsActive(enable);
    }

    public void PlaySoundAndHaptic(SoundType uISoundType)
    {
        SoundPreset uISound = _sounds[uISoundType];
        AudioSource uiSoundSource = GetFreeSource();
        uiSoundSource.clip = uISound.audioClip;
        uiSoundSource.volume = uISound.volume;
        uiSoundSource.Play();

        MMVibrationManager.Haptic(uISound.hapticType, false, true, this);
    }

    private AudioSource GetFreeSource()
    {
        foreach (var source in _uiSources)
            if(!source.isPlaying)
                return source;
        AudioSource newSource = Instantiate(_uiSources[0], _uiSources[0].gameObject.transform.parent);
        newSource.Stop();
        return newSource;
    }
}

public enum SoundType
{
    none = 0,
    simpleClick = 1,
    coinsCollect = 2,
    pickUp = 3,
    smallCarHit = 4,
    mediumCarHit = 5,
    explode = 6,
    metalHit = 7,
    plasticHit = 8,
}

[Serializable]
public class SoundPreset
{
    public SoundType soundType;
    public AudioClip audioClip;
    public float volume = 1;
    [Header("Haptics")]
    public HapticTypes hapticType;
}