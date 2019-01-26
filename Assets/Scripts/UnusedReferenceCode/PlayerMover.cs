using System;
using UnityEngine;

/// <summary>
/// This class encapsulates all of the logic and state related to moving the player through
/// the world. This includes movement mechanics as well as collision detection.
/// </summary>
public class PlayerMover 
{
    [Serializable]
    public struct Configuration 
    {
        [Tooltip("The maximum speed that the player can move in the XZ plane.")]
        public float MaxSpeed;

        [Tooltip("The y-speed that the player gets set when jumping.")]
        public float JumpSpeed;

        [Tooltip("Acceleration applied when moving in the XZ plane.")]
        public float Acceleration;

        [Tooltip("XZ acceleration applied against velocity when no movement inputs are sent by the player.")]
        public float Deceleration;

        [Tooltip("Acceleration along the y-axis constantly applied.")]
        public float Gravity;

        [Tooltip("Impulse applications will cause at least this much difference in y-speed.")]
        public float MinimumJumpFromImpulse;

        [Tooltip("Maximum speed the player can travel along the y-axis.")]
        public float MaxVerticalSpeed;

        [Tooltip("Radius of the player used to compute wall collisions.")]
        public float BodyRadius;

        [Tooltip("Layers to take in to account when performing floor collisions.")]
        public LayerMask FloorMask;

        [Tooltip("Layers to take in to account when performing wall collisions.")]
        public LayerMask WallMask;
    }

    public Vector3 Position { get; private set; }
    public Vector3 Velocity { get; private set; }
    public bool Standing { get { return _bindingBody != null; } }
    public int LatestStandingLayer { get; private set; }

    const float GROUND_RAY_DEPTH = 0.5f;

    readonly float _height;
    readonly Configuration _config;

    Rigidbody _bindingBody;     // If the player is standing, this is the rigidbody they are standing on. Null in the air.
    Vector3 _relativePosition;  // When standing, position relative to _bindingCollider. Absolute position in air.
    Vector3 _lastRelativePosition;
    bool _wasBoundLastStep;

    public PlayerMover(float height, Vector3 initialPos, Configuration config)
    {
        _height = height;
        Position = initialPos;
        _config = config;
        _relativePosition = initialPos;
        _lastRelativePosition = initialPos;
    }

    /// <summary>
    /// Returns the world space position that the player should be rendered at each frame. This
    /// position is lerped between physics steps in the relative platform space.
    /// <summary>
    public Vector3 GetRenderPosition()
    {
        var relativeLerped = Vector3.Lerp(_lastRelativePosition, _relativePosition, (Time.time - Time.fixedTime) / Time.fixedDeltaTime);
        var basePos = _bindingBody ? _bindingBody.transform.position : Vector3.zero;
        return basePos + relativeLerped;
    }

    /// <summary>
    /// Directly sets the player position. It will also set the player velocity to zero.
    /// <summary>
    /// <param name="position"> World space position to move the player to. </param>
    public void SetPosition(Vector3 position)
    {
        _relativePosition = position;
        _lastRelativePosition = position;
        _bindingBody = null;
        Velocity = Vector3.zero;
    }

    /// <summary>
    /// Returns the player speed as fraction of their maximum speed. If the player is moving
    /// faster than the maximum possible by walking alone, the value is clamped off at one.
    /// <summary>
    public float GetSpeedRatio()
    {
        return Mathf.Clamp01(Velocity.WithY(0f).sqrMagnitude / (_config.MaxSpeed * _config.MaxSpeed));
    }

    /// <summary>
    /// Applies a direct velocity change to the player.
    /// <summary>
    /// <param name="impulse"> Velocity to add to the player's. </param>
    public void ApplyImpulse(Vector3 impulse)
    {
        Velocity += impulse;
        _relativePosition = Position;
        _lastRelativePosition = _relativePosition;
        _bindingBody = null;

        if (Velocity.y < _config.MinimumJumpFromImpulse) {
            Velocity = Velocity.WithY(_config.MinimumJumpFromImpulse);
        }
        else if (Velocity.y > _config.MaxVerticalSpeed) {
            Velocity = Velocity.WithY(_config.MaxVerticalSpeed);
        }
    }

