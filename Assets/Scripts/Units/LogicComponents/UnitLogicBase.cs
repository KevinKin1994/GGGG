using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitLogicBase : MonoBehaviour
{
    protected UnitBase ownerUnit;

    public virtual void Init(UnitBase unit) => ownerUnit = unit;
}
