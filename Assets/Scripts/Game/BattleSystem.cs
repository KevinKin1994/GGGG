using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    public static BattleSystem Instance;
    
    private BattleStep currentStep;

    private List<UnitBase> battleUnits = new List<UnitBase>();
    private void Awake()
    {
        Instance = this;
        
        //读取队伍

        // 读取地图

        // 读取怪物
    }

    IEnumerator Battle()
    {
        while (true)
        {
            switch (currentStep)
            {
                case BattleStep.ChooseActor:
                    OnStep_ChooseActor();
                    break;
                
            }
        }
        yield return null;
    }

    public void NextStep(BattleStep next)
    {
        
    }

    public void OnStep_ChooseActor()
    {
        battleUnits.Sort((a, b) =>
            a.OwnUnitBattle.GetValue<float>(StatuType.currentAC).CompareTo(b.OwnUnitBattle.GetValue<float>(StatuType.currentAC)));

        UnitBase actor = battleUnits[0];

        for (int i = 0; i < battleUnits.Count; i++)
        {
            
        }


    }
    
    public enum BattleStep
    {
        ChooseActor,
    }
}
