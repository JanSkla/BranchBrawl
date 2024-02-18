using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class PartBuilderInv : MonoBehaviour
{
    public PartBuilderInvChild Selected; //NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>().PlayerGunManager.FindGUpgradeDataByUpId();

    [SerializeField]
    public GunPlaceholder GunPlaceholder;

    [SerializeField]
    private GameObject _listObject;
    [SerializeField]
    private GameObject _childPrefab;

    private void OnEnable()
    {
        UpdateList();
    }

    public void UpdateList()
    {
        int selectedId = 0;
        if (Selected != null)
            selectedId = Selected.UpgradeId;
        foreach (Transform child in _listObject.transform)
        {
            DestroyRecursive(child);
        }
        var localPLayerGUInv = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>().PlayerGunManager.GUpgradeInv;

        foreach (var listObj in localPLayerGUInv)
        {
            var newObj = Instantiate(_childPrefab);
            newObj.transform.SetParent(_listObject.transform);
            newObj.transform.localScale = Vector3.one;
            PartBuilderInvChild p = newObj.GetComponent<PartBuilderInvChild>();

            p.PartBuilderInv = this;
            p.SetGUpgradeData(listObj);
            if(selectedId != 0 && p.UpgradeId == selectedId)
            {
                Selected = p;
            }
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
