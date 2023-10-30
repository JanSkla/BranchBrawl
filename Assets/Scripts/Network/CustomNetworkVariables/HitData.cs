using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements.Experimental;
using UnityEngine.UIElements;

public struct HitData : INetworkSerializable
{
    public bool IsHit;
    public ulong HitNwID;
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        if (serializer.IsReader)
        {
            var reader = serializer.GetFastBufferReader();
            reader.ReadValueSafe(out IsHit);
            reader.ReadValueSafe(out HitNwID);
        }
        else
        {
            var writer = serializer.GetFastBufferWriter();
            writer.WriteValueSafe(IsHit);
            writer.WriteValueSafe(HitNwID);
        }
    }
}
