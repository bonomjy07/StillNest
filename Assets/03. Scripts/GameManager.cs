using System;
using System.Collections.Generic;
using FishNet;
using FishNet.Connection;
using FishNet.Object.Synchronizing;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : NetworkSingleton<GameManager>
{
    [Header("Title")]
    [SerializeField] private GameObject _titlePanel;
    [SerializeField] private TMP_Text _gameTitleText;
    [SerializeField] private Button _startButton;

    [Header("GameOver")]
    [SerializeField] private GameObject _gameOverPanel;
    [SerializeField] private TMP_Text _gameOverText;
    [SerializeField] private Button _goHomeButton;

    [SerializeField] private GameObject _mainHUD;

    public GameState CurrentState
    {
        get => _currentState.Value; 
        private set => _currentState.Value = value;
    }
    private readonly SyncVar<GameState> _currentState = new();
    
    private readonly List<CoopPlayer> _coopPlayers = new();

    protected override void Awake()
    {
        Time.timeScale = 1f;

        _gameOverPanel.SetActive(true);

        GameEventHub.Instance
                    .OnLocalClient
                    .Subscribe(player => _coopPlayers.Add(player))
                    .AddTo(this);
        
        _currentState.OnChange += OnStateChange;
    }

    private void OnStateChange(GameState prev, GameState next, bool asServer)
    {
        if (!asServer)
        {
            if (next == GameState.Playing)
            {
                _titlePanel.SetActive(false);
                _mainHUD.SetActive(true);
            }
            else if (next == GameState.Gameover)
            {
                Time.timeScale = 0f; // stop

                _gameOverPanel.SetActive(true);
            }
            else if (next == GameState.Title)
            {
                Time.timeScale = 1f;

                _titlePanel.SetActive(true);
                _gameOverPanel.SetActive(false);
                _mainHUD.SetActive(false);
            }
        }
    }

    private void Start()
    {
        ShowTitle();
    }

    private void ShowTitle()
    {
        CurrentState = GameState.Title;
        _startButton.onClick.AddListener(OnGameStartButtonClicked);
        _goHomeButton.onClick.AddListener(OnGoHomeButtonClicked);
    }

    private void OnGameStartButtonClicked()
    {
        if (IsServerInitialized)
        {
            CurrentState = GameState.Playing;
        }
    }

    public void GameOver()
    {
        if (IsServerInitialized)
        {
            CurrentState = GameState.Gameover;
        }
    }

    private void OnGoHomeButtonClicked()
    {
        ResetGame();
    }

    private void ResetGame()
    {
        // HealthBar, FloatingMoney Reset
        if (IsClientInitialized)
        {
            HealthBarManager.Instance.ResetSetting();
            FloatingMoneyTextManager.Instance.ResetSetting();
        }

        if (IsServerInitialized)
        {
            CurrentState = GameState.Title;
        }

        SpawnManager.Instance.ResetSetting();
        _coopPlayers.ForEach(player => player.Clear());
    }
}
