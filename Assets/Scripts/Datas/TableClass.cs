using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitData 
{
    public int Id { get; set; }
    
    public int UnitImgId { get; set; }
    
    public float Hpmax { get; set; }

    public int moveRange { get; set; }
    
    public string ownSkills { get; set; }

    public string GetValues(StatuType type)
    {
        switch (type)
        {
            case StatuType.Hp: return Hpmax.ToString(); 
            default:
                return "0";
        }
    }
}
