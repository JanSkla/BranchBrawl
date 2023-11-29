using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public struct PlayerGameData : INetworkSerializable, IEquatable<PlayerGameData>
{
    public ulong PlayerNwId;
    public int Crowns;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        if (serializer.IsReader)
        {
            var reader = serializer.GetFastBufferReader();
            reader.ReadValueSafe(out PlayerNwId);
            reader.ReadValueSafe(out Crowns);
        }
        else
        {
            var writer = serializer.GetFastBufferWriter();
            writer.WriteValueSafe(PlayerNwId);
            writer.WriteValueSafe(Crowns);
        }
    }

    //IEquatable
    public bool Equals(PlayerGameData other)
    {
        return PlayerNwId == other.PlayerNwId && Crowns == other.Crowns;
    }
}
