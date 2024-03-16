using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class GBasicMuzzle : GMuzzle
{
    [SerializeField]
    private GameObject _muzzle;
    [SerializeField]
    private LineRenderer _line;
    public override void Shoot(ShootData shot, Player owner) //TODO precalculateShot
    {
        if (!_muzzle) Debug.LogError("No muzzle assigned");

        HitData hitData = new();

        //Vector3 playerCameraPos = NetworkManager.LocalClient.PlayerObject.GetComponent<PlayerManager>().PlayerObject.GetComponent<PlayerCamera>().FpsCam.transform.position;

        if (Physics.Raycast(_muzzle.transform.position, _muzzle.transform.forward, out RaycastHit hit, Mathf.Infinity, ~(1 << LayerMask.NameToLayer("LocalPlayer"))))
        {
            hitData.IsHit = true;
            GameObject hitTarget = hit.collider.gameObject;

            while (hitTarget.transform.parent != null)
            {
                hitTarget = hitTarget.transform.parent.gameObject;
            }

            if (hitTarget.GetComponent<PlayerHealth>())
            {
                hitData.HitNwID = hitTarget.GetComponent<NetworkObject>().NetworkObjectId;
                Debug.DrawRay(_muzzle.transform.position, transform.forward * hit.distance, Color.green, 1);

                if (IsServer)
                {
                    hitTarget.GetComponent<PlayerHealth>().Damage(shot.Amount);
                }
            }
        }
        else
        {
            hitData.IsHit = false;
            Debug.DrawRay(_muzzle.transform.position, transform.forward * 100, Color.red, 1);
        }
        ShootSendNetworkRpc(shot, hitData);
        ShotVisual(shot);
    }
    private void ShootSendNetworkRpc(ShootData shootData, HitData hit)
    {
        if (IsServer || IsHost)
        {
            ShootClientRpc(shootData);
        }
        else
        {
            ShootServerRpc(shootData, hit);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void ShootServerRpc(ShootData shootData, HitData hit, ServerRpcParams serverRpcParams = default)
    {
        if (hit.IsHit)
        {
            GameObject hitTargetGO = GetNetworkObject(hit.HitNwID).gameObject;
            hitTargetGO.GetComponent<PlayerHealth>().Damage(shootData.Amount);
        }

        SimulatedShoot(shootData);

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
        ShootClientRpc(shootData, clientRpcParams);
    }

    [ClientRpc]
    private void ShootClientRpc(ShootData shootData, ClientRpcParams clientRpcParams = default)
    {
        SimulatedShoot(shootData);
    }
    private void SimulatedShoot(ShootData shootData)
    {
        Debug.DrawRay(_muzzle.transform.position, transform.forward * 100, Color.blue, 1);
        ShotVisual(shootData);
    }
    private void ShotVisual(ShootData shootData)
    {
        RaycastHit hit;
        Vector3 pointOfInterest = _muzzle.transform.position + _muzzle.transform.forward * 100;

        if (Physics.Raycast(_muzzle.transform.position, transform.forward * 100, out hit, 100))
        {
            pointOfInterest = hit.point;
            Debug.Log("hit" + hit.point);
        }

        LineRenderer line = Instantiate(_line.gameObject).GetComponent<LineRenderer>();
        line.startColor = new(shootData.Amount / 100, 1, 1);
        line.SetPositions(new Vector3[] { _muzzle.transform.position, pointOfInterest });
    }
}
