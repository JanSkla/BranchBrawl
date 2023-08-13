using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
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
            fpsCam = Instantiate(FirstPersonCameraPrefab, new Vector3(0, 0.5f, 0.3f), new Quaternion());
            fpsCam.transform.SetParent(transform);
        }
        InGameUI = GameObject.Find("InGameUI");
        InGameUI.SetActive(true);
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

    public GameObject GetFacingPickable()
    {
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.TransformDirection(Vector3.forward), out hit, 2) && hit.collider.gameObject.layer == 7)
        {
            GameObject facingPickable = hit.collider.gameObject;
            while(facingPickable.transform.parent != null)
            {
                facingPickable = facingPickable.transform.parent.gameObject;
            }
            return facingPickable;
        }
        return null;
    }
}
