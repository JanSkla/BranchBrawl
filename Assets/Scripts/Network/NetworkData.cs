using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class NetworkData : NetworkBehaviour
{
    public NetworkVariable<FixedString32Bytes> JoinCode;
    public NetworkList<ulong> PlayerObjectNwIds = new();
}
