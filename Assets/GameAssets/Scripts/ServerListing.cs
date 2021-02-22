using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks.Data;
using Mirror;
using Steel.Networking;
using Steel.Steam;

public class ServerListing : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI serverInfoText;

    private ServerInfo serverInfo;

    public void RefreshListing(ServerInfo info)
    {
        serverInfo = info;
        serverInfoText.text = $"{serverInfo.Name} : {serverInfo.Players}/{serverInfo.MaxPlayers}";
    }

    public void ConnectToListing()
    {
        SSteamClient.singleton.ConnectToServer(serverInfo.Address.ToString());
    }
}
