using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour, IPlayerController
{
    [SerializeField] private ScriptableStats _stats;
    private Rigidbody2D _rb;
    private CapsuleCollider2D _col;
    private FrameInput _frameInput;
    private Vector2 _frameVelocity;
    private bool _cachedQueryStartInColliders;

    #region Interface

    public Vector2 FrameInput => _frameInput.Move;
    public event Action<bool, float> GroundedChanged;
    public event Action Jumped;

    #endregion

    private float _time;
    private bool _slowGravity = false;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<CapsuleCollider2D>();

        _cachedQueryStartInColliders = Physics2D.queriesStartInColliders;
    }

    private void Update()
    {
        _time += Time.deltaTime;
        //revert to 0 on scene change
        GatherInput();
    }

    private void GatherInput()
    {
        
        _frameInput = new FrameInput
        {
            JumpDown = Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.C),
            JumpHeld = Input.GetButton("Jump") || Input.GetKey(KeyCode.C),
            JumpUp = Input.GetButtonUp("Jump"),
            Move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")),
            MouseDown = Input.GetMouseButtonDown(0),
            MouseHeld = Input.GetMouseButton(0),
            MouseUp = Input.GetMouseButtonUp(0)
        };

        if (_stats.SnapInput)
        {
            _frameInput.Move.x = Mathf.Abs(_frameInput.Move.x) < _stats.HorizontalDeadZoneThreshold ? 0 : Mathf.Sign(_frameInput.Move.x);
            _frameInput.Move.y = Mathf.Abs(_frameInput.Move.y) < _stats.VerticalDeadZoneThreshold ? 0 : Mathf.Sign(_frameInput.Move.y);
        }

        if (_frameInput.JumpDown)
        {
            _timeJumpWasPressed = _time;
        }
        if (_frameInput.JumpUp)
        {
            if (_time - _timeJumpWasPressed < _stats.JumpSquat && _grounded)
            {
                _shortHopToConsume = true;
            }
        }
        if (_frameInput.JumpHeld)
        {
            if (_time - _timeJumpWasPressed >= _stats.JumpSquat)
            {
                _fullHopToConsume = true;
            }
        }

        if (_frameInput.MouseDown && CanUseProjectile) _frameMouseClicked = _time;
        if (_frameInput.MouseHeld && _frameMouseClicked == 0 && CanUseProjectile) _frameMouseClicked = _time;

        if (_frameInput.MouseHeld && _time - _frameMouseClicked < _stats.ProjectileHoldTime * 3)
        {
            _slowGravity = true;
        }
        else
        {
            _slowGravity = false;
        }

        if (_frameInput.MouseUp)
        {
            if (_time - _frameMouseClicked < _stats.ProjectileHoldTime)
            {
                _projectileLevel = 1;
                _projectileToConsume = true;
                //_frameMouseUp = _time;
            }
            else if (_time - _frameMouseClicked < _stats.ProjectileHoldTime * 2)
            {
                _projectileLevel = 2;
                _projectileToConsume = true;
                //_frameMouseUp = _time;
            }
            else if(_time - _frameMouseClicked < _stats.ProjectileHoldTime * 3)
            {
                _projectileLevel = 3;
                _projectileToConsume = true;
            }
            _frameMouseClicked = 0;
            _frameMouseUp = _time;
        }
    }

    private void FixedUpdate()
    {
        CheckCollisions();

        HandleJump();
        HandleDirection();
        HandleGravity();
        HandleProjectile();

        ApplyMovement();
    }

    #region Collisions

    private float _frameLeftGrounded = float.MinValue;
    private bool _grounded;

    private void CheckCollisions()
    {
        Physics2D.queriesStartInColliders = false;

        // Ground and Ceiling
        bool groundHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.down, _stats.GrounderDistance, ~_stats.PlayerLayer);
        bool ceilingHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.up, _stats.GrounderDistance, ~_stats.PlayerLayer);
        bool leftWallHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.left, _stats.GrounderDistance, ~_stats.PlayerLayer);
        bool rightWallHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.right, _stats.GrounderDistance, ~_stats.PlayerLayer);

        // Hit a Ceiling
        if (ceilingHit) _frameVelocity.y = Mathf.Min(0, _frameVelocity.y);
        if (leftWallHit || rightWallHit) _frameVelocity.x = Mathf.Sign(_frameVelocity.x) * Mathf.Min(0, Mathf.Abs(_frameVelocity.x));


        // Landed on the Ground
        if (!_grounded && groundHit)
        {
            _grounded = true;
            _coyoteUsable = true;
            _bufferedJumpUsable = true;
            //_endedJumpEarly = false;
            GroundedChanged?.Invoke(true, Mathf.Abs(_frameVelocity.y));
        }
        // Left the Ground
        else if (_grounded && !groundHit)
        {
            _grounded = false;
            _frameLeftGrounded = _time;
            GroundedChanged?.Invoke(false, 0);
        }

        Physics2D.queriesStartInColliders = _cachedQueryStartInColliders;
    }

    #endregion


    #region Jumping

    //private bool _jumpToConsume = false;
    private bool _shortHopToConsume = false;
    private bool _fullHopToConsume = false;

    private bool _bufferedJumpUsable;
    //private bool _endedJumpEarly;
    private bool _coyoteUsable;
    private float _timeJumpWasPressed;

    private bool HasBufferedJump => _bufferedJumpUsable && _time < _timeJumpWasPressed + _stats.JumpBuffer;
    private bool CanUseCoyote => _coyoteUsable && !_grounded && _time < _frameLeftGrounded + _stats.CoyoteTime;

    private void HandleJump()
    {
        //if (!_endedJumpEarly && !_grounded && !_frameInput.JumpHeld && _rb.velocity.y > 0) _endedJumpEarly = true;
        /*
        if (!_endedJumpEarly && !_grounded && !_frameInput.JumpHeld && _rb.velocity.y > 0 && _time - _frameLeftGrounded < _stats.HoldTime)
        {
            _endedJumpEarly = true;
        }
        if (!_jumpToConsume && !HasBufferedJump) return;

        if (_jumpToConsume)
        {
            if (_grounded || CanUseCoyote) ExecuteJump();
        }
        _jumpToConsume = false;
        */
        if (!_shortHopToConsume && !_fullHopToConsume && !HasBufferedJump) return;


        if (_shortHopToConsume || _fullHopToConsume)
        {
            if (_grounded || CanUseCoyote) ExecuteJump();
        }

        _shortHopToConsume = false;
        _fullHopToConsume = false;
    }

    private void ExecuteJump()
    {
        //_endedJumpEarly = false;
        _timeJumpWasPressed = 0;
        _bufferedJumpUsable = false;
        _coyoteUsable = false;
        if (_shortHopToConsume)
        {
            _frameVelocity.y = _stats.ShortHopPower;
        }
        else
        {
            _frameVelocity.y = _stats.FullHopPower;
        }
        //_frameVelocity.y = _stats.JumpPower;
        Jumped?.Invoke();
    }

    #endregion

    #region Horizontal

    private void HandleDirection()
    {
        if (_frameInput.Move.x == 0)
        {
            var deceleration = _grounded ? _stats.GroundDeceleration : _stats.AirDeceleration;
            _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, 0, deceleration * Time.fixedDeltaTime);
        }
        else
        {
            _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, _frameInput.Move.x * _stats.MaxSpeed, _stats.Acceleration * Time.fixedDeltaTime);
        }
    }

    #endregion

    #region Projectile

    private float _frameMouseClicked;
    private float _frameMouseUp;
    private int _projectileLevel;
    private bool _projectileToConsume;
    private bool CanUseProjectile;
    private float _frameLastFired;

    private void HandleProjectile()
    {
        CanUseProjectile = _time >= _stats.ProjectileCooldown + _frameLastFired;
        //if (_time >= _frameLastFired + _stats.ProjectileCooldown) _frameLastFired = 0;
        //Debug.Log(CanUseProjectile + " " + _projectileToConsume);
        if (!_projectileToConsume)
        {
            return;
        }
        if (CanUseProjectile)
        {
            FireProjectile();
        }

        _projectileToConsume = false;
        
    }

    private void FireProjectile()
    {
        //Debug.Log("firing projectile");
        Vector3 mousePos3D = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos = new Vector2(mousePos3D.x, mousePos3D.y);

        if (!_grounded)
        {
            Vector2 rbPosition = new Vector2(_rb.transform.position.x, _rb.transform.position.y);

            Vector2 direction = (mousePos - rbPosition).normalized;
            _frameVelocity += -direction * _stats.ProjectileVelocity * _projectileLevel;
        }

        _frameLastFired = _time;
        // have an action here that calls the projectile into existence
    }

    #endregion

    #region Gravity

    private void HandleGravity()
    {
        

        if (_grounded && _frameVelocity.y <= 0f)
        {
            _frameVelocity.y = _stats.GroundingForce;
        }
        else
        {
            var inAirGravity = _stats.FallAcceleration;
            //if (_endedJumpEarly && _frameVelocity.y > 0) inAirGravity *= _stats.JumpEndEarlyGravityModifier;
            if (_slowGravity && CanUseProjectile && _frameVelocity.y < 0) inAirGravity *= _stats.MouseHeldGravityModifier;
            //if (_endedJumpEarly && _frameVelocity.y > 0 && _time > _stats.AirTime + _frameLeftGrounded) inAirGravity *= _stats.JumpEndEarlyGravityModifier;
            _frameVelocity.y = Mathf.MoveTowards(_frameVelocity.y, -_stats.MaxFallSpeed, inAirGravity * Time.fixedDeltaTime);
        }
    }

    #endregion

    private void ApplyMovement() => _rb.velocity = _frameVelocity;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_stats == null) Debug.LogWarning("Please assign a ScriptableStats asset to the Player Controller's Stats slot", this);
    }
#endif
}

public struct FrameInput
{
    public bool JumpDown;
    public bool JumpHeld;
    public bool JumpUp;
    public Vector2 Move;
    public bool MouseDown;
    public bool MouseHeld;
    public bool MouseUp;
}

public interface IPlayerController
{
    public event Action<bool, float> GroundedChanged;

    public event Action Jumped;
    public Vector2 FrameInput { get; }
}