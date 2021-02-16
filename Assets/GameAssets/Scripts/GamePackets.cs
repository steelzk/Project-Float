using Mirror;
using Steamworks;

namespace Steel.Networking
{
    public struct JoinPacket : NetworkMessage
    {
        public byte[] authTicket;
        public ulong steamId;
    }
}