using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[CustomPropertyDrawer(typeof(SceneField))]
public class SceneFieldPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        SerializedProperty sceneAsset = property.FindPropertyRelative("sceneAsset");
        SerializedProperty sceneNumber = property.FindPropertyRelative("sceneNumber");

        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        if (sceneAsset != null)
        {
            sceneAsset.objectReferenceValue = EditorGUI.ObjectField(
                position,
                sceneAsset.objectReferenceValue,
                typeof(SceneAsset), // Only SceneAsset is allowed
                false);

            if (sceneAsset.objectReferenceValue != null)
            {
                string scenePath = AssetDatabase.GetAssetPath(sceneAsset.objectReferenceValue);
                sceneNumber.intValue = SceneUtility.GetBuildIndexByScenePath(scenePath);
            }
        }

        EditorGUI.EndProperty();
    }
}
