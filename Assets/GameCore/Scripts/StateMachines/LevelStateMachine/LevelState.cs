using UnityEngine;

public abstract class LevelState : MonoBehaviour
{
    [SerializeField] protected CameraManager _cameraManager;
    [SerializeField] protected CarsInitializator _carsInitializator;

    protected LevelStateManager LevelStateMachine;
    private LevelStateType _stateType; //marker fo levelStateManager

    public virtual void InitState(LevelStateManager levelStateMachine)
    {
        LevelStateMachine = levelStateMachine;
    }

    public abstract void EnterState();

    public abstract void ExitState();

    public abstract void StateUpdate();


}
