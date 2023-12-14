using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeSceneManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _upgradesContainer;

    [SerializeField]
    private GameObject _upgradeCardprefab;

    private int _selectCount = 3;

    private List<UpgradeOption> _upgradeOptions = new();

    void Start()
    {
        for (int i = 0; i < _selectCount; i++)
        {
            var newUpgrade = UpgradeManager.GetRandomUpgrade();

            var newCard = Instantiate(_upgradeCardprefab);
            newCard.transform.SetParent(_upgradesContainer.transform);
            newCard.GetComponent<Button>().onClick.AddListener(() => UpgradeSelected(newUpgrade.Id));

            var newOption = new UpgradeOption(newCard, newUpgrade.Id);
            _upgradeOptions.Add(newOption);
        }
    }

    private void UpgradeSelected(int id)
    {
        var upgrade = UpgradeManager.GetUpgradeById(id);
        Debug.Log("Selected" + upgrade.Id + ":" + id);
        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<UpgradeManager>().AddUpgrade(upgrade);

        _upgradesContainer.SetActive(false);
    }

    private struct UpgradeOption
    {
        public GameObject UpgradeCard;
        public int UpgradeId;

        public UpgradeOption(GameObject uc, int uId)
        {
            UpgradeCard = uc;
            UpgradeId = uId;
        }
    }

    
}
