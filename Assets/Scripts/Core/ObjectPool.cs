using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Object pool for number tiles to avoid instantiation overhead.
/// </summary>
public class ObjectPool : SingletonInstance<ObjectPool>
{
    [SerializeField] private GameObject NumberTilePrefab;
    private Queue<GameObject> numberTilePool = new Queue<GameObject>();

    /// <summary>
    /// Gets a number tile from the pool or creates a new one.
    /// </summary>
    public GameObject GetNumberTile()
    {
        return numberTilePool.Count == 0 ? Instantiate(NumberTilePrefab) : numberTilePool.Dequeue();
    }

    /// <summary>
    /// Returns a number tile back to the pool for reuse.
    /// </summary>
    public void ReturnNumberTile(GameObject numberTile)
    {
        numberTile.SetActive(false);  // Deactivate before returning to the pool
        numberTilePool.Enqueue(numberTile);  // Add the tile into the pool
    }
}
