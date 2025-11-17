using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveLevelManager : Singleton<ActiveLevelManager>
{
    LevelData activeLevel;
    public LevelData ActiveLevel { get => activeLevel; private set => activeLevel = value; }

    private void Start()
    {
        if (activeLevel == null)
        {
            Debug.Log("Level Manager requires data setup");
        }
    }
    public void SetActiveGrid(LevelData levelToSetActive)
    {
        activeLevel = levelToSetActive;
    }

    public bool TryMoveUnit(UnitData unit, GridTile toTile, bool force = false)
    {
        if (activeLevel == null) { return false; }
        if (GridInformant.instance.TryGetUnit(toTile.q, toTile.r, out _) && !force) { return false; }
        if (!GridInformant.instance.TryGetTileFromUnit(unit, out GridTile fromTile)){ return false; }

        activeLevel.units.Remove(fromTile.coords);
        activeLevel.units.Add(toTile.coords, unit);
        return true;
    }
}
