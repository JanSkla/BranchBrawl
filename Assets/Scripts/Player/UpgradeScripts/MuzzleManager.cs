using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MuzzleManager
{
    private static string _muzzlePrefabPathPrefix = "Prefabs/GunMuzzles/";

    private static string[] _muzzleTypes = new string[]
    {
        "GMuzzle"
    };


    public static GMuzzle InstantiateGMuzzle()
    {
        GMuzzle go = Resources.Load<GameObject>(_muzzlePrefabPathPrefix + _muzzleTypes[0]).GetComponent<GMuzzle>();
        return Object.Instantiate(go);
    }

    public static void MuzzleInstantiateOnDestiny(GDestiny desitny)
    {
        GMuzzle gMuzzle = InstantiateGMuzzle();

        gMuzzle.NetworkObject.AutoObjectParentSync = false;
        gMuzzle.transform.SetParent(desitny.PositionPoint.transform, false);

        desitny.Part = gMuzzle;
    }
    public static void NetworkMuzzleInstantiateOnDestiny(GDestiny parentDestiny)
    {
        GMuzzle gMuzzle = InstantiateGMuzzle();

        gMuzzle.NetworkObject.Spawn(true);

        GameObject parentGO = parentDestiny.PositionPoint.Parent;

        if (parentGO.GetComponent<GBase>() != null)
        {
            parentDestiny.PositionPoint.Parent.GetComponent<GBase>().NetworkAddParentOnDestiny(gMuzzle.NetworkObjectId);
        }
        else if (parentGO.GetComponent<GUpgrade>() != null)
        {
            parentDestiny.PositionPoint.Parent.GetComponent<GUpgrade>().NetworkAddParentOnDestiny(parentDestiny.PositionPoint.DestinyIndex, gMuzzle.NetworkObjectId);
        }
        else
        {
            Debug.LogError("Neco je zle");
        }

        //gMuzzle.NetworkObject.TrySetParent(desitny.PositionPoint.transform, false);

        parentDestiny.Part = gMuzzle;
    }
}
