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

    private Player _player;

    void Start()
    {
        _player = transform.parent.gameObject.GetComponent<Player>();
        if (_player.IsLocalPlayer)
        {
            CreateCamera();
            OnGameStart();
        }
    }

    void Update()
    {
        if (fpsCam && _inGameUI)
        {
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
            return hit.collider.gameObject;
        }
        return null;
    }

    public void CreateCamera()
    {
        fpsCam = Instantiate(FirstPersonCameraPrefab);
        fpsCam.transform.SetParent(transform);
        fpsCam.transform.localPosition = CameraOffset;
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
