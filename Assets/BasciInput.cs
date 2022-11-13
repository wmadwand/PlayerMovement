using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasciInput : MonoBehaviour
{
    public float moveSpeed = 5;
    public float rotSpeed = 5;

    private Animator animator;
    private Rigidbody rigidbody;

    Vector3 direction;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        rigidbody.freezeRotation = true;
    }

    private void Update()
    {
        var hor = Input.GetAxisRaw("Horizontal");
        var ver = Input.GetAxisRaw("Vertical");

        direction = new Vector3(-ver, 0, hor).normalized;
    }

    private void FixedUpdate()
    {
        rigidbody.velocity = direction * moveSpeed * Time.deltaTime;

        if (direction == Vector3.zero) { return; }

        var targetRot = Quaternion.LookRotation(direction, Vector3.up);
        var resultRot = Quaternion.Slerp(rigidbody.rotation, targetRot, rotSpeed * Time.deltaTime);
        rigidbody.MoveRotation(resultRot);
    }
}
