using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Pathfinder
{
    public static List<GridTile> FindPath(GridTile start, GridTile end, int range, bool ignoreBlocks, bool ignoreWalls)
    {
        GridInformant GII = GridInformant.Instance;

        // Keep list of tiles to search and tiles already searched
        List<GridTile> toSearch = new() { start };
        List<GridTile> processed = new();

        // Keep going as long as we have any tiles to search
        while (toSearch.Any())
        {
            GridTile current = toSearch[0];
            foreach (GridTile t in toSearch)
            {
                // The current tile is the tile with the
                // lowest sum of
                // tiles away from the start tile,
                // and the lowest possible amount of tiles from the end
                if (t.F < current.F || t.F == current.F && t.H < current.H)
                {
                    current = t;
                }
            }

            processed.Add(current);
            toSearch.Remove(current);

            // End is reached, return the path that lead here
            if (current == end)
            {
                GridTile currentPathTile = end;
                List<GridTile> path = new();
                while (currentPathTile != start)
                {
                    path.Add(currentPathTile);
                    currentPathTile = currentPathTile.cameFrom;
                }
                path.Reverse();

                // AI will look for the tile the target is standing on, remove that
                if (path.Count != 0)
                {
                    if (GridInformant.Instance.TryGetUnit(path[^1], out _))
                        path.RemoveAt(path.Count - 1);
                    else if (path[^1].blocksMovement && !ignoreBlocks)
                        path.RemoveAt(path.Count - 1);
                }

                if (range >= 0)
                {
                    while (path.Count > range)
                    {
                        path.RemoveAt(path.Count - 1);
                    }
                }

                return path;
            }

            // Determine which neighbours to investigate
            List<GridTile> investigateNeighbours;
            if (ignoreBlocks)
            {
                if (ignoreWalls)
                    investigateNeighbours = GII.GetAllNeighbourTiles(current).Where(t => !GII.TryGetUnit(t, out _) && !processed.Contains(t)).ToList();
                else
                    investigateNeighbours = GII.GetAllNeighbourTiles(current).Where(t => !GII.TryGetUnit(t, out _) && !processed.Contains(t) && !t.blocksMovement).ToList();

                // AI will look for a path where a unit is standing, make sure to find that tile
                foreach (GridTile t in GII.GetAllNeighbourTiles(current))
                {
                    if (t == end && !investigateNeighbours.Contains(t))
                    {
                        investigateNeighbours.Add(t);
                    }
                }
            }
            else
            {
                if (ignoreWalls)
                    investigateNeighbours = GII.GetAllNeighbourTiles(current).Where(t => !GII.TryGetUnit(t, out _) && !processed.Contains(t)).ToList();
                else
                    investigateNeighbours = GII.GetAllNeighbourTiles(current).Where(t => !GII.TryGetUnit(t, out _) && !processed.Contains(t) && !t.blocksMovement).ToList();

                // AI will look for a path where a unit is standing, make sure to find that tile
                List<GridTile> curNeighbours = GII.GetAllNeighbourTiles(current);
                foreach (GridTile t in curNeighbours)
                {
                    if (t == end && !investigateNeighbours.Contains(t))
                    {
                        investigateNeighbours.Add(t);
                    }
                }
            }

            // Check each non-blocked neighbouring tile that has not already been processed
            foreach (GridTile neighbour in investigateNeighbours)
            {
                // Prephare to set cost of neighbour
                bool inSearch = toSearch.Contains(neighbour);
                int costToNeighbour = current.G + GetDistance(current, neighbour);

                // If neighbour hasn't already been searched, or a new path has determined a lower cost than a previous one
                // Update connection and cost
                if (!inSearch || costToNeighbour < neighbour.G)
                {
                    neighbour.G = costToNeighbour;
                    neighbour.cameFrom = current;

                    // If not previously searched, determine heuristic value
                    if (!inSearch)
                    {
                        neighbour.H = GetDistance(neighbour, end);
                        toSearch.Add(neighbour);
                    }
                }
            }
        }

        // No possible path, return null
        return null;
    }

    public static List<GridTile> FindPath(GridTile start, GridTile end)
    {
        return FindPath(start, end, -1, false, false);
    }

    public static List<GridTile> FindPath(GridTile start, GridTile end, int range)
    {
        return FindPath(start, end, range, false, false);
    }

    public static List<GridTile> FindPath(int qStrat, int rStart, int qEnd, int rEnd)
    {
        if(!GridInformant.Instance.TileExists(qStrat, rStart, out GridTile start)) { return new(); }
        if (!GridInformant.Instance.TileExists(qEnd, rEnd, out GridTile end)) { return new(); }

        return FindPath(start, end);
    }

    public static int GetDistance(GridTile a, GridTile b)
    {
        int xDist = Mathf.Abs(a.q - b.q);
        int yDist = Mathf.Abs(a.r - b.r);
        int distTot = xDist + yDist;

        return distTot;
    }
}
