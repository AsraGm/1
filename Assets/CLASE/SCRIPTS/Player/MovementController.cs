using Fusion;
using Fusion.Addons.KCC;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(KCC))]
public class MovementController : NetworkBehaviour
{
    [Header("Velocidades")]
    [SerializeField] private float walkSpeed = 5.5f;
    [SerializeField] private float runSpeed = 7.7f;
    [SerializeField] private float crouchSpeed = 3.9f;

    [Header("Animator")]
    [SerializeField] private Animator animator;

    private KCC kcc;

    private void Awake()
    {
        kcc = GetComponent<KCC>();
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData input))
        {
            Movement(input);
            UpdateAnimator(input);
        }
    }

    private void Movement(NetworkInputData input)
    {
        if (kcc == null) return;
               
        Vector3 moveDirection = transform.rotation * new Vector3(input.move.x, 0f, input.move.y);
               
        float speed = GetSpeed(input);
        kcc.SetKinematicVelocity(moveDirection.normalized * speed);
    }

    private float GetSpeed(NetworkInputData input)
    {        
        if (input.move.y < 0 || input.move.x != 0)
            return walkSpeed;
       
        if (input.isRunning)
            return runSpeed;

        return walkSpeed;
    }

    private void UpdateAnimator(NetworkInputData input)
    {
        if (animator == null) return;

        bool isMoving = input.move.magnitude > 0.01f;
        animator.SetBool("IsWalking", isMoving);
        animator.SetBool("IsRunning", input.isRunning && isMoving);
        animator.SetFloat("WalkingZ", input.move.y);
        animator.SetFloat("WalkingX", input.move.x);
    }

}

