using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks.Data;
using Mirror;
using Steel.Networking;

public class ServerListing : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI serverInfoText;

    private ServerInfo serverInfo;

    public static void Create(ServerInfo info, Transform listingParent, GameObject listingPrefab)
    {
        var listing = Instantiate(listingPrefab, listingParent);
        listing.GetComponent<ServerListing>().RefreshListing(info);
    }

    public void RefreshListing(ServerInfo info)
    {
        serverInfo = info;
        serverInfoText.text = $"{serverInfo.Name} : {serverInfo.Players}/{serverInfo.MaxPlayers}";
    }

    public void ConnectToListing()
    {
        SNetworkManager.singleton.networkAddress = serverInfo.Address.ToString();
        SNetworkManager.singleton.StartClient();
    }


}
