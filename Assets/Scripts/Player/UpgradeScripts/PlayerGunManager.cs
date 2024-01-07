using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public NetworkVariable<GunBaseSaveData> GunCurrentData = new(new GunBaseSaveData(new GunBaseChildData(1, new GunBaseChildData[]{
        new GunBaseChildData(1, new GunBaseChildData[2]),
        new GunBaseChildData(1, new GunBaseChildData[2])
    }))); // "1{1{,,},1{,,},}" -- in text

    private void Start()
    {
        //Debug.Log(GunBaseSaveData.ParseToText(new GunBaseSaveData().Child));
        //Debug.Log(GunBaseSaveData.ParseToText(GunBaseSaveData.ParseText(GunBaseSaveData.ParseToText(new GunBaseSaveData().Child))));
        Debug.Log("original" + GunBaseSaveData.ParseToText(GunCurrentData.Value.Child));
        Debug.Log("new" + GunBaseSaveData.ParseToText(GunBaseSaveData.ParseText(GunBaseSaveData.ParseToText(GunCurrentData.Value.Child))));
    }

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
    public class GunBaseSaveData : INetworkSerializable
    {
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
            GBase gBase = Instantiate(gBasePrefab).GetComponent<GBase>();

            GBDSpawnShared(_childPrefab, gBase.Destiny);

            return gBase;
        }
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            if (serializer.IsReader)
            {
                var reader = serializer.GetFastBufferReader();
                reader.ReadValueSafe(out string serializeText);

                //from string to GBSD
                ParseText(serializeText);
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
            //int upId = int.Parse(text.Substring(0, text.IndexOf("{")));
            //int arrSize = (UpgradeManager.GetUpgradeById(upId) as UpgradeWithPart).GetBranchCount();
            //GunBaseChildData output = new(upId, new GunBaseChildData[arrSize]);

            //int from = text.IndexOf("{") + 1;
            //int to = text.Length;
            //int j = 1;
            //for (int i = from; i < text.Length; i++)
            //{
            //    if(text[i] == '{')
            //    {
            //        j++;
            //    }
            //    else if(text[i] == '}')
            //    {
            //        j--;
            //        if (j == 0)
            //        {
            //            to = i;
            //            break;
            //        }
            //    }
            //}

            //string shortenedText = text.Substring(from, to - from);

            if (text.IndexOf("{") == -1) return null;

            return RecursiveAssignGBCD(text);

            GunBaseChildData RecursiveAssignGBCD(string textInner)
            {
                Debug.Log("----");
                Debug.Log(textInner);
                int upId = int.Parse(text.Substring(0, text.IndexOf("{")));
                int arrSize = (UpgradeManager.GetUpgradeById(upId) as UpgradeWithPart).GetBranchCount();
                (int from, int to)[] parts = new (int from, int to)[arrSize];
                //int from = textInner.IndexOf("{") + 1;
                //int to = textInner.Length;

                GunBaseChildData output = new(upId, new GunBaseChildData[arrSize]);

                textInner = textInner[(textInner.IndexOf("{")+1)..textInner.LastIndexOf("}")];

                int j = 0;
                int l = 0;
                for (int i = 0; i < textInner.Length; i++)
                {
                    if(j == 0 && textInner[i] == ',')
                    {
                        l++;
                    }
                    else if (textInner[i] == '{')
                    {
                        j++;
                        if(j == 1)
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

                Debug.Log("arrSize" + arrSize);
                Debug.Log(textInner);

                for (int i = 0; i < arrSize; i++)
                {
                    Debug.Log(parts[0].from+".."+parts[0].to);
                    Debug.Log(parts[1].from + ".." + parts[1].to);


                    string shortenedText = textInner[parts[i].from..parts[i].to];

                    Debug.Log(textInner[parts[i].to]);
                    Debug.Log(shortenedText);

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
                output += gbcd.UpgradeId.ToString();
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

        //testing purposes 1 // "1{1{,,},1{,,},}" -- in text

        public GunBaseSaveData(GunBaseChildData chPrefab)
        {
            _childPrefab = chPrefab;
        }
        //Has no upgrades
        public GunBaseSaveData() { }
    }

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
                if (!gUpgrade.Destiny[i].Part.GetType().IsSubclassOf(typeof(GUpgrade))) return;

                ChildPrefabs[i] = new GunBaseChildData(gUpgrade.Destiny[i].Part as GUpgrade);
            }
        }

        public GPart Spawn(Transform parentTransfrom)
        {
            GUpgrade gUpgrade = (UpgradeManager.GetUpgradeById(UpgradeId) as UpgradeWithPart).InstantiatePrefab().GetComponent<GUpgrade>();
            gUpgrade.transform.SetParent(parentTransfrom, false);

            for (int i = 0; i < ChildPrefabs.Length; i++)
            {
                GBDSpawnShared(ChildPrefabs[i], gUpgrade.Destiny[i]);
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
