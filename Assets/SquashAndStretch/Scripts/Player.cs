using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private SquashAndStretch _jump;
    [SerializeField] private SquashAndStretch _land;
    [SerializeField] private Rigidbody2D      _rigidbody;
    [SerializeField] private float            _jumpForce = 60;
    [SerializeField] private LayerMask        _groundLayer;
    [SerializeField] private float            _distance = 0.1f;

    private bool  _isJumping;
    private bool  _isOnTheGround;
    private bool  _jumpQueued;
    private float _groundCheckDelay = 0.1f;
    private float _groundCheckTimer;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && _isOnTheGround && !_jumpQueued)
        {
            _jumpQueued = true;
        }
    }

    void FixedUpdate()
    {
        if (_jumpQueued)
        {
            _jump.PlaySquashAndStretch();
            _rigidbody.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
            _isJumping        = true;
            _jumpQueued       = false;
            _groundCheckTimer = _groundCheckDelay;
        }

        if (_groundCheckTimer > 0f)
        {
            _groundCheckTimer -= Time.fixedDeltaTime;
        }
        else
        {
            CheckGround();
        }
    }

    private void CheckGround()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, _distance, _groundLayer);
        if (hit.collider != null)
        {
            _isOnTheGround = true;

            if (_isJumping)
            {
                _isJumping = false;
                _land.PlaySquashAndStretch();
            }
        }
        else
        {
            _isOnTheGround = false;
        }
    }
}