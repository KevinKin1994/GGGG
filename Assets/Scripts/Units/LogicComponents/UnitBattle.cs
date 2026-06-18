using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UnitBattle : UnitLogicBase
{
    public float hp { get; private set; }
    public float ap { get; private set; }

    protected Dictionary<StatuType, string> battleStatus;
    
    public void Init()
    {
        battleStatus = new Dictionary<StatuType, string>(ownerUnit.OwnUnitStatus.allStatus);

        UnitBase.moveEvent += OnMove;
    }

    public T GetValue<T>(StatuType type)
    {
        return PublicFunctions.GetValueFromDict<T, StatuType>(battleStatus, type);
    }

    public void ChangeValue<T>(StatuType type,T value)
    {
        PublicFunctions.ChangeValueForDict<StatuType,T>(battleStatus,type,value);
    }

    public void OnMove()
    {
        ap--;
    }
    


    public void OnBeHit(string damage)
    {
        hp -= float.Parse(damage);
        if (hp <= 0 )
        {
            Die();
        }
    }

    public void Die()
    {
        
    }
    
}
