using UnityEditor;
using UnityEngine;
using static DistanceSensor;

[CustomEditor(typeof(DistanceSensor))]
public class DistanceSensorEditor : Editor
{
	// Labels for the inspector fields
	private static readonly GUIContent _distanceTypeLabel = new GUIContent("Distance Type", "Select the type of distance measurement.");
	private static readonly GUIContent _axisLabel = new GUIContent("Axis", "Choose the axis along which distance is measured.");
	private static readonly GUIContent _detectionSideLabel = new GUIContent("Detection Side", "Select which side(s) along the axis to detect (Both, Negative, Positive).");
	private static readonly GUIContent _detectionDistanceLabel = new GUIContent("Detection Distance", "The threshold distance for detection.");
	private static readonly GUIContent _targetLabel = new GUIContent("Target", "The object to measure distance from.");
	private static readonly GUIContent _startDetectingTimeLabel = new GUIContent("Start Detecting Time", "Initial time the sensor will need to be active.");
	private static readonly GUIContent _detectionConditionLabel = new GUIContent("Detection Condition", "Specify whether detection occurs inside or outside the defined range.");

	// Serialized properties
	private SerializedProperty _distanceType;
	private SerializedProperty _axis;
	private SerializedProperty _detectionSide;
	private SerializedProperty _detectionDistance;
	private SerializedProperty _target;
	private SerializedProperty _startDetectingTime;
	private SerializedProperty _detectionCondition;

	private void OnEnable()
	{
		_distanceType = serializedObject.FindProperty("_distanceType");
		_axis = serializedObject.FindProperty("_axis");
		_detectionSide = serializedObject.FindProperty("_detectionSide");
		_detectionDistance = serializedObject.FindProperty("_detectionDistance");
		_target = serializedObject.FindProperty("_target");
		_startDetectingTime = serializedObject.FindProperty("_startDetectingTime");
		_detectionCondition = serializedObject.FindProperty("_detectionCondition");
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		// Core settings
		EditorGUILayout.PropertyField(_distanceType, _distanceTypeLabel);
		EditorGUILayout.PropertyField(_detectionCondition, _detectionConditionLabel);
		EditorGUILayout.PropertyField(_target, _targetLabel);
		EditorGUILayout.PropertyField(_startDetectingTime, _startDetectingTimeLabel);

		// Conditional fields based on distance type
		var distanceType = (TypeOfDistance)_distanceType.intValue;
		switch (distanceType)
		{
			case TypeOfDistance.SingleAxis:
				EditorGUILayout.PropertyField(_detectionDistance, _detectionDistanceLabel);
				EditorGUILayout.PropertyField(_axis, _axisLabel);
				EditorGUILayout.PropertyField(_detectionSide, _detectionSideLabel);
				break;

			case TypeOfDistance.Magnitude:
				EditorGUILayout.PropertyField(_detectionDistance, _detectionDistanceLabel);
				break;
		}

		serializedObject.ApplyModifiedProperties();
	}
}
