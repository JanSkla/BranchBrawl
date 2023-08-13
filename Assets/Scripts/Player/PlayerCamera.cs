using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerCamera : NetworkBehaviour
{
    [SerializeField]
    private Camera FirstPersonCameraPrefab;

    private GameObject InGameUI;

    private Camera fpsCam;

    // Start is called before the first frame update
    void Start()
    {
        if (IsOwner)
        {
            fpsCam = Instantiate(FirstPersonCameraPrefab);
            fpsCam.transform.SetParent(transform);
        }
        InGameUI = GameObject.Find("InGameUI");
    }

    // Update is called once per frame
    void Update()
    {
        if (fpsCam && IsLocalPlayer)
        {
            float mouseY = Input.GetAxis("Mouse Y");

            Vector3 rotationInput = new Vector3(-mouseY, 0, 0);

            fpsCam.transform.Rotate(rotationInput);
        }
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.TransformDirection(Vector3.forward), out hit, 2) && hit.collider.gameObject.layer == 7)
        {
            InGameUI.GetComponent<InGameUI>().ChangeCursorColor(Color.cyan);
        }
        else
        {
            InGameUI.GetComponent<InGameUI>().ChangeCursorColor(Color.black);
        }
    }
}
