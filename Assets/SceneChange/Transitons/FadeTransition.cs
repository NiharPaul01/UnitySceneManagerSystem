using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Scene Transitions/Fade")]
public class FadeTransition : SceneTransition
{
    public float duration = 1f;

    private CanvasGroup canvasGroup;

    protected override IEnumerator OnTransitionOut()
    {
        canvasGroup = instance.GetComponentInChildren<CanvasGroup>();

        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = t / duration;
            yield return null;
        }
    }

    protected override IEnumerator OnTransitionIn()
    {
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = 1 - (t / duration);
            yield return null;
        }
    }
}


/*
 HOW TO IMPLEMENT AND USE THIS FADE TRANSITION

 1. Create the Transition Prefab:
    - Create a new Canvas in your scene (Screen Space - Overlay recommended).
    - Add a full-screen UI Image as a child:
        • Set the Image color (usually black or white for fade).
        • Stretch it to cover the entire canvas (anchors 0-1).
    - Add a CanvasGroup component to the Canvas or the Image.
        • This will control the alpha for fading.
    - Save the Canvas as a Prefab.

 2. Create the ScriptableObject:
    - Right-click in the Project window.
    - Create → Scene Transitions → Fade.
    - Assign your transition prefab to the "Transition Prefab" field.
    - Set the duration for the fade effect.

 3. How It Works:
    - PlayTransitionOut():
        • Spawns the prefab.
        • Gets the CanvasGroup.
        • Gradually increases alpha from 0 → 1 over duration.
    - PlayTransitionIn():
        • Gradually decreases alpha from 1 → 0 over duration.
        • Cleanup() destroys the prefab after fade-in.

 4. Important Notes:
    - The prefab must have a CanvasGroup component.
    - CanvasGroup alpha controls the transparency.
    - Ensure the Canvas is rendered on top of everything (use Sorting Layer or Canvas order).
    - Adjust duration to match the desired speed of the fade.

 5. Example Scene Loader Usage:

        public IEnumerator LoadScene(string sceneName, SceneTransition transition)
        {
            yield return transition.PlayTransitionOut();

            SceneManager.LoadScene(sceneName);

            yield return transition.PlayTransitionIn();
        }

 This simple fade transition can be used as a base for more advanced transitions,
 like color fades, image overlays, or combining with other effects.
*/