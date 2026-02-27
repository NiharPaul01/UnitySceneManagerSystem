using System.Collections;
using UnityEngine;

public class ToggleScene : MonoBehaviour
{
   
    
    
    public SceneField nextScene; // SceneField for next scene

    [Header("AdditiveLoadAtStart")]
    [SerializeField] private bool loadSceneAtStart = false;
    [SerializeField] private float delayInStartingLoad = 1f;
    [Header("ChangeScene")]
    [SerializeField] bool changeSceneAtStart = false;
    [SerializeField] private float changeTime;

    private SceneTransitionManager sceneTransitionManager;


   

    private void Start()
    {
        //Assign sceneTransitionManager
        sceneTransitionManager = FindFirstObjectByType<SceneTransitionManager>(); 
        if (sceneTransitionManager == null)
        {
            Debug.LogError("No SceneTransitionManager found in the scene!");
        }

        //if scene to preload
        if (loadSceneAtStart)
        {
            StartCoroutine(LateStart(delayInStartingLoad));
        }

        //if scene to change automatically
        if (changeSceneAtStart)
        {
            ScenechangeDelay(changeTime);
        }
    }

    

    private IEnumerator LateStart(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Preload the next scene at start if requested
        if (loadSceneAtStart && nextScene != null)
        {
            sceneTransitionManager.PreloadScene(nextScene.SceneNumber);
        }
    }

    /// <summary>
    /// Immediately changes to the next scene
    /// </summary>
    public void Scenechange()
    {
        //Disable playercontrols

        //GameInput.Dispose();

        

        

        
        if (nextScene == null)
        {
            Debug.LogWarning("Next scene is not assigned!");
            return;
        }

        if (!loadSceneAtStart)
        {
            sceneTransitionManager.GoToSceneAsync(nextScene.SceneNumber);
        }
        else
        {
            sceneTransitionManager.ActivatePreloadedScene();
        }
    }

    /// <summary>
    /// Changes scene after a delay
    /// </summary>
    public void ScenechangeDelay(float seconds)
    {
        StartCoroutine(DelayedSceneChange(seconds));
    }

    private IEnumerator DelayedSceneChange(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Scenechange();
    }
}
