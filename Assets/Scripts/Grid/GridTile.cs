using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridTile
{
    [JsonConstructor]
    public GridTile(int q, int r, int s)
    {
        // q + r + s = 0
        this.q = q;
        this.r = r;
        this.s = s;
        this.coords = $"{this.q}, {this.r}, {this.s}";
        Debug.Assert(q + r + s == 0, $"IMPROPER GRID COORDINATES: q + r + s != 0 {q}, {r}, {s}");
    }
    public GridTile(int q, int r) : this(q, r, -(q + r)) { }

    public readonly int q; //Coloumn
    public readonly int r; //Row
    public readonly int s; //Some other number
    public readonly string coords;

    // Pathfinding
    public GridTile cameFrom;
    public int G { get; set; }
    public int H { get; set; }
    public int F { get => (G + H); }

    public bool blocksMovement = false;

    public static (int q, int r, int s) GetCoordsFromCoordString(string coordString)
    {
        string[] splitString = coordString.Split(',');
        Debug.Assert(splitString.Count() == 3, $"IMPROPER GRID COORD-STRING - Correct format is: q, r, s");
        int[] coords = new int[splitString.Count()];
        for (int i = 0; i < splitString.Count(); i++)
        {
            if (!int.TryParse(splitString[i], out coords[i]))
            {
                Debug.LogError("Unable to parse Coordstring into integear, reaturning 0, 0, 0");
                return (0, 0, 0);
            }
        }

        if (coords[0] + coords[1] + coords[2] != 0)
        {
            Debug.LogError("IMPROPER GRID COORDINATES: q + r + s != 0, returning (0, 0, 0)");
            return (0, 0, 0);
        }

        return (coords[0], coords[1], coords[2]);
    }

    public static string GetStringFromCoords(int q, int r, int s)
    {
        return $"{q}, {r}, {s}";
    }

    public static string GetStringFromCoords(int q, int r)
    {
        return GetStringFromCoords(q, r, -(q + r));
    }
}
