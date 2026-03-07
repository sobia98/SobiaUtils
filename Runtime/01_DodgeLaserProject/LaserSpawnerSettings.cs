using Unity.Netcode;
using System;

namespace Sobia.LaserProject
{
    public struct LaserSpawnerSettings : INetworkSerializable, IEquatable<LaserSpawnerSettings>
    {
        public float SpawnRangeX;
        public float SpawnYPosition;
        public float MinSpawnInterval;
        public float MaxSpawnInterval;
        public float Lifetime;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref SpawnRangeX);
            serializer.SerializeValue(ref SpawnYPosition);
            serializer.SerializeValue(ref MinSpawnInterval);
            serializer.SerializeValue(ref MaxSpawnInterval);
            serializer.SerializeValue(ref Lifetime);
        }

        public bool Equals(LaserSpawnerSettings other)
        {
            return SpawnRangeX.Equals(other.SpawnRangeX) &&
                   SpawnYPosition.Equals(other.SpawnYPosition) &&
                   MinSpawnInterval.Equals(other.MinSpawnInterval) &&
                   MaxSpawnInterval.Equals(other.MaxSpawnInterval) &&
                   Lifetime.Equals(other.Lifetime);
        }

        public override bool Equals(object obj)
        {
            return obj is LaserSpawnerSettings other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(SpawnRangeX, SpawnYPosition, MinSpawnInterval, MaxSpawnInterval, Lifetime);
        }
    }
}