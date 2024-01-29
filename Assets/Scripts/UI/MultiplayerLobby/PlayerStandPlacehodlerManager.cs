using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerStandPlacehodlerManager : MonoBehaviour
{
    [SerializeField]
    private PlayerStand _playerstandPrefab;

    public void RerenderCurrentPlayers(ulong[] playerNwIds)
    {
        foreach (Transform child in transform)
        {
            Tools.DestroyWithChildren(child.gameObject);
        }

        for (int i = 0; i < playerNwIds.Length; i++)
        {
            int row = (int)Mathf.Ceil((float)i / 2);
            int sideLROffset = (int)Mathf.Pow(-1, i) * row;

            Vector3 pos = new(sideLROffset * 5, -1, row * 4);

            var go = Instantiate(_playerstandPrefab.gameObject);
            go.transform.SetParent(transform, false);
            go.transform.localPosition = pos;

            var playerManager = NetworkManager.Singleton.ConnectedClients[(ulong)i].PlayerObject.GetComponent<PlayerManager>();

            go.GetComponent<PlayerStand>().SetNickname(playerManager.PlayerName.Value.ToString());

            if (playerManager.IsLocalPlayer)
            {
                go.GetComponent<PlayerStand>().SetNameVisibility(false);
            }
        }
    }
}
