using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGrid : MonoBehaviour
{
    public int x { get; private set; }
    
    public int y { get; private set; }

    public bool CouldUnitPass;

    public MapGridType GridType;

    public UnitBase occupiedUnit;

    public void SetPosition(int gridX, int gridY)
    {
        x = gridX;
        y = gridY;
    }
    
    public void OnUnitPass()
    {
        
    }
    
}

public enum MapGridType
{
    Normal,
    Grass,
    Burning
}
