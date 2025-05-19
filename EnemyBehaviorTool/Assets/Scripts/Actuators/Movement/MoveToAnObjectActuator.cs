using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MoveToAnObjectActuator : MovementActuator
{
    const float ALMOST_REACHED_ONE = 0.995f; // Threshold to consider the interpolation as complete

	const float MIN_DIRECTION_CHANGE = 0.01f;

	[SerializeField]
    private Transform _objectPosition;

    [SerializeField]
    private float _timeToReach;


    private Rigidbody2D _rb;
    private bool _moving;

    private float _travelElapsedTime;
    private float _t; 

    private Vector2 _startInterpolationPosition;

	private Vector2 _previousPosition;
	private AnimatorManager _animatorManager;
	// Called when the actuator starts
	public override void StartActuator()
    {
        _rb = GetComponent<Rigidbody2D>();
        _travelElapsedTime = 0f;
        _t = 0f;
        _moving = true;

        // Validate the waypoint is set
        if (_objectPosition == null)
        {
            Debug.LogError($"MoveToAnObject error in {name}: No object assigned.");

        }
        else
        {
			// Store current position as the starting point for interpolation
			_startInterpolationPosition = _rb.position;
			_previousPosition = _rb.position;
		}

        // If an AnimatorManager exists, tell it to follow
        _animatorManager = GetComponent<AnimatorManager>();
        if (_animatorManager != null) _animatorManager.Follow();
    }

    // Called every frame if is in the actual State
    public override void UpdateActuator()
    {
        if (!_moving || _objectPosition == null)
            return;

        // Move toward the target if it's set
        if (_objectPosition.position != null)
            MoveTowardsTarget(_objectPosition.position);
        else
        {
            Debug.LogWarning("The MoveToAnObjectActuator goal Transform is null");
        }
    }

    // Handles the interpolation and movement logic toward the target
    private void MoveTowardsTarget(Vector2 targetPos)
    {
        _travelElapsedTime += Time.deltaTime;
        _t = _travelElapsedTime / _timeToReach;

        // Apply easing if enabled
        if (_isAccelerated)
        {
            _t = EasingFunction.GetEasingFunction(_easingFunction)(0, 1, _t);
            if (_t >= ALMOST_REACHED_ONE)
                _t = 1f;
        }

        // Interpolate the position between start and target
        Vector2 newPosition = Vector2.Lerp(_startInterpolationPosition, targetPos, _t);
        _rb.MovePosition(newPosition);


		float deltaX = newPosition.x - _previousPosition.x;

		if (Mathf.Abs(deltaX) > MIN_DIRECTION_CHANGE)
		{
			if (deltaX < 0)
				_animatorManager.XLeftChangeAndFlip();
			else if (deltaX > 0)
				_animatorManager.XRightChangeAndFlip();
		}


		_previousPosition = newPosition;

		// If movement completed stop movement
		if (_t >= 1f || _objectPosition == null)
        {
            _moving = false;
        }
    }

    public override void DestroyActuator()
    {
        // No specific cleanup required currently
    }
    // Draw gizmos in the editor 
    private void OnDrawGizmos()
    {
        if (!_debugActuator) return;
        if (_objectPosition != null)
        {
            Gizmos.color = new Color(1f, 0.5f, 0f);
            Gizmos.DrawSphere(_objectPosition.position, 0.2f);
        }
    }
}
