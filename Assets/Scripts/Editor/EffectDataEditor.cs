using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EffectData))]
public class EffectDataEditor : Editor
{
    SerializedProperty effectType;
    SerializedProperty desc;
    SerializedProperty objectFx;
    SerializedProperty parameters;

    void OnEnable()
    {
        effectType = serializedObject.FindProperty("effectType");
        desc = serializedObject.FindProperty("desc");
        objectFx = serializedObject.FindProperty("Object_fx");
        parameters = serializedObject.FindProperty("parameters");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(effectType);
        EditorGUILayout.PropertyField(desc);
        EditorGUILayout.PropertyField(objectFx);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Effect Parameters", EditorStyles.boldLabel);

        EffectType currentEffectType = (EffectType)effectType.enumValueIndex;
        
        // Show object field with filtered type based on selected effect type
        switch (currentEffectType)
        {
            case EffectType.Projectile:
                parameters.objectReferenceValue = EditorGUILayout.ObjectField(
                    "Projectile Parameters",
                    parameters.objectReferenceValue,
                    typeof(ProjectileParameters),
                    false);
                break;
            case EffectType.Explosion:
                parameters.objectReferenceValue = EditorGUILayout.ObjectField(
                    "Explosion Parameters",
                    parameters.objectReferenceValue,
                    typeof(ExplosionParameters),
                    false);
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }
}