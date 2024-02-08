using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class NetworkCountdownText : NetworkBehaviour
{
    [SerializeField]
    private int _timeLength;
    [SerializeField]
    private UnityEvent _doneAction;

    public void StartCountDown()
    {
        StartCoroutine(nameof(StartLocalCountdown));
    }
    IEnumerator StartLocalCountdown()
    {
        var cTxt = GetComponent<TextMeshProUGUI>();

        for (int i = 0; i < _timeLength; i++)
        {

            SendServerCountdownStateClientRpc(i);
            int timeLeft = _timeLength - i;
            switch (timeLeft)
            {
                case 1:
                    cTxt.color = Color.red;
                    break;
                case 2:
                    cTxt.color = Color.yellow;
                    break;
                default:
                    break;
            }

            cTxt.text = (timeLeft).ToString();
            yield return new WaitForSeconds(1);
        }
        cTxt.text = "0";
        _doneAction.Invoke();
        yield return null;
    }

    [ClientRpc]
    private void SendServerCountdownStateClientRpc(int i)
    {
        var cTxt = GetComponent<TextMeshProUGUI>();

        int timeLeft = _timeLength - i;
        switch (timeLeft)
        {
            case 1:
                cTxt.color = Color.red;
                break;
            case 2:
                cTxt.color = Color.yellow;
                break;
            default:
                break;
        }

        cTxt.text = (timeLeft).ToString();
    }
}