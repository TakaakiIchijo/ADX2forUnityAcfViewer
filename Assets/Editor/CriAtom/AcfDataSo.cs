using System.Collections.Generic;
using UnityEngine;
using ACFDataClass;

public class AcfDataSo : ScriptableObject
{
    public List<CategoryInfo> categoryInfoList = new List<CategoryInfo>();
    public List<AisacControlInfo> aisacControlInfoList = new List<AisacControlInfo>();
}

namespace ACFDataClass
{
    [System.Serializable]
    public class CategoryInfo
    {
        public string name;
        public uint groupNo;
        public uint id;
        public uint numCueLimits;
        public float volume;

        public CategoryInfo(uint groupNo, uint id, string name, uint numCueLimits, float volume)
        {
            this.groupNo = groupNo;
            this.id = id;
            this.name = name;
            this.numCueLimits = numCueLimits;
            this.volume = volume;
        }
    }
    
    [System.Serializable]
    public class AisacControlInfo
    {
        public string name;
        public uint id;

        public AisacControlInfo(string name, uint id)
        {
            this.name = name;
            this.id = id;
        }
    }
}