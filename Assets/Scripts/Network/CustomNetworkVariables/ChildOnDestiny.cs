using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public struct ChildOnDestiny : INetworkSerializable, IEquatable<ChildOnDestiny>
{
    public int DestinyIndex;
    public ulong ChildNwId;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        if (serializer.IsReader)
        {
            var reader = serializer.GetFastBufferReader();
            reader.ReadValueSafe(out DestinyIndex);
            reader.ReadValueSafe(out ChildNwId);
        }
        else
        {
            var writer = serializer.GetFastBufferWriter();
            writer.WriteValueSafe(DestinyIndex);
            writer.WriteValueSafe(ChildNwId);
        }
    }

    //IEquatable
    public bool Equals(ChildOnDestiny other)
    {
        return DestinyIndex == other.DestinyIndex && ChildNwId == other.ChildNwId;
    }
    //~IEquatable
}
