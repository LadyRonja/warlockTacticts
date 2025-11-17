using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class PlaceTileCommand : Command
{
    GridTile tileToPlace;
    GridTile oldTile = null;

    public PlaceTileCommand(GridTile tile)
    {
        this.tileToPlace = tile;
    }

    public override void Execute()
    {
        base.Execute();

        LevelEditManager editor = LevelEditManager.Instance;

        _ = editor.LevelBeingEdited.tiles.TryGetValue(tileToPlace.coords, out oldTile);

        editor.RemoveTile(tileToPlace.q, tileToPlace.r);
        editor.CreateTile(tileToPlace.q, tileToPlace.r, tileToPlace);
    }

    public override void Undo()
    {
        LevelEditManager.Instance.RemoveTile(tileToPlace.q, tileToPlace.r);
        if(oldTile != null)
        {
            LevelEditManager.Instance.CreateTile(oldTile.q, oldTile.r, oldTile);
        }
    }
}
