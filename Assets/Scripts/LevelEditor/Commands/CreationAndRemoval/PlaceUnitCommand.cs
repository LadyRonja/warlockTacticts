using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PlaceUnitCommand : Command
{
    int q;
    int r;
    UnitData data;
    UnitData oldData;

    public PlaceUnitCommand(int q, int r, UnitData data)
    {
        this.q = q;
        this.r = r;
        this.data = data;
    }

    public override void Execute()
    {
        base.Execute();

        LevelEditManager editor = LevelEditManager.Instance;

        string coords = GridTile.GetStringFromCoords(q, r);
        _ = editor.LevelBeingEdited.units.TryGetValue(coords, out oldData);

        editor.CreateUnit(q, r, data);
    }

    public override void Undo()
    {
        LevelEditManager.Instance.RemoveUnit(q, r);
        if (oldData == null) { return; }
        LevelEditManager.Instance.CreateUnit(q, r, oldData);
    }
}
