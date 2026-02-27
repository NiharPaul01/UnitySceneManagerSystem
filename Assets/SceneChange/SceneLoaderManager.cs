using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class SceneGroup
{
    [Header("Scenes to Load")]
    public string GroupName = "New Scene Group";
    public List<SceneField> scenesToLoad = new List<SceneField>();

    [Header("Optional Transition")]
    public SceneTransition transition;
}

public class SceneLoaderManager : MonoBehaviour
{
    
    public SceneGroup[] sceneGroups;

    [Header("Optional: Delay Between Scene Loads")]
    public float delayBetweenScenes = 0f;
    

    private List<Scene> loadedScenes = new List<Scene>();
    private int currentGroupIndex = -1;
    private bool isLoading = false;


    private void Start()
    {
        if (sceneGroups.Length > 0)
            LoadGroup(0); // Load first group automatically
    }

    #region PUBLIC API

    private void LoadGroup(int groupIndex)
    {
        if (isLoading) return;

        if (groupIndex < 0 || groupIndex >= sceneGroups.Length)
        {
            Debug.LogWarning("Invalid group index");
            return;
        }

        StartCoroutine(SwitchGroupRoutine(groupIndex));
    }

    // 1️) Load next group in sequence
    public void LoadNextGroup()
    {
        if (sceneGroups.Length == 0) return;

        int nextIndex = (currentGroupIndex + 1) % sceneGroups.Length;
        LoadGroup(nextIndex);
    }

    // 2️) Load group by name
    public void LoadNextGroup(string name)
    {
        if (sceneGroups.Length == 0) return;

        // Find the index of the group with the given name
        int index = Array.FindIndex(sceneGroups, g => g.GroupName == name);

        if (index == -1)
        {
            Debug.LogWarning($"Scene group with name '{name}' not found.");
            return;
        }

        LoadGroup(index);
    }

    // 3️) Load group by index
    public void LoadNextGroup(int index)
    {
        if (sceneGroups.Length == 0 || index < 0 || index >= sceneGroups.Length)
        {
            Debug.LogWarning("Invalid group index.");
            return;
        }

        LoadGroup(index);
    }

    public void UnloadCurrentGroup()
    {
        if (isLoading) return;
        StartCoroutine(UnloadScenesRoutine());
    }

    #endregion

    #region CORE LOGIC

    private IEnumerator SwitchGroupRoutine(int newIndex)
    {
        isLoading = true;

        SceneTransition transition = sceneGroups[newIndex].transition;


        //  Transition OUT (if exists)
        if (transition != null)
            yield return StartCoroutine(transition.PlayTransitionOut());

        // Unload current group first
        if (loadedScenes.Count > 0)
            yield return StartCoroutine(UnloadScenesRoutine());

        // Load new group
        yield return StartCoroutine(LoadScenesSequentially(newIndex));

        currentGroupIndex = newIndex;

        // 4 Transition IN (if exists)
        if (transition != null)
            yield return StartCoroutine(transition.PlayTransitionIn());
        isLoading = false;
    }

    private IEnumerator LoadScenesSequentially(int index)
    {
        SceneGroup group = sceneGroups[index];

        foreach (SceneField sceneField in group.scenesToLoad)
        {
            int sceneNumber = sceneField.SceneNumber;

            AsyncOperation op = SceneManager.LoadSceneAsync(sceneNumber, LoadSceneMode.Additive);
            while (!op.isDone)
                yield return null;

            Scene loadedScene = SceneManager.GetSceneByBuildIndex(sceneNumber);
            if (loadedScene.IsValid() && loadedScene.isLoaded)
            {
                loadedScenes.Add(loadedScene);
                Debug.Log($"Loaded scene: {loadedScene.name}");
            }

            if (delayBetweenScenes > 0f)
                yield return new WaitForSeconds(delayBetweenScenes);
        }
    }

    private IEnumerator UnloadScenesRoutine()
    {
        foreach (Scene scene in loadedScenes)
        {
            if (scene.IsValid() && scene.isLoaded)
            {
                AsyncOperation op = SceneManager.UnloadSceneAsync(scene);
                while (!op.isDone)
                    yield return null;

                Debug.Log($"Unloaded scene: {scene.name}");
            }
        }

        loadedScenes.Clear();
    }

    #endregion
}