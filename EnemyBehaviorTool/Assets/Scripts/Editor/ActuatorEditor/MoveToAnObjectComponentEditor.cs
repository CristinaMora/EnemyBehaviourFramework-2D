using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static MoveToAPointActuator;

[CustomEditor(typeof(MoveToAnObjectActuator))]
public class MoveToAnObjectComponentEditor : ActuatorEditor
{
	SerializedProperty _objectPosition;
	SerializedProperty _timeToReach;
	SerializedProperty _isAccelerated;
	SerializedProperty _easingFunction;

	private static readonly GUIContent _easingFunctionToAPointLabel = new GUIContent("Easing Function", "Easing function that will describe the progress of the position.");
	private static readonly GUIContent _timeToReachLabel = new GUIContent("Time To Reach", "Time it takes to reach the object.");
	private static readonly GUIContent _objectTransformLabel = new GUIContent("Object Transform", "Reference to the object transform.");
	private static readonly GUIContent _isAcceleratedLabel = new GUIContent("Is Accelerated", "Is the movement towards the waypoint accelerated?");
	
	private void OnEnable()
	{
		_objectPosition = serializedObject.FindProperty("_objectPosition");
		_timeToReach = serializedObject.FindProperty("_timeToReach");
		_isAccelerated = serializedObject.FindProperty("_isAccelerated");
		_easingFunction = serializedObject.FindProperty("_easingFunction");
	}
	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		EditorGUILayout.PropertyField(_objectPosition, _objectTransformLabel);

		_timeToReach.floatValue = Mathf.Max(0, _timeToReach.floatValue);
		EditorGUILayout.PropertyField(_timeToReach, _timeToReachLabel);

		EditorGUILayout.PropertyField(_isAccelerated, _isAcceleratedLabel);

		if (_isAccelerated.boolValue)
		{
			EditorGUILayout.PropertyField(_easingFunction, _easingFunctionLabel);
			EditorGUI.indentLevel++;
			EasingFunction.Ease easingEnum = (EasingFunction.Ease)_easingFunction.intValue;
			DrawEasingCurve(easingEnum, new Vector2(45, 15), new Vector2(15, 2), "X: Time", "Y: Position ", new Vector2(40, 20), new Vector2(60, 20));
			EditorGUI.indentLevel--;
		}
		serializedObject.ApplyModifiedProperties();
	}
    
}
