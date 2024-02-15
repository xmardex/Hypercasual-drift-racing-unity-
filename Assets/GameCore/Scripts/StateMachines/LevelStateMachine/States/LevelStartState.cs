using UnityEngine;

public class LevelStartState : LevelState
{
    public override void EnterState()
    {
        _cameraManager.EnableOverlays(true);
        _carsInitializator.EnableMovementOnCars(true);
    }

    public override void ExitState()
    {
        
    }

    public override void StateUpdate()
    {
        
    }
}
