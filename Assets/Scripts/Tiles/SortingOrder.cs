using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Manages rendering order for tile and text.
/// </summary>
public class SortingOrder : MonoBehaviour
{
    private Canvas tileCanvas;
    private List<MeshRenderer> textMeshRenderers = new List<MeshRenderer>();
    private int defaultSortingOrder;

    private void Awake()
    {
        tileCanvas = GetComponent<Canvas>();
        var texts = GetComponentsInChildren<TMP_Text>(true);
        foreach (var text in texts)
        {
            var renderer = text.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                textMeshRenderers.Add(renderer);
            }
        }

        if (tileCanvas != null)
        {
            tileCanvas.overrideSorting = true;
        }

        if (textMeshRenderers.Count > 0)
        {
            defaultSortingOrder = textMeshRenderers[0].sortingOrder;
        }
    }

    public void SetSortingOrder(int sortingOrder)
    {
        if (tileCanvas != null)
        {
            tileCanvas.sortingOrder = sortingOrder;
        }

        foreach (var renderer in textMeshRenderers)
        {
            if (renderer != null)
            {
                renderer.sortingOrder = sortingOrder + 1;
            }
        }
    }

    public void ResetSortingOrder()
    {
        if (tileCanvas != null)
        {
            tileCanvas.sortingOrder = 0;
        }

        foreach (var renderer in textMeshRenderers)
        {
            if (renderer != null)
            {
                renderer.sortingOrder = defaultSortingOrder;
            }
        }
    }
}
