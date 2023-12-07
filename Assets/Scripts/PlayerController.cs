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
    public GameObject projectilePrefab;
    [SerializeField] GameEvent TriggerInteraction;
    [SerializeField] GameEvent MannaChanged;
    [SerializeField] GameObject pointer;
    public int playerManna;
    public int maxManna = 6;

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
        playerManna = maxManna;
    }

    private void Update()
    {
        _time += Time.deltaTime;
        //revert to 0 on scene change
        GatherInput();
    }
    private enum FireState
    {
        playing,
        inUI
    }

    private FireState currentFireState = FireState.playing;

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

        if (_frameInput.MouseDown && CanUseProjectile)
        {
            _frameMouseClicked = _time;
            pointer.SetActive(true);
            //Debug.Log(playerManna);
        }
        if (_frameInput.MouseHeld && _frameMouseClicked == 0 && CanUseProjectile)
        {
            _frameMouseClicked = _time;
        }

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
            _frameMouseUp = _time;
            pointer.SetActive(false);
            //Debug.Log(Mathf.Ceil((_time - _frameMouseClicked)/_stats.ProjectileHoldTime));
            _projectileLevel = (int)Mathf.Min(Mathf.Ceil((_time - _frameMouseClicked) / _stats.ProjectileHoldTime), playerManna);
            //Debug.Log(_projectileLevel + " projectile level");
            _frameMouseClicked = 0;
            if (_projectileLevel > 0)
            {
                _projectileToConsume = true;
                //playerManna -= _projectileLevel;
            }
            

            /*
            pointer.SetActive(false);
            if (_time - _frameMouseClicked < _stats.ProjectileHoldTime)
            {
                if (playerManna > 0 && !_grounded)
                {
                    _projectileToConsume = true;
                    _projectileLevel = 1;
                    playerManna -= 1;
                    //Debug.Log("lvl 1 projectile fired");
                }
                else
                {
                    _projectileToConsume = true;
                    _projectileLevel = 0;
                }
            }
            else if (_time - _frameMouseClicked < _stats.ProjectileHoldTime * 2)
            {
                if (playerManna > 1 && !_grounded)
                {
                    _projectileToConsume = true;
                    _projectileLevel = 2;
                    playerManna -= 2;
                }
                else if (playerManna > 0 && !_grounded)
                {
                    _projectileToConsume = true;
                    _projectileLevel = 1;
                    playerManna -= 1;
                }
                else 
                { 
                    _projectileToConsume = true;
                    _projectileLevel = 0;
                }
            }
            else if (_time - _frameMouseClicked < _stats.ProjectileHoldTime * 3)
            {
                if (playerManna > 2 && !_grounded)
                {
                    _projectileToConsume = true;
                    _projectileLevel = 3;
                    playerManna -= 3;
                }

                else if (playerManna > 1 && !_grounded)
                {
                    _projectileToConsume = true;
                    _projectileLevel = 2;
                    playerManna -= 2;
                }
                else if (playerManna > 0 && !_grounded)
                {
                    _projectileToConsume = true;
                    _projectileLevel = 1;
                    playerManna -= 1;
                }
                else
                {
                    _projectileToConsume = true;
                    _projectileLevel = 0;
                }
            }
            _frameMouseClicked = 0;
            _frameMouseUp = _time;
            MannaChanged.Raise(this, playerManna);
            //Debug.Log(_projectileToConsume);
            */
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

    private bool _shortHopToConsume = false;
    private bool _fullHopToConsume = false;

    private bool _bufferedJumpUsable;
    private bool _coyoteUsable;
    private float _timeJumpWasPressed;

    private bool HasBufferedJump => _bufferedJumpUsable && _time < _timeJumpWasPressed + _stats.JumpBuffer;
    private bool CanUseCoyote => _coyoteUsable && !_grounded && _time < _frameLeftGrounded + _stats.CoyoteTime;

    private void HandleJump()
    {
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
        Jumped?.Invoke();
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
        
        if (!_projectileToConsume)
        {
            return;
        }
        

        if (CanUseProjectile)
        {
            FireProjectile();
        }
        //Debug.Log(currentFireState);
        _projectileToConsume = false;

    }

    private void FireProjectile()
    {
        switch (currentFireState)
        {
            case FireState.playing:
                Vector3 mousePos3D = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 mousePos = new Vector2(mousePos3D.x, mousePos3D.y);
                Vector2 rbPosition = new Vector2(_rb.transform.position.x, _rb.transform.position.y);
                Vector2 direction = (mousePos - rbPosition).normalized;

                if (!_grounded)
                {
                    _frameVelocity += -direction * _stats.ProjectileVelocity * _projectileLevel;
                }

                _frameLastFired = _time;


                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                Instantiate(projectilePrefab, transform.position + new Vector3(direction.x, direction.y, 1), Quaternion.identity);
                playerManna -= _projectileLevel;
                MannaChanged.Raise(this, playerManna);
                return;
            case FireState.inUI:
                TriggerInteraction.Raise(this, null);
                return;
        }
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
            var maxFallSpeed = _stats.MaxFallSpeed;
            if (_slowGravity && CanUseProjectile && _frameVelocity.y < 0) maxFallSpeed = _stats.ChargingFallSpeed;
            _frameVelocity.y = Mathf.MoveTowards(_frameVelocity.y, -maxFallSpeed, inAirGravity * Time.fixedDeltaTime);
        }
    }

    #endregion
    public void SwitchState(Component sender, object data)
    {
        if (sender.GetType() != typeof(DialogueManager))
        {
            if (currentFireState == FireState.playing)
            {
                currentFireState = FireState.inUI;
            }
            else if (currentFireState == FireState.inUI)
            {
                currentFireState = FireState.playing;
            }
            //Debug.Log(currentFireState);
        }

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "MovingPlatform")
        {
            transform.parent = collision.transform;
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("MovingPlatform"))
        {
            transform.parent = null;
        }
    }

    public void OnMannaRestored(Component sender, object data)
    {
        playerManna = maxManna;
        MannaChanged.Raise(this, playerManna);
        //Debug.Log("manna restored");
    }

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