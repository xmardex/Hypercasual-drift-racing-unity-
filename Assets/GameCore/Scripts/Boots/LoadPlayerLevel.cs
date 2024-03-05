using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadPlayerLevel : MonoBehaviour
{
    [SerializeField] private LevelHolderSO _levelHolderSO;
    [SerializeField] private Slider _loadingSlider;
    [SerializeField] private TMP_Text _loadingProgressText;
    [SerializeField] private TMP_Text _levelNumText;
    [SerializeField] private float _loadingSliderSpeed;

    private void Start() 
    {
        int savedLevel = PlayerPrefs.GetInt(Constants.CURRENT_LEVEL_PREFS, 0);

        if (_levelHolderSO.TryGetLevelByNum(savedLevel, out LevelSO levelSO))
        {
            StartLoadLevel(levelSO);
        }
        else
        {
            // TEMP:
            StartLoadLevel(_levelHolderSO.AllLevels[0]);

            // All levels are completed or corrupted saved data
        }
    }

    private void StartLoadLevel(LevelSO levelSO)
    {
        AsyncOperation loadLevelAsync = SceneManager.LoadSceneAsync(levelSO.SceneName, LoadSceneMode.Single);
        loadLevelAsync.allowSceneActivation = false; 
        _levelNumText.text = $"LEVEL: {levelSO.LevelNum}";
        StartCoroutine(LoadProgressIE(loadLevelAsync));
    }

    IEnumerator LoadProgressIE(AsyncOperation loadLevelAsync)
    {
        float progress = 0;
        while (progress <= 0.9f)
        {
            progress = Mathf.Clamp01(loadLevelAsync.progress / 0.9f);
            _loadingProgressText.text = (progress * 100) + "%";
            _loadingSlider.value = Mathf.Lerp(_loadingSlider.value, progress, Time.deltaTime * _loadingSliderSpeed);
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        _loadingProgressText.text = "100%";
        _loadingSlider.value = 1;
        yield return new WaitForSeconds(0.5f);
        loadLevelAsync.allowSceneActivation = true; 
    }
}
