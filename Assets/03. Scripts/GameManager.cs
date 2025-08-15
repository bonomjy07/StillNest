using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
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
    public GameState CurrentState { get; private set; } = GameState.Title;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ShowTitle();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void Awake()
    {
        base.Awake();
        Time.timeScale = 1f;

        _gameOverPanel.SetActive(true);
    }

    public void ShowTitle()
    {
        CurrentState = GameState.Title;
        _startButton.onClick.AddListener(OnGameStartButtonClicked);
        _goHomeButton.onClick.AddListener(OnGoHomeButtonClicked);

        _titlePanel.SetActive(true);
        _mainHUD?.SetActive(false);
        _gameOverPanel.SetActive(false);

    }

    private void OnGameStartButtonClicked()
    {
        CurrentState = GameState.Playing;
        
        _titlePanel.SetActive(false);
        _mainHUD.SetActive(true);
    }

    public void GameOver()
    {
        CurrentState = GameState.Gameover;
        Time.timeScale = 0f; // stop

        _gameOverPanel.SetActive(true);
    }

    private void OnGoHomeButtonClicked()
    {
        Time.timeScale = 1f;
        ResetGame();
    }


    private void ResetGame()
    {
        CurrentState = GameState.Title;
        Time.timeScale = 1f;

        _titlePanel?.SetActive(true);
        _gameOverPanel?.SetActive(false);
        _mainHUD?.SetActive(false);

        // SpawnManager Reset
        SpawnManager.Instance.ResetSetting();

        // PlacementManager Reset
        PlacementManager.Instance.ResetSetting();
        PlayerManager.Instance.CurrentPlayer.ResetSetting();

        // HealthBar, FloatingMoney Reset
        HealthBarManager.Instance.ResetSetting();
        FloatingMoneyTextManager.Instance.ResetSetting();

    }


}
