using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoundDisplay : MonoBehaviour
{
    [SerializeField]
    private GameObject _roundIconPrefab;

    [SerializeField]
    private Sprite _fcrIcon;
    [SerializeField]
    private Sprite _uIcon;
    [SerializeField]
    private Sprite _crIcon;
    [SerializeField]
    private Sprite _dotdotdotIcon;

    [SerializeField]
    private Image _current;
    [SerializeField]
    private TextMeshProUGUI _currentText;

    [SerializeField]
    private HorizontalLayoutGroup _left;
    [SerializeField]
    private HorizontalLayoutGroup _right;

    private GameManager _gameManager;

    private int preoffset = 3;

    void Start()
    {
        _gameManager = GameObject.Find("GameManager(Clone)").GetComponent<GameManager>();

        for (int i = 0; i < _gameManager.RoundsList.Count; i++)
        {
            
            if(i < _gameManager.CurrentRoundListIndex - preoffset)
            {
                i = _gameManager.CurrentRoundListIndex - preoffset;
            }
            if(i == _gameManager.CurrentRoundListIndex - preoffset)
            {
                i = _gameManager.CurrentRoundListIndex - preoffset + 1;
                GameObject image = Instantiate(_roundIconPrefab);
                image.transform.SetParent(_left.transform);
                image.GetComponent<Image>().sprite = _dotdotdotIcon;
            }

            Sprite sprite = DelegateSprite((RoundType)_gameManager.RoundsList[i]);

            if (i > _gameManager.CurrentRoundListIndex)
            {
                GameObject image = Instantiate(_roundIconPrefab);
                image.transform.SetParent(_right.transform);
                image.GetComponent<Image>().sprite = sprite;
            }
            else if(i == _gameManager.CurrentRoundListIndex)
            {
                _current.sprite = sprite;
                _currentText.text = sprite.name;
            }
            else
            {
                GameObject image = Instantiate(_roundIconPrefab);
                image.transform.SetParent(_left.transform);
                image.GetComponent<Image>().sprite = sprite;
            }
        }
    }

    private Sprite DelegateSprite(RoundType rtype)
    {
        switch (rtype)
        {
            case RoundType.FirstCombat:
                return _fcrIcon;
            case RoundType.Upgrade:
                return _uIcon;
            case RoundType.Combat:
                return _crIcon;
            default:
                return _crIcon;
        }
    }
}
