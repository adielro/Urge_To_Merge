using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EffectController : MonoBehaviour
{
    [SerializeField] private Image targetImage;
    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] private float maxStrength = 1f;
    [SerializeField] private string shaderPropertyName = "_OverlayStrength";
    [SerializeField] private float idleLoopDuration = 2f;
    [SerializeField] private float idleMinStrength = 0.3f;
    [SerializeField] private float idleMaxStrength = 1f;
    
    private Material materialInstance;
    private float currentStrength = 0f;
    private Coroutine effectCoroutine;
    private Coroutine idleCoroutine;
    private bool pendingActivation = false;
    private System.Action pendingCallback = null;

    private void OnEnable()
    {
        if (targetImage == null)
            targetImage = GetComponent<Image>();
            
        if (targetImage != null)
        {
            materialInstance = new Material(targetImage.material);
            targetImage.material = materialInstance;
            
            // Initialize to 0
            materialInstance.SetFloat(shaderPropertyName, 0f);
        }

        // If Activate was called before we were enabled, do it now
        if (pendingActivation)
        {
            pendingActivation = false;
            Activate(pendingCallback);
            pendingCallback = null;
        }
    }

    public void Activate(System.Action onComplete = null)
    {
        if (!gameObject.activeInHierarchy) 
        {
            // Defer activation until OnEnable
            pendingActivation = true;
            pendingCallback = onComplete;
            return;
        }

        if (effectCoroutine != null) StopCoroutine(effectCoroutine);
        effectCoroutine = StartCoroutine(AnimateEffect(maxStrength, () => 
        {
            onComplete?.Invoke();
            StartIdleAnimation();
        }));
    }

    public void Deactivate(System.Action onComplete = null)
    {
        if (!gameObject.activeInHierarchy) return;

        StopIdleAnimation();
        if (effectCoroutine != null) StopCoroutine(effectCoroutine);
        effectCoroutine = StartCoroutine(AnimateEffect(0f, onComplete));
    }

    public void StartIdleAnimation()
    {
        if (!gameObject.activeInHierarchy) return;
        
        StopIdleAnimation();
        idleCoroutine = StartCoroutine(IdleLoop());
    }

    public void StopIdleAnimation()
    {
        if (idleCoroutine != null)
        {
            StopCoroutine(idleCoroutine);
            idleCoroutine = null;
        }
    }    private IEnumerator AnimateEffect(float targetStrength, System.Action onComplete = null)
    {
        float startStrength = currentStrength;
        float elapsed = 0f;

        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animationDuration;
            t = t * t * (3f - 2f * t); // Smoothstep
            
            currentStrength = Mathf.Lerp(startStrength, targetStrength, t);
            if (materialInstance != null)
            {
                materialInstance.SetFloat(shaderPropertyName, currentStrength);
            }
            yield return null;
        }

        currentStrength = targetStrength;
        if (materialInstance != null)
        {
            materialInstance.SetFloat(shaderPropertyName, currentStrength);
        }
        
        onComplete?.Invoke();
    }

    private IEnumerator IdleLoop()
    {
        while (true)
        {
            // Use global time for synchronization across all instances
            float cycleTime = idleLoopDuration * 2f; // Full cycle (min->max->min)
            float normalizedTime = (Time.time % cycleTime) / cycleTime;
            
            // Ping-pong between 0 and 1
            float t = normalizedTime < 0.5f ? normalizedTime * 2f : (1f - normalizedTime) * 2f;
            
            // Smoothstep
            t = t * t * (3f - 2f * t);
            
            float targetStrength = Mathf.Lerp(idleMinStrength, idleMaxStrength, t);
            currentStrength = targetStrength;
            
            if (materialInstance != null)
            {
                materialInstance.SetFloat(shaderPropertyName, currentStrength);
            }
            
            yield return null;
        }
    }
    
    private void OnDestroy()
    {
        StopIdleAnimation();
        if (materialInstance != null)
        {
            Destroy(materialInstance);
        }
    }
}
