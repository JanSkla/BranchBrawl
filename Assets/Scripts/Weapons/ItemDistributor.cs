using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.VisualScripting;
using UnityEngine;

public class ItemDistributor : NetworkBehaviour
{
    [SerializeField]
    private Texture2D _itemSpawnHeatmap;
    [SerializeField]
    private Transform _bottomLeft;
    [SerializeField]
    private Transform _topRight;

    private RoundManager _roundManager;

    private void Start()
    {
        _roundManager = GameObject.Find("RoundManager").GetComponent<RoundManager>();
        _roundManager.AllPlayersLoaded += SpawnItems;
    }
    public void SpawnItems()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            var gameManager = GameObject.Find("GameManager(Clone)").GetComponent<GameManager>();
            if (gameManager)
            {
                if (gameManager.RoundsList[gameManager.CurrentRoundListIndex] == (int)RoundType.FirstCombat)
                    LocalSpawnItems();
            }
        }
    }
    private void LocalSpawnItems()
    {
        Vector2 bottomLeft = new(_bottomLeft.position.x, _bottomLeft.position.z);
        Vector2 topRight = new(_topRight.position.x, _topRight.position.z);

        var positions = FastPoissonDiskSampling.Sampling(bottomLeft, topRight, 10);

        Vector2 widthXheight = topRight - bottomLeft;

        foreach (var pos in positions)
        {
            //Spawn and distribute along texture
            Vector2 pixel = (pos-bottomLeft) / widthXheight * _itemSpawnHeatmap.width;
            Color rate = _itemSpawnHeatmap.GetPixel((int)pixel.x, (int)pixel.y);

            if(rate.r >= Random.Range(0f, 1f))
            {
                //Drop to ground lvl
                Physics.Raycast(new Vector3(pos.x, transform.position.y, pos.y), Vector3.down, out RaycastHit hit, 500, LayerMask.GetMask("Terrain"));

                SpawnPickableGunObject(new(hit.point.x, hit.point.y + 1, hit.point.z));

                //var obj = Instantiate(_itemToSpawn);
                //obj.transform.position = new(hit.point.x, hit.point.y + 1, hit.point.z);
                //obj.transform.rotation = Random.rotation;

                //obj.GetComponent<NetworkObject>().Spawn();
                //obj.GetComponent<NetworkObject>().DestroyWithScene = true;
            }
        }
    }

    private void SpawnPickableGunObject(Vector3 spawnPos)
    {
        string gunSpawnString = "";

        if(Random.Range(0,10) == 0)
        {
            gunSpawnString = "" + (int)Upgrades.G2Splitter + "{,}";
        }

        GunBaseSaveData gunScheme = new(gunSpawnString);

        var gun = gunScheme.NetworkSpawn();

        gun.transform.position = spawnPos;
        gun.transform.rotation = Random.rotation;

        SpawnPickableGunObjClientRpc(gun.NetworkObjectId, spawnPos, gun.transform.rotation);

        var rb = gun.AddComponent<Rigidbody>();
        //gun.AddComponent<NetworkTransform>();
        //gun.AddComponent<NetworkRigidbody>();

        var collider = gun.AddComponent<BoxCollider>();
        collider.size = new(1f,2f,3.5f);
        collider.center = new(0f, 0f, 1f);

        gun.gameObject.layer = LayerMask.NameToLayer("Pickable");

        rb.isKinematic = false;
    }
    [ClientRpc]
    private void SpawnPickableGunObjClientRpc(ulong gunNwId, Vector3 pos, Quaternion rotation)
    {
        Debug.Log("eNwId:" + gunNwId);
        if (NetworkManager.Singleton.IsHost) return;

        Debug.Log("NwId:" + gunNwId);

        var gun = NetworkManager.Singleton.SpawnManager.SpawnedObjects[gunNwId];
        gun.transform.position = pos;
        gun.transform.rotation = rotation;

        var rb = gun.AddComponent<Rigidbody>();

        var collider = gun.AddComponent<BoxCollider>();
        collider.size = new(1f, 2f, 3.5f);
        collider.center = new(0f, 0f, 1f);

        gun.gameObject.layer = LayerMask.NameToLayer("Pickable");

        rb.isKinematic = false;
    }
}
