using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    private readonly Dictionary<Vector2Int, MapGrid> grids = new Dictionary<Vector2Int, MapGrid>();
        
    public void RegisterGrid(MapGrid grid)
    {
        if (grid == null)
        {
            return;
        }

        grids[new Vector2Int(grid.x, grid.y)] = grid;
    }

    public void UnregisterGrid(MapGrid grid)
    {
        if (grid == null)
        {
            return;
        }

        Vector2Int position = new Vector2Int(grid.x, grid.y);
        if (grids.TryGetValue(position, out MapGrid registeredGrid) && registeredGrid == grid)
        {
            grids.Remove(position);
        }
    }

    public void ClearGrids()
    {
        grids.Clear();
    }

    public MapGrid GetGrid(Vector2Int position)
    {
        return grids.TryGetValue(position, out MapGrid grid) ? grid : null;
    }

    public MapGrid GetGrid(int x, int y)
    {
        return GetGrid(new Vector2Int(x, y));
    }
}