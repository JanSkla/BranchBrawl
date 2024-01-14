using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Background : MonoBehaviour, ISelectHandler
{
    [SerializeField]
    private HostJoinMenu _hostJoin;
    public void OnSelect(BaseEventData eventData)
    {
        _hostJoin.gameObject.SetActive(false);
    }
}
