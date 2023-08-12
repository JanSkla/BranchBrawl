using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public struct Item : INetworkSerializable
{
    public int Id;
    public ulong NetworkObjectId;
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        if (serializer.IsReader)
        {
            var reader = serializer.GetFastBufferReader();
            reader.ReadValueSafe(out Id);
            reader.ReadValueSafe(out NetworkObjectId);
        }
        else
        {
            var writer = serializer.GetFastBufferWriter();
            writer.WriteValueSafe(Id);
            writer.WriteValueSafe(NetworkObjectId);
        }
    }
}
