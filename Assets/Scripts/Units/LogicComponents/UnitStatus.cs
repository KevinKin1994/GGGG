using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitStatus : UnitLogicBase
{
    public Dictionary<StatuType, string> allStatus { get; private set; }
    
    public void Init()
    {
        allStatus = new Dictionary<StatuType, string>();
        for (int i = 0; i < PublicFunctions.GetEnumLength<StatuType>(); i++)
        {
           
            StatuType t = (StatuType)i;
            allStatus.Add(t, ownerUnit.ownUnitData.GetValues(t));
        }

    }
}

public enum StatuType
{
    Hp,
    Mp,
    Atk,
    Spd,
}
