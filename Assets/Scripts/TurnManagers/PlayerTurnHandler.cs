using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerTurnHandler : Singleton<PlayerTurnHandler>
{
    bool inputEnabled = true;
    Unit selectedUnit = null;

    CancellationTokenSource cts = new CancellationTokenSource();

    private void OnDestroy()
    {
        cts.Cancel();
    }

    void Update()
    {
        if (!inputEnabled) return;

        if (Input.GetMouseButtonDown((int)MouseButton.Left))
        {
            OnLeftclick();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnSpaceDown();
        }
    }

    private void OnLeftclick()
    {
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if(!GridInformant.Instance.TryGetTileFromWorldPos(worldPos, out GridTile clickedTile)) { return; } // If  a tile was not clicked, exit function

        // If there is no selected unit, try and set one
        if(selectedUnit == null)
        {
            //Debug.Log("Trying to set unit");
            TrySetSelectedUnit(clickedTile);
            return;
        }

        // If the selected unit is controlled by the player, try to move it
        if (selectedUnit.data.controlledByPlayer)
        {
            //Debug.Log("Trying to move unit");
            TryToMoveSelectedUnit(clickedTile);
            return;
        }
        else {
            //Debug.Log("Deselecting enemy unit");
            selectedUnit= null;
            return;
        }
    }

    private void OnSpaceDown()
    {
        EndTurn();
    }

    public void EndTurn()
    {
        inputEnabled = false;
        TurnManger.Instance.SetTurn(TurnManger.TurnTakers.ENEMY);
    }

    public void StartTurn()
    {
        inputEnabled = true;
    }

    private void TrySetSelectedUnit(GridTile fromTile)
    {
        _ = GridInformant.Instance.TryGetUnit(fromTile, out selectedUnit);
    }

    private async void TryToMoveSelectedUnit(GridTile toTile)
    {
        inputEnabled = false;
        if(!GridInformant.Instance.TryGetTileFromUnit(selectedUnit.data, out GridTile fromTile)) { inputEnabled = true; return; }

        List<GridTile> path = Pathfinder.FindPath(fromTile, toTile);
        if(path == null) { inputEnabled = true; selectedUnit = null; return; }
        if(path.Count == 0) { inputEnabled = true; selectedUnit = null; return; }
        
        await StartMovement(selectedUnit, path, cts.Token);
    }

    public async UniTask StartMovement(Unit unitToMove, List<GridTile> path, CancellationToken token)
    {
        try
        {
            await UnitMovementHandler.Instance.MovePath(selectedUnit, path, token);
        }
        catch (OperationCanceledException)
        {
            Debug.Log("Task cancelled");
        }

        selectedUnit = null;
        inputEnabled = true;
    }
}
