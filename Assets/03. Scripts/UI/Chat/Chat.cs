using System;
using DG.Tweening;
using FishNet.Connection;
using FishNet.Object;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Chat : MonoBehaviour
{
    [Header("[Message Prefabs]")]
    [SerializeField]
    private GameObject _myMessagePrefab;
    [SerializeField]
    private GameObject _otherMessagePrefab;

    [Header("[Components]")]
    [SerializeField]
    private ScrollRect _scroll;
    [SerializeField]
    private TMP_InputField _inputField;
    [SerializeField]
    private Button _submitButton;

    public static Chat Instance;
    public string Message => _inputField.text;
    
    private void Awake()
    {
        Instance = this;
    }

    public void AddMessage(string message, bool isMyMessage)
    {
        GameObject chatMsgPrefab = isMyMessage ? _myMessagePrefab : _otherMessagePrefab;
        GameObject chatMsgGo = Instantiate(chatMsgPrefab, _scroll.content);
        if (chatMsgGo && chatMsgGo.TryGetComponent(out ChatMessage chatMsg))
        {
            chatMsg.Initialize(message);
        }

        // Scroll to the bottom
        _scroll.DOVerticalNormalizedPos(0, 0.2f)
               .SetEase(Ease.OutCubic);
    }

    public void Clear()
    {
        foreach (RectTransform chatElement in _scroll.content)
        {
            Destroy(chatElement.gameObject);
        }
    }
}
