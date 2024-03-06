using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private bool _useHP;
    [SerializeField] private bool _useLights;
    [SerializeField] private bool _enableDebug;
    [SerializeField] private LevelSO _levelSO;
    [SerializeField] private SplineColoringManager _splineColoringManager;
    [SerializeField] private LevelStateManager _stateManager;
    [SerializeField] private UILevelManager _uiLevelManager;
    [SerializeField] private CameraManager _cameraManager;
    [SerializeField] private CarsInitializator _carsInitializator;
    [SerializeField] private ChasingStarsManager _carsStarsManager;
    [SerializeField] private CoinsGainTween _coinsGainTween;

    [SerializeField] private RoadTrigger _startTrigger;
    [SerializeField] private RoadTrigger _finishTrigger;

    private CarReferences _playerCarReferences;

    private int _levelCollectedCoinsCount = 0;
    private int _totalCoinsCount = 0;

    private bool _loseGame = false;
    private bool _winGame = false;

    public Action OnGameEnd;

    private void Awake()
    {
        _splineColoringManager.Initialize();
        Debug.unityLogger.logEnabled = _enableDebug;
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
        _uiLevelManager.ApplyLevelSO(_levelSO);
        _uiLevelManager.ActivateCanvas(UICanvasType.mainMenu, true);

        _carsInitializator.InitializeCars(_useHP, _useLights, canMoveOnStart: false);
        _carsStarsManager.Initialize(_carsInitializator.AllAICars);
        _cameraManager.Initialize();
        _stateManager.Initialize();
        CoinsInitialize();
    }

    private void CoinsInitialize()
    {
        _totalCoinsCount = PlayerPrefs.GetInt(Constants.COINS_PREFS, 0);

        Coin[] coins = FindObjectsByType<Coin>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach (Coin coin in coins)
        {
            coin.OnCollectCoin += AddCoin;
        }

        _uiLevelManager.UpdateCoinsCount(_totalCoinsCount);
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
        _uiLevelManager.ActivateCanvas(UICanvasType.lose, true);
        OnGameEnd?.Invoke();
    }

    private void WinGame()
    {
        _winGame = true;
        StopGame();
        _stateManager.ChangeState(LevelStateType.winGame);
        _uiLevelManager.UpdateCollectButton(_levelCollectedCoinsCount);
        _uiLevelManager.ActivateCanvas(UICanvasType.win, true);
        OnGameEnd?.Invoke();
    }

    private void RetryLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void CollectLevelCoins()
    {
        _coinsGainTween.DoCoinTween(_levelCollectedCoinsCount, _totalCoinsCount, LoadNextLevel);

        _totalCoinsCount += _levelCollectedCoinsCount;
        _levelCollectedCoinsCount = 0;
    }

    private void LoadNextLevel()
    {
        SaveLevelProgress();
    }

    private void SaveLevelProgress()
    {
        PlayerPrefs.SetInt(Constants.COINS_PREFS, _totalCoinsCount);
        PlayerPrefs.SetInt(Constants.CURRENT_LEVEL_PREFS, _levelSO.LevelNum + 1);
    }

    private void SubscribeToEvents()
    {
        _uiLevelManager.OnPlayBtnClick += StartGame;
        _uiLevelManager.OnRetryBtnClick += RetryLevel;
        _uiLevelManager.OnCollectBtnClick += CollectLevelCoins;

        _uiLevelManager.OnSettingsOpenBtnClick += OpenSettings;
        _uiLevelManager.OnSettingsCloseBtnClick += CloseSettings;

        _startTrigger.OnCarTriggerEnter += BeginRace;
        _finishTrigger.OnCarTriggerEnter += FinishRace;

        if(_useHP)
            _playerCarReferences.CarHealth.OnDead += LoseGame;

        _playerCarReferences.CarController.CarSplinePointer.OnLevelDistancePercentageChange +=
            _uiLevelManager.UpdateLevelProgerssSlider;
    }

    private void UnsubscribeFromEvents()
    {
        _uiLevelManager.OnPlayBtnClick -= StartGame;
        _uiLevelManager.OnRetryBtnClick -= RetryLevel;
        _uiLevelManager.OnCollectBtnClick -= CollectLevelCoins;

        _uiLevelManager.OnSettingsOpenBtnClick -= OpenSettings;
        _uiLevelManager.OnSettingsCloseBtnClick -= CloseSettings;

        _startTrigger.OnCarTriggerEnter -= BeginRace;
        _finishTrigger.OnCarTriggerEnter -= FinishRace;

        if (_useHP)
            _playerCarReferences.CarHealth.OnDead -= LoseGame;

        _playerCarReferences.CarController.CarSplinePointer.OnLevelDistancePercentageChange -= 
            _uiLevelManager.UpdateLevelProgerssSlider;
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

    private void OpenSettings()
    {
        PauseGame();
        _uiLevelManager.ActivateCanvas(UICanvasType.settings, true);
    }

    private void CloseSettings()
    {
        UnPauseGame();
        _uiLevelManager.ActivateCanvas(UICanvasType.settings, false);
    }

    private void PauseGame()
    {
        Time.timeScale = 0.0f;
    }

    private void UnPauseGame()
    {
        Time.timeScale = 1.0f;
    }

    private void AddCoin(Coin coin)
    {
        _levelCollectedCoinsCount++;
        coin.OnCollectCoin -= AddCoin;
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }
}
