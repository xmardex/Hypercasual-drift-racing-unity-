using UnityEngine;

[CreateAssetMenu(fileName = "LevelSO", menuName = "Office_Driver/LevelSO", order = 0)]
public class LevelSO : ScriptableObject {
    [SerializeField] private string _sceneName;
    [SerializeField] private LevelSetupsSO _setups;
}