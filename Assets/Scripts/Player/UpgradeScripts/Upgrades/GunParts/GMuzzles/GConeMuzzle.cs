using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class GConeMuzzle : GMuzzle
{
    [SerializeField]
    private GameObject _muzzle;
    [SerializeField]
    private Projectile _projectilePrefab;

    [SerializeField]
    private GameObject _shotIndicator;

    private void Start()
    {
        _shotIndicator.SetActive(false);
    }

    public override void Shoot(ShootData shot, Player owner) //TODO precalculateShot
    {
        if (!_muzzle) Debug.LogError("No muzzle assigned");
        Debug.Log("Shoots successfully");

        Projectile projectile = Instantiate(_projectilePrefab.gameObject).GetComponent<Projectile>();
        projectile.transform.position = _muzzle.transform.position;
        projectile.transform.rotation = _muzzle.transform.rotation;

        projectile.GetComponent<NetworkObject>().Spawn();
        projectile.Owner = owner;
        projectile.DamageAmount = shot.Amount;
        projectile.ApplyForce(_muzzle.transform.forward);

        ShootSendNetworkRpc(shot);
        ShotVisual(shot);
    }
    private void ShootSendNetworkRpc(ShootData shootData)
    {
        if (IsServer || IsHost)
        {
            ShootClientRpc(shootData);
        }
        else
        {
            ShootServerRpc(shootData);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void ShootServerRpc(ShootData shootData, ServerRpcParams serverRpcParams = default)
    {
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
        ShotVisual(shootData);
    }
    private void ShotVisual(ShootData shootData)
    {
        _shotIndicator.SetActive(true);
        StartCoroutine(nameof(HideShotVisual), 1);
        Debug.Log("shoootts");
    }
    private void HideShotVisual()
    {
        if(_shotIndicator.activeSelf)
            _shotIndicator.SetActive(false);

    }
}
