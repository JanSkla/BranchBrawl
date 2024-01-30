using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class GMuzzle : GPart
{
    [SerializeField]
    private GameObject _muzzle;
    [SerializeField]
    private LineRenderer _line;
    public override void Shoot(ShootData shot) //TODO precalculateShot
    {
        if (!_muzzle) Debug.LogError("No muzzle assigned");
        Debug.Log("Shoots successfully");

        HitData hitData = new();

        Vector3 playerCameraPos = NetworkManager.LocalClient.PlayerObject.GetComponent<PlayerManager>().PlayerObject.GetComponent<PlayerCamera>().FpsCam.transform.position;

        if (Physics.Raycast(playerCameraPos, _muzzle.transform.forward, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Player")))
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
                hitTarget.GetComponent<PlayerHealth>().Damage(shot.Amount);
            }
        }
        else
        {
            hitData.IsHit = false;
            Debug.DrawRay(playerCameraPos, transform.forward * 100, Color.red, 1);
        }
        Debug.Log("Hit" + hitData.IsHit);
        ShootSendNetworkRpc(shot, hitData);
        ShotVisual();
    }
    private void ShootSendNetworkRpc(ShootData shootData, HitData hit)
    {
        if (IsServer || IsHost)
        {
            ShootClientRpc();
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
    private void SimulatedShoot()
    {
        Debug.DrawRay(_muzzle.transform.position, transform.forward * 100, Color.blue, 1);
        ShotVisual();
    }
    private void ShotVisual()
    {
        Debug.Log("a");
        RaycastHit hit;
        Vector3 pointOfInterest = Vector3.zero;
        if (Physics.Raycast(_muzzle.transform.position, transform.forward * 100, out hit, 1))
        {
            pointOfInterest = hit.transform.InverseTransformPoint(hit.point);
        }

        LineRenderer line = Instantiate(_line.gameObject).GetComponent<LineRenderer>();
        line.SetPositions(new Vector3[] { _muzzle.transform.position, pointOfInterest });
    }

    public void ReplaceMuzzle(UpgradeWithPart guPrefab, bool isNetwork = false)
    {
        Debug.Log("ReplaceMuzzle");
        GUpgrade gu = guPrefab.InstantiatePrefab();

        GPoint gp = transform.parent.GetComponent<GPoint>();

        GameObject parentparentGO = gp.Parent;

        GDestiny parentGDestRef;

        if (parentparentGO.GetComponent<GUpgrade>())
        {
            int di = gp.DestinyIndex;

            parentGDestRef = parentparentGO.GetComponent<GUpgrade>().Destiny[di];
        }
        else if (parentparentGO.GetComponent<GBase>())
        {
            parentGDestRef = parentparentGO.GetComponent<GBase>().Destiny;
        }
        else
        {
            Debug.Log("There is no GUpgrade nor GBase");
            return;
        }

        if (isNetwork)
        {
            gu.NetworkObject.Spawn();
            gu.NetworkObject.TrySetParent(parentGDestRef.PositionPoint.transform, false); //TODO
        }
        else
        {
            gu.NetworkObject.AutoObjectParentSync = false;
            gu.transform.SetParent(parentGDestRef.PositionPoint.transform, false);
        }
        parentGDestRef.Part = gu;

        int guDLength = gu.Destiny.Length;

        for (int i = 0; i < guDLength; i++) //spawn muzzle for new endings
        {
            if (isNetwork)
                PlayerGunManager.NetworkMuzzleInstantiateOnDestiny(gu.Destiny[i]);
            else
                PlayerGunManager.MuzzleInstantiateOnDestiny(gu.Destiny[i]);
        }

        DestroyPartRecursive();
    }
}
