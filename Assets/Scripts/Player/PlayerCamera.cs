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
    private Player _player;

    public GameObject FpsCam;

    void Start()
    {
        _player = GetComponent<Player>();
        if (_player.IsLocalPlayer)
        {
            CreateCamera();
            OnGameStart();
        }
    }

    void Update()
    {
        if (FpsCam && _inGameUI)
        {
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(FpsCam.transform.position, FpsCam.transform.TransformDirection(Vector3.forward), out hit, 4, LayerMask.GetMask("Pickable")))
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
        if (Physics.Raycast(FpsCam.transform.position, FpsCam.transform.TransformDirection(Vector3.forward), out hit, 4, LayerMask.GetMask("Pickable")))
        {
            return hit.collider.gameObject;
        }
        return null;
    }

    public void CreateCamera()
    {
        FpsCam = Instantiate(FirstPersonCameraPrefab);
        FpsCam.transform.SetParent(_player.Head.transform);
        FpsCam.transform.localPosition = CameraOffset;
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
        FpsCam.SetActive(false);
    }

    private void EnableFirstCamera()
    {
        FpsCam.SetActive(true);
    }

    private void FindInGameUI()
    {
        _inGameUI = GameObject.Find("InGameUI");
    }
}
