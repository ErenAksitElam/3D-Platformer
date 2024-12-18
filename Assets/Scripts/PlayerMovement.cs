using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody rb;
    public float movementSpeed = 5f;
    public float jumpForce = 5f;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    public float gravity = -9.81f;
    private float velocityY;

    public Transform cam;
    public CharacterController controller;

    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask ground;

    [SerializeField] AudioSource jumpSound;

    public Animator anim;

    [SerializeField] float chainJump = 1f;

    public ParticleSystem dust;



    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontalInput, 0f, verticalInput).normalized;

        //rb.velocity = new Vector3(horizontalInput * movementSpeed, rb.velocity.y, verticalInput * movementSpeed);

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * movementSpeed * Time.deltaTime);
        }

        //anim.SetFloat("isMoving", direction.magnitude);

        if (direction.magnitude == 1)
        {
            anim.SetFloat("isMoving", 1);
            CreateDust();
            
        }
        else if (direction.magnitude == 0)
        {
            anim.SetFloat("isMoving", 0);
        }


        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            Jump();

        }

        velocityY += gravity * Time.deltaTime;
        controller.Move(new Vector3(0f, velocityY, 0f) * Time.deltaTime);

        if (IsGrounded())
        {
            StartCoroutine(WaitAndPrint());
            anim.SetBool("isGrounded", true);
        }
        if (!IsGrounded())
        {
            anim.SetBool("isGrounded", false);
        }
    }

    void Jump()
    {
        velocityY = Mathf.Sqrt(jumpForce * -2f * gravity * chainJump);
        jumpSound.Play();
        if (chainJump >= 5f)
        {
            chainJump = 1f;
        }
        chainJump += 2f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy Head"))
        {
            Destroy(collision.transform.parent.gameObject);
            Jump();
        }

        if (collision.gameObject.CompareTag("ToLevel1"))
        {
            LoadScene1();
        }
    }

    void LoadScene1()
    {
        SceneManager.LoadScene("Level_1");
    }
    bool IsGrounded()
    {
        return Physics.CheckSphere(groundCheck.position, .3f, ground);
    }

    IEnumerator WaitAndPrint()
    {

        yield return new WaitForSeconds(2);

        if (IsGrounded())
        {
            chainJump = 1f;
        }
    }

    void CreateDust()
    {
        dust.Play();
    }
}
