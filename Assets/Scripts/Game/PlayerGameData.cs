using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public struct PlayerGameData : INetworkSerializable, IEquatable<PlayerGameData>
{
    public ulong PMNwId;
    public int Crowns;
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        if (serializer.IsReader)
        {
            var reader = serializer.GetFastBufferReader();
            reader.ReadValueSafe(out PMNwId);
            reader.ReadValueSafe(out Crowns);
        }
        else
        {
            var writer = serializer.GetFastBufferWriter();
            writer.WriteValueSafe(PMNwId);
            writer.WriteValueSafe(Crowns);
        }
    }

    //IEquatable
    public bool Equals(PlayerGameData other)
    {
        return PMNwId == other.PMNwId && Crowns == other.Crowns;
    }
    //~IEquatable
}
