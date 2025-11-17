using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditAdditionManager : Singleton<EditAdditionManager> 
{
    [HideInInspector] public LevelEditManager editor;
    UnitData[] unitDataPrefabs = null;
    [HideInInspector] public int selectedUnitIndex = 0;

    private void Start()
    {
        SetUp();
    }

    private void SetUp()
    {
        unitDataPrefabs = Resources.LoadAll<UnitData>(Paths.UnitDataFolderPath);
        if(unitDataPrefabs == null ) { Debug.LogError("No unit data found"); }
    }

    public void AddGround()
    {
        (int q, int r) = GetCoordsFromMousePos();

        GridTile tileToPlace = new(q, r);
        PlaceTileCommand c = new PlaceTileCommand(tileToPlace);
        c.Execute();
    }

    public void AddUnit()
    {
        if(unitDataPrefabs == null) { return; }
        if(selectedUnitIndex >= unitDataPrefabs.Length) { return; }

        (int q, int r) = GetCoordsFromMousePos();

        UnitData newUnitData = Instantiate(unitDataPrefabs[selectedUnitIndex]);
        PlaceUnitCommand c = new PlaceUnitCommand(q, r, newUnitData);
        c.Execute();
    }

    private (int q, int r) GetCoordsFromMousePos()
    {
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        (int _q, int _r, _) = GridLayoutRules.GetTileCoordsFromPositionFlatTop(editor.LevelBeingEdited.layoutData, worldPos);
        return (_q, _r);
    }
}
