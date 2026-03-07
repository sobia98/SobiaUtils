using Unity.Netcode;

namespace Sobia.LaserProject
{
    public struct PlayerMovementSettings : INetworkSerializable
    {
        public float Speed;
        public float JumpHeight;

        public PlayerMovementSettings(float speed, float jumpHeight)
        {
            Speed = speed;
            JumpHeight = jumpHeight;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Speed);
            serializer.SerializeValue(ref JumpHeight);
        }
    }
}