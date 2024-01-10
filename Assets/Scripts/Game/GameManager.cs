
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    private int _goalCrowns = 3;

    private NetworkList<int> _roundsList = new();

    public NetworkList<PlayerGameData> PlayersGameData = new();

    private int _currentRoundListIndex = 0;
    private bool _currentRoundActive = false;
    void Awake()
    {
        if (NetworkManager.IsServer)
        {
            GetComponent<NetworkObject>().Spawn(false);
        }

        StartGame();
    }
    public void StartGame()
    {
        if (!NetworkManager.IsServer) return;

        var nwClients = NetworkManager.Singleton.ConnectedClients;

        for (int i = 0; i < nwClients.Count; i++)
        {
            nwClients[(ulong)i].PlayerObject.GetComponent<PlayerManager>().SpawnPlayerGunManager();
            PlayersGameData.Add(new PlayerGameData()
            {
                PMNwId = nwClients[(ulong)i].PlayerObject.GetComponent<NetworkObject>().NetworkObjectId,
                Crowns = 0
            });
        }

        _roundsList.Add((int)RoundType.Upgrade); //REMOVE
        _roundsList.Add((int)RoundType.FirstCombat);
        _roundsList.Add((int)RoundType.Upgrade);
        _roundsList.Add((int)RoundType.Combat);
        StartCurrentRound();
    }
    private void StartCurrentRound()
    {
        if (!NetworkManager.IsServer) return;

        bool isWin = false;

        foreach (var PGD in PlayersGameData)
        {
            if (PGD.Crowns == _goalCrowns)
            {
                LoadWinnerScene();
                isWin = true;
            }
        }

        if (isWin) return;

        if (_currentRoundActive) return;
        RoundType currentType = (RoundType)_roundsList[_currentRoundListIndex];
        switch (currentType)
        {
            case RoundType.FirstCombat:
                StartCombatRound();
                break;
            case RoundType.Combat:
                StartCombatRound();
                _roundsList.Add((int)RoundType.Upgrade);
                _roundsList.Add((int)RoundType.Combat);
                break;
            case RoundType.Upgrade:
                StartUpgradeRound();
                break;
        }
    }
    private void StartCombatRound()
    {
        if (!NetworkManager.IsServer) return;

        NetworkManager.Singleton.SceneManager.LoadScene("CombatRound", LoadSceneMode.Single);
    }
    private void StartUpgradeRound()
    {
        if (!NetworkManager.IsServer) return;

        NetworkManager.Singleton.SceneManager.LoadScene("UpgradeRound", LoadSceneMode.Single);
    }
    private void LoadWinnerScene()
    {
        if (!NetworkManager.IsServer) return;

        NetworkManager.Singleton.SceneManager.LoadScene("WinnerScene", LoadSceneMode.Single);
    }
    public void CurrentRoundStarted()
    {
        if (!NetworkManager.IsServer) return;

        _currentRoundActive = true;
    }
    public void CurrentRoundFinished()
    {
        if (!NetworkManager.IsServer) return;

        _currentRoundActive = false;
        _currentRoundListIndex++;
        if (_roundsList.Count < _currentRoundListIndex) return;
        StartCurrentRound();
    }

    public void NewGameCleanup()
    {
        if (!NetworkManager.IsServer) return;

        var nwClients = NetworkManager.Singleton.ConnectedClients;

        for (int i = 0; i < nwClients.Count; i++)
        {
            nwClients[(ulong)i].PlayerObject.GetComponent<PlayerManager>().DespawnPlayerGunManager();
        }
    }
}
public enum RoundType
{
    FirstCombat = 0,
    Combat = 1,
    Upgrade = 2
}