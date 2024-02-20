using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private bool _useHP;
    [SerializeField] private SplineColoringManager _splineColoringManager;
    [SerializeField] private LevelStateManager _stateManager;
    [SerializeField] private UILevelManager _uiLevelManager;
    [SerializeField] private CameraManager _cameraManager;
    [SerializeField] private CarsInitializator _carsInitializator;
    [SerializeField] private CarReferences _playerCarReferences;

    [SerializeField] private RoadTrigger _startTrigger;
    [SerializeField] private RoadTrigger _finishTrigger;

    private bool _loseGame = false;
    private bool _winGame = false;

    private void Awake()
    {
        _splineColoringManager.Initialize();
    }

    private void Start()
    {
        SubscribeToEvents();
        InitializeLevel();
        _stateManager.ChangeState(LevelStateType.preStart);
    }

    private void InitializeLevel()
    {
        _uiLevelManager.Initialize();
        _uiLevelManager.ActivateCanvas(UICanvasType.levelMenu, true);

        _carsInitializator.InitializeCars(_useHP, canMoveOnStart: false);

        _cameraManager.Initialize();
        _stateManager.Initialize();
    }

    private void StartGame()
    {
        _uiLevelManager.ActivateCanvas(UICanvasType.inGameHud, true);
        _stateManager.ChangeState(LevelStateType.startGame);
    }

    private void BeginRace()
    {
        _playerCarReferences.CarStuck.Initialize(this);
    }

    private void FinishRace()
    {
        if (!_loseGame)
            WinGame();
    }

    private void LoseGame()
    {
        _loseGame = true;
        _cameraManager.EnableOverlays(false);
        _playerCarReferences.CarStuck.StopCheck();
        _stateManager.ChangeState(LevelStateType.loseGame);
        //TODO: change to lose canvas
        _uiLevelManager.ActivateCanvas(UICanvasType.levelMenu, true);
    }

    private void WinGame()
    {
        _winGame = true;
        _stateManager.ChangeState(LevelStateType.winGame);
        //TODO: change to win canvas
        _uiLevelManager.ActivateCanvas(UICanvasType.levelMenu, true);
    }

    private void SubscribeToEvents()
    {
        _uiLevelManager.OnPlayBtnClick += StartGame;
        _startTrigger.OnCarTriggerEnter += BeginRace;
        _finishTrigger.OnCarTriggerEnter += FinishRace;
        _playerCarReferences.CarHealth.OnDead += LoseGame;
    }

    private void UnsubscribeFromEvents()
    {
        _uiLevelManager.OnPlayBtnClick -= StartGame;
        _startTrigger.OnCarTriggerEnter -= BeginRace;
        _finishTrigger.OnCarTriggerEnter -= FinishRace;
        _playerCarReferences.CarHealth.OnDead -= LoseGame;
    }

    public void CarStuckWithPolice()
    {
        Debug.Log("CAR STUCK WITH POLICE -> GAME OVER");
        LoseGame();
    }

    public void CarStuckSimple()
    {
        Debug.Log("CAR STUCK SIMPLE RESET");
    }

    public void CarOffRoad()
    {
        Debug.Log("CAR OFF ROAD RESET");
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }
}
