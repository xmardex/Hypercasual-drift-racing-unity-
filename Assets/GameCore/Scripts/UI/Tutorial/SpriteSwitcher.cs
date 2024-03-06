using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class SpriteSwitcher : MonoBehaviour
{
    [SerializeField] private Sprite[] _sprites;
    [SerializeField] private float _delay;

    private Image _image;
    private Coroutine _switchSpriteIE;

    private void Awake()
    {
        _image = GetComponent<Image>();
    }

    private void OnEnable()
    {
        _switchSpriteIE = StartCoroutine(SwitchSpriteIE());
    }

    private void OnDisable()
    {
        if (_switchSpriteIE != null)
            StopCoroutine(_switchSpriteIE);
    }

    private IEnumerator SwitchSpriteIE()
    {
        if (_sprites.Length == 0)
        {
            Debug.LogWarning("No sprites assigned to the SpriteSwitcher.");
            yield break;
        }

        while (true)
        {
            for (int i = 0; i < _sprites.Length; i++)
            {
                _image.sprite = _sprites[i];
                yield return new WaitForSeconds(_delay);
            }
            yield break;
        }
    }
}
