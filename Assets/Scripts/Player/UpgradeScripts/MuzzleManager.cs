using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class MuzzleManager
{
    private static string _muzzlePrefabPathPrefix = "Prefabs/GunMuzzles/";

    private static string[] _muzzleTypes = new string[]
    {
        "GMuzzle",
        "GConeMuzzle",
        "GFireMuzzle",
        "GFireConeMuzzle"
    };


    public static GMuzzle InstantiateGMuzzle(int idx)
    {
        Debug.Log("index" + idx);
        Debug.Log("prefab location" + _muzzlePrefabPathPrefix + _muzzleTypes[idx]);
        GMuzzle go = Resources.Load<GameObject>(_muzzlePrefabPathPrefix + _muzzleTypes[idx]).GetComponent<GMuzzle>();
        return Object.Instantiate(go);
    }

    public static void MuzzleInstantiateOnDestiny(GDestiny desitny)
    {
        GMuzzle gMuzzle = (GMuzzle)InstantiateGMuzzleByPrevUpgradeIds(desitny.PreviousUpgradeIds);

        gMuzzle.NetworkObject.AutoObjectParentSync = false;
        gMuzzle.transform.SetParent(desitny.PositionPoint.transform, false);

        desitny.Part = gMuzzle;
    }
    public static void NetworkMuzzleInstantiateOnDestiny(GDestiny parentDestiny)
    {
        GMuzzle gMuzzle = (GMuzzle)InstantiateGMuzzleByPrevUpgradeIds(parentDestiny.PreviousUpgradeIds);

        gMuzzle.NetworkObject.Spawn(true);

        GPart parentGO = parentDestiny.PositionPoint.Parent;

        if (parentGO.GetComponent<GBase>())
        {
            parentGO.GetComponent<GBase>().NetworkAddParentOnDestiny(gMuzzle.NetworkObjectId);
        }
        else if (parentGO.GetComponent<GUpgrade>())
        {
            parentGO.GetComponent<GUpgrade>().NetworkAddParentOnDestiny(parentDestiny.PositionPoint.DestinyIndex, gMuzzle.NetworkObjectId);
        }
        else
        {
            Debug.Log(parentGO);
            Debug.LogError("Neco je zle");
        }

        //gMuzzle.NetworkObject.TrySetParent(desitny.PositionPoint.transform, false);

        parentDestiny.Part = gMuzzle;
    }

    public static GPart InstantiateGMuzzleByPrevUpgradeIds(int[] upgradeIds)
    {
        Debug.Log(upgradeIds);
        Debug.Log(upgradeIds.Contains(4));
        foreach(var i in upgradeIds)
        {
            Debug.Log("a" + i);
        }
        if (upgradeIds.Contains((int)Upgrades.GFireEnhancer))
        {
            if (upgradeIds.Contains((int)Upgrades.GConeEnhancer))
            {
                return InstantiateGMuzzle(3);
            }
            return InstantiateGMuzzle(2);
        }
        if (upgradeIds.Contains((int)Upgrades.GConeEnhancer))
        {
            return InstantiateGMuzzle(1);
        }
        return InstantiateGMuzzle(0);
    }
}
