using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SelectCommand : Command
{
    HashSet<(int q, int r)> locations;
    HashSet<GridLayers> layers;

    HashSet<(int q, int r)> oldLocations;
    HashSet<GridLayers> oldLayers;

    UnitData oldData = null;

    delegate void HighlightFunction(int q, int r);
    static Dictionary<GridLayers, HighlightFunction> LayerToHighligthFunctionLookUp = new()
    {
        { GridLayers.UNIT, SelectorHighlightManager.Instance.SetHighlightUnit },
        { GridLayers.TERRAIN, SelectorHighlightManager.Instance.SetHighlightTerrain }
    };

    public SelectCommand(HashSet<(int q, int r)> locations, HashSet<GridLayers> layers)
    {
        this.locations = locations;
        this.layers = layers;
    }

    public override void Execute()
    {
        base.Execute();

        oldLocations = EditSelectionManager.Instance.selectedLocations;
        oldLayers = EditSelectionManager.Instance.selectedLayers;

        if (oldLocations.Count == 1 && oldLayers.Contains(GridLayers.UNIT))
        {
            (int oldQ, int oldR) = oldLocations.First(a => true);
            if (GridInformant.Instance.TryGetUnit(oldQ, oldR, out Unit worldUnit))
            {
                oldData = worldUnit.data;
            }
        }

        SelectorHighlightManager.Instance.DeselectAll();

        foreach (var pos in locations)
        {
            foreach (var layer in layers)
            {
                if(LayerToHighligthFunctionLookUp.TryGetValue(layer, out HighlightFunction functionToCall))
                {
                    functionToCall(pos.q, pos.r);

                    if(locations.Count == 1 && layers.Contains(GridLayers.UNIT))
                    {
                        if (LevelEditManager.Instance.LevelBeingEdited.units.TryGetValue(GridTile.GetStringFromCoords(pos.q, pos.r), out UnitData ud))
                        {
                            EditUnitDataManager.Instance.DisplayData(ud);
                            EditorUnitDataUI.Instance.DisplayUI();
                        }
                        else { EditorUnitDataUI.Instance.HideUI(); }
                    }
                    else { EditorUnitDataUI.Instance.HideUI(); }
                }
            }
        }

        EditSelectionManager.Instance.selectedLocations = locations;
        EditSelectionManager.Instance.selectedLayers = layers;
    }
    public override void Undo()
    {
        SelectorHighlightManager.Instance.DeselectAll();
        
        EditorUnitDataUI.Instance.HideUI();

        EditSelectionManager.Instance.selectedLocations = new();
        EditSelectionManager.Instance.selectedLayers = new();

        if (oldLayers == null || oldLayers.Count == 0) { return; }

        foreach (var pos in oldLocations)
        {
            foreach (var layer in oldLayers)
            {
                if (LayerToHighligthFunctionLookUp.TryGetValue(layer, out HighlightFunction functionToCall))
                {
                    functionToCall(pos.q, pos.r);
                }
            }
        }

        if (oldData != null)
        {
            EditUnitDataManager.Instance.DisplayData(oldData);
            EditorUnitDataUI.Instance.DisplayUI();
        }

        EditSelectionManager.Instance.selectedLocations = oldLocations;
        EditSelectionManager.Instance.selectedLayers = oldLayers;
    }
}
