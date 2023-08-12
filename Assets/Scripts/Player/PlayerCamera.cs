using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerCamera : NetworkBehaviour
{
    [SerializeField]
    private Camera FirstPersonCameraPrefab;

    // Start is called before the first frame update
    void Start()
    {
        if (IsOwner)
        {
            Camera fpsCam = Instantiate(FirstPersonCameraPrefab);
            fpsCam.transform.SetParent(transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
