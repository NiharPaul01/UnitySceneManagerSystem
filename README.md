Open OpenThis.scene inside SceneChange folder to checkout.
you can change transition by changing scriptable object reference.

There are two ways to load scene:
1. keep a bootstrapped scene and load/unload other scenes as needed
2. keep Essentials(like SceneTransitionManager in this case) as persistant

Either of them can take a scriptable object that will load a prefab on runtime to create transion effect. 

Current Transition effects: Fade, Dissolve by various ways

Future Scope: I am planning to add  Video and Animation Transitions in near future. i also want to add more types of shader transion effect.
