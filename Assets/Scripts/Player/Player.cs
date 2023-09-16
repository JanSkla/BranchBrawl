using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField]
    private GameObject handPrefab;
    [SerializeField]
    public GameObject head;

    public GameObject hand;
    // Start is called before the first frame update
    void Start()
    {
        if (IsServer)
        {
            hand = Instantiate(handPrefab);
            hand.layer = IsLocalPlayer ? 8 : 6;
            hand.GetComponent<NetworkObject>().Spawn(); /////CHECK HERE
            hand.GetComponent<NetworkObject>().TrySetParent(transform);/////CHECK HERE/////CHECK HERE
        }
        if (IsClient)
        {
            hand = transform.Find("Hand(Clone)").gameObject;
        }
    }
}
