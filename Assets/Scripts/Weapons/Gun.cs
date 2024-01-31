using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class Gun : NetworkBehaviour
{
    [SerializeField]
    private GunData gunData;
    [SerializeField]
    private GameObject _muzzle;
    [SerializeField]
    private LineRenderer _line;

    private float _timeSinceLastShot = 0f;


    void Update()
    {
        _timeSinceLastShot += Time.deltaTime;
    }

    public void Shoot(bool firstShot)
    {
        if (!gunData.isAuto && !firstShot) return;
        if (!CanShoot() || gunData.currentAmmo <= 0) return;

        HitData hitData = new();

        Vector3 playerCameraPos = NetworkManager.LocalClient.PlayerObject.GetComponent<PlayerManager>().PlayerObject.GetComponent<PlayerCamera>().FpsCam.transform.position;

        if (Physics.Raycast(playerCameraPos, transform.forward, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Player")))
        {
            hitData.IsHit = true;
            GameObject hitTarget = hit.collider.gameObject;

            while (hitTarget.transform.parent != null)
            {
                hitTarget = hitTarget.transform.parent.gameObject;
            }

            hitData.HitNwID = hitTarget.GetComponent<NetworkObject>().NetworkObjectId;
            Debug.DrawRay(playerCameraPos, transform.forward * hit.distance, Color.green, 1);

            if (IsServer)
            {
                hitTarget.GetComponent<PlayerHealth>().Damage(gunData.damage);
            }
        }
        else
        {
            hitData.IsHit = false;
            Debug.DrawRay(playerCameraPos, transform.forward * 100, Color.red, 1);
        }

        ShootSendNetworkRpc(hitData);
        _timeSinceLastShot = 0f;
        ShotVisual();
    }

    private void ShotVisual()
    {
        Debug.Log("a");
        RaycastHit hit;
        Vector3 pointOfInterest = _muzzle.transform.position + _muzzle.transform.forward * 100;
        if (Physics.Raycast(_muzzle.transform.position, transform.forward * 100, out hit, 100, 8))
        {
            pointOfInterest = hit.point;
            Debug.Log("hit" + hit.point);
        }

        LineRenderer line = Instantiate(_line.gameObject).GetComponent<LineRenderer>();
        line.SetPositions(new Vector3[] { _muzzle.transform.position, pointOfInterest });
    }

    private bool CanShoot() => !gunData.reloading && _timeSinceLastShot > 1f / gunData.firerate;

    private void SimulatedShoot()
    {
        Debug.DrawRay(_muzzle.transform.position, transform.forward * 100, Color.blue, 1);
    }

    private void ShootSendNetworkRpc(HitData hit)
    {
        if (IsServer || IsHost)
        {
            ShootClientRpc();
        }
        else
        {
            ShootServerRpc(hit);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void ShootServerRpc(HitData hit, ServerRpcParams serverRpcParams = default)
    {
        if (hit.IsHit)
        {
            GameObject hitTargetGO = GetNetworkObject(hit.HitNwID).gameObject;
            hitTargetGO.GetComponent<PlayerHealth>().Damage(gunData.damage);
        }

        SimulatedShoot();

        //wont call origin client
        ulong[] ignoreClients = { serverRpcParams.Receive.SenderClientId };
        var clientIds = NetworkManager.ConnectedClientsIds.Except(ignoreClients);
        ClientRpcParams clientRpcParams = new ClientRpcParams()
        {
            Send = new ClientRpcSendParams()
            {
                TargetClientIds = clientIds.ToList(),
            }
        };
        ShootClientRpc(clientRpcParams);
    }

    [ClientRpc]
    private void ShootClientRpc(ClientRpcParams clientRpcParams = default)
    {
        SimulatedShoot();
    }
}