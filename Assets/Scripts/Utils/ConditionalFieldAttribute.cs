using System;
using System.Reflection;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;


// Loosely based on https://github.com/Deadcows/MyBox/blob/master/Attributes/ConditionalFieldAttribute.cs

[CustomPropertyDrawer(typeof(ConditionalFieldAttribute))]
public class ConditionalFieldAttributeDrawer : PropertyDrawer
{
	private bool drawProperty = true;

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		if (!(attribute is ConditionalFieldAttribute conditional)) return EditorGUI.GetPropertyHeight(property);

		drawProperty = true;

		try {

			var conditionalInfo = (ConditionalFieldAttribute)attribute;

			var segmentedPropertyPath = property.propertyPath.Split('.');
			segmentedPropertyPath[segmentedPropertyPath.Length - 1] = conditionalInfo.propertyName;

			var value = GetPropertyValue(property.serializedObject.targetObject, segmentedPropertyPath);
			drawProperty = conditionalInfo.invert ^ conditionalInfo.IsValidValue(value);
		} catch (Exception e) {
			//Debug.LogException(e);
		}
		
		if (!drawProperty) return -2;

		return EditorGUI.GetPropertyHeight(property);
	}

	public static object GetPropertyValue(object obj, string[] segmentedPropertyPath) {

		for (var i = 0; i < segmentedPropertyPath.Length; i++) {
			var _propertyInfo = obj.GetType().GetField(segmentedPropertyPath[i], BindingFlags.Public | BindingFlags.Instance);
			if (_propertyInfo != null)
				obj = _propertyInfo.GetValue(obj);
			else
				obj = null;
		}

		return obj;
	}

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		if (!drawProperty) return;
		EditorGUI.PropertyField(position, property, label, true);
	}
}
#endif

/// <summary>
/// Conditionally Show/Hide field in inspector, based on some other field or property value
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class ConditionalFieldAttribute : PropertyAttribute
{
	public string propertyName;
	public object[] validValues;
	public bool invert;

	public ConditionalFieldAttribute(string propertyName, params object[] validValues) {
		this.propertyName = propertyName;
		this.validValues = validValues;
	}

	public bool IsValidValue(object otherValue) {
		for (int i = 0; i < this.validValues.Length; i++) {
			if(otherValue.Equals(this.validValues[i])) {
				return true;
			}
		}
		return false;
	}
}