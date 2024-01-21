using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayButton : MonoBehaviour, ISelectHandler, IDeselectHandler
{

    [SerializeField]
    private HostJoinMenu _hostJoin;

    public void OnSelect(BaseEventData eventData)
    {
        _hostJoin.gameObject.SetActive(true);
    }
    public void OnDeselect(BaseEventData eventData)
    {
        //_hostJoin.gameObject.SetActive(false);
    }
}
