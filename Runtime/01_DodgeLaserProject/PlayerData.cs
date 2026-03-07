using Unity.Collections;
using Unity.Netcode;

namespace Sobia.LaserProject
{
    public struct PlayerData : INetworkSerializable, System.IEquatable<PlayerData>
    {
        public ulong ClientId;
        public float CurrentTime;
        public FixedString64Bytes ClientName;

        public PlayerData(ulong id, float time, string name)
        {
            ClientId = id;
            CurrentTime = time;
            ClientName = new FixedString64Bytes(name);
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref ClientId);
            serializer.SerializeValue(ref CurrentTime);
            serializer.SerializeValue(ref ClientName);
        }

        public bool Equals(PlayerData other)
        {
            return ClientId == other.ClientId &&
                   CurrentTime == other.CurrentTime &&
                   ClientName.Equals(other.ClientName);
        }

        public override bool Equals(object obj)
        {
            return obj is PlayerData other && Equals(other);
        }

        public override int GetHashCode()
        {
            return ClientId.GetHashCode() ^ CurrentTime.GetHashCode();
        }
    }
}