using System.Collections;
using UnityEngine;


public abstract class SceneTransition : ScriptableObject
{
    [Header("Transition Prefab")]
    public GameObject transitionPrefab;

    protected GameObject instance;

    public IEnumerator PlayTransitionOut()
    {
        Spawn();
        yield return OnTransitionOut();
    }

    public IEnumerator PlayTransitionIn()
    {
        yield return OnTransitionIn();
        Cleanup();
    }

    protected virtual void Spawn()
    {
        if (transitionPrefab == null)
        {
            Debug.LogError("Transition prefab missing!");
            return;
        }

        instance = Instantiate(transitionPrefab);
        DontDestroyOnLoad(instance);
    }

    protected virtual void Cleanup()
    {
        if (instance != null)
            Destroy(instance);
    }

    protected abstract IEnumerator OnTransitionOut();
    protected abstract IEnumerator OnTransitionIn();
}