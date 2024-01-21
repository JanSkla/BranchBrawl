using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ShootData : INetworkSerializable
{
    public int Amount;
    public ShootType Type;
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        if (serializer.IsReader)
        {
            var reader = serializer.GetFastBufferReader();
            reader.ReadValueSafe(out Amount);
            reader.ReadValueSafe(out int type );

            Type = (ShootType)type;
        }
        else
        {
            var writer = serializer.GetFastBufferWriter();
            writer.WriteValueSafe(Amount);
            writer.WriteValueSafe((int)Type);
        }
    }
}

public enum ShootType
{
    Bullet = 0
}
