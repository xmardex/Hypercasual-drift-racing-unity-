using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private GameObject _hpPanel;

    public void SetHpMinMaxValue(float min, float max)
    {
        _slider.minValue = min;
        _slider.maxValue = max;
    }

    public void ChangeValueTo(float newValue)
    {
        _slider.value = newValue;
    }

    public void HideHP()
    {
        _hpPanel.SetActive(false);
    }

    public void ShowHP()
    {
        _hpPanel.SetActive(true);
    }
}
