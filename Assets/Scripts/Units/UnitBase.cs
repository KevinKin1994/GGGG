using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UnitBase
{
    public UnitData ownUnitData;

    public UnitStatus OwnUnitStatus;

    public UnitMove OwnUnitMove;
    
    public static event Action moveEvent;

    public void Move(MapGrid target)
    {
        if (OwnUnitMove != null)
        {
            OwnUnitMove.Move(target);
            moveEvent?.Invoke();
        }
        
    }
}
