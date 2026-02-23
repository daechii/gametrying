using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 3.5f;
    Rigidbody2D rb;
    Vector2 moveInput;
    [SerializeField] private Animator _animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        _animator = GetComponent<Animator>();
    }
    void Update()
    {
        rb.linearVelocity = new Vector2(moveInput.x * speed, moveInput.y * speed);
        transform.position += (Vector3)(moveInput * speed * Time.fixedDeltaTime);
        //Debug.Log($"moveInput: {moveInput}, скорость: {rb.linearVelocity}");

    }
    public void Move(InputAction.CallbackContext context)
    {
        _animator.SetBool("IsWalking", true);

        if (context.canceled)
        {
            _animator.SetBool("IsWalking", false);
            _animator.SetFloat("LastInputX", moveInput.x);
            _animator.SetFloat("LastInputY", moveInput.y);
        }

        moveInput = context.action.ReadValue<Vector2>();

        _animator.SetFloat("CurrentInputX", moveInput.x);
        _animator.SetFloat("CurrentInputY", moveInput.y);
    }
    
}

    