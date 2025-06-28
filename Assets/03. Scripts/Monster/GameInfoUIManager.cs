using TMPro;
using UnityEngine;

public class GameInfoUIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _waveText;
    [SerializeField] private TextMeshProUGUI _monsterCountText;
    [SerializeField] private TextMeshProUGUI _timeText;

    [SerializeField] private GameObject _wavePopup;
    [SerializeField] private TextMeshProUGUI _wavePopupText;
    private Animator _wavePopupAnimator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _wavePopupAnimator = _wavePopup.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateWave(int wave)
    {
        _waveText.text = "Wave " + wave;
    }

    public void UpdateMonsterCount(int curCount, int maxCount)
    {
        Color colorGreen;
        ColorUtility.TryParseHtmlString("#4FF840", out colorGreen); // 기존색 저장

        if (curCount > maxCount - 10) // 최대치 임박
            _monsterCountText.color = Color.red;

        _monsterCountText.text = $"{curCount} / {maxCount}";
    }

    public void UpdateTime(float time)
    {
        int min = (int)time / 60;
        int sec = (int)time % 60;

        _timeText.text = string.Format("{0:00} : {1:00}", min, sec);
    }

    public void AlertWave(int wave)
    {
        if (_wavePopupAnimator == null)
            _wavePopupAnimator = _wavePopup.GetComponent<Animator>();

        _wavePopup.SetActive(true);
        _wavePopupText.text = $"Wave  {wave}";
        _wavePopupAnimator.Play("WavePopupAnim", 0, 0f);
        // WavePopupAnim 상태 애니메이션을 Layer 0(Base) 에서 0초부터 재생
    }
}
