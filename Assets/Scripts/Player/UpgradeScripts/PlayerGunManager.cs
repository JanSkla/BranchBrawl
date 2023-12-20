using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerGunManager : NetworkBehaviour
{
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
        private GunBaseChildData _childPrefabs;
        public GunBaseSaveData(GBase thisPrefab)
        {
            _prefab = thisPrefab;

            if (thisPrefab.Destiny.Part.GetType() != typeof(GUpgrade)) return;

            _childPrefabs = new GunBaseChildData(thisPrefab.Destiny.Part as GUpgrade);
        }

        public void Spawn(GPart parent)
        {
            Instantiate(_prefab);
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
            Instantiate(_prefab);
        }
    }
}
