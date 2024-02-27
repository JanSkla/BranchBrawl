using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GChargeEnhancer : GEnhancer
{
    [SerializeField]
    private int _currentCharges = 0;
    [SerializeField]
    private int _requiredCharges = 4;
    [SerializeField]
    private GameObject[] _childModelStates;
    public GChargeEnhancer()
    {
        Destiny = new GDestiny[1];
    }
    public override void Shoot(ShootData shot, Player owner)
    {
        _childModelStates[_currentCharges].SetActive(false);
        _currentCharges++;
        _childModelStates[_currentCharges].SetActive(true);
        Debug.Log("charging " + _currentCharges + " " + _requiredCharges);

        if (_currentCharges >= _requiredCharges)
        {
            shot.Amount *= _requiredCharges;
            Destiny[0].Part.Shoot(shot, owner);
            _currentCharges = 0;

            Debug.Log("afterSHOT " + _currentCharges + " " + _requiredCharges);
        }
    }
}
