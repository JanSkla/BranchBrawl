using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeSceneManager : NetworkBehaviour
{
    [SerializeField]
    private GameObject _upgradeSelect;
    [SerializeField]
    private GameObject _upgradeSelectContainer;
    [SerializeField]
    private GameObject _gunBuilder;
    [SerializeField]
    public GunPlaceholder GunPlaceholder;

    private static readonly int _selectCount = 3;

    private bool _optionSelected = false;

    private UpgradeOption[] _upgradeOptions = new UpgradeOption[_selectCount];

    void Start()
    {
        _upgradeSelect.SetActive(true);

        for (int i = 0; i < _selectCount; i++)
        {
            var newUpgrade = UpgradeManager.GetRandomUpgrade();

            var newCard = newUpgrade.InstantiateSelectionCard(UpgradeSelected, i);
            newCard.transform.SetParent(_upgradeSelectContainer.transform, false);
            //newCard.Button.onClick.AddListener(() => UpgradeSelected(newUpgrade.Id));

            var newOption = new UpgradeOption(newCard.gameObject, newUpgrade.Id);
            _upgradeOptions[i] = newOption;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void UpgradeSelected(int idx)
    {
        if (_optionSelected) return;
        _optionSelected = true;
        for (int i = 0; i < _upgradeOptions.Length; i++)
        {
            if (i != idx)
            {
                _upgradeOptions[i].UpgradeCard.GetComponent<Button>().interactable = false;
            }
        }

        var upgrade = UpgradeManager.GetUpgradeById(_upgradeOptions[idx].UpgradeId);
        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<UpgradeManager>().AddUpgrade(upgrade);


        //_upgradeSelect.SetActive(false);
        _gunBuilder.SetActive(true);
        GunPlaceholder.PartBuilderInv.UpdateList();

        Invoke(nameof(DelayedUpgradeSelectDisable), 1);
    }
    private void DelayedUpgradeSelectDisable()
    {
        _upgradeSelect.SetActive(false);
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
