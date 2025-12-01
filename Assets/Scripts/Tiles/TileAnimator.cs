using System.Collections;
using UnityEngine;

/// <summary>
/// Handles tile movement and celebration animations.
/// </summary>
public class TileAnimator : MonoBehaviour
{
    private Animator animator;
    private bool ableToDrag = true;

    public bool AbleToDrag
    {
        get => ableToDrag;
        set => ableToDrag = value;
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Animates the tile flying from one position to another.
    /// </summary>
    public void StartFlyAnimation(Vector3 startPos, Vector3 endPos, float duration = 1f)
    {
        StartCoroutine(FlyRoutine(startPos, endPos, duration));
    }

    private IEnumerator FlyRoutine(Vector3 startPos, Vector3 endPos, float duration)
    {
        ableToDrag = false;
        float elapsed = 0f;
        transform.position = startPos;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            float easeT = t * t * (3f - 2f * t);
            transform.position = Vector3.Lerp(startPos, endPos, easeT);

            yield return null;
        }

        // Ensure the tile ends exactly at the endPos
        transform.position = endPos;
        ableToDrag = true;
    }

    /// <summary>
    /// Triggers celebration animation and removes tile when complete.
    /// </summary>
    public void CelebrateGoal()
    {
        ableToDrag = false;
        animator?.SetTrigger("celebrate");
    }
}
