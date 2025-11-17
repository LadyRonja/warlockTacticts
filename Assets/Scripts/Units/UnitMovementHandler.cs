using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using System;

public class UnitMovementHandler : Singleton<UnitMovementHandler>
{
    public async UniTask MovePath(Unit unitToMove, List<GridTile> path, CancellationToken token)
    {
        try
        {
            for (int i = 0; i < path.Count; i++)
            {
                await MoveStep(unitToMove, path[i], token);
            }
        }
        catch (OperationCanceledException)
        {
            Debug.Log("Tasks cancelled");
        }
    }

    public async UniTask MoveStep(Unit unitToMove, GridTile toTile, CancellationToken token)
    {
        Vector3 startPos = unitToMove.transform.position;
        Vector3 endPos = GridInformant.Instance.GetPositionWorldFromTile(toTile);

        float timeToMove = 0.5f;
        float timePassed = 0;

        while (timePassed < timeToMove)
        {
            try
            {
                unitToMove.transform.position = Vector3.Lerp(startPos, endPos, (timePassed / timeToMove));

                timePassed += Time.deltaTime;
                await UniTask.WaitForEndOfFrame(this);
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Task cancelled");
            }

        }

        unitToMove.transform.position = endPos;
        if (!ActiveLevelManager.Instance.TryMoveUnit(unitToMove.data, toTile))
        {
            Debug.LogError("Unable to move unit along predicted path!");
        }
    }
}
