using UnityEngine;
using Mirror;
using Steamworks;

namespace Steel.Networking
{
    public class SNetworkManager : NetworkManager
    {
        public new static SNetworkManager singleton;

        public override void Awake()
        {
            base.Awake();

            // singleton reference
            if (singleton == null)
                singleton = this;
            else if (singleton != this)
                Destroy(this);
        }

        #region ServerSide
        public override void OnStartServer()
        {
            base.OnStopServer();
            Debug.Log("Server Started");
        }

        public override void OnServerConnect(NetworkConnection conn)
        {
            base.OnServerConnect(conn);

            Debug.Log($"Incomming connection from: {conn.connectionId}");
        }

        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            base.OnServerAddPlayer(conn);
        }
        #endregion
        #region ClientSide

        public override void OnStartClient()
        {
            base.OnStartClient();
        }

        public override void OnStartHost()
        {
            base.OnStartHost();

            SSceneManager.GoClientScene();
        }

        public override void OnStopHost()
        {
            base.OnStopHost();

            SSceneManager.GoMainMenu();
        }

        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);

            SSceneManager.GoClientScene();
        }

        public override void OnClientDisconnect(NetworkConnection conn)
        {
            base.OnClientDisconnect(conn);

            SSceneManager.GoMainMenu();
        }
        #endregion
    }
}
