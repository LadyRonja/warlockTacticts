using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeselectCommand : Command
{
    HashSet<(int q, int r)> oldLocations;
    HashSet<GridLayers> oldLayers;
    UnitData oldData = null;

    delegate void HighlightFunction(int q, int r);
    static Dictionary<GridLayers, HighlightFunction> LayerToHighligthFunctionLookUp = new()
    {
        { GridLayers.UNIT, SelectorHighlightManager.Instance.SetHighlightUnit },
        { GridLayers.TERRAIN, SelectorHighlightManager.Instance.SetHighlightTerrain }
    };

    public override void Execute()
    {
        base.Execute();

        oldLocations = EditSelectionManager.Instance.selectedLocations;
        oldLayers = EditSelectionManager.Instance.selectedLayers;

        EditSelectionManager.Instance.selectedLocations = new();
        EditSelectionManager.Instance.selectedLayers = new();

        if(oldLocations.Count == 1 && oldLayers.Contains(GridLayers.UNIT))
        {
            (int oldQ, int oldR) = oldLocations.First(a => true);
            if(GridInformant.Instance.TryGetUnit(oldQ, oldR, out Unit worldUnit))
            {
                oldData = worldUnit.data;
            }
        }

        SelectorHighlightManager.Instance.DeselectAll();
        EditorUnitDataUI.Instance.HideUI();
    }
    public override void Undo()
    {
        if (oldLayers == null || oldLayers.Count == 0)
        {
            EditorUnitDataUI.Instance.HideUI();
            return; 
        }

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

        if(oldData!= null)
        {
            EditUnitDataManager.Instance.DisplayData(oldData);
            EditorUnitDataUI.Instance.DisplayUI();
        }

        EditSelectionManager.Instance.selectedLocations = oldLocations;
        EditSelectionManager.Instance.selectedLayers = oldLayers;
    }
}
