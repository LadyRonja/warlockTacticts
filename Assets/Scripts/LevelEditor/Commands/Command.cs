using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Command
{
    public virtual void Execute()
    {
        CommandManager.Instance.AddToHistory(this);
    }
    public abstract void Undo();
}
