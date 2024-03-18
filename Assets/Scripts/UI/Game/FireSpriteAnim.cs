using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FireSpriteAnim : MonoBehaviour
{
    [SerializeField]
    private Image _image;

    [SerializeField]
    private float _speed;

    [SerializeField]
    private Sprite[] _sprites;

    private float _totalDeltaTime = 0;
    private int _frame = 0;

    private void Update()
    {
        _totalDeltaTime += Time.deltaTime;

        if(_totalDeltaTime > _speed)
        {
            _totalDeltaTime -= _speed;
            _frame++;
            _image.sprite = _sprites[_frame % _sprites.Length];
        }
    }
}
