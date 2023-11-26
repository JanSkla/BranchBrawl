
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    public NetworkList<int> RoundsList = new NetworkList<int>();
    private int _currentRoundListIndex = 0;
    private bool _currentRoundActive = false;
    void Awake()
    {
        if (NetworkManager.IsServer)
        {
            GetComponent<NetworkObject>().Spawn(false);
        }
        //DontDestroyOnLoad(this.gameObject);
    }
    public void StartGame()
    {
        if (!NetworkManager.IsServer) return;

        //RoundsList = new NetworkList<int>();

        RoundsList.Add((int)RoundType.FirstCombat);
        //RoundsList.Add((int)RoundType.Upgrade);
        RoundsList.Add((int)RoundType.Combat);
        //RoundsList.Add((int)RoundType.Upgrade);
        RoundsList.Add((int)RoundType.Combat);
        StartCurrentRound();
    }
    private void StartCurrentRound()
    {
        if (!NetworkManager.IsServer) return;

        if (_currentRoundActive) return;
        RoundType currentType = (RoundType)RoundsList[_currentRoundListIndex];
        switch (currentType)
        {
            case RoundType.FirstCombat:
                StartCombatRound();
                break;
            case RoundType.Combat:
                StartCombatRound();
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
        if (RoundsList.Count < _currentRoundListIndex) return;
        StartCurrentRound();
    }
}
public enum RoundType
{
    FirstCombat = 0,
    Combat = 1,
    Upgrade = 2
}