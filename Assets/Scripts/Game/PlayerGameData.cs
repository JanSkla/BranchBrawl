using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public struct PlayerGameData : INetworkSerializable, IEquatable<PlayerGameData>
{
    public ulong ClientId;
    public int Crowns;
    public FixedString32Bytes PlayerName;
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        if (serializer.IsReader)
        {
            var reader = serializer.GetFastBufferReader();
            reader.ReadValueSafe(out ClientId);
            reader.ReadValueSafe(out Crowns);
            reader.ReadValueSafe(out PlayerName);
        }
        else
        {
            var writer = serializer.GetFastBufferWriter();
            writer.WriteValueSafe(ClientId);
            writer.WriteValueSafe(Crowns);
            writer.WriteValueSafe(PlayerName);
        }
    }

    //IEquatable
    public bool Equals(PlayerGameData other)
    {
        return ClientId == other.ClientId && Crowns == other.Crowns && PlayerName == other.PlayerName;
    }
    //~IEquatable
}
