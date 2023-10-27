using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Hand : NetworkBehaviour
{
    // Start is called before the first frame update
    public override void OnNetworkSpawn()
    {

        Debug.Log("HandSpawn");
    }
    void Start()
    {
        Debug.Log("HandStart");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
