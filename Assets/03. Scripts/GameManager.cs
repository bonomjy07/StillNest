using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private GameObject _titlePanel;
    [SerializeField] private TMP_Text _gameTitleText;
    [SerializeField] private Button _startButton;

    [SerializeField] private GameObject _gameOverPanel;
    [SerializeField] private TMP_Text _gameOverText;
    [SerializeField] private Button _goHomeButton;

    [SerializeField] private GameObject _mainHUD;
    //[SerializeField] private GameObject _gameMap;
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

        Debug.Log("GM Awake 1");
        //SceneManager.sceneLoaded += OnSceneLoaded;
        Debug.Log("GM Awake 2");
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
        Debug.Log("GM ShowTitle 1");

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
        Time.timeScale = 0f; // 겜멈춤

        Debug.Log("GAME OVER???");
        _gameOverPanel.SetActive(true);
        //_mainHUD.SetActive(false);
    }

    private void OnGoHomeButtonClicked()
    {
        Time.timeScale = 1f;
        Debug.Log("Button Press??");
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        // 바뀐 로직으로다가 여기서 ResetGame()
        ResetGame();
    }

    //private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    //{
    //    Debug.Log("GM OSL 1");
    //    Debug.Log("GM OSL 2");
    //}
    //private void OnDestroy()
    //{
    //    SceneManager.sceneLoaded -= OnSceneLoaded;
    //}

    private void ResetGame()
    {
        Debug.Log("GM ResetGame 1");
        CurrentState = GameState.Title;
        Time.timeScale = 1f;

        //_titlePanel = GameObject.Find("TitleUI");
        //_gameOverPanel = GameObject.Find("GameOverUI");
        //_mainHUD = GameObject.Find("MainHUD");
        //_startButton = GameObject.Find("StartButton").GetComponent<Button>();
        //_goHomeButton = GameObject.Find("GoHomeButton").GetComponent<Button>();

        _titlePanel?.SetActive(true);
        _gameOverPanel?.SetActive(false);
        _mainHUD?.SetActive(false);
        //_startButton.onClick.AddListener(OnGameStartButtonClicked);
        //_goHomeButton.onClick.AddListener(OnGoHomeButtonClicked);

        Debug.Log("GM ResetGame 2");

        // 스폰매니저 초기화
        SpawnManager.Instance.ResetSetting();

        // 플레이스먼트매니저 초기화
        PlacementManager.Instance.ResetSetting();
        PlayerManager.Instance.CurrentPlayer.ResetSetting();

        // 헬스바와 머니텍스트 초기화
        HealthBarManager.Instance.ResetSetting();
        FloatingMoneyTextManager.Instance.ResetSetting();

        // 음... 지금 소환되어있는 헬스바 같은거나, 몬스터, 머니, 히어로 다 제거해줘야함..

    }


}
