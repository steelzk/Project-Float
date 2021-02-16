using UnityEngine;
using UnityEngine.UI;
using Steamworks.Data;
using Steamworks;
using TMPro;
using Steel.Networking;
using Steel.Steam;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject serverList;
    [SerializeField] private GameObject serverListingPrefab;

    [SerializeField] private GameObject joinTab;
    [SerializeField] private GameObject hostTab;

    [SerializeField] private TMP_InputField serverNameInput;
    [SerializeField] private TMP_InputField maxPlayersInput;

    private Steamworks.ServerList.Internet internetListing;

    private void Awake()
    {
        internetListing = new Steamworks.ServerList.Internet();
        internetListing.OnResponsiveServer += OnServerRequest;
    }

    public void ShowJoinTab()
    {
        hostTab.SetActive(false);
        joinTab.SetActive(true);
    }

    public void ShowHostTab()
    {
        hostTab.SetActive(true);
        joinTab.SetActive(false);
    }

    public void RefreshListings()
    {
        internetListing.RunQueryAsync();
    }

    public void StartHostGame()
    {
        SSteamSettings.current.server.maxPlayerCount = int.Parse(maxPlayersInput.text);
        SNetworkManager.singleton.StartHost();
    }

    private void OnServerRequest(ServerInfo info)
    {
        ServerListing.Create(info, serverList.transform, serverListingPrefab);
    }
}
