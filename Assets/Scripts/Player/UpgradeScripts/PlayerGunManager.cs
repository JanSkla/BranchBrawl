using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerGunManager : NetworkBehaviour
{
    private readonly List<GUpgradeData> _gUpgradeInv = new();

    //private GunBaseSaveData _gunCurrentData;
    public NetworkVariable<GunBaseSaveData> GunCurrentData = new(new GunBaseSaveData());
    //public GunBaseSaveData GunCurrentData = new GunBaseSaveData(new GunBaseChildData(1, new GunBaseChildData[2]));
    //public NetworkVariable<GunBaseSaveData> GunCurrentData = new(new GunBaseSaveData(new GunBaseChildData(1, new GunBaseChildData[]{
    //    new GunBaseChildData(1, new GunBaseChildData[2]),
    //    new GunBaseChildData(1, new GunBaseChildData[2])
    //public NetworkVariable<GunBaseSaveData> GunCurrentData = new(new GunBaseSaveData(new GunBaseChildData(1, new GunBaseChildData[]{
    //    new GunBaseChildData(1, new GunBaseChildData[2]),
    //    new GunBaseChildData(2, new GunBaseChildData[1])
    //}))); // "1{1{,,},1{,,},}" -- in text //REMOVE

    //private void Start()
    //{
    //    AddGUpgrade(1);  //REMOVE
    //    AddGUpgrade(2);
    //    //Debug.Log(GunBaseSaveData.ParseToText(new GunBaseSaveData().Child));
    //    //Debug.Log(GunBaseSaveData.ParseToText(GunBaseSaveData.ParseText(GunBaseSaveData.ParseToText(new GunBaseSaveData().Child))));
    //    //Debug.Log("original" + GunBaseSaveData.ParseToText(GunCurrentData.Value.Child));
    //    //Debug.Log("new" + GunBaseSaveData.ParseToText(GunBaseSaveData.ParseText(GunBaseSaveData.ParseToText(GunCurrentData.Value.Child))));
    //}

    public IEnumerable<GUpgradeData> GUpgradeInv
    {
        get { return _gUpgradeInv; }
    }

    public GUpgradeData FindGUpgradeDataByUpId(int upgradeId)
    {
        return _gUpgradeInv.Find(e => e.UpgradeId == upgradeId);
    }
    public void AddGUpgrade(int upgradeId)
    {
        var findSimiliar = FindGUpgradeDataByUpId(upgradeId);
        if (findSimiliar != null)
        {
            findSimiliar.TotalCount++;
        }
        else
        {
            _gUpgradeInv.Add(new GUpgradeData()
            {
                UpgradeId = upgradeId,
                TotalCount = 1,
                UsedCount = 0
            });
        }
    }
    public bool RemoveGUpgrade(int upgradeId)
    {
        var findSimiliar = FindGUpgradeDataByUpId(upgradeId);
        if (findSimiliar != null)
        {
            findSimiliar.TotalCount--;
            if (findSimiliar.TotalCount < 1)
            {
                _gUpgradeInv.Remove(findSimiliar);
            }
            return true;
        }
        else
        {
            Debug.Log("GUpgradeData type does not exist in the list");
            return false;
        }
    }
    public bool UseGUpgrade(int upgradeId)
    {
        var findSimiliar = FindGUpgradeDataByUpId(upgradeId);
        if (findSimiliar != null)
        {
            findSimiliar.UsedCount++;
            return true;
            //if (findSimiliar.UsedCount < findSimiliar.TotalCount)
            //{
            //    findSimiliar.UsedCount++;
            //    return true;
            //}
            //else
            //{
            //    Debug.Log("Can't use more than in inventory");
            //    return false;
            //}
        }
        else
        {
            Debug.LogWarning("GUpgradeData type does not exist in the list");
            return false;
        }
    }
    public bool UnuseGUpgrade(int upgradeId)
    {
        var findSimiliar = FindGUpgradeDataByUpId(upgradeId);
        if (findSimiliar != null)
        {
            findSimiliar.UsedCount--;
            return true;
            //if (findSimiliar.UsedCount > 0)
            //{
            //    findSimiliar.UsedCount--;
            //    return true;
            //}
            //else
            //{
            //    Debug.Log("Can't unuse less than in inventory");
            //    return false;
            //}
        }
        else
        {
            Debug.LogWarning("GUpgradeData type does not exist in the list");
            return false;
        }
    }
}
