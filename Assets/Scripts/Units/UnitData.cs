using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Unit Data", menuName = "Scriptable Objects/Unit Data", order = 1)]
public class UnitData : IDdScriptableObject
{
    [Header("Game Info")]
    public string unitName = "UNNAMED UNIT DATA";
    public bool controlledByPlayer = true;
    public int maxHealth = 15;
    public int curHealth = 15;
    public int moveRange = 3;
    public float maxMana = 4;
    public float speed = 15f;

    [Header("Development tools")]
    public bool aquireDefaultValueOnLoad = true;
    [HideInInspector] public string unitIDforClonedData = "DO NOT ALTER IN EDITOR";
}
