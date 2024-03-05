using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelHolderSO", menuName = "LevelHolderSO", order = 0)]
public class LevelHolderSO : ScriptableObject {
    [SerializeField] private List<LevelSO> levelSOs = new List<LevelSO>();
    public List<LevelSO> AllLevels => levelSOs;

    public bool TryGetLevelByNum(int num, out LevelSO levelSO)
    {
        levelSO = levelSOs.Find(level => level.LevelNum == num);
        return levelSO != null;
    }


}