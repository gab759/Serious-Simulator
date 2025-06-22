using UnityEngine;

public class HelicopterController : MonoBehaviour
{
    public float liftForce = 10f;
    public float moveSpeed = 5f;
    public float turnSpeed = 50f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Vector3 inputDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        Vector3 moveDir = new Vector3(inputDir.z, 0, -inputDir.x) * moveSpeed;

        rb.AddForce(moveDir, ForceMode.Acceleration);

        if (Input.GetKey(KeyCode.E))
            rb.AddForce(Vector3.up * liftForce, ForceMode.Acceleration);
        if (Input.GetKey(KeyCode.R))
            rb.AddForce(Vector3.down * liftForce, ForceMode.Acceleration);

        float yaw = Input.GetAxis("Horizontal") * turnSpeed * Time.fixedDeltaTime;
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, yaw, 0f));
    }
}
