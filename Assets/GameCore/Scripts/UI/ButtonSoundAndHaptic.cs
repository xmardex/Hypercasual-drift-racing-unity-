using UnityEngine;
using UnityEngine.UI;

public class ButtonSoundAndHaptic : MonoBehaviour
{
    [SerializeField] private SoundType _soundType;

    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(PlaySound);
    }

    private void PlaySound()
    {
        GameSoundAndHapticManager.Instance?.PlaySoundAndHaptic(_soundType);
    }
}
