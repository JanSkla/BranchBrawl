using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class Gun : NetworkBehaviour
{
    [SerializeField] private GunData gunData;
    private float timeSinceLastShot = 0f;

    void Start()
    {
    }
    void Update()
    {
        timeSinceLastShot += Time.deltaTime;
    }

    public void Shoot()
    {
        Debug.Log("tries to shoot");
        if (!CanShoot() || gunData.currentAmmo <= 0) return;

        Debug.Log(gunData.name + " Shoots");
        Debug.DrawRay(transform.position, transform.forward * 100, Color.red, 1);
        timeSinceLastShot = 0f;

        SimulatedShootSendNetworkRpc();
    }

    private bool CanShoot() => !gunData.reloading && timeSinceLastShot > 1f / gunData.firerate;

    private void SimulatedShoot()
    {
        Debug.Log(gunData.name + " Shoots //simulated");
        Debug.DrawRay(transform.position, transform.forward * 100, Color.red, 1);
    }

    private void SimulatedShootSendNetworkRpc()
    {
        if (IsServer || IsHost)
        {
            Debug.Log("IsHOst");
            SimulatedShootClientRpc();
        }
        else
        {
            Debug.Log("IsCLient");
            SimulatedShootServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SimulatedShootServerRpc(ServerRpcParams rserverRpcParams = default)
    {
        SimulatedShoot();
        ulong[] ignoreClients = { rserverRpcParams.Receive.SenderClientId };
        var clientIds = NetworkManager.ConnectedClientsIds.Except(ignoreClients);
        //doesnt read on other clients
        ClientRpcParams clientRpcParams = new ClientRpcParams()
        {
            Send = new ClientRpcSendParams()
            {
                TargetClientIds = clientIds.ToList(),
            }
        };
        SimulatedShootClientRpc(clientRpcParams);
    }

    [ClientRpc]
    private void SimulatedShootClientRpc(ClientRpcParams clientRpcParams = default)
    {
        SimulatedShoot();
    }
}