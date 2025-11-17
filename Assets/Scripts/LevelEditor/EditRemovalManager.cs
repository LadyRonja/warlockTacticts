using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditRemovalManager : Singleton<EditRemovalManager>
{
    [HideInInspector] public LevelEditManager editor;

    public void RemoveGround()
    {
        (int q, int r) = GetCoordsFromMousePos();

        RemoveTileCommand c = new(q, r);
        c.Execute();
    }

    public void RemoveUnit()
    {
        (int q, int r) = GetCoordsFromMousePos();

        RemoveUnitCommand c = new(q, r);
        c.Execute();
    }

    private (int q, int r) GetCoordsFromMousePos()
    {
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        (int _q, int _r, _) = GridLayoutRules.GetTileCoordsFromPositionFlatTop(editor.LevelBeingEdited.layoutData, worldPos);
        return (_q, _r);
    }
}
