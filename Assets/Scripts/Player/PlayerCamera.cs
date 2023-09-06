using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.Netcode;
using UnityEngine;

public class PlayerCamera : NetworkBehaviour
{
    [SerializeField]
    private GameObject FirstPersonCameraPrefab;

    [SerializeField]
    private Vector3 CameraOffset = new Vector3(0, 0.5f, 0.3f);

    private GameObject _inGameUI;

    private GameObject fpsCam;

    void Start()
    {
        if (IsOwner)
        {
            LoadCamera();
        }
    }

    void Update()
    {
        if (fpsCam && _inGameUI)
        {
            //if (fpsCam && IsLocalPlayer)
            //{
            //    float mouseY = Input.GetAxis("Mouse Y");

            //    Vector3 rotationInput = new Vector3(-mouseY, 0, 0);

            //    fpsCam.transform.Rotate(rotationInput);
            //}
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.TransformDirection(Vector3.forward), out hit, 4, LayerMask.GetMask("Pickable")))
            {
                _inGameUI.GetComponent<InGameUI>().ChangeCursorColor(Color.cyan);
            }
            else
            {
                _inGameUI.GetComponent<InGameUI>().ChangeCursorColor(Color.black);
            }
        }
    }

    public GameObject GetFacingPickable()
    {
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.TransformDirection(Vector3.forward), out hit, 4, LayerMask.GetMask("Pickable")))
        {
            Debug.Log(hit.collider.gameObject.name);
            GameObject facingPickable = hit.collider.gameObject;
            while(facingPickable.transform.parent != null)
            {
                facingPickable = facingPickable.transform.parent.gameObject;
            }
            return facingPickable;
        }
        return null;
    }

    public void LoadCamera()
    {
        fpsCam = Instantiate(FirstPersonCameraPrefab, CameraOffset, new Quaternion());
        fpsCam.transform.SetParent(transform);
    }
    public void OnGameStart()
    {
        if (!_inGameUI)
        {
            FindInGameUI();
        }
        EnableFirstCamera();
    }

    private void DisableFirstCamera()
    {
        fpsCam.SetActive(false);
    }

    private void EnableFirstCamera()
    {
        fpsCam.SetActive(true);
    }

    private void FindInGameUI()
    {
        _inGameUI = GameObject.Find("InGameUI");
    }
}
