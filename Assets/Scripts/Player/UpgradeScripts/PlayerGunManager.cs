using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerGunManager : NetworkBehaviour
{
    private static string _muzzlePrefab = "Prefabs/GunParts/GMuzzle";
    private static string _basePrefab = "Prefabs/GunParts/GBase";


    private readonly List<GUpgradeData> _gUpgradeInv = new();

    //private GunBaseSaveData _gunCurrentData;
    //public GunBaseSaveData GunCurrentData = new GunBaseSaveData();
    //public GunBaseSaveData GunCurrentData = new GunBaseSaveData(new GunBaseChildData(1, new GunBaseChildData[2]));
    public GunBaseSaveData GunCurrentData = new GunBaseSaveData(new GunBaseChildData(1, new GunBaseChildData[]{
        new GunBaseChildData(1, new GunBaseChildData[2]),
        new GunBaseChildData(1, new GunBaseChildData[2])
    }));

    public IEnumerable<GUpgradeData> GUpgradeInv
    {
        get { return _gUpgradeInv; }
    }

    public void AddGUpgrade(UpgradeWithPart uwp)
    {
        var findSimiliar = _gUpgradeInv.Find(e => e.UpgradeId == uwp.Id);
        if (findSimiliar != null)
        {
            findSimiliar.TotalCount++;
        }
        else
        {
            _gUpgradeInv.Add(new GUpgradeData()
            {
                UpgradeId = uwp.Id,
                TotalCount = 1,
                UsedCount = 0
            });
        }
    }
    public void RemoveGUpgrade(UpgradeWithPart uwp)
    {
        var findSimiliar = _gUpgradeInv.Find(e => e.UpgradeId == uwp.Id);
        if (findSimiliar != null)
        {
            findSimiliar.TotalCount--;
            if (findSimiliar.TotalCount < 1)
            {
                _gUpgradeInv.Remove(findSimiliar);
            }
        }
        else
        {
            Debug.Log("prefabResource does not exist in the list");
        }
    }
    public class GunBaseSaveData
    {
        private GunBaseChildData _childPrefab;

        //Save from gBase gameobject
        public GunBaseSaveData(GBase gBase)
        {
            if (!gBase.Destiny.Part.GetType().IsSubclassOf(typeof(GUpgrade))) return;

            _childPrefab = new GunBaseChildData(gBase.Destiny.Part as GUpgrade);
        }

        public GBase Spawn()
        {
            GameObject gBasePrefab = Resources.Load(_basePrefab) as GameObject;
            GBase gBase = Instantiate(gBasePrefab).GetComponent<GBase>();

            GBDSpawnShared(_childPrefab, gBase.Destiny);

            return gBase;
        }

        //testing purposes

        public GunBaseSaveData(GunBaseChildData chPrefab)
        {
            _childPrefab = chPrefab;
        }
        //Has no upgrades
        public GunBaseSaveData() { }
    }

    public class GunBaseChildData //TADY NECO TODO
    {
        private int _upgradeId;
        private GunBaseChildData[] _childPrefabs;
        public GunBaseChildData(GUpgrade gUpgrade)
        {
            _upgradeId = gUpgrade.UpgradeId;
            _childPrefabs = new GunBaseChildData[gUpgrade.Destiny.Length];

            for (int i = 0; i < gUpgrade.Destiny.Length; i++)
            {
                if (!gUpgrade.Destiny[i].Part.GetType().IsSubclassOf(typeof(GUpgrade))) return;

                _childPrefabs[i] = new GunBaseChildData(gUpgrade.Destiny[i].Part as GUpgrade);
            }
        }

        public GPart Spawn(Transform parentTransfrom)
        {
            GUpgrade gUpgrade = (UpgradeManager.GetUpgradeById(_upgradeId) as UpgradeWithPart).InstantiatePrefab().GetComponent<GUpgrade>();
            gUpgrade.transform.SetParent(parentTransfrom, false);

            for (int i = 0; i < _childPrefabs.Length; i++)
            {
                GBDSpawnShared(_childPrefabs[i], gUpgrade.Destiny[i]);
            }
            return gUpgrade;
            //foreach (var childPrefab in _childPrefabs)
            //{
            //    var gSource = Instantiate(_prefab);

            //    if (!childPrefab.IsUnityNull())
            //    {
            //        childPrefab.Spawn(gSource);
            //    }
            //    else
            //    {
            //        var gMuzzleprefab = Resources.Load(_muzzlePrefab) as GMuzzle;
            //        var gMuzzle = Instantiate(gMuzzleprefab);
            //        gMuzzle.transform.SetParent(gSource.transform);

            //        gSource.Destiny[1].Part = gMuzzle;
            //    }

            //    gSource.transform.SetParent(parent.transform);
            //    childPrefab.Spawn(gSource);
            //}
        }

        //testing purposes

        public GunBaseChildData(int UpgradeId, GunBaseChildData[] chPrefab)
        {
            _upgradeId = UpgradeId;
            _childPrefabs = chPrefab;
        }
    }

    //Helper func for GBD classes
    private static void GBDInstantiateOnDestiny(GDestiny desitny)
    {
        GameObject gMuzzleprefab = Resources.Load(_muzzlePrefab) as GameObject;
        GMuzzle gMuzzle = Instantiate(gMuzzleprefab).GetComponent<GMuzzle>();
        gMuzzle.transform.SetParent(desitny.Position, false);

        desitny.Part = gMuzzle;
    }
    private static void GBDSpawnShared(GunBaseChildData childData, GDestiny desitny)
    {
        if (!childData.IsUnityNull())
        {
            desitny.Part = childData.Spawn(desitny.Position);
        }
        else
        {
            GBDInstantiateOnDestiny(desitny);
        }
    }
}
