using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

public class ChatTester : NetworkBehaviour
{
    private void Update()
    {
        //If owner and space bar is pressed.
        if (base.IsOwner && Input.GetKeyDown(KeyCode.Space))
        {
            RpcSendChat($"{Chat.Instance.Message}", OwnerId);        
        }
    }

    [ServerRpc]
    private void RpcSendChat(string msg, int sendId)
    {
        Debug.Log($"Received {msg} on the server.");
        ReceiveChat(msg, OwnerId);
    }
    
    [ObserversRpc]
    private void ReceiveChat(string msg, int senderId)
    {
        Debug.Log($"{senderId} to {ClientManager.Connection.ClientId}. msg: {msg}");

        bool isMe = senderId == ClientManager.Connection.ClientId;
        

        Chat.Instance.AddMessage(msg, isMe);
    }
}
