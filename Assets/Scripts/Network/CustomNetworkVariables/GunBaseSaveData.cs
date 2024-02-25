using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using static PlayerGunManager;


public class GunBaseSaveData : INetworkSerializable
{
    private static string _basePrefab = "Prefabs/GunParts/GBase";

    private GunBaseChildData _childGBCD;

    public GunBaseChildData Child
    {
        get { return _childGBCD; }
    }

    //Save from gBase gameobject
    public GunBaseSaveData(GBase gBase)
    {
        if (!gBase.Destiny.Part.GetType().IsSubclassOf(typeof(GUpgrade))) return;

        _childGBCD = new GunBaseChildData(gBase.Destiny.Part as GUpgrade);
    }

    public GBase Spawn()
    {
        GameObject gBasePrefab = Resources.Load(_basePrefab) as GameObject;
        GBase gBase = Object.Instantiate(gBasePrefab).GetComponent<GBase>();
        gBase.NetworkObject.AutoObjectParentSync = false;

        GBDSpawnShared(_childGBCD, gBase.Destiny);

        return gBase;
    }
    public GBase NetworkSpawn()
    {
        GameObject gBasePrefab = Resources.Load(_basePrefab) as GameObject;
        GBase gBase = Object.Instantiate(gBasePrefab).GetComponent<GBase>();

        gBase.NetworkObject.Spawn(true);

        GBDNetworkSpawnShared(_childGBCD, gBase.Destiny);

        return gBase;
    }
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        if (serializer.IsReader)
        {
            var reader = serializer.GetFastBufferReader();
            reader.ReadValueSafe(out string serializeText);

            //from string to GBSD
            _childGBCD = ParseText(serializeText, this);
            //~
        }
        else
        {
            //to string, fomrat: "1{101{1{11}11{111,11111},1}"
            string serializeText = ParseToText(_childGBCD);
            //~

            var writer = serializer.GetFastBufferWriter();
            writer.WriteValueSafe(serializeText);
        }
    }

    public static GunBaseChildData ParseText(string text, GunBaseSaveData parent)
    {
        if (text.IndexOf("{") == -1) return null;

        var val = RecursiveAssignGBCD(text);
        val.PreviousPart = null;

        return val; 

        GunBaseChildData RecursiveAssignGBCD(string textInner)
        {
            Debug.Log("----");
            Debug.Log(textInner);
            int upId = int.Parse(textInner.Substring(0, textInner.IndexOf("{")));
            int arrSize = (UpgradeManager.GetUpgradeById(upId) as UpgradeWithPart).GetBranchCount();
            (int from, int to)[] parts = new (int from, int to)[arrSize];

            GunBaseChildData output = new(upId, new GunBaseChildData[arrSize], parent);

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

                output.ChildGBCDs[i] = RecursiveAssignGBCD(shortenedText);
                output.ChildGBCDs[i].PreviousPart = output;
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
            for (int i = 0; i < gbcdInner.ChildGBCDs.Length; i++)
            {
                if (gbcdInner.ChildGBCDs[i] != null)
                {
                    RecursiveAssingText(gbcdInner.ChildGBCDs[i]);
                }
                output += ",";
            }
            output += "}";
        }
    }

    //testing purposes 1 // "1{1{,,},1{,,},}," -- in text

    public GunBaseSaveData(GunBaseChildData chPrefab)
    {
        _childGBCD = chPrefab;
    }
    //Has no upgrades
    public GunBaseSaveData() { }

    public class GunBaseChildData
    {
        public GunBaseChildData? PreviousPart;
        public int UpgradeId;
        public GunBaseChildData[] ChildGBCDs;
        public GunBaseChildData(GUpgrade gUpgrade)
        {
            UpgradeId = gUpgrade.UpgradeId;
            ChildGBCDs = new GunBaseChildData[gUpgrade.Destiny.Length];

            for (int i = 0; i < gUpgrade.Destiny.Length; i++)
            {
                if (!gUpgrade.Destiny[i].Part.GetType().IsSubclassOf(typeof(GUpgrade))) continue;

                ChildGBCDs[i] = new GunBaseChildData(gUpgrade.Destiny[i].Part as GUpgrade);
            }
        }

        public GPart Spawn(Transform parentTransfrom)
        {
            GUpgrade gUpgrade = (UpgradeManager.GetUpgradeById(UpgradeId) as UpgradeWithPart).InstantiatePrefab().GetComponent<GUpgrade>();
            gUpgrade.NetworkObject.AutoObjectParentSync = false;
            gUpgrade.transform.SetParent(parentTransfrom, false);

            //get previous upgrade parts
            var prevPart = PreviousPart;
            List<int> prevIds = new List<int>();
            while (prevPart != null)
            {
                prevIds.Add(PreviousPart.UpgradeId);
                prevPart = prevPart.PreviousPart;
            }

            for (int i = 0; i < ChildGBCDs.Length; i++)
            {
                gUpgrade.Destiny[i].PreviousUpgradeIds = prevIds.ToArray();
                GBDSpawnShared(ChildGBCDs[i], gUpgrade.Destiny[i]);
            }
            return gUpgrade;
        }
        public GPart NetworkSpawn(GDestiny parentDestiny)
        {
            GUpgrade gUpgrade = (UpgradeManager.GetUpgradeById(UpgradeId) as UpgradeWithPart).InstantiatePrefab().GetComponent<GUpgrade>();

            gUpgrade.NetworkObject.Spawn(true);

            GameObject parentGO = parentDestiny.PositionPoint.Parent;

            if(parentGO.GetComponent<GBase>() != null)
            {
                parentDestiny.PositionPoint.Parent.GetComponent<GBase>().NetworkAddParentOnDestiny(gUpgrade.NetworkObjectId);
            }
            else if (parentGO.GetComponent<GUpgrade>() != null)
            {
                parentDestiny.PositionPoint.Parent.GetComponent<GUpgrade>().NetworkAddParentOnDestiny(parentDestiny.PositionPoint.DestinyIndex, gUpgrade.NetworkObjectId);
            }
            else
            {
                Debug.LogError("Neco je zle");
            }

                
            //gUpgrade.NetworkObject.TrySetParent(parentTransfrom, false);

            for (int i = 0; i < ChildGBCDs.Length; i++)
            {
                GBDNetworkSpawnShared(ChildGBCDs[i], gUpgrade.Destiny[i]);
            }
            return gUpgrade;
        }
        //testing purposes

        public GunBaseChildData(int upgradeId, GunBaseChildData[] chGBCD, GunBaseSaveData parent)
        {
            UpgradeId = upgradeId;
            ChildGBCDs = chGBCD;
            PreviousPart = null;
        }
        //testing purposes

        public GunBaseChildData(int upgradeId, GunBaseChildData[] chGBCD, GunBaseChildData parent)
        {
            UpgradeId = upgradeId;
            ChildGBCDs = chGBCD;
            PreviousPart = parent;
        }
    }

    //Helper func for GBD classes
    private static void GBDSpawnShared(GunBaseChildData childData, GDestiny desitny)
    {
        if (!childData.IsUnityNull())
        {
            desitny.Part = childData.Spawn(desitny.PositionPoint.transform);
        }
        else
        {
            MuzzleManager.MuzzleInstantiateOnDestiny(desitny);
        }
    }
    private static void GBDNetworkSpawnShared(GunBaseChildData childData, GDestiny desitny)
    {
        if (!childData.IsUnityNull())
        {
            desitny.Part = childData.NetworkSpawn(desitny);
        }
        else
        {
            MuzzleManager.NetworkMuzzleInstantiateOnDestiny(desitny);
        }
    }
}
