using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasciInput : MonoBehaviour
{
    [SerializeField] private float walkSpeed = 5;
    [SerializeField] private float sprintSpeed = 5;
    [SerializeField] private float rotSpeed = 5;

    [SerializeField] private string _animatorIsGrounded;
    [SerializeField] private string _animatorIsRolling;
    [SerializeField] private string _animatorSpeed;
    [SerializeField] private string _animatorWalk;

    [Header("Keybinds")]
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode rollKey = KeyCode.LeftAlt;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask WhatIsGround;
    public bool grounded;

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
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, WhatIsGround);

        if (grounded)
        {
            _animator.SetTrigger(_animatorIsGrounded);
        }

        StateHandler();
    }

    private void StateHandler()
    {
        //Mode - Sprinting
        if (grounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            _moveSpeed = sprintSpeed;
        }

        //Mode - walking
        else if (grounded)
        {
            state = MovementState.walking;
            _moveSpeed = walkSpeed;

            _animator.SetTrigger(_animatorIsGrounded);
        }
    }

    private void FixedUpdate()
    {
        _rigidbody.velocity = _direction * _moveSpeed;

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
