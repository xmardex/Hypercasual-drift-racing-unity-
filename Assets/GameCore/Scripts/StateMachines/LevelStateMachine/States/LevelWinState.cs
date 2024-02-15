using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelWinState : LevelState
{
    public override void EnterState()
    {
        _cameraManager.EnableOverlays(false);
        _carsInitializator.EnableMovementOnCars(false);
    }

    public override void ExitState()
    {
        
    }

    public override void StateUpdate()
    {
        
    }
}
