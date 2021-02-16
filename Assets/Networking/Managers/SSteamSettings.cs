using UnityEngine;
using Steamworks;

namespace Steel.Steam
{
    [CreateAssetMenu(menuName="Steel/Steam Settings", fileName="New Steam Settings")]
    public class SSteamSettings : ScriptableObject
    {
        public static SSteamSettings current;

        public uint applicationId = 1541370;
        public bool steamInitialized { get; private set; }

        public GameServer server = new GameServer();

        public void InitServer()
        {
            current = this;

            // init steamserver
            SteamServerInit serverInit = new SteamServerInit()
            {
                DedicatedServer = server.isDedicated,
                GameDescription = server.serverDescription,
                GamePort = server.gamePort,
                QueryPort = server.queryPort,
                SteamPort = server.steamPort,
                ModDir = server.modDir,
                VersionString = server.serverVersion,
                Secure = server.isSecure //* needed for server to be listed
            };

            // initialize steam client
            try
            {
                SteamServer.Init(applicationId, serverInit, true);
            }
            catch (System.Exception e)
            {
                Debug.Log($"STEAM_API Server Could not Initialize!: {e}");
            }

            // check if initialized
            if (SteamServer.IsValid)
            {
                current.steamInitialized = true;
            }
        }

        public void InitClient()
        {
            current = this;

            // open steam version
            if (SteamClient.RestartAppIfNecessary(applicationId))
            {
                Application.Quit();
                return;
            }

            // initialize steam client
            try
            {
                SteamClient.Init(applicationId, true);
            } catch (System.Exception e)
            {
                Debug.Log($"STEAM_API Client Could not Initialize!: {e}");
            }

            // check if initialized
            if (SteamServer.IsValid)
            {
                current.steamInitialized = true;
            }
        }

        public void Shutdown()
        {
            if (SteamClient.IsValid)
            {
                SteamClient.Shutdown();
            }
            if (SteamServer.IsValid)
            {
                SteamServer.Shutdown();
            }
        }

        [System.Serializable]
        public class GameServer
        {
            [Header("Initialization Settings")]
            public System.Net.IPAddress ip = System.Net.IPAddress.Any;
            [Tooltip("The port used to listen for connections.")]
            public ushort gamePort = 27015;
            [Tooltip("The port used to show the server in ServerList.Internet (should be the same as gamePort).")]
            public ushort queryPort = 27017;
            [Tooltip("The port used to connect to steam. Normally set to gamePort + 1.")]
            public ushort steamPort = 27016;
            public string serverVersion = "1.0.0.0";
            [Tooltip("The name of the project folder.")]
            public string modDir = "Project Float";

            [Header("Server Settings")]
            public SteamId serverId;
            public string serverName = "Project Float Server";
            public string serverDescription = "Project Float Description";
            public string serverMap = "map_float";
            public int maxPlayerCount = 20;
            public bool enableHeartbeats = true;
            public bool anonymousServerLogin = true;
            [Tooltip("Used if anonymousServerLogin is set to false.")]
            public string gameServerToken = "See https://steamcommunity.com/dev/managegameservers";
            public bool passwordProtected = false;
            public bool isDedicated = true;
            [Tooltip("Needed for server to be shown in ServerList!")]
            public bool isSecure = true;
        }
    }
}
