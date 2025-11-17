using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum NeighbourDirections { NW, N, NE, SE, S, SW}


public class GridInformant : Singleton<GridInformant>
{
    LevelData activeLevel;

    Dictionary<NeighbourDirections, (int q, int r)> relationalCoordsLookupTable = new(){
                                                    { NeighbourDirections.NW, (-1, 1) },
                                                    { NeighbourDirections.N, (0, 1) },
                                                    { NeighbourDirections.NE, (1, 0) },
                                                    { NeighbourDirections.SE, (1, -1) },
                                                    { NeighbourDirections.S, (0, -1) },
                                                    { NeighbourDirections.SW, (-1, 0) }
    };

    private void Start()
    {
        if (activeLevel == null)
        {
            Debug.Log("Grid Informant requires data setup");
        }
    }

    public void SetActiveGrid(LevelData levelToSetActive)
    {
        activeLevel = levelToSetActive;
    }

    #region Tile data
    public bool TryGetNeighbourTile(int q, int r, NeighbourDirections inDirection, out GridTile neighbour)
    {
        neighbour = null;
        if(activeLevel == null) { return false; }

        (int qDir, int rDir) = relationalCoordsLookupTable.GetValueOrDefault(inDirection);
        string neighbourCoords = GridTile.GetStringFromCoords(q + qDir, r + rDir);
        return activeLevel.tiles.TryGetValue(neighbourCoords, out neighbour);
    }

    public bool TryGetNeighbourTile(GridTile ofTile, NeighbourDirections inDirection, out GridTile neighbour)
    {
        return TryGetNeighbourTile(ofTile.q, ofTile.r, inDirection, out neighbour);
    }

    public List<GridTile> GetAllNeighbourTiles(int q, int r)
    {
        if(activeLevel == null) { return new(); }

        List<GridTile> output = new();
        NeighbourDirections[] directions = System.Enum.GetValues(typeof(NeighbourDirections)) as NeighbourDirections[];

        foreach (var dir in directions)
        {
            if(TryGetNeighbourTile(q, r, dir, out GridTile foundNeighbour))
            {
                output.Add(foundNeighbour);
            }
        }

        return output;
    }
    public List<GridTile> GetAllNeighbourTiles(GridTile ofTile)
    {
        return GetAllNeighbourTiles(ofTile.q, ofTile.r);
    }

    public bool TileExists(int q, int r, out GridTile tile)
    {
        tile = null;
        if(activeLevel== null) { return false; }

        return activeLevel.tiles.TryGetValue(GridTile.GetStringFromCoords(q, r), out tile);
    }

    public bool TryGetTileFromWorldPos(Vector2 worldPos, out GridTile tile)
    {
        tile = null;
        if(activeLevel == null) {return false;}

        // Convert worldPos to tile coords
        (int q, int r, _) = GridLayoutRules.GetTileCoordsFromPositionFlatTop(activeLevel.layoutData, worldPos);
        string coordString = GridTile.GetStringFromCoords(q, r);

        return activeLevel.tiles.TryGetValue(coordString, out tile);
    }

    public Vector2 GetPositionWorldFromTile(GridTile tile)
    {
        if(activeLevel == null) { Debug.LogError("Active level not set"); return Vector2.zero; }

        return GridLayoutRules.GetPositionForFlatTopTile(activeLevel.layoutData, tile.q, tile.r);
    }

    public bool TryGetTileFromUnit(UnitData fromUnit, out GridTile tile)
    {
        tile = null;
        if(activeLevel == null) { return false;}
       
        // FirstOrDefault retruns a non-nullable value and thus it must be enusred the value exists before proceeding.
        if(!activeLevel.units.ContainsValue(fromUnit)) { return false; }
        string unitPos = activeLevel.units.First(u => u.Value == fromUnit).Key;

        #region Silly but more optimized method
        /*bool found = false;
        string unitPos = activeLevel.units.FirstOrDefault(u => u.Value == fromUnit && (found = true)).Key;
        if (found)
        {
            return activeLevel.tiles.TryGetValue(unitPos, out tile);
        }
        else
        {
            return false;
        }*/
        #endregion

        return activeLevel.tiles.TryGetValue(unitPos, out tile);

    }
    #endregion

    #region Unit data
    public bool TryGetUnit(int q, int r, out Unit unit)
    {
        unit = null;
        if(activeLevel == null) { return false; }


        if(activeLevel.units.TryGetValue(GridTile.GetStringFromCoords(q ,r), out UnitData ud))
        {
            unit = FindObjectsOfType<Unit>().FirstOrDefault(u => u.data.unitIDforClonedData == ud.unitIDforClonedData);
            if(unit == null) { Debug.LogError("Unit does not exist in-world"); Debug.Log($"Data: {ud.unitIDforClonedData}"); return false; }
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool TryGetUnit(GridTile onTile, out Unit unit)
    {
        unit = null;
        return TryGetUnit(onTile.q, onTile.r, out unit);
    }

    public bool TryGetUnit(UnitData unitData, out Unit unit)
    {
        unit = FindObjectsOfType<Unit>().FirstOrDefault(u => u.data.unitIDforClonedData == unitData.unitIDforClonedData);
        if (unit == null) { Debug.LogError("Unit does not exist in-world"); return false; }
        return true;
    }
    #endregion
}
