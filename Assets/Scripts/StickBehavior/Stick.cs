using Microsoft.Win32.SafeHandles;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Unity.Burst.CompilerServices;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class Stick : NetworkBehaviour
{
    [SerializeField]
    private GameObject _stickPartPrefab;

    private List<GameObject> _stickParts = new List<GameObject>();
    private List<Vector3> _verticles = new List<Vector3>();

    //Stick generation specs
    [SerializeField]
    private float _minInitialLength = 1.0f;
    [SerializeField]
    private float _maxInitialLength = 3.0f;
    [SerializeField]
    private float _minInitialWidth = 1.0f;
    [SerializeField]
    private float _maxInitialWidth = 3.0f;
    [SerializeField]
    private float _lengthDownfall = 0.6f;
    [SerializeField]
    private float _widthDownfall = 0.6f;
    [SerializeField]
    private float _extraBranchWidthDownfall = 0.3f;
    [SerializeField]
    private int _maximalStickLevel = 11;

    private enum BranchSplit
    {
        End,
        Main,
        MainMain,
        MainExtra,
        MainExtraExtra
    }

    private enum BranchType
    {
        Main,
        Extra
    }
    
    //Stick genereation util props
    private int i;

    // Start is called before the first frame update
    public override void OnNetworkSpawn()
    {
        if (NetworkManager.IsServer)
        {
            GenerateStickParts();
        }
    }

    private void GenerateStickParts() //generates first stick part then starts recursion
    {
        float width = Random.Range(_minInitialLength, _maxInitialLength);
        float length = Random.Range(_minInitialLength, _maxInitialLength);

        _verticles.Add(new Vector3(0, 0, 0));
        Vector3 newVerticle = new Vector3(0, 0, length);
        _verticles.Add(newVerticle);

        GameObject newStickPart = Instantiate(_stickPartPrefab, transform.position + Vector3.Lerp(_verticles.ElementAt(0), _verticles.ElementAt(1), 0.5f), Quaternion.LookRotation(newVerticle));
        newStickPart.transform.localScale = new Vector3(newStickPart.transform.localScale.x * width, newStickPart.transform.localScale.y * width, newStickPart.transform.localScale.z * length);
        //newStickPart.transform.parent = transform;
        newStickPart.GetComponent<NetworkObject>().Spawn();
        newStickPart.GetComponent<NetworkObject>().TrySetParent(GetComponent<NetworkObject>());
        _stickParts.Add(newStickPart);

        i = 1;
        int level = 1;

        int previousVerticleIndex = i;
        switch (selectSplitOption(1))
        {
            case BranchSplit.Main:
                NewStickPartRecursive(level, previousVerticleIndex, length, width);
                break;
            case BranchSplit.MainMain:
                NewStickPartRecursive(level + 1, previousVerticleIndex, length, width);
                NewStickPartRecursive(level + 1, previousVerticleIndex, length, width);
                break;
            case BranchSplit.MainExtra:
                NewStickPartRecursive(level + 1, previousVerticleIndex, length, width);
                NewStickPartRecursive(level + 1, previousVerticleIndex, length / 2, width, BranchType.Extra);
                break;
            case BranchSplit.MainExtraExtra:
                NewStickPartRecursive(level + 1, previousVerticleIndex, length, width);
                NewStickPartRecursive(level + 1, previousVerticleIndex, length / 2, width, BranchType.Extra);
                NewStickPartRecursive(level + 1, previousVerticleIndex, length / 2, width, BranchType.Extra);
                break;
            case BranchSplit.End:
                break;
        }

        //int i = 1;
        //for (int level = 1; level < _maximalStickLevel; level++)
        //{

        //    Vector3 previousVerticle = _verticles.ElementAt(i);
        //    for (int j = 0; j < 2; j++)
        //    {
        //        length *= _lengthDownfall;
        //        float angleTheta = Random.Range(-_maximalJointRadian, _maximalJointRadian) * Mathf.PI;
        //        Debug.Log(angleTheta);
        //        float anglePhi = (Random.Range(-_maximalJointRadian, _maximalJointRadian) + 0.5f) * Mathf.PI;
        //        Debug.Log(anglePhi);

        //        float x = Mathf.Cos(angleTheta) * Mathf.Sin(anglePhi);
        //        float y = Mathf.Sin(angleTheta) * Mathf.Cos(anglePhi);
        //        float z = Mathf.Cos(anglePhi);

        //        newVerticle = previousVerticle + new Vector3(x, y, z).normalized * length;

        //        _verticles.Add(newVerticle);

        //        newStickPart = Instantiate(_stickPartPrefab, Vector3.Lerp(previousVerticle, newVerticle, 0.5f), Quaternion.LookRotation(previousVerticle - newVerticle));
        //        newStickPart.transform.localScale = newStickPart.transform.localScale * length;
        //        newStickPart.transform.parent = transform;
        //        _stickParts.Add(newStickPart);
        //        i++;
        //    }
        //}
    }
    //final

    private void NewStickPartRecursive(int level, int previousVerticleIndex, float length, float width, BranchType branchType = BranchType.Main)
    {
        if (level > _maximalStickLevel) return;

        width *= (branchType == BranchType.Main ? _widthDownfall : _extraBranchWidthDownfall);
        if (width < 0.2f) return;

        length *= _lengthDownfall;

        Vector3 previousVerticle = _verticles[previousVerticleIndex];

        Vector3 previousPart = _stickParts.ElementAt(previousVerticleIndex - 1).transform.eulerAngles;

        float alpha = (branchType == BranchType.Main ? Random.Range(0.0f, 50.0f) : Random.Range(20.0f, 100.0f));  //alpha angle
        float o = length * Mathf.Cos(alpha * Mathf.PI / 180); 
        float r = length * Mathf.Sin(alpha * Mathf.PI / 180);

        Vector3 partPos = Quaternion.Euler(previousPart.x, previousPart.y, previousPart.z) * Vector3.forward * o;
        partPos = Quaternion.Euler(previousPart.x, previousPart.y, previousPart.z) * Quaternion.Euler(Random.Range(0, 360), 90, 0) * Vector3.forward * r + partPos;

        Vector3 newVerticle = previousVerticle + Quaternion.Euler(0, 0, 0) * partPos;

        _verticles.Add(newVerticle);

        GameObject newStickPart = Instantiate(_stickPartPrefab, transform.position + Vector3.Lerp(previousVerticle, newVerticle, 0.5f), Quaternion.LookRotation(newVerticle - previousVerticle));
        newStickPart.transform.localScale = new Vector3(newStickPart.transform.localScale.x * width, newStickPart.transform.localScale.y * width, newStickPart.transform.localScale.z * length);
        //newStickPart.transform.SetParent(transform);
        newStickPart.GetComponent<NetworkObject>().Spawn();
        newStickPart.GetComponent<NetworkObject>().TrySetParent(GetComponent<NetworkObject>());
        _stickParts.Add(newStickPart);

        GameObject prevObject = _stickParts.ElementAt(previousVerticleIndex - 1);

        ulong newNwId = newStickPart.GetComponent<NetworkObject>().NetworkObjectId;
        newStickPart.GetComponent<StickPart>().ConnectedEdgeNwIds.Add(prevObject.GetComponent<NetworkObject>().NetworkObjectId);

        for (int i = 0; i < prevObject.GetComponent<StickPart>().ConnectedEdgeNwIds.Count; i++)
        {
            ulong nwId = prevObject.GetComponent<StickPart>().ConnectedEdgeNwIds[i];
            if (nwId > prevObject.GetComponent<NetworkObject>().NetworkObjectId) //higher ids than newId are children of previous stick
            {
                GameObject adjescentObject = GetNetworkObject(nwId).gameObject;
                newStickPart.GetComponent<StickPart>().ConnectedEdgeNwIds.Add(nwId);
                adjescentObject.GetComponent<StickPart>().ConnectedEdgeNwIds.Add(newNwId);
            }
        }

        prevObject.GetComponent<StickPart>().ConnectedEdgeNwIds.Add(newNwId);

        //foreach (ulong prevId in prevObject.GetComponent<StickPart>().ConnectedEdgeNwIds)
        //{
        //    Debug.Log(prevId);
        //    prevObject.GetComponent<StickPart>().ConnectedEdgeNwIds.Add(prevId);
        //}

        i++;

        previousVerticleIndex = i;

        switch (selectSplitOption(level * (branchType == BranchType.Main ? 8 : 20)))
        {
            case BranchSplit.Main:
                NewStickPartRecursive(level, previousVerticleIndex, length, width);
                break;
            case BranchSplit.MainMain:
                NewStickPartRecursive(level + 1, previousVerticleIndex, length, width);
                NewStickPartRecursive(level + 1, previousVerticleIndex, length, width);
                break;
            case BranchSplit.MainExtra:
                NewStickPartRecursive(level + 1, previousVerticleIndex, length, width);
                NewStickPartRecursive(level + 1, previousVerticleIndex, length, width, BranchType.Extra);
                break;
            case BranchSplit.MainExtraExtra:
                NewStickPartRecursive(level + 1, previousVerticleIndex, length, width);
                NewStickPartRecursive(level + 1, previousVerticleIndex, length, width, BranchType.Extra);
                NewStickPartRecursive(level + 1, previousVerticleIndex, length, width, BranchType.Extra);
                break;
            case BranchSplit.End:
                break;
        }
    }

    private BranchSplit selectSplitOption(int endPercentage = 0)
    {
        int percentageLeft = 100 - endPercentage;

        int randomizer = Random.Range(0, 100);

        if (randomizer < percentageLeft * 0.45f)         return BranchSplit.Main;
        else if (randomizer < percentageLeft * 0.70f)    return BranchSplit.MainMain;
        else if (randomizer < percentageLeft * 0.90f)    return BranchSplit.MainExtra;
        else if (randomizer < percentageLeft * 1.00f)    return BranchSplit.MainExtraExtra;

        else return BranchSplit.End;
    }

    //////Math based solution (slightly stuttered)

    //private void NewStickPartRecursive(int level, int previousVerticleIndex, float length, float width, BranchType branchType = BranchType.Main)
    //{
    //    if (level > _maximalStickLevel) return;

    //    width *= (branchType == BranchType.Main ? _widthDownfall : _extraBranchWidthDownfall);
    //    if (width < 0.2f) return;

    //    Debug.Log(width);

    //    length *= _lengthDownfall;

    //    Vector3 previousVerticle = _verticles[previousVerticleIndex];

    //    Vector3 previousPart = _stickParts.ElementAt(previousVerticleIndex - 1).transform.eulerAngles;

    //    float radianTheta; //theta
    //    float radianPhi; //phi

    //    if (branchType == BranchType.Main)
    //    {
    //        radianTheta = previousPart.x * Mathf.PI / 180; //theta
    //        radianPhi = previousPart.y * Mathf.PI / 180; //phi
    //    }
    //    else //BranchType.Extra
    //    {
    //        radianTheta = Mathf.PI - previousPart.x * Mathf.PI / 180; //theta
    //        radianPhi = previousPart.y * Mathf.PI / 180; //phi
    //    }

    //    float t = Random.Range(0f, 2.0f) * Mathf.PI;
    //    float alpha = 0.1f * Mathf.PI;

    //    float x = length * (Mathf.Sin(alpha) * Mathf.Cos(radianPhi) * Mathf.Cos(radianTheta) * Mathf.Cos(t) + Mathf.Sin(alpha) * Mathf.Sin(radianTheta) * Mathf.Sin(t) + -Mathf.Cos(alpha) * Mathf.Sin(radianPhi) * Mathf.Cos(radianTheta));
    //    float y = length * (Mathf.Sin(alpha) * Mathf.Cos(radianPhi) * Mathf.Sin(radianTheta) * Mathf.Cos(t) + Mathf.Sin(alpha) * Mathf.Cos(radianTheta) * Mathf.Sin(t) + Mathf.Cos(alpha) * Mathf.Sin(radianPhi) * Mathf.Sin(radianTheta)); ;
    //    float z = length * (Mathf.Sin(alpha) * Mathf.Sin(radianPhi) * Mathf.Cos(t) + Mathf.Cos(alpha) * Mathf.Cos(radianPhi));

    //    //float x = length * Mathf.Cos(radianTheta) * Mathf.Sin(radianPhi);
    //    //float y = length * Mathf.Sin(radianTheta) * Mathf.Sin(radianPhi);
    //    //float z = length * Mathf.Cos(radianPhi);

    //    Vector3 newVerticle = previousVerticle + new Vector3(x, -y, z).normalized * length;

    //    _verticles.Add(newVerticle);

    //    GameObject newShow = Instantiate(_joint, newVerticle, new Quaternion());

    //    GameObject newStickPart = Instantiate(_stickPartPrefab, Vector3.Lerp(previousVerticle, newVerticle, 0.5f), Quaternion.LookRotation(newVerticle - previousVerticle));
    //    newStickPart.transform.localScale = new Vector3(newStickPart.transform.localScale.x * width, newStickPart.transform.localScale.y * width, newStickPart.transform.localScale.z * length);
    //    newStickPart.transform.parent = transform;
    //    _stickParts.Add(newStickPart);
    //    i++;

    //    previousVerticleIndex = i;

    //    switch (selectSplitOption(level * 10))
    //    {
    //        case BranchSplit.Main:
    //            NewStickPartRecursive(level + 1, previousVerticleIndex, length, width);
    //            break;
    //        case BranchSplit.MainMain:
    //            NewStickPartRecursive(level + 1, previousVerticleIndex, length, width);
    //            NewStickPartRecursive(level + 1, previousVerticleIndex, length, width);
    //            break;
    //        case BranchSplit.MainExtra:
    //            NewStickPartRecursive(level + 1, previousVerticleIndex, length, width);
    //            NewStickPartRecursive(level + 1, previousVerticleIndex, length, width, BranchType.Extra);
    //            break;
    //        case BranchSplit.MainExtraExtra:
    //            NewStickPartRecursive(level + 1, previousVerticleIndex, length, width);
    //            NewStickPartRecursive(level + 1, previousVerticleIndex, length, width, BranchType.Extra);
    //            NewStickPartRecursive(level + 1, previousVerticleIndex, length, width, BranchType.Extra);
    //            break;
    //        case BranchSplit.End:
    //            break;
    //    }
    //}

    //////no branch splits version


    ////[SerializeField]
    ////private float _spreadChanceMultiplier = 0.5f;

    ////private void GenerateStickParts() //generates first stick part then starts recursion
    ////{

    ////    float length = Random.Range(_minInitialLength, _maxInitialLength);

    ////    _verticles.Add(new Vector3(0, 0, 0));
    ////    Vector3 newVerticle = new Vector3(length, 0);
    ////    _verticles.Add(newVerticle);

    ////    GameObject newStickPart = Instantiate(_stickPartPrefab, Vector3.Lerp(_verticles.ElementAt(0), _verticles.ElementAt(1), 0.5f), Quaternion.LookRotation(newVerticle));
    ////    newStickPart.transform.localScale = newStickPart.transform.localScale * length;
    ////    newStickPart.transform.parent = transform;
    ////    _stickParts.Add(newStickPart);

    ////    i = 1;
    ////    int level = 1;

    ////    int previousVerticleIndex = i;
    ////    for (int j = 0; j < getNumberOfChildren(0); j++)
    ////    {
    ////        NewStickPartRecursive(level, previousVerticleIndex, length);
    ////    }

    ////    //int i = 1;
    ////    //for (int level = 1; level < _maximalStickLevel; level++)
    ////    //{

    ////    //    Vector3 previousVerticle = _verticles.ElementAt(i);
    ////    //    for (int j = 0; j < 2; j++)
    ////    //    {
    ////    //        length *= _lengthDownfall;
    ////    //        float angleTheta = Random.Range(-_maximalJointRadian, _maximalJointRadian) * Mathf.PI;
    ////    //        Debug.Log(angleTheta);
    ////    //        float anglePhi = (Random.Range(-_maximalJointRadian, _maximalJointRadian) + 0.5f) * Mathf.PI;
    ////    //        Debug.Log(anglePhi);

    ////    //        float x = Mathf.Cos(angleTheta) * Mathf.Sin(anglePhi);
    ////    //        float y = Mathf.Sin(angleTheta) * Mathf.Cos(anglePhi);
    ////    //        float z = Mathf.Cos(anglePhi);

    ////    //        newVerticle = previousVerticle + new Vector3(x, y, z).normalized * length;

    ////    //        _verticles.Add(newVerticle);

    ////    //        newStickPart = Instantiate(_stickPartPrefab, Vector3.Lerp(previousVerticle, newVerticle, 0.5f), Quaternion.LookRotation(previousVerticle - newVerticle));
    ////    //        newStickPart.transform.localScale = newStickPart.transform.localScale * length;
    ////    //        newStickPart.transform.parent = transform;
    ////    //        _stickParts.Add(newStickPart);
    ////    //        i++;
    ////    //    }
    ////    //}
    ////}
    //////final

    ////private void NewStickPartRecursive(int level, int previousVerticleIndex, float length)
    ////{
    ////    if (level > _maximalStickLevel) return;

    ////    Vector3 previousVerticle = _verticles[previousVerticleIndex];

    ////    Vector3 previousPart = _stickParts.ElementAt(previousVerticleIndex - 1).transform.eulerAngles;
    ////    Debug.Log(previousPart);

    ////    length *= _lengthDownfall;
    ////    //float radianTheta = 0 * Mathf.PI + previousPart.x * Mathf.PI / 180; //theta
    ////    //float radianPhi = 0 * Mathf.PI + previousPart.y * Mathf.PI / 180; //phi
    ////    float radianTheta = Random.Range(-_maximalJointRadian, _maximalJointRadian) * Mathf.PI + previousPart.x * Mathf.PI / 180; //theta
    ////    float radianPhi = Random.Range(-_maximalJointRadian, _maximalJointRadian) * Mathf.PI + previousPart.y * Mathf.PI / 180; //phi
    ////    Debug.Log(radianTheta);
    ////    Debug.Log(radianPhi);

    ////    float x = Mathf.Cos(radianTheta) * Mathf.Sin(radianPhi);
    ////    float y = Mathf.Sin(radianTheta) * Mathf.Cos(radianPhi);
    ////    float z = Mathf.Cos(radianPhi);

    ////    Vector3 newVerticle = previousVerticle + new Vector3(x, y, z).normalized * length;

    ////    _verticles.Add(newVerticle);

    ////    GameObject newStickPart = Instantiate(_stickPartPrefab, Vector3.Lerp(previousVerticle, newVerticle, 0.5f), Quaternion.LookRotation(newVerticle - previousVerticle));
    ////    newStickPart.transform.localScale = newStickPart.transform.localScale * length;
    ////    newStickPart.transform.parent = transform;
    ////    _stickParts.Add(newStickPart);
    ////    i++;

    ////    previousVerticleIndex = i;

    ////    for (int j = 0; j < getNumberOfChildren(level); j++)
    ////    {
    ////        NewStickPartRecursive(level + 1, previousVerticleIndex, length);
    ////    }
    ////}

    ////private int getNumberOfChildren(int endChance)
    ////{
    ////    int chanceSpread = 7;

    ////    int randomizer = Random.Range(0, chanceSpread + endChance * (int)_spreadChanceMultiplier);

    ////    if (randomizer < 4) return 1;
    ////    else if (randomizer < 7) return 2;
    ////    else if (randomizer == 7) return 3;

    ////    else return 0;
    ////}

    //even older

    //private void NewStickPartRecursive(int level, Vector3 previousVerticle, float length)
    //{
    //    if (level > _maximalStickLevel) return;

    //    length *= _lengthDownfall;
    //    float angleTheta = Random.Range(-_maximalJointRadian, _maximalJointRadian) * Mathf.PI;
    //    float anglePhi = (Random.Range(-_maximalJointRadian, _maximalJointRadian) + 0.5f) * Mathf.PI;

    //    float x = Mathf.Cos(angleTheta) * Mathf.Sin(anglePhi);
    //    float y = Mathf.Sin(angleTheta) * Mathf.Cos(anglePhi);
    //    float z = Mathf.Cos(anglePhi);

    //    Vector3 newVerticle = previousVerticle + new Vector3(x, y, z).normalized * length;

    //    _verticles.Add(newVerticle);

    //    GameObject newStickPart = Instantiate(_stickPartPrefab, Vector3.Lerp(previousVerticle, newVerticle, 0.5f), Quaternion.LookRotation(previousVerticle - newVerticle));
    //    newStickPart.transform.localScale = newStickPart.transform.localScale * length;
    //    newStickPart.transform.parent = transform;
    //    _stickParts.Add(newStickPart);

    //    previousVerticle = _verticles.ElementAt(i++);
    //    for (int j = 0; j < 2; j++)
    //    {
    //        NewStickPartRecursive(level + 1, previousVerticle, length);
    //    }
    //}


    ////-----Weird end spread version
    //private void NewStickPartRecursive(int level, Vector3 previousVerticle, float length, int i)
    //{
    //    if (level > _maximalStickLevel) return;

    //    length *= _lengthDownfall;
    //    float angleTheta = Random.Range(-_maximalJointRadian, _maximalJointRadian) * Mathf.PI;
    //    Debug.Log(angleTheta);
    //    float anglePhi = (Random.Range(-_maximalJointRadian, _maximalJointRadian) + 0.5f) * Mathf.PI;
    //    Debug.Log(anglePhi);

    //    float x = Mathf.Cos(angleTheta) * Mathf.Sin(anglePhi);
    //    float y = Mathf.Sin(angleTheta) * Mathf.Cos(anglePhi);
    //    float z = Mathf.Cos(anglePhi);

    //    Vector3 newVerticle = previousVerticle + new Vector3(x, y, z).normalized * length;

    //    _verticles.Add(newVerticle);

    //    GameObject newStickPart = Instantiate(_stickPartPrefab, Vector3.Lerp(previousVerticle, newVerticle, 0.5f), Quaternion.LookRotation(previousVerticle - newVerticle));
    //    newStickPart.transform.localScale = newStickPart.transform.localScale * length;
    //    newStickPart.transform.parent = transform;
    //    _stickParts.Add(newStickPart);

    //    previousVerticle = _verticles.ElementAt(i + 1);
    //    for (int j = 0; j < 2; j++)
    //    {
    //        NewStickPartRecursive(level + 1, previousVerticle, length, i + 1);
    //    }
    //}

    //private void NewStickPartRecursiveFailed(float length, int i)
    //{
    //    length *= _lengthDownfall;
    //    float angleTheta = Random.Range(-_maximalJointRadian, _maximalJointRadian) * Mathf.PI;
    //    Debug.Log(angleTheta);
    //    float anglePhi = (Random.Range(-_maximalJointRadian, _maximalJointRadian) + 0.5f) * Mathf.PI;
    //    Debug.Log(anglePhi);

    //    float x = Mathf.Cos(angleTheta) * Mathf.Sin(anglePhi);
    //    float y = Mathf.Sin(angleTheta) * Mathf.Cos(anglePhi);
    //    float z = Mathf.Cos(anglePhi);

    //    Vector3 newVerticle = _verticles.ElementAt(i) + new Vector3(x, y, z).normalized * length;

    //    _verticles.Add(newVerticle);

    //    Vector3 previousVerticle = _verticles.ElementAt(i);

    //    GameObject newStickPart = Instantiate(_stickPartPrefab, Vector3.Lerp(previousVerticle, newVerticle, 0.5f), Quaternion.LookRotation(previousVerticle - newVerticle));
    //    newStickPart.transform.localScale = newStickPart.transform.localScale * length;
    //    newStickPart.transform.parent = transform;
    //    _stickParts.Add(newStickPart);

    //    if (i < _maximalStickLevel)
    //    {
    //        for (int j = 0; j < 1; j++)
    //        {
    //            NewStickPartRecursiveFailed(length, i++);
    //        }
    //    }
    //}
}
