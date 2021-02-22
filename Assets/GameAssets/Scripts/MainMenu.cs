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
    [SerializeField] private Toggle localNetworkToggle;

    private Steamworks.ServerList.Internet internetListing;
    private Steamworks.ServerList.LocalNetwork localListing;

    private void OnEnable()
    {
        ShowJoinTab();
        ChangeListType();
    }

    private void OnDisable()
    {
        internetListing.OnResponsiveServer -= OnServerRequest;
        localListing.OnResponsiveServer -= OnServerRequest;
    }

    public void ChangeListType()
    {
        if (localNetworkToggle.isOn)
        {
            localListing = new Steamworks.ServerList.LocalNetwork();
            localListing.OnResponsiveServer += OnServerRequest;
            internetListing.OnResponsiveServer -= OnServerRequest;
        } else
        {
            internetListing = new Steamworks.ServerList.Internet();
            internetListing.OnResponsiveServer += OnServerRequest;
            localListing.OnResponsiveServer -= OnServerRequest;
        }
        RefreshListings();
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
        Debug.Log("Refreshing Server Listings...");

        if (localNetworkToggle.isOn)
            localListing.RunQueryAsync();
        else
            internetListing.RunQueryAsync();
    }

    public void StartHostGame()
    {
        SSteamSettings.current.server.maxPlayerCount = int.Parse(maxPlayersInput.text);
        SSteamSettings.current.server.serverName = serverNameInput.text;
        SSteamClient.singleton.HostLobby();
    }

    public void QuitGame() => Application.Quit();

    private void OnServerRequest(ServerInfo info)
    {
        Debug.Log($"ServerRequest: {info.Address}");

        var listing = Instantiate(serverListingPrefab, serverList.transform).GetComponent<ServerListing>();
        listing.RefreshListing(info);
    }
}
