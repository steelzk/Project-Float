using UnityEngine;
using System.Collections.Generic;
using Mirror;
using Steamworks;
using Steel.Networking;

namespace Steel.Steam
{
    public class SSteamAuthenticator : NetworkAuthenticator
    {
        [System.Serializable]
        private struct SteamConnection
        {
            public NetworkConnection connection;
            public SteamId steamId;
            public string steamName;
        }

        private struct AuthRequest : NetworkMessage
        {
            public AuthTicket ticket;
            public SteamId steamId;
            public string steamName;
        }

        private struct AuthResponse : NetworkMessage
        {
            public Steamworks.AuthResponse authResult;
        }

        private struct DisconnectMessage : NetworkMessage
        {
            public SteamId steamId;
        }

        private List<SteamConnection> currentConnections = new List<SteamConnection>();
        private List<SteamConnection> connectionsToAuth = new List<SteamConnection>();

        //* Called on server when needing to authenticate a user
        private void OnValidateTicket(SteamId userId, SteamId ownerId, Steamworks.AuthResponse response)
        {
            Debug.Log("[Server] Received AuthTicket to authenticate!");

            // creates a new AuthReponse object to store steams authentication response
            AuthResponse responsePacket = new AuthResponse()
            {
                authResult = response
            };

            // find user needing to be authenticated
            for (int i=0; i< connectionsToAuth.Count; i++)
            {
                if (connectionsToAuth[i].steamId == userId)
                {
                    // tell user to connect/disconnect
                    connectionsToAuth[i].connection.Send(responsePacket);

                    // accept/deny user depending on auth reponse
                    if (response == Steamworks.AuthResponse.OK)
                    {
                        Debug.Log($"[Server] Player: { connectionsToAuth[i].steamName} has succesfully authenticated!");

                        // accept connection
                        ServerAccept(connectionsToAuth[i].connection);

                        currentConnections.Add(connectionsToAuth[i]);
                    } else
                    {
                        ServerReject(connectionsToAuth[i].connection);
                        SteamServer.EndSession(connectionsToAuth[i].steamId);
                        connectionsToAuth[i].connection.Disconnect();
                        Debug.Log($"[Server] Player: {connectionsToAuth[i].steamName} failed to authenticate : {response}");
                    }

                    // remove user from pending list
                    connectionsToAuth.RemoveAt(i);
                }
            }
        }

        private void OnServerAuthRequest(NetworkConnection conn, AuthRequest request)
        {
            Debug.Log($"[Server] Attempting to authenticate player {request.steamName}...");

            // create connection object
            SteamConnection connection = new SteamConnection()
            {
                connection = conn,
                steamId = request.steamId,
                steamName = request.steamName
            };

            // start authenticate session with given ticket data
            if (!SteamServer.BeginAuthSession(request.ticket.Data, request.steamId))
            {
                Debug.Log($"[Server] Could not authenticate user ticket data!");
                conn.Disconnect();
                return;
            }

            // add connection to connections to authenticate
            connectionsToAuth.Add(connection);
        }

        private void OnClientAuthResponse(NetworkConnection conn, AuthResponse response)
        {
            Debug.Log("[Client] Received Auth Response from server!");

            // check ticket result / connect/disconnect
            if (response.authResult == Steamworks.AuthResponse.OK)
            {
                Debug.Log("[Client] AuthReponse Accepted!");
                ClientAccept(conn);
            } else
            {
                Debug.Log("[Client] AuthReponse Rejected!");
                ClientReject(conn);
            }
        }

        private void OnServerPlayerDisconnect(NetworkConnection conn, DisconnectMessage message)
        {
            for (int i=0; i<currentConnections.Count; i++)
            {
                if (currentConnections[i].steamId == message.steamId)
                {
                    SteamServer.EndSession(message.steamId);
                    NetworkServer.RemovePlayerForConnection(conn, true);
                    Debug.Log($"[Server] User has Disconnected: {currentConnections[i].steamName}");
                    currentConnections.RemoveAt(i);
                }
            }
        }

        public override void OnStartServer()
        {
            base.OnStartServer();

            NetworkServer.RegisterHandler<AuthRequest>(OnServerAuthRequest, false);
            NetworkServer.RegisterHandler<DisconnectMessage>(OnServerPlayerDisconnect);
            SteamServer.OnValidateAuthTicketResponse += OnValidateTicket;
        }

        public override void OnStopServer()
        {
            base.OnStopServer();

            NetworkServer.UnregisterHandler<AuthRequest>();
            NetworkServer.UnregisterHandler<DisconnectMessage>();
            SteamServer.OnValidateAuthTicketResponse -= OnValidateTicket;
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            NetworkClient.RegisterHandler<AuthResponse>(OnClientAuthResponse, false);
        }

        public override void OnStopClient()
        {

            if (NetworkClient.connection.authenticationData != null)
            {
                var message = new DisconnectMessage()
                {
                    steamId = SteamClient.SteamId
                };

                NetworkClient.Send(message);
            }

            NetworkClient.connection.authenticationData = null;

            base.OnStopClient();
            NetworkClient.UnregisterHandler<AuthResponse>();
        }

        //* Called on client when attempting to be authenticated
        public override void OnClientAuthenticate(NetworkConnection conn)
        {
            Debug.Log("[Client] Sending auth ticket to server!");

            // create auth request
            AuthRequest request = new AuthRequest()
            {
                ticket = SteamUser.GetAuthSessionTicket(),
                steamId = SteamClient.SteamId,
                steamName = SteamClient.Name
            };

            conn.authenticationData = request.ticket.Data;

            // send request to server
            NetworkClient.Send(request);
        }

        //* Called on server when a connection needs to be authenticated
        public override void OnServerAuthenticate(NetworkConnection conn)
        {

        }
    }
}
