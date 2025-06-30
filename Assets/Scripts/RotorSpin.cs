using UnityEngine;

public class RotorSpin : MonoBehaviour
{
    public float maxRotationSpeed = 2000f;
    public float acceleration = 50f;
    private float currentSpeed = 0f;

    void Update()
    {
        if (currentSpeed < maxRotationSpeed)
        {
            currentSpeed += acceleration * Time.deltaTime;
            currentSpeed = Mathf.Min(currentSpeed, maxRotationSpeed);
        }

        transform.Rotate(Vector3.up * currentSpeed * Time.deltaTime);
    }
}
