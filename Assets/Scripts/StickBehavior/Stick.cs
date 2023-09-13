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

    private Dictionary<ulong, int> _gunBarrels = new Dictionary<ulong, int>();

    //Stick generation specs
    [SerializeField]
    private float _minInitialLength = 1.0f;
    [SerializeField]
    private float _maxInitialLength = 3.0f;
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
    }
    //final

    private void NewStickPartRecursive(int level, int previousVerticleIndex, float length, float width, BranchType branchType = BranchType.Main)
    {
        if (level > _maximalStickLevel) return;

        width *= (branchType == BranchType.Main ? _widthDownfall : _extraBranchWidthDownfall);
        if (width < 0.2f) return;

        length *= _lengthDownfall;

        Vector3 previousVerticle = _verticles[previousVerticleIndex];

        GameObject prevObject = _stickParts.ElementAt(previousVerticleIndex - 1);

        Vector3 previousPart = prevObject.transform.eulerAngles;

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

        //

        ulong newNwId = newStickPart.GetComponent<NetworkObject>().NetworkObjectId;
        ulong prevNwId = prevObject.GetComponent<NetworkObject>().NetworkObjectId;

        newStickPart.GetComponent<StickPart>().ConnectedEdgeNwIdsN.Add(prevNwId);

        for (int i = 0; i < prevObject.GetComponent<StickPart>().ConnectedEdgeNwIdsP.Count; i++)
        {
            ulong adjescentNwId = prevObject.GetComponent<StickPart>().ConnectedEdgeNwIdsP[i];
            GameObject adjescentObject = GetNetworkObject(adjescentNwId).gameObject;

            newStickPart.GetComponent<StickPart>().ConnectedEdgeNwIdsN.Add(adjescentNwId);
            adjescentObject.GetComponent<StickPart>().ConnectedEdgeNwIdsN.Add(newNwId);
        }

        prevObject.GetComponent<StickPart>().ConnectedEdgeNwIdsP.Add(newNwId);

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

        if (randomizer < percentageLeft * 0.45f) return BranchSplit.Main;
        else if (randomizer < percentageLeft * 0.70f) return BranchSplit.MainMain;
        else if (randomizer < percentageLeft * 0.90f) return BranchSplit.MainExtra;
        else if (randomizer < percentageLeft * 1.00f) return BranchSplit.MainExtraExtra;

        else return BranchSplit.End;
    }

    public static void FindGunBarrels(StickPart originPart)
    {
        originPart.gameObject.transform.parent.gameObject.GetComponent<Stick>().SetGunBarrels(originPart);
    }
    private void SetGunBarrels(StickPart originPart)
    {
        Dictionary<ulong, int> endingsN = new Dictionary<ulong, int>(); //ulong NwId, int strength(distance from origin)
        Dictionary<ulong, int> endingsP = new Dictionary<ulong, int>(); //ulong NwId, int strength(distance from origin)

        if (originPart.ConnectedEdgeNwIdsP.Count + originPart.ConnectedEdgeNwIdsN.Count == 0)
        {
            endingsP.Add(originPart.gameObject.GetComponent<NetworkObject>().NetworkObjectId, 0);
            return;
        }

        ulong[] searched = new ulong[_stickParts.Count];
        bool isN = true;
        int j = 0;
        FindEndings(originPart, 0);

        void FindEndings(StickPart originPart, int strength)
        {
            ulong originNwId = originPart.gameObject.GetComponent<NetworkObject>().NetworkObjectId;
            searched[j] = originNwId;
            j++;
            if (originPart.ConnectedEdgeNwIdsN.Count == 0 || originPart.ConnectedEdgeNwIdsP.Count == 0)
            {
                if (isN)
                {
                    endingsN.Add(originNwId, strength);
                }
                else
                {
                    endingsP.Add(originNwId, strength);
                }
                if (strength != 0) return;
            }
            foreach (ulong conENwId in originPart.ConnectedEdgeNwIdsN)
            {
                if(!searched.Contains(conENwId))
                {
                    StickPart subPart = GetNetworkObject(conENwId).gameObject.GetComponent<StickPart>();
                    FindEndings(subPart, strength + 1);
                }
            }
            isN = false;
            foreach (ulong conENwId in originPart.ConnectedEdgeNwIdsP)
            {
                if (!searched.Contains(conENwId))
                {
                    StickPart subPart = GetNetworkObject(conENwId).gameObject.GetComponent<StickPart>();
                    FindEndings(subPart, strength + 1);
                }
            }
        }

        int N = 0;
        int P = 0;
        foreach (var value in endingsN)
        {
            N += value.Value;
            GetNetworkObject(value.Key).gameObject.GetComponent<Renderer>().material.color = Color.red;
        }

        foreach (var value in endingsP)
        {
            P += value.Value;
            GetNetworkObject(value.Key).gameObject.GetComponent<Renderer>().material.color = Color.blue;
        }
        _gunBarrels = N > P ? endingsN : endingsP;
    }
    /////////??????????????????????? fix, nefunguje
    //private void SetGunBarrels(StickPart originPart)
    //{
    //    Dictionary<ulong, int> endingsN = new Dictionary<ulong, int>(); //ulong NwId, int strength(distance from origin)
    //    Dictionary<ulong, int> endingsP = new Dictionary<ulong, int>(); //ulong NwId, int strength(distance from origin)

    //    if (originPart.ConnectedEdgeNwIdsP.Count + originPart.ConnectedEdgeNwIdsN.Count == 0)
    //    {
    //        endingsP.Add(originPart.gameObject.GetComponent<NetworkObject>().NetworkObjectId, 0);
    //        return;
    //    }

    //    ulong[] searched = new ulong[_stickParts.Count];
    //    bool isN = true;
    //    int j = 0;
    //    FindEndings(originPart, 0);

    //    void FindEndings(StickPart originPart, int strength)
    //    {
    //        ulong originNwId = originPart.gameObject.GetComponent<NetworkObject>().NetworkObjectId;

    //        searched[j] = originNwId;
    //        j++;
    //        if ((originPart.ConnectedEdgeNwIdsN.Count == 0 || originPart.ConnectedEdgeNwIdsP.Count == 0) && strength != 0)
    //        {
    //            if (isN)
    //            {
    //                endingsN.Add(originNwId, strength);
    //            }
    //            else
    //            {
    //                endingsP.Add(originNwId, strength);
    //            }
    //        }
    //        foreach (ulong conENwId in originPart.ConnectedEdgeNwIdsN)
    //        {
    //            if (!searched.Contains(conENwId))
    //            {
    //                StickPart subPart = GetNetworkObject(conENwId).gameObject.GetComponent<StickPart>();
    //                FindEndings(subPart, strength + 1);
    //            }
    //        }
    //        isN = false;
    //        foreach (ulong conENwId in originPart.ConnectedEdgeNwIdsP)
    //        {
    //            if (!searched.Contains(conENwId))
    //            {
    //                StickPart subPart = GetNetworkObject(conENwId).gameObject.GetComponent<StickPart>();
    //                FindEndings(subPart, strength + 1);
    //            }
    //        }
    //    }

    //    int N = 0;
    //    int P = 0;
    //    foreach (var value in endingsN)
    //    {
    //        N += value.Value;
    //        GetNetworkObject(value.Key).gameObject.GetComponent<Renderer>().material.color = Color.red;
    //    }

    //    foreach (var value in endingsP)
    //    {
    //        P += value.Value;
    //        GetNetworkObject(value.Key).gameObject.GetComponent<Renderer>().material.color = Color.blue;
    //    }
    //    _gunBarrels = N > P ? endingsN : endingsP;
    //}
}
