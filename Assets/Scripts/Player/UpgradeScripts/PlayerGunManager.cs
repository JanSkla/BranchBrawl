using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerGunManager : NetworkBehaviour
{
    private static string _muzzlePrefab = "Prefabs/GunParts/GMuzzle";
    private readonly List<GUpgradeData> _gUpgradeInv = new();

    private GunBaseSaveData _gunCurrentData;

    public IEnumerable<GUpgradeData> GUpgradeInv
    {
        get { return _gUpgradeInv; }
    }

    public void AddGUpgrade(UpgradeWithPart uwp)
    {
        var findSimiliar = _gUpgradeInv.Find(e => e.UpgradeId == uwp.Id);
        if(findSimiliar != null)
        {
            findSimiliar.TotalCount++;
        }
        else
        {
            _gUpgradeInv.Add(new GUpgradeData()
            {
                UpgradeId = uwp.Id,
                PrefabResource = uwp.UpgradePrefabResource,
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
        private GBase _prefab;
        private GunBaseChildData _childPrefab;
        public GunBaseSaveData(GBase thisPrefab)
        {
            _prefab = thisPrefab;

            if (thisPrefab.Destiny.Part.GetType() != typeof(GUpgrade)) return;

            _childPrefab = new GunBaseChildData(thisPrefab.Destiny.Part as GUpgrade);
        }

        public GBase Spawn()
        {

            var gSource = Instantiate(_prefab);

            if (!_childPrefab.IsUnityNull())
            {
                _childPrefab.Spawn(gSource);
            }
            else
            {
                var gMuzzleprefab = Resources.Load(_muzzlePrefab) as GMuzzle;
                var gMuzzle = Instantiate(gMuzzleprefab);
                gMuzzle.transform.SetParent(gSource.transform);

                gSource.Destiny.Part = gMuzzle;
            }

            return gSource;
        }
    }
    public class GunBaseChildData
    {
        private GUpgrade _prefab;
        private GunBaseChildData[] _childPrefabs;
        public GunBaseChildData(GUpgrade thisPrefab)
        {
            _prefab = thisPrefab;
            _childPrefabs = new GunBaseChildData[thisPrefab.Destiny.Length];

            for (int i = 0; i < thisPrefab.Destiny.Length; i++)
            {
                if (thisPrefab.Destiny[i].Part.GetType() != typeof(GUpgrade)) return;

                _childPrefabs[i] = new GunBaseChildData(thisPrefab.Destiny[i].Part as GUpgrade);
            }
        }

        public void Spawn(GPart parent)
        {
            for (int i = 0; i < _childPrefabs.Length; i++)
            {
                var gSource = Instantiate(_prefab);

                if (!_childPrefabs[i].IsUnityNull())
                {
                    _childPrefabs[i].Spawn(gSource);
                }
                else
                {
                    var gMuzzleprefab = Resources.Load(_muzzlePrefab) as GMuzzle;
                    var gMuzzle = Instantiate(gMuzzleprefab);
                    gMuzzle.transform.SetParent(gSource.transform);

                    gSource.Destiny[i].Part = gMuzzle;
                }

                gSource.transform.SetParent(parent.transform);
            }
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
    }
}
