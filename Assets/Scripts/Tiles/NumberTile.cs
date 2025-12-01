using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

/// <summary>
/// Draggable number tile that can merge with other tiles.
/// </summary>
public class NumberTile : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private TMP_Text numberText;
    [SerializeField] private ParticleSystem preMergeEffect;
    
    private int numberValue;
    private SlotTile assignedSlot;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector3 originalPosition;
    private NumberTile targetTile;
    private SortingOrder sortingOrder;
    private Animator animator;

    // Component references
    private TileDragDetector dragDetector;
    private TileStatusEffects statusEffects;
    private TileMysteryEffect mysteryEffect;
    private TileAnimator tileAnimator;

    public SlotTile AssignedSlot => assignedSlot;
    public bool IsTransformTile => mysteryEffect != null && mysteryEffect.IsTransformTile;
    public bool IsFrozen => statusEffects != null && statusEffects.IsFrozen;
    public bool IsBurning => statusEffects != null && statusEffects.IsBurning;
    public int FreezeTurnsRemaining => statusEffects?.FreezeTurnsRemaining ?? 0;
    public int BurnTurnsRemaining => statusEffects?.BurnTurnsRemaining ?? 0;

    public int NumberValue
    {
        get => numberValue;
        set
        {
            numberValue = value;
            UpdateText();
        }
    }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        sortingOrder = GetComponent<SortingOrder>();
        animator = GetComponent<Animator>();
        
        // Get component references
        dragDetector = GetComponent<TileDragDetector>();
        statusEffects = GetComponent<TileStatusEffects>();
        mysteryEffect = GetComponent<TileMysteryEffect>();
        tileAnimator = GetComponent<TileAnimator>();
    }

    /// <summary>
    /// Assigns this tile to a specific slot.
    /// </summary>
    public void AssignSlot(SlotTile slot)
    {
        transform.SetParent(slot.transform);
        assignedSlot = slot;
        slot.OccupySlot(this);
    }

    private void UpdateText()
    {
        numberText.text = numberValue.ToString();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!tileAnimator.AbleToDrag || IsFrozen) return;
        
        SoundManager.Instance?.PlayDragTileSound();

        originalPosition = rectTransform.position;
        canvasGroup.blocksRaycasts = false;
        rectTransform.SetAsLastSibling();
        sortingOrder?.SetSortingOrder(100);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!tileAnimator.AbleToDrag || IsFrozen) return;

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform.parent as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out localPoint);

        rectTransform.localPosition = new Vector3(localPoint.x, localPoint.y, 0);

        NumberTile mergeTarget = dragDetector.GetTileUnderFinger(eventData) ?? dragDetector.GetTileWithMostOverlap();

        if (mergeTarget != null)
        {
            int mergedValue = NumberManager.Instance.GetMergeResultPreview(NumberValue, mergeTarget.NumberValue);
            numberText.text = mergedValue.ToString();
            animator.SetBool("pre_merge", true);
            if (!preMergeEffect.isPlaying)
                preMergeEffect.Play();
        }
        else
        {
            numberText.text = numberValue.ToString();
            animator.SetBool("pre_merge", false);
            preMergeEffect.Stop();
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!tileAnimator.AbleToDrag || IsFrozen) return;
        canvasGroup.blocksRaycasts = true;
        animator.SetBool("pre_merge", false);
        preMergeEffect.Stop();

        NumberTile mergeTarget = dragDetector.GetTileUnderFinger(eventData) ?? dragDetector.GetTileWithMostOverlap();

        if (mergeTarget != null)
        {
            TryMerge(mergeTarget);
        }
        else
        {
            tileAnimator.StartFlyAnimation(rectTransform.position, originalPosition);
        }

        sortingOrder?.ResetSortingOrder();
    }

    /// <summary>
    /// Merges this tile with another tile.
    /// </summary>
    private void TryMerge(NumberTile otherTile)
    {
        if (otherTile.IsFrozen)
        {
            numberText.text = numberValue.ToString();
            tileAnimator.StartFlyAnimation(rectTransform.position, originalPosition);
            return;
        }

        bool transformInvolved = IsTransformTile || otherTile.IsTransformTile;
        if (otherTile.IsTransformTile)
        {
            otherTile.mysteryEffect?.ConsumeTransformEffect();
        }

        otherTile.statusEffects?.ClearFreeze();
        otherTile.statusEffects?.ClearBurn();

        ObjectPool.Instance.ReturnNumberTile(otherTile.gameObject);

        statusEffects?.ClearBurn();

        NumberValue = NumberManager.Instance.MergeNumbers(NumberValue, otherTile.NumberValue);

        assignedSlot.ClearSlot();
        AssignSlot(otherTile.assignedSlot);

        GameEvents.RaiseNumberMerged(this);

        if (transformInvolved)
        {
            mysteryEffect?.ApplyTransformMergeEffect();
        }

        if (gameObject.activeInHierarchy)
        {
            tileAnimator.StartFlyAnimation(transform.position, assignedSlot.transform.position, 0.2f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out NumberTile otherTile))
        {
            targetTile = otherTile;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out NumberTile otherTile) && targetTile == otherTile)
        {
            targetTile = null;
        }
    }

    /// <summary>
    /// Called by animation event to remove this tile after celebration.
    /// </summary>
    public void OnCelebrationComplete()
    {
        statusEffects?.ClearFreeze();
        statusEffects?.ClearBurn();
        assignedSlot?.ClearSlot();
        ObjectPool.Instance.ReturnNumberTile(gameObject);
    }

    // Wrapper methods that delegate to components
    public void ActivateTransformMode() => mysteryEffect?.ActivateTransformMode();
    
    public void Freeze(int turns) => statusEffects?.Freeze(turns);
    
    public void Burn(int turns) => statusEffects?.Burn(turns);
    
    public void CelebrateGoal() => tileAnimator?.CelebrateGoal();
    
    public void StartFlyAnimation(Vector3 startPos, Vector3 endPos, float duration = 1f) 
        => tileAnimator?.StartFlyAnimation(startPos, endPos, duration);
}