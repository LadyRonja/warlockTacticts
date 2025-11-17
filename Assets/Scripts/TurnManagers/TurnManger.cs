using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManger : Singleton<TurnManger>
{
    public enum TurnTakers { PLAYER, ENEMY }

    public void SetTurn(TurnTakers toWhom)
    {
        switch (toWhom)
        {
            case TurnTakers.PLAYER:
                PlayerTurnHandler.Instance.StartTurn();
                break;
            case TurnTakers.ENEMY:
                EnemyTurnHandler.Instance.StartTurn();
                break;
            default:
                Debug.LogError("Reached default in enum switch statement, missing cases?");
                break;
        }
    }
}
