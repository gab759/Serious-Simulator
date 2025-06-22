using UnityEngine;

public class CarController : MonoBehaviour
{
    public float motorForce = 1500f;
    public float steeringForce = 30f;

    private float horizontalInput;
    private float verticalInput;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        GetInput();
        Move();
        Steer();
    }

    void GetInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
    }

    void Move()
    {
        Vector3 force = transform.right * verticalInput * motorForce * Time.fixedDeltaTime;
        rb.AddForce(force, ForceMode.Acceleration);
    }

    void Steer()
    {
        float steer = horizontalInput * steeringForce * Time.fixedDeltaTime;
        Quaternion turnOffset = Quaternion.Euler(0, steer, 0);
        rb.MoveRotation(rb.rotation * turnOffset);
    }
}
