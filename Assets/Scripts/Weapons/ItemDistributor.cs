using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ItemDistributor : MonoBehaviour
{
    [SerializeField]
    GameObject _itemToSpawn;
    [SerializeField]
    Texture2D _itemSpawnHeatmap;
    [SerializeField]
    Transform _bottomLeft;
    [SerializeField]
    Transform _topRight;

    private void Start()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            var gameManager = GameObject.Find("GameManager(Clone)").GetComponent<GameManager>();
            if (gameManager)
            {
                if (gameManager.RoundsList[gameManager.CurrentRoundListIndex] == (int)RoundType.FirstCombat)
                    SpawnItems();
            }
        }
    }
    public void SpawnItems()
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
                var obj = Instantiate(_itemToSpawn);
                obj.transform.position = new(hit.point.x, hit.point.y + 1, hit.point.z);
                obj.transform.rotation = Random.rotation;

                obj.GetComponent<NetworkObject>().Spawn();
                obj.GetComponent<NetworkObject>().DestroyWithScene = true;
            }
        }
    }
}
