using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EditSelectionManager : Singleton<EditSelectionManager>
{
    [HideInInspector] public LevelEditManager editor;

    [HideInInspector] public HashSet<(int q,int r)> selectedLocations = new();
    [HideInInspector] public HashSet<GridLayers> selectedLayers = new();

    public void SelectUnit()
    {
        (int q, int r) = GetCoordsFromMousePos();
        SelectUnit(q, r);
    }
    private void SelectUnit(int q, int r)
    {
        if(!GridInformant.Instance.TryGetUnit(q, r, out _))
        {
            DeselectCommand dc = new DeselectCommand();
            dc.Execute();
            return;
        }

        HashSet<(int q, int r)> newLocation = new()  {  (q, r)  };

        HashSet<GridLayers> newLayers = new(){ GridLayers.UNIT };

        SelectCommand c = new SelectCommand(newLocation, newLayers);
        c.Execute();
    }

    public void SelectTerrain()
    {
        (int q, int r) = GetCoordsFromMousePos();
        SelectTerrain(q, r);
    }

    private void SelectTerrain(int q, int r)
    {
        if (!GridInformant.Instance.TileExists(q, r, out _))
        {
            DeselectCommand dc = new DeselectCommand();
            dc.Execute();
            return;
        }

        HashSet<(int q, int r)> newLocation = new() { (q, r) };
        HashSet<GridLayers> newLayers = new() { GridLayers.TERRAIN };

        SelectCommand c = new SelectCommand(newLocation, newLayers);
        c.Execute();
    }


    private (int q, int r) GetCoordsFromMousePos()
    {
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        (int _q, int _r, _) = GridLayoutRules.GetTileCoordsFromPositionFlatTop(editor.LevelBeingEdited.layoutData, worldPos);
        return (_q, _r);
    }
}
