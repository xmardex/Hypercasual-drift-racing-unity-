using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelStateManager : MonoBehaviour
{
    [SerializeField] private List<LevelStateContainer> statesContainers = new List<LevelStateContainer>();
    private Dictionary<LevelStateType, LevelState> states = new Dictionary<LevelStateType, LevelState>();
    private LevelState GetState(LevelStateType stateType) => states[stateType];

    private LevelState _currentState;

    public void Initialize()
    {
        foreach (LevelStateContainer container in statesContainers)
        {
            container.state.InitState(this);
            states.Add(container.type, container.state);
        }
    }

    public void ChangeState(LevelStateType type)
    {
        _currentState?.ExitState();
        _currentState = GetState(type);
        _currentState.EnterState();
    }

    private void Update()
    {
        _currentState?.StateUpdate();
    }

}

[Serializable] 
public class LevelStateContainer
{
    //for inspector
    public string stateName;

    public LevelStateType type;
    public LevelState state;
}