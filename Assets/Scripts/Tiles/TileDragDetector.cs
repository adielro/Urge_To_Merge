using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Detects nearby tiles during drag operations.
/// </summary>
public class TileDragDetector : MonoBehaviour
{
    private NumberTile thisTile;
    private BoxCollider2D boxCollider;

    private void Awake()
    {
        thisTile = GetComponent<NumberTile>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    /// <summary>
    /// Detects the tile currently under the touch/mouse position.
    /// </summary>
    public NumberTile GetTileUnderFinger(PointerEventData eventData)
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = eventData.position
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject.TryGetComponent(out NumberTile tile) && tile != thisTile)
            {
                return tile;
            }
        }

        return null;
    }

    /// <summary>
    /// Finds the tile with the largest overlapping area.
    /// </summary>
    public NumberTile GetTileWithMostOverlap()
    {
        NumberTile bestTile = null;
        float maxOverlap = 0f;

        Collider2D[] collidingTiles = Physics2D.OverlapBoxAll(transform.position, boxCollider.size, 0);

        foreach (Collider2D collider in collidingTiles)
        {
            if (collider.TryGetComponent(out NumberTile tile) && tile != thisTile)
            {
                float overlapArea = CalculateOverlap(tile);
                if (overlapArea > maxOverlap)
                {
                    maxOverlap = overlapArea;
                    bestTile = tile;
                }
            }
        }

        return bestTile;
    }

    /// <summary>
    /// Calculates the overlapping area between this tile and another.
    /// </summary>
    private float CalculateOverlap(NumberTile tile)
    {
        Collider2D myCollider = boxCollider;
        Collider2D otherCollider = tile.GetComponent<Collider2D>();

        Bounds myBounds = myCollider.bounds;
        Bounds otherBounds = otherCollider.bounds;

        float xOverlap = Mathf.Max(0, Mathf.Min(myBounds.max.x, otherBounds.max.x) - Mathf.Max(myBounds.min.x, otherBounds.min.x));
        float yOverlap = Mathf.Max(0, Mathf.Min(myBounds.max.y, otherBounds.max.y) - Mathf.Max(myBounds.min.y, otherBounds.min.y));

        return xOverlap * yOverlap;
    }
}
