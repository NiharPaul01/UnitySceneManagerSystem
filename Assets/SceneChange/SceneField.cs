using UnityEngine;

[System.Serializable]
public class SceneField
{
    [SerializeField] private UnityEngine.Object sceneAsset; // Optional: for reference in Inspector
    [SerializeField] private int sceneNumber;                // Build index of the scene

    public int SceneNumber => sceneNumber;

    // Optional: implicit conversion to int
    public static implicit operator int(SceneField obj)
    {
        return obj.sceneNumber;
    }
}
