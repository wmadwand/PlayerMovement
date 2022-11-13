using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasciInput : MonoBehaviour
{
    [SerializeField] private float walkSpeed = 5;
    [SerializeField] private float sprintSpeed = 5;
    [SerializeField] private float rotSpeed = 5;
    [SerializeField] private float jumpPower = 5;

    [SerializeField] private string _animatorIsGrounded;
    [SerializeField] private string _animatorIsRolling;
    [SerializeField] private string _animatorSpeed;
    [SerializeField] private string _animatorJump;
    [SerializeField] private string _animatorWalk;

    [Header("Keybinds")]
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode rollKey = KeyCode.LeftAlt;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask WhatIsGround;
    public bool _isGrounded;

    public MovementState state;

    private float _moveSpeed;
    private Animator _animator;
    private Rigidbody _rigidbody;

    private Vector3 _direction;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        _rigidbody.freezeRotation = true;
    }

    private void Update()
    {
        var hor = Input.GetAxisRaw("Horizontal");
        var ver = Input.GetAxisRaw("Vertical");

        _direction = new Vector3(-ver, 0, hor).normalized;

        // ground check
        _isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, WhatIsGround);
        _animator.SetBool(_animatorIsGrounded, _isGrounded);

        if (_isGrounded)
        {
            _animator.SetTrigger(_animatorIsGrounded);
        }

        StateHandler();
    }

    private void StateHandler()
    {
        //Mode - Sprinting
        if (_isGrounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            _moveSpeed = sprintSpeed;
        }

        //Mode - walking
        else if (_isGrounded)
        {
            state = MovementState.walking;
            _moveSpeed = walkSpeed;

            _animator.SetTrigger(_animatorIsGrounded);
        }

        if (Input.GetKey(jumpKey) && _isGrounded)
        {
            _rigidbody.AddForce(transform.up * jumpPower, ForceMode.Impulse);
            _animator.SetTrigger(_animatorJump);
        }

        if (Input.GetKeyDown(rollKey) && _rigidbody.velocity.magnitude > 0)
        {
            _animator.SetTrigger(_animatorIsRolling);
            Debug.Log("_animatorIsRolling");
        }
    }

    private void FixedUpdate()
    {
        if (_isGrounded)
        {
            _rigidbody.velocity = _direction * _moveSpeed;

            _animator.SetFloat(_animatorSpeed, _rigidbody.velocity.magnitude);
        }


        if (_direction == Vector3.zero) { return; }

        var targetRot = Quaternion.LookRotation(_direction, Vector3.up);
        var resultRot = Quaternion.Slerp(_rigidbody.rotation, targetRot, rotSpeed * Time.deltaTime);
        _rigidbody.MoveRotation(resultRot);
    }
}

public enum MovementState
{
    walking,
    sprinting
}
