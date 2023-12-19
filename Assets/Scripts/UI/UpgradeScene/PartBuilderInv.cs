using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class PartBuilderInv : MonoBehaviour
{
    [SerializeField]
    private GameObject _listObject;
    private GameObject _childPrefab;

    void Start()
    {
        _childPrefab = Resources.Load("Prefabs/GunUpgrades/PartBuilderInv/PartBuilderInvChild") as GameObject;
    }

    private void OnEnable()
    {
        UpdateList();
    }

    private void UpdateList()
    {
        foreach (Transform child in _listObject.transform)
        {
            DestroyRecursive(child);
        }
        var localPLayerGUInv = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>().PlayerGunManager.GUpgradeInv;

        foreach (var listObj in localPLayerGUInv)
        {
            var newObj = Instantiate(_childPrefab);
            newObj.transform.SetParent(_listObject.transform);
            PartBuilderInvChild p = newObj.GetComponent<PartBuilderInvChild>();
            p.Count.text = (listObj.TotalCount - listObj.UsedCount).ToString() + "x";
            p.NamePlaceholder.text = listObj.PrefabResource;
        }
    }
    private void DestroyRecursive(Transform obj)
    {
        Destroy(obj.gameObject);
        foreach (Transform child in obj.transform)
        {
            DestroyRecursive(child);
        }
    }
}
