using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public struct PlayerGameData : INetworkSerializable, IEquatable<PlayerGameData>
{
    public ulong PlayerManagerNwId;
    public int Crowns;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        if (serializer.IsReader)
        {
            var reader = serializer.GetFastBufferReader();
            reader.ReadValueSafe(out PlayerManagerNwId);
            reader.ReadValueSafe(out Crowns);
        }
        else
        {
            var writer = serializer.GetFastBufferWriter();
            writer.WriteValueSafe(PlayerManagerNwId);
            writer.WriteValueSafe(Crowns);
        }
    }

    //IEquatable
    public bool Equals(PlayerGameData other)
    {
        return PlayerManagerNwId == other.PlayerManagerNwId && Crowns == other.Crowns;
    }
    //~IEquatable
}
