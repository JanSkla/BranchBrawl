using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GChargeEnhancer : GEnhancer
{
    [SerializeField]
    private int _currentCharges = 0;
    [SerializeField]
    private int _requiredCharges = 4;
    //[SerializeField]
    //private GameObject[] _childModelStates;
    [SerializeField]
    private Animator _animatorL;
    [SerializeField]
    private Animator _animatorR;
    [SerializeField]
    private ParticleSystem _chargeEffect;
    public GChargeEnhancer()
    {
        Destiny = new GDestiny[1];
    }
    private void Start()
    {
        _chargeEffect.Stop();
    }
    public override void Shoot(ShootData shot, Player owner)
    {
        //_childModelStates[_currentCharges].SetActive(false);
        _currentCharges++;
        _animatorL.SetInteger("charges", _currentCharges);
        _animatorR.SetInteger("charges", _currentCharges);
        //_childModelStates[_currentCharges].SetActive(true);
        Debug.Log("charging " + _currentCharges + " " + _requiredCharges);

        if (_currentCharges == _requiredCharges)
        {
            _chargeEffect.Play();
        }

        if (_currentCharges > _requiredCharges)
        {
            _chargeEffect.Stop();
            shot.Amount *= _requiredCharges;
            Destiny[0].Part.Shoot(shot, owner);
            _currentCharges = 0;
            _animatorL.SetInteger("charges", _currentCharges);
            _animatorR.SetInteger("charges", _currentCharges);

            Debug.Log("afterSHOT " + _currentCharges + " " + _requiredCharges);
        }
    }
}
