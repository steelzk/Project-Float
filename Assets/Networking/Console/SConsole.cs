using UnityEngine;
using System;
using Steamworks;
using Steel.Networking;

public class SConsole : MonoBehaviour
{
    public string consoleTitle = "Project Float Dedicated Server";

    private Windows.ConsoleWindow console = new Windows.ConsoleWindow();
    private Windows.ConsoleInput input = new Windows.ConsoleInput();

    private void Awake()
    {
        console.Initialize();
        console.SetTitle(consoleTitle);

        input.OnInputText += OnInputText;
        Application.logMessageReceived += HandleLog;
    }

    private void OnInputText(string obj)
    {
        switch (obj)
        {
            case "quit":
                Application.Quit();
                break;
            case "stop":
                Application.Quit();
                break;
            case "playercount":
                Console.WriteLine($"Player Count: {SNetworkManager.singleton.numPlayers}/{SteamServer.MaxPlayers}");
                break;
            case "getip":
                Console.WriteLine($"IP: {SteamServer.PublicIp}");
                break;
            default:
                break;
        }
    }

    private void HandleLog(string message, string trace, LogType type)
    {
        switch (type)
        {
            case LogType.Warning:
                Console.ForegroundColor = ConsoleColor.Yellow;
                break;
            case LogType.Error:
                Console.ForegroundColor = ConsoleColor.Red;
                break;
            default:
                Console.ForegroundColor = ConsoleColor.White;
                break;
        }

        if (Console.CursorLeft != 0)
            input.ClearLine();

        input.RedrawInputLine();
    }

    private void Update()
    {
        input.Update();
    }

    private void OnDestroy()
    {
        console.Shutdown();
    }
}
