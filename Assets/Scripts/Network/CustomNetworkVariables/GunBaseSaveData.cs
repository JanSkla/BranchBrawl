using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using static PlayerGunManager;


public class GunBaseSaveData : INetworkSerializable
{
    private static string _basePrefab = "Prefabs/GunParts/GBase";

    private GunBaseChildData _childPrefab;

    public GunBaseChildData Child
    {
        get { return _childPrefab; }
    }

    //Save from gBase gameobject
    public GunBaseSaveData(GBase gBase)
    {
        if (!gBase.Destiny.Part.GetType().IsSubclassOf(typeof(GUpgrade))) return;

        _childPrefab = new GunBaseChildData(gBase.Destiny.Part as GUpgrade);
    }

    public GBase Spawn()
    {
        GameObject gBasePrefab = Resources.Load(_basePrefab) as GameObject;
        GBase gBase = Object.Instantiate(gBasePrefab).GetComponent<GBase>();
        gBase.NetworkObject.AutoObjectParentSync = false;

        GBDSpawnShared(_childPrefab, gBase.Destiny);

        return gBase;
    }
    public GBase NetworkSpawn()
    {
        GameObject gBasePrefab = Resources.Load(_basePrefab) as GameObject;
        GBase gBase = Object.Instantiate(gBasePrefab).GetComponent<GBase>();

        gBase.NetworkObject.Spawn();

        GBDNetworkSpawnShared(_childPrefab, gBase.Destiny);

        return gBase;
    }
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        if (serializer.IsReader)
        {
            var reader = serializer.GetFastBufferReader();
            reader.ReadValueSafe(out string serializeText);

            //from string to GBSD
            _childPrefab = ParseText(serializeText);
            //~
        }
        else
        {
            //to string, fomrat: "1{101{1{11}11{111,11111},1}"
            string serializeText = ParseToText(_childPrefab);
            //~

            var writer = serializer.GetFastBufferWriter();
            writer.WriteValueSafe(serializeText);
        }
    }

    public static GunBaseChildData ParseText(string text)
    {
        if (text.IndexOf("{") == -1) return null;

        return RecursiveAssignGBCD(text);

        GunBaseChildData RecursiveAssignGBCD(string textInner)
        {
            Debug.Log("----");
            Debug.Log(textInner);
            int upId = int.Parse(textInner.Substring(0, textInner.IndexOf("{")));
            int arrSize = (UpgradeManager.GetUpgradeById(upId) as UpgradeWithPart).GetBranchCount();
            (int from, int to)[] parts = new (int from, int to)[arrSize];

            GunBaseChildData output = new(upId, new GunBaseChildData[arrSize]);

            textInner = textInner[(textInner.IndexOf("{") + 1)..textInner.LastIndexOf("}")];

            int j = 0;
            int l = 0;
            for (int i = 0; i < textInner.Length; i++)
            {
                if (j == 0 && textInner[i] == ',')
                {
                    l++;
                }
                else if (textInner[i] == '{')
                {
                    j++;
                    if (j == 1)
                    {
                        parts[l].from = i - 1;
                    }
                }
                else if (textInner[i] == '}')
                {
                    j--;
                    if (j == 0)
                    {
                        parts[l].to = i + 1;
                    }
                }
            }

            for (int i = 0; i < arrSize; i++)
            {
                string shortenedText = textInner[parts[i].from..parts[i].to];

                if (shortenedText.IndexOf("{") == -1) continue;

                output.ChildPrefabs[i] = RecursiveAssignGBCD(shortenedText);
            }

            return output;
        }
    }

    public static string ParseToText(GunBaseChildData gbcd)
    {
        if (gbcd == null) return ",";

        string output = "";

        RecursiveAssingText(gbcd);

        return output + ",";

        void RecursiveAssingText(GunBaseChildData gbcdInner)
        {
            output += gbcdInner.UpgradeId.ToString();
            output += "{";
            for (int i = 0; i < gbcdInner.ChildPrefabs.Length; i++)
            {
                if (gbcdInner.ChildPrefabs[i] != null)
                {
                    RecursiveAssingText(gbcdInner.ChildPrefabs[i]);
                }
                output += ",";
            }
            output += "}";
        }
    }

    //testing purposes 1 // "1{1{,,},1{,,},}," -- in text

    public GunBaseSaveData(GunBaseChildData chPrefab)
    {
        _childPrefab = chPrefab;
    }
    //Has no upgrades
    public GunBaseSaveData() { }

    public class GunBaseChildData
    {
        public int UpgradeId;
        public GunBaseChildData[] ChildPrefabs;
        public GunBaseChildData(GUpgrade gUpgrade)
        {
            UpgradeId = gUpgrade.UpgradeId;
            ChildPrefabs = new GunBaseChildData[gUpgrade.Destiny.Length];

            for (int i = 0; i < gUpgrade.Destiny.Length; i++)
            {
                if (!gUpgrade.Destiny[i].Part.GetType().IsSubclassOf(typeof(GUpgrade))) continue;

                ChildPrefabs[i] = new GunBaseChildData(gUpgrade.Destiny[i].Part as GUpgrade);
            }
        }

        public GPart Spawn(Transform parentTransfrom)
        {
            GUpgrade gUpgrade = (UpgradeManager.GetUpgradeById(UpgradeId) as UpgradeWithPart).InstantiatePrefab().GetComponent<GUpgrade>();
            gUpgrade.NetworkObject.AutoObjectParentSync = false;
            gUpgrade.transform.SetParent(parentTransfrom, false);

            for (int i = 0; i < ChildPrefabs.Length; i++)
            {
                GBDSpawnShared(ChildPrefabs[i], gUpgrade.Destiny[i]);
            }
            return gUpgrade;
        }
        public GPart NetworkSpawn(Transform parentTransfrom)
        {
            GUpgrade gUpgrade = (UpgradeManager.GetUpgradeById(UpgradeId) as UpgradeWithPart).InstantiatePrefab().GetComponent<GUpgrade>();

            gUpgrade.NetworkObject.Spawn();



            Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
            var parent = parentTransfrom.GetComponent<NetworkObject>();
            if (!gUpgrade.NetworkObject.AutoObjectParentSync)
            {
                Debug.Log("1");
            }

            if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsListening)
            {
                Debug.Log("2");
            }

            if (!NetworkManager.Singleton.IsServer)
            {
                Debug.Log("3");
            }

            if (!gUpgrade.IsSpawned)
            {
                Debug.Log("4");
            }

            if (parent != null && !parent.IsSpawned)
            {
                Debug.Log("5");
            }




            gUpgrade.NetworkObject.TrySetParent(parentTransfrom, false);

            for (int i = 0; i < ChildPrefabs.Length; i++)
            {
                GBDNetworkSpawnShared(ChildPrefabs[i], gUpgrade.Destiny[i]);
            }
            return gUpgrade;
        }
        //testing purposes

        public GunBaseChildData(int upgradeId, GunBaseChildData[] chPrefab)
        {
            UpgradeId = upgradeId;
            ChildPrefabs = chPrefab;
        }
    }

    //Helper func for GBD classes
    private static void GBDSpawnShared(GunBaseChildData childData, GDestiny desitny)
    {
        if (!childData.IsUnityNull())
        {
            desitny.Part = childData.Spawn(desitny.Position);
        }
        else
        {
            MuzzleInstantiateOnDestiny(desitny);
        }
    }
    private static void GBDNetworkSpawnShared(GunBaseChildData childData, GDestiny desitny)
    {
        if (!childData.IsUnityNull())
        {
            desitny.Part = childData.NetworkSpawn(desitny.Position);
        }
        else
        {
            NetworkMuzzleInstantiateOnDestiny(desitny);
        }
    }
}
