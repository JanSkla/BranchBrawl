using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public struct PlayerGameData : INetworkSerializable, IEquatable<PlayerGameData>
{
    public ulong ClientId;
    public int Crowns;
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        if (serializer.IsReader)
        {
            var reader = serializer.GetFastBufferReader();
            reader.ReadValueSafe(out ClientId);
            reader.ReadValueSafe(out Crowns);
        }
        else
        {
            var writer = serializer.GetFastBufferWriter();
            writer.WriteValueSafe(ClientId);
            writer.WriteValueSafe(Crowns);
        }
    }

    //IEquatable
    public bool Equals(PlayerGameData other)
    {
        return ClientId == other.ClientId && Crowns == other.Crowns;
    }
    //~IEquatable
}
