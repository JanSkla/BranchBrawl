using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class PartBuilderInv : MonoBehaviour
{
    public PartBuilderInvChild Selected; //NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>().PlayerGunManager.FindGUpgradeDataByUpId();

    [SerializeField]
    private GameObject _listObject;
    [SerializeField]
    private GameObject _childPrefab;

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

            p.PartBuilderInv = this;
            p.SetGUpgradeData(listObj);
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
