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

    [HideInInspector]
    public GameObject hand;

    private bool _isAlive = true;
    public bool IsAlive
    {
        get { return _isAlive; }
        set
        {
            _isAlive = value;
            Debug.Log("Died" + value);

            if (!_isAlive)
            {
                Die();
            }
            if (IsLocalPlayer)
            {
                if(GetComponent<LocalPlayer>()._inGameUI != null)
                    GetComponent<LocalPlayer>()._inGameUI.UpdateScreen(_isAlive);
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        if (IsLocalPlayer)
        {
            name = "Local Player";
            GetComponent<LocalPlayer>().enabled = true;
        }
        if (IsServer)
        {
            hand = Instantiate(handPrefab);
            hand.layer = IsLocalPlayer ? 8 : 6;
            hand.GetComponent<NetworkObject>().Spawn();
            hand.GetComponent<NetworkObject>().TrySetParent(transform);
        }
        if (IsClient)
        {
            hand = transform.Find("Hand(Clone)").gameObject;
            hand.layer = IsLocalPlayer ? 8 : 6;
        }
    }

    public void Die()
    {
        GetComponent<Renderer>().material.color = Color.red;
        Debug.Log(gameObject.name + " died");
    }
}
