using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Scene Transitions/Shader Wipe")]
public class ShaderWipeTransition : SceneTransition
{
    public float duration = 1f;
    [SerializeField]private Material runtimeMaterial;

     private Canvas targetCanvas;
     private Camera renderCamera;

    protected override IEnumerator OnTransitionOut()
    {

        SetCanvasCamera();
        yield return Animate(1, 0);
    }

    protected override IEnumerator OnTransitionIn()
    {
        SetCanvasCamera();
        yield return Animate(0, 1);
    }


    private void SetCanvasCamera()
    {
        targetCanvas = instance.GetComponentInChildren<Canvas>();
        if (targetCanvas != null)
        {
            // If no camera assigned, use the main camera by default
            if (renderCamera == null)
                renderCamera = Camera.main;

            targetCanvas.worldCamera = renderCamera;
        }
    }

    private IEnumerator Animate(float from, float to)
    {
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float value = Mathf.Lerp(from, to, t / duration);
            runtimeMaterial.SetFloat("_Cutoff", value);
            yield return null;
        }
    }
}

/*
 HOW TO IMPLEMENT AND USE THIS SHADER WIPE TRANSITION

 1. Create the Wipe Shader:
    - Create a shader (Shader Graph or HLSL).
    - Add a float property named "_Cutoff".
    - Use this value to control visibility (for example:
        • Alpha clip threshold
        • Lerp between black and scene
        • Screen-space mask wipe)
    - Ensure "_Cutoff" range is 0 → 1.

 2. Create the Transition Prefab:
    - Create a canvas with camera overlay full-screen Quad, Image, or Mesh.
    - Assign a Material that uses your wipe shader.
    - Make sure the object covers the entire screen.
    - Save it as a Prefab.

 3. Create the ScriptableObject:
    - Right-click in Project window.
    - Create → Scene Transitions → Shader Wipe.
    - Assign the transition prefab.
    - Set the duration.

 4. How It Works:
    - PlayTransitionOut():
        • Spawns the prefab.
        • Gets the Renderer.
        • Creates a runtime material instance.
        • Animates _Cutoff from 0 → 1.

    - PlayTransitionIn():
        • Animates _Cutoff from 1 → 0.
        • Cleanup() (from base class) destroys the instance.

 5. Important Notes:
    - renderer.material creates a material instance at runtime,
      preventing modification of the shared material.
    - The shader MUST contain a float property named "_Cutoff".
    - The prefab must include a Renderer component.
    - Ensure the object renders on top (sorting layer / render queue).

 6. Example Scene Loader Usage:

        public IEnumerator LoadScene(string sceneName, SceneTransition transition)
        {
            yield return transition.PlayTransitionOut();

            SceneManager.LoadScene(sceneName);

            yield return transition.PlayTransitionIn();
        }

 You can create different wipe styles (radial, diagonal,
 noise-based, pixel dissolve, etc.) by modifying the shader
 while keeping this same transition logic.
*/