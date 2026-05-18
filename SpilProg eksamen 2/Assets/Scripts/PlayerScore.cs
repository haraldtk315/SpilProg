using System;
using Unity.Netcode;

public struct PlayerScore : INetworkSerializable, IEquatable<PlayerScore>
{

    public ulong player;
    public int score;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref player);
        serializer.SerializeValue(ref score);
    }

    public bool Equals(PlayerScore score)
    {
        return this.score == score.score && player == score.player;
    }

}
