using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance { get; private set; }
    public SceneTransition transition;

    //Load Scene at start

    private AsyncOperation preloadOperation;

    public void PreloadScene(int sceneIndex)
    {
        // Start preloading the scene
        StartCoroutine(PreloadSceneRoutine(sceneIndex));
    }
    private IEnumerator PreloadSceneRoutine(int sceneIndex)
    {
        //yield return new WaitForSeconds(4f);
        // Begin async scene loading but don't activate
        preloadOperation = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
        preloadOperation.allowSceneActivation = false;

        // Optional: Wait until 90% loaded
        while (preloadOperation.progress < 0.9f)
        {
            yield return null;
        }

        Debug.Log("Scene preloaded and ready to activate.");
    }

    public void ActivatePreloadedScene()
    {
        if (preloadOperation != null)
        {
            StartCoroutine(ActivateSceneRoutine());
        }
        else
        {
            Debug.LogWarning("No scene has been preloaded!");
        }
    }

    private IEnumerator ActivateSceneRoutine()
    {

        Scene previousScene = SceneManager.GetActiveScene();
        // Play transition out
        if (transition != null)
            yield return StartCoroutine(transition.PlayTransitionOut());

        

        

        // Activate the preloaded scene
        preloadOperation.allowSceneActivation = true;
        

        while (!preloadOperation.isDone)
        {
            yield return null;
        }


        // set new scene as active
        Scene newScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1); // The most recently loaded scene
        SceneManager.SetActiveScene(newScene);


        //unload previous scene
        yield return SceneManager.UnloadSceneAsync(previousScene);

        // Play transition in (fade from black / reveal)
        if (transition != null)
            yield return StartCoroutine(transition.PlayTransitionIn());

        yield return null;


        //update lighting after scene change
        DynamicGI.UpdateEnvironment();
        RenderSettings.skybox = RenderSettings.skybox;


    }

    


    // Dont load Scene at start

    public void GoToSceneAsync(int sceneIndex)
    {
        StartCoroutine(GoToSceneAsyncRoutine(sceneIndex));
    }
    private IEnumerator GoToSceneAsyncRoutine(int sceneIndex)
    {
        // Play transition out
        if (transition != null)
            yield return StartCoroutine(transition.PlayTransitionOut());




        // Load new scene asynchronously
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        operation.allowSceneActivation = false;

        // Wait until the scene is loaded in background
        while (operation.progress < 0.9f)
        {
            yield return null;
        }

        //  Activate the scene after fade-out completes
        operation.allowSceneActivation = true;
        while (!operation.isDone)
        {
            yield return null;
        }
        //  Play transition in
        if (transition != null)
            yield return StartCoroutine(transition.PlayTransitionIn());

        //  Optional: update lighting
        DynamicGI.UpdateEnvironment();
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

        }
        else
        {
            Destroy(gameObject);
        }

    }

    
}
