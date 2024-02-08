using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeSceneManager : NetworkBehaviour
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

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
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

    public void StartNextRound()
    {
        if (NetworkManager.IsServer)
        {
            SaveGBDClientRPC();
        }
    }


    private int _GBDsSaved = 0;

    [ClientRpc]
    public void SaveGBDClientRPC()
    {
        Debug.Log("aaaa");
        Debug.Log(GameObject.Find("GunPlaceholder"));
        Debug.Log(GameObject.Find("GunPlaceholder").transform.GetChild(0));
        GameObject gunObject = GameObject.Find("GunPlaceholder").transform.GetChild(0).gameObject;
        GBase gunBase = gunObject.GetComponent<GBase>();
        if (gunBase == null) Debug.LogError("No gbase in gunplaceholder");

        GunBaseSaveData gbd = new(gunBase);

        Debug.Log(GunBaseSaveData.ParseToText(gbd.Child));

        ConfirmSaveGBDCServerRPC(gbd);
    }
    [ServerRpc (RequireOwnership = false)]
    public void ConfirmSaveGBDCServerRPC(GunBaseSaveData gunBase, ServerRpcParams serverRpcParams = default)
    {
        NetworkManager.ConnectedClients[serverRpcParams.Receive.SenderClientId].PlayerObject.GetComponent<PlayerManager>().PlayerGunManager.GunCurrentData.Value = gunBase;
        _GBDsSaved++;

        if (_GBDsSaved < NetworkManager.Singleton.ConnectedClientsIds.Count) return;
        
        GameObject.Find("GameManager(Clone)").GetComponent<GameManager>().CurrentRoundFinished();
    }
}
