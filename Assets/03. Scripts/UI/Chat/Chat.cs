using System;
using DG.Tweening;
using FishNet.Connection;
using FishNet.Object;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Chat : NetworkBehaviour
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
        _inputField.onSubmit.AddListener(OnSubmit);
        _submitButton.onClick.AddListener(() => OnSubmit(_inputField.text));
    }

    private void OnSubmit(string message)
    {
        Debug.Log($"[OnSubmit] IsServer={IsServer}, IsClient={IsClient}, IsOwner={IsOwner}");

        if (string.IsNullOrWhiteSpace(message))
        {
            return;
        }

        SendChat();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SendChat()
    {
        Debug.Log($"Sending chat message on server");

        string message = _inputField.text.Trim();
        
        BcastChatMessage(message);
    }
    
    [ObserversRpc]
    private void BcastChatMessage(string message)
    {
        // This method should be called on the server to broadcast the message to all clients
        if (string.IsNullOrWhiteSpace(message))
        {
            return;
        }

        // Here we would typically use a NetworkManager or similar to send the message to all clients
        // For now, we will just call AddMessage on all clients
        AddMessage(message, IsOwner);
        
        _inputField.text = string.Empty;
        _inputField.ActivateInputField();
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

    [Server]
    public void AssignOwnership(NetworkConnection conn)
    {

        NetworkObject.GiveOwnership(conn);
        Debug.Log($"[Chat] Ownership assigned to client {conn.ClientId}");
    }
}
