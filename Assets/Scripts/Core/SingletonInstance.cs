using UnityEngine;

/// <summary>
/// Generic singleton pattern for MonoBehaviour.
/// </summary>
public class SingletonInstance<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance;

    protected virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = this as T;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
