using FishNet.Object;
using UnityEngine;

public class ChatTester : NetworkBehaviour
{
    private void Update()
    {
        //If owner and space bar is pressed.
        if (base.IsOwner && Input.GetKeyDown(KeyCode.Space))
        {
            RpcSendChat($"{Chat.Instance.Message}");        
        }
    }

    [ServerRpc]
    private void RpcSendChat(string msg)
    {
        Debug.Log($"Received {msg} on the server.");
        RpcSetNumber(msg);
    }
    
    [ObserversRpc]
    private void RpcSetNumber(string msg)
    {
        Debug.Log($"Received number {msg} from the server.");
        
        Chat.Instance.AddMessage(msg, true);
    }
}
