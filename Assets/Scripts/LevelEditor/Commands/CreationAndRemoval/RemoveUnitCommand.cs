using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class RemoveUnitCommand : Command
{
    int q;
    int r;
    UnitData oldData;

    public RemoveUnitCommand(int q, int r)
    {
        this.q = q;
        this.r = r;
    }

    public override void Execute()
    {
        base.Execute();

        LevelEditManager editor = LevelEditManager.Instance;

        if (editor.LevelBeingEdited.units.TryGetValue(GridTile.GetStringFromCoords(q, r), out oldData))
        {
            editor.RemoveUnit(q, r);
        }
    }

    public override void Undo()
    {
        if (oldData == null) { return; }

        LevelEditManager.Instance.CreateUnit(q, r, oldData);
    }
}
