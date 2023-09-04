using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public struct TransformState : INetworkSerializable
{
    public int Tick;
    public Vector3 Position;
    public Quaternion Rotation;
    public Quaternion Facing;
    public bool HasStartedMoving;
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        if (serializer.IsReader)
        {
            var reader = serializer.GetFastBufferReader();
            reader.ReadValueSafe(out Tick);
            reader.ReadValueSafe(out Position);
            reader.ReadValueSafe(out Rotation);
            reader.ReadValueSafe(out Facing);
            reader.ReadValueSafe(out HasStartedMoving);
        }
        else
        {
            var writer = serializer.GetFastBufferWriter();
            writer.WriteValueSafe(Tick);
            writer.WriteValueSafe(Position);
            writer.WriteValueSafe(Rotation);
            writer.WriteValueSafe(Facing);
            writer.WriteValueSafe(HasStartedMoving);
        }
    }
}
