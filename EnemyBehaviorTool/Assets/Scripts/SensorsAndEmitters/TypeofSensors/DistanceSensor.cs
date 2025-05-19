using System;
using UnityEngine;
using UnityEditor;

// Observation: The distance between the two objects is measured from their center.
public class DistanceSensor : Sensor
{
    // Different modes for distance calculation
    public enum TypeOfDistance
    {
        Magnitude = 0,  // Measure the entire distance (magnitude)
        SingleAxis = 1  // Measure distance along a single axis (X or Y)
    };

    // Conditions for detection
    public enum DetectionCondition
    {
        InsideMagnitude = 0, // Trigger detection when inside the detection range
        OutsideMagnitude = 1 // Trigger detection when outside the detection range
    };

    // Axes for single-axis distance calculation
    public enum Axis
    {
        Y = 0, // Measure distance along the Y-axis
        X = 1  // Measure distance along the X-axis
    };

	// Detection side configuration
	public enum DetectionSide
	{
		Both = 0,       // Detect on both directions along the axis
		Negative = 1,   // Detect only in negative direction (left or down)
		Positive = 2    // Detect only in positive direction (right or up)
	}


	[SerializeField] private TypeOfDistance _distanceType = TypeOfDistance.Magnitude; // Default distance type
    [SerializeField] private Axis _axis = Axis.X; // Default axis for single-axis detection
    [SerializeField] private DetectionSide _detectionSide = DetectionSide.Both; // Default detection side

    [SerializeField]
    private GameObject _target; // Target object to measure distance against

    [SerializeField, Min(0)]
    private float _detectionDistance = 5f; // Threshold distance for detection

    [SerializeField, Tooltip("External trigger used for area detection.")]
    private Collider2D _areaTrigger; // External trigger for area detection

    [SerializeField]
    private DetectionCondition _detectionCondition = DetectionCondition.InsideMagnitude; // Default detection condition

    // Initializes the sensor settings
    public override void StartSensor()
    {
        base.StartSensor();
        if (_target == null)
        {
            Debug.LogWarning($"No target set in Distance Sensor in object {name}");
        }
    }

    // Determines if the sensor should transition based on distance
    private void Update()
    {
        base.UpdateSensor();

        // Exit early if the target is null or the timer has not finished
        if (_target == null || !_timerFinished)
            return;

        Vector2 selfPos = transform.position; // Position of the sensor
        Vector2 targetPos = _target.transform.position; // Position of the target
        bool detected = false; // Tracks whether detection occurred

        switch (_distanceType)
        {
            case TypeOfDistance.Magnitude:
                // Check if the distance is within the threshold
                bool isWithinMagnitude = Vector2.Distance(selfPos, targetPos) <= _detectionDistance;
                detected = (_detectionCondition == DetectionCondition.InsideMagnitude) ? isWithinMagnitude : !isWithinMagnitude;
                break;

            case TypeOfDistance.SingleAxis:
				float delta = (_axis == Axis.X) ? (targetPos.x - selfPos.x) : (targetPos.y - selfPos.y);
				float absDelta = Mathf.Abs(delta);

				bool correctSide = _detectionSide == DetectionSide.Both
					|| (_detectionSide == DetectionSide.Negative && delta < 0)
					|| (_detectionSide == DetectionSide.Positive && delta > 0);

				bool inRange = absDelta <= _detectionDistance && correctSide;
				detected = (_detectionCondition == DetectionCondition.InsideMagnitude) ? inRange : !inRange;
				break;
		}

        // Trigger the detection event if the condition is met
        if (detected)
        {
            EventDetected();
        }
    }

    // Draws the detection range in the scene view
    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (!_debugSensor) return; // Exit if debug mode is not enabled
        Gizmos.color = new Color(0, 0, 1, 0.3f); // Set gizmo color to blue

        switch (_distanceType)
        {
            case TypeOfDistance.Magnitude:
                // Draw a circular detection range
                Handles.color = new Color(0, 0, 1, 0.3f);
                Handles.DrawSolidDisc(transform.position, Vector3.forward, _detectionDistance);
                break;

            case TypeOfDistance.SingleAxis:
                Vector3 size;
                Vector3 offset = Vector3.zero;

                if (_axis == Axis.X)
                {
                    if (_detectionSide == DetectionSide.Both)
                    {
                        size = new Vector3(_detectionDistance * 2, 10, 1); // Full width on both sides
                    }
                    else
                    {
						size = new Vector3(_detectionDistance, 10, 1);
						offset.x = (_detectionSide == DetectionSide.Negative)
							? -_detectionDistance / 2
							: _detectionDistance / 2;
					}
                }
                else // Axis.Y
                {
                    if (_detectionSide == DetectionSide.Both)
                    {
                        size = new Vector3(10, _detectionDistance * 2, 1); // Full height on both sides
                    }
                    else
                    {
						size = new Vector3(10, _detectionDistance, 1);
						offset.y = (_detectionSide == DetectionSide.Positive)
							? _detectionDistance / 2
							: -_detectionDistance / 2;
					}
                }

                // Draw the detection range as a rectangle
                Gizmos.DrawCube(transform.position + offset, size);
                break;
        }
#endif
    }

    // Set a new detection distance
    public void SetDetectionDistance(float newValue)
    {
        _detectionDistance = newValue;
    }

    // Set a new target for the sensor
    public void SetTarget(GameObject g)
    {
        _target = g;
    }
}