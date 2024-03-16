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

    private GameObject _inGameUI;
    private Player _player;

    public GameObject FpsCam;
    private GameObject _cameraGO;

    [SerializeField]
    private float _pickupRange;

    public bool IsEnabled = true;

    public override void OnNetworkSpawn()
    {
        _player = GetComponent<Player>();
        if (IsEnabled && _player.IsLocalPlayer)
        {
            CreateCamera();
            OnGameStart();
        }
    }

    void Update()
    {
        if (!IsEnabled) return;
        if (FpsCam && _inGameUI)
        {
            RaycastHit hit = new();
            if (Physics.Raycast(FpsCam.transform.position, FpsCam.transform.TransformDirection(Vector3.forward), out hit, _pickupRange, LayerMask.GetMask("Pickable")))
            {
                _inGameUI.GetComponent<InGameUI>().Game.GetComponent<GameUI>().ChangeCursorColor(Color.cyan);
            }
            else
            {
                _inGameUI.GetComponent<InGameUI>().Game.GetComponent<GameUI>().ChangeCursorColor(Color.black);
            }
        }
    }

    public GameObject GetFacingPickable()
    {
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(FpsCam.transform.position, FpsCam.transform.TransformDirection(Vector3.forward), out hit, _pickupRange, LayerMask.GetMask("Pickable")))
        {
            return hit.collider.gameObject;
        }
        return null;
    }

    public void CreateCamera()
    {
        _cameraGO = Instantiate(FirstPersonCameraPrefab);
        FpsCam = _cameraGO.transform.GetChild(0).gameObject;
        _cameraGO.transform.SetParent(_player.Head.transform, false);
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
        _cameraGO.SetActive(false);
    }

    private void EnableFirstCamera()
    {
        _cameraGO.SetActive(true);
    }

    private void FindInGameUI()
    {
        _inGameUI = GameObject.Find("InGameUI");
    }
}
