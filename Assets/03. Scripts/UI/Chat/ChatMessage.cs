using UnityEngine;

public class ChatMessage : MonoBehaviour
{
    [Header("[Properties]")]
    public string message;

    [Header("[Components]")]
    [SerializeField] private TMPro.TextMeshProUGUI _text;

    private void Awake()
    {
        if (_text == null)
        {
            _text = GetComponent<TMPro.TextMeshProUGUI>();
        }
    }

    public void Initialize(string message)
    {
        this.message = message;
        UpdateText();
    }

    private void UpdateText()
    {
        if (_text == null)
        {
            return;
        }
        
        _text.text = message;
    }
}
