using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileReservationManager : MonoBehaviour
{
    private Dictionary<Vector2Int, GameObject> reservations = new Dictionary<Vector2Int, GameObject>();

    public bool TryReserveTile(int x, int y, GameObject requester)
    {
        Vector2Int key = new Vector2Int(x, y);
        if (!reservations.ContainsKey(key))
        {
            reservations[key] = requester;
            return true;
        }
        return false;
    }

    public void ReleaseTile(int x, int y, GameObject requester)
    {
        Vector2Int key = new Vector2Int(x, y);
        if (reservations.ContainsKey(key) && reservations[key] == requester)
            reservations.Remove(key);
    }

    public Vector2Int FindNearestFreeTile(int x, int y, int radius)
    {
        for (int r = 0; r <= radius; r++)
        {
            for (int dx = -r; dx <= r; dx++)
            {
                for (int dy = -r; dy <= r; dy++)
                {
                    Vector2Int c = new Vector2Int(x + dx, y + dy);
                    if (!reservations.ContainsKey(c)) return c;
                }
            }
        }
        return new Vector2Int(-1, -1);
    }
}
