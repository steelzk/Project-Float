using Mirror;
using UnityEngine;
using Steamworks;

namespace Steel.Networking
{
    [System.Serializable]
    public class PlayerCreationInfo
    {
        public Color playerColor;

        public PlayerCreationInfo()
        {
            playerColor = Color.white;
        }
    }

    public struct JoinPacket : NetworkMessage
    {
        public byte[] authTicket;
        public ulong steamId;
        public PlayerCreationInfo playerCreationInfo;
    }
}