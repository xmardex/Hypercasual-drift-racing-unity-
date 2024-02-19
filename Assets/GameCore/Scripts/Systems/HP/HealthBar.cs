using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private GameObject _hpPanel;

    private Coroutine _lerpSliderIE;

    public void SetHpMinMaxValue(float min, float max)
    {
        _slider.minValue = min;
        _slider.maxValue = max;
    }

    public void ChangeValueTo(float newValue)
    {
        if (_lerpSliderIE != null)
            StopCoroutine(_lerpSliderIE);

        _lerpSliderIE = StartCoroutine(LerpBarIE(newValue));
    }

    public void HideHP()
    {
        _hpPanel.SetActive(false);
    }

    public void ShowHP()
    {
        _hpPanel.SetActive(true);
    }

    private IEnumerator LerpBarIE(float newValue)
    {
        float elapsedTime = 0f;
        float duration = Constants.HEALTH_BAR_LERP_SPEED; 

        float initialValue = _slider.value;

        while (elapsedTime < duration)
        {
            _slider.value = Mathf.Lerp(initialValue, newValue, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null; 
        }

        _slider.value = newValue;
    }
}
