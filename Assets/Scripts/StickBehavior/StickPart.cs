using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class StickPart : NetworkBehaviour
{
    public NetworkList<ulong> ConnectedEdgeNwIdsP = new NetworkList<ulong>(); //positive
    public NetworkList<ulong> ConnectedEdgeNwIdsN = new NetworkList<ulong>(); //negative

    //public override void OnNetworkSpawn()
    //{
    //    ConnectedEdgeNwIds.OnListChanged += OnListChanged;
    //}

    //private void OnListChanged(NetworkListEvent<ulong> t)
    //{
    //    string n = "ss";

    //    for (int i = 0; i < ConnectedEdgeNwIds.Count; i++)
    //    {
    //        n += "//"+ConnectedEdgeNwIds[i];
    //    }

    //    name = n;
    //}


    //void Start()
    //{
    //    ConnectedEdgeIds.Add(1);
    //}
}
