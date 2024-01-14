using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayButton : MonoBehaviour, ISelectHandler, IDeselectHandler
{

    [SerializeField]
    private GameObject _hostJoin;

    public void OnSelect(BaseEventData eventData)
    {
        _hostJoin.SetActive(true);
    }
    public void OnDeselect(BaseEventData eventData)
    {
        //_hostJoin.SetActive(false);
    }
}