    /// <summary>
    /// Steps the player movement forward by one frame. This is expected to be called on FixedUpdate.
    /// <summary>
    public void Step(Vector3 movementDirection, bool jumping)
    {
        // Handle walking off edges and jumping.
        if (_wasBoundLastStep && !_bindingBody) {
            _relativePosition = Position;
        }
        if (jumping && _bindingBody) {
            _relativePosition += _bindingBody.position;
            _bindingBody = null;
            Velocity = Velocity.WithY(_config.JumpSpeed);
        }

        // Integrate accelerations in to velocity.
        if (_bindingBody) {
            Velocity = Velocity.WithY(0f);
        } else {
            Velocity += Vector3.up * _config.Gravity * Time.fixedDeltaTime;
        }
        Velocity = updateVelocityXZ(Velocity, movementDirection, _config);

        // Update relative position using new velocity.
        _lastRelativePosition = _relativePosition;
        _relativePosition += Velocity * Time.fixedDeltaTime;

        // Perform collision detection on the naive position update.
        var worldPosAfterWalls = moveAndCollideWalls();
        moveAndCollideFloors(worldPosAfterWalls);
    }

    Vector3 moveAndCollideWalls()
    {
        var basePos = _bindingBody ? _bindingBody.position : Vector3.zero;
        var newWorldPos = basePos + _relativePosition;
        var deltaXZ = (_relativePosition - _lastRelativePosition).WithY(0);
        var dirXZ = deltaXZ.normalized;

        RaycastHit wallHitInfo;
        var wallRayDidHit = Physics.SphereCast(
            basePos + _lastRelativePosition - dirXZ * _config.BodyRadius + Vector3.up * _config.BodyRadius,
            _config.BodyRadius,
            dirXZ,
            out wallHitInfo,
            deltaXZ.magnitude + _config.BodyRadius,
            _config.WallMask.value
        );

        if (wallRayDidHit) {
            var wallTang = Vector3.Cross(Vector3.up, wallHitInfo.normal);
            var projectedDelta = Vector3.Project(deltaXZ, wallTang.normalized);
            var newXZ = basePos + _lastRelativePosition + projectedDelta;
            newWorldPos = newXZ.WithY(newWorldPos.y);
        }

        return newWorldPos;
    }

    void moveAndCollideFloors(Vector3 newWorldPos)
    {
        RaycastHit verticalHitInfo;
        var rayDidHit = Physics.Raycast(
            newWorldPos + Vector3.up * _height,
            Vector3.down, 
            out verticalHitInfo, 
            _height + (_bindingBody ? GROUND_RAY_DEPTH : 0f),
            _config.FloorMask.value
        ); 

        var didHaveBindingBody = _bindingBody != null;

        if (rayDidHit) {
            newWorldPos.y = verticalHitInfo.point.y;
            _bindingBody = verticalHitInfo.rigidbody;
            LatestStandingLayer = verticalHitInfo.collider.gameObject.layer;
            _relativePosition = newWorldPos - _bindingBody.position;
        } else {
            _bindingBody = null;
            _relativePosition = newWorldPos;
        }

        var nowHasBindingBody = _bindingBody != null;

        if (didHaveBindingBody != nowHasBindingBody) {
            _lastRelativePosition = _relativePosition;
        }

        _wasBoundLastStep = nowHasBindingBody;
        Position = newWorldPos;
    }

    static Vector3 updateVelocityXZ(Vector3 velocity, Vector3 movementDirection, Configuration config)
    {
        // If movement direction is non-zero, accelerate towards it.
        if (movementDirection.sqrMagnitude > 0.5f) {
            velocity += movementDirection.WithY(0) * config.Acceleration * Time.fixedDeltaTime;

            // Cap the ground plane velocity off at the configured speed.
            var flatSpeedSquared = velocity.WithY(0).sqrMagnitude;
            if (flatSpeedSquared > config.MaxSpeed * config.MaxSpeed) {
                var flatSpeed = Mathf.Sqrt(flatSpeedSquared);
                var scaledVel = velocity * config.MaxSpeed / flatSpeed;
                velocity = scaledVel.WithY(velocity.y);
            }
        } 
        // If there is no movement direction, decelerate the player.
        else {
            var decelAmount = config.Deceleration * Time.fixedDeltaTime;
            if (velocity.WithY(0f).sqrMagnitude > decelAmount * decelAmount) {
                velocity -= decelAmount * velocity.WithY(0f).normalized;
            } else {
                velocity.x = 0;
                velocity.z = 0;
            }
        }

        return velocity;
    }
}