using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeSceneManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _upgradeSelect;
    [SerializeField]
    private GameObject _gunBuilder;

    [SerializeField]
    private GameObject _upgradeCardprefab;

    private int _selectCount = 3;

    private List<UpgradeOption> _upgradeOptions = new();

    void Start()
    {
        _upgradeSelect.SetActive(true);
        _gunBuilder.SetActive(false);

        for (int i = 0; i < _selectCount; i++)
        {
            var newUpgrade = UpgradeManager.GetRandomUpgrade();

            var newCard = Instantiate(_upgradeCardprefab);
            newCard.transform.SetParent(_upgradeSelect.transform);
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

        _upgradeSelect.SetActive(false);
        _gunBuilder.SetActive(true);
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
