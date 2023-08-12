using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class StickGenerator : MonoBehaviour
{
    [SerializeField]
    private GameObject _stickPrefab;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GenerateStick();
        }
    }

    private void GenerateStick()
    {
        for (int z = -5; z < 5; z++)
        {
            for (int x = -5; x < 5; x++)
            {
                GameObject stick = Instantiate(_stickPrefab, new Vector3(x* 10, 0, z * 8), new Quaternion());
                stick.GetComponent<NetworkObject>().Spawn();
            }
        } 
    }
}
