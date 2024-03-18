using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public struct InputState : INetworkSerializable
{
    public int Tick;
    public Vector3 movementInput;
    public Vector3 rotationInput;
    public float DeltaTime;
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        if (serializer.IsReader)
        {
            var reader = serializer.GetFastBufferReader();
            reader.ReadValueSafe(out Tick);
            reader.ReadValueSafe(out movementInput);
            reader.ReadValueSafe(out rotationInput);
            reader.ReadValueSafe(out DeltaTime);
        }
        else
        {
            var writer = serializer.GetFastBufferWriter();
            writer.WriteValueSafe(Tick);
            writer.WriteValueSafe(movementInput);
            writer.WriteValueSafe(rotationInput);
            writer.WriteValueSafe(DeltaTime);
        }
    }

}
