using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickGenerator : MonoBehaviour
{
    [SerializeField]
    private GameObject _stickPrefab;

    // Start is called before the first frame update
    void Start()
    {
        GenerateStick();
    }

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
        for (int z = 0; z < 10; z++)
        {
            for (int x = 0; x < 10; x++)
            {
                Instantiate(_stickPrefab, new Vector3(x* 10, 0, z * 8), new Quaternion());
            }
        } 
    }
}
