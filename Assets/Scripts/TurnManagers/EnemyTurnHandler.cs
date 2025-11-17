using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using System;

public class EnemyTurnHandler : Singleton<EnemyTurnHandler>
{
    CancellationTokenSource cts = new CancellationTokenSource();

    private void OnDestroy()
    {
        cts.Cancel();
    }

    public async void StartTurn()
    {
        await MoveEachAIUnit(cts.Token);
    }

    public void EndTurn()
    {
        TurnManger.Instance.SetTurn(TurnManger.TurnTakers.PLAYER);
    }

    async UniTask MoveEachAIUnit(CancellationToken token)
    {
        List<Unit> allAIUnits = FindObjectsOfType<Unit>().Where(u => !u.data.controlledByPlayer).ToList();

        try
        {
            foreach (Unit unit in allAIUnits)
            {
                List<GridTile> pathToClosestPlayernit = UnitMovementBehaivor.GetPathToNearestPlayerControlledUnit(unit);

                await UnitMovementHandler.Instance.MovePath(unit, pathToClosestPlayernit, token);
            }
        }
        catch (OperationCanceledException)
        {
            Debug.Log("Tasks cancelled");
        }

        EndTurn();
    }
}
