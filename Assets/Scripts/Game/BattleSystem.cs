using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    private BattleStep currentStep;
    private void Awake()
    {
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

    public void OnStep_ChooseActor()
    {
        currentStep = BattleStep.ChooseActor
    }
    
    public enum BattleStep
    {
        ChooseActor,
    }
}
