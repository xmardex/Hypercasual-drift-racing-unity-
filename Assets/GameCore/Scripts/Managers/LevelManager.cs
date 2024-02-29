using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private bool _useHP;
    [SerializeField] private LevelSO _levelSO;
    [SerializeField] private SplineColoringManager _splineColoringManager;
    [SerializeField] private LevelStateManager _stateManager;
    [SerializeField] private UILevelManager _uiLevelManager;
    [SerializeField] private CameraManager _cameraManager;
    [SerializeField] private CarsInitializator _carsInitializator;
    [SerializeField] private ChasingStarsManager _carsStarsManager;

    [SerializeField] private RoadTrigger _startTrigger;
    [SerializeField] private RoadTrigger _finishTrigger;

    private CarReferences _playerCarReferences;

    private bool _loseGame = false;
    private bool _winGame = false;

    public Action OnGameEnd;

    private void Awake()
    {
        _splineColoringManager.Initialize();
    }

    private void Start()
    {
        InitializeLevel();
        SubscribeToEvents();
        _stateManager.ChangeState(LevelStateType.preStart);
    }

    private void InitializeLevel()
    {
        _playerCarReferences = _carsInitializator.PrespawnedPlayer.GetComponent<CarReferences>();

        _uiLevelManager.Initialize();
        _uiLevelManager.ActivateCanvas(UICanvasType.mainMenu, true);

        _carsInitializator.InitializeCars(_useHP, canMoveOnStart: false);
        _carsStarsManager.Initialize(_carsInitializator.AllAICars);
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
        StopGame();
        _stateManager.ChangeState(LevelStateType.loseGame);
        //TODO: change to lose canvas
        _uiLevelManager.ActivateCanvas(UICanvasType.lose, true);
        OnGameEnd?.Invoke();
    }

    private void WinGame()
    {
        _winGame = true;
        StopGame();
        _stateManager.ChangeState(LevelStateType.winGame);
        //TODO: change to win canvas
        _uiLevelManager.ActivateCanvas(UICanvasType.win, true);
        OnGameEnd?.Invoke();
    }

    private void RetryLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void SubscribeToEvents()
    {
        _uiLevelManager.OnPlayBtnClick += StartGame;
        _uiLevelManager.OnRetryBtnClick += RetryLevel;
        _startTrigger.OnCarTriggerEnter += BeginRace;
        _finishTrigger.OnCarTriggerEnter += FinishRace;
        _playerCarReferences.CarHealth.OnDead += LoseGame;
    }

    private void UnsubscribeFromEvents()
    {
        _uiLevelManager.OnPlayBtnClick -= StartGame;
        _uiLevelManager.OnRetryBtnClick -= RetryLevel;
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

    private void StopGame()
    {
        _cameraManager.EnableOverlays(false);
        _playerCarReferences.CarStuck.StopCheck();
        _carsInitializator.EnableMovementOnCars(false);
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }
}
