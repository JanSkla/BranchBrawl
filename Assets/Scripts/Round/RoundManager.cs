using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoundManager : NetworkBehaviour
{
    private InGameUI _inGameUI;

    [SerializeField]
    private GameObject playAgainBtn;

    private int _alivePlayerCount;
    public int AlivePlayerCount
    {
        get { return _alivePlayerCount; }
        set
        {
            Debug.Log(value + " alive");
            _alivePlayerCount = value;
            if (_alivePlayerCount <= 1)
            {
                State = RoundState.Over;
            }
        }
    }

    private RoundState _state;
    public RoundState State
    {
        get { return _state; }
        set
        {
            _state = value;
            switch (value)
            {
                case RoundState.Running:
                    GameSetRunning();
                    return;
                case RoundState.Over:
                    GameOver();
                    return;
            }
        }
    }
    void Start()
    {
        StartGame();
    }
    private void GameSetRunning()
    {
        _inGameUI.UpdateGameScreen(false);
    }
    private void GameOver()
    {
        _inGameUI.UpdateGameScreen(true);
        playAgainBtn.GetComponent<NetworkSuccessBtn>().Fulfilled += PlayAgain;
    }
    private void StartGame()
    {
        if (NetworkManager.IsServer || NetworkManager.IsHost)
        {
            foreach (var client in NetworkManager.Singleton.ConnectedClients)
            {
                client.Value.PlayerObject.GetComponent<PlayerManager>().SpawnPlayerObject();
            }
            _alivePlayerCount = NetworkManager.Singleton.ConnectedClientsIds.Count;
            SetAlivePlayerCountClientRpc(AlivePlayerCount);
        }
        _inGameUI = GameObject.Find("InGameUI").GetComponent<InGameUI>();

        State = RoundState.Running;
    }
    public void PlayAgain()
    {
        if (NetworkManager.IsServer)
        {
            foreach (var client in NetworkManager.Singleton.ConnectedClients)
            {
                client.Value.PlayerObject.GetComponent<PlayerManager>().DespawnPlayerObject();
            }
            GameObject.Find("GameManager(Clone)").GetComponent<GameManager>().CurrentRoundFinished();
        }
    }
    [ClientRpc]
    private void SetAlivePlayerCountClientRpc(int alivePlayerCount)
    {
        _alivePlayerCount = alivePlayerCount;
        Debug.Log(AlivePlayerCount + " Alive");
    }
}
public enum RoundState
{
    Running,
    Over
}
