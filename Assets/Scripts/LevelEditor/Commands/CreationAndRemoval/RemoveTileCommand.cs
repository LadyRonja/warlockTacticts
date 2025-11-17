using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveTileCommand : Command
{
    GridTile oldTile = null;
    int q;
    int r;

    public RemoveTileCommand(int q, int r)
    {
        this.q = q;
        this.r = r;
    }

    public override void Execute()
    {
        base.Execute();

        LevelEditManager editor = LevelEditManager.Instance;

        if(editor.LevelBeingEdited.tiles.TryGetValue(GridTile.GetStringFromCoords(q,r), out oldTile))
        {
            editor.RemoveTile(q, r);
        }
    }

    public override void Undo()
    {
        if(oldTile == null) { return; }

        LevelEditManager.Instance.CreateTile(q, r, oldTile);
    }
}
