using Lofelt.NiceVibrations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSoundAndHapticManager : MonoBehaviour
{
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioSource[] _soundSources;
    
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
        HapticController.hapticsEnabled = enable;
    }

    public void StopAllExtraSounds()
    {
        foreach(var sound in _soundSources)
        {
            sound.Stop();
        }
    }
    public void PlaySoundAndHaptic(SoundType uISoundType, bool PlayerVibro = true, float duration = 0)
    {
        SoundPreset uISound = _sounds[uISoundType];
        AudioSource uiSoundSource = GetFreeSource();
        uiSoundSource.clip = uISound.audioClip;
        uiSoundSource.volume = uISound.volume;
        uiSoundSource.Play();

        if(duration != 0)
            StartCoroutine(WaitSoundIE(uiSoundSource, duration));

        if(PlayerVibro)
            HapticPatterns.PlayPreset(uISound.presetType);
    }

    private AudioSource GetFreeSource()
    {
        foreach (var source in _soundSources)
            if(!source.isPlaying)
                return source;
        AudioSource newSource = Instantiate(_soundSources[0], _soundSources[0].gameObject.transform.parent);
        newSource.Stop();
        return newSource;
    }

    IEnumerator WaitSoundIE(AudioSource source, float duration)
    {
        yield return new WaitForSeconds(duration);
        source.Stop();
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
    firstChase = 9,
    nitro = 10,
}

[Serializable]
public class SoundPreset
{
    public SoundType soundType;
    public AudioClip audioClip;
    public float volume = 1;
    [Header("HAPTIC")]
    public HapticPatterns.PresetType presetType;
}