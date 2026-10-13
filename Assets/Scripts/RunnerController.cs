using UnityEngine;

public class RunnerController : MonoBehaviour
{
    public Transform[] lanePoints;
    public float baseForwardSpeed = 9f;
    public float laneMoveSpeed = 10f;
    public float jumpForce = 6.5f;
    public float gravity = -20f;
    public float slideDuration = 0.8f;
    public Collider normalCollider;
    public Collider slideCollider;

    private int laneIndex = 1;
    private float verticalVelocity;
    private bool isGrounded = true;
    private bool isSliding = false;
    private float slideTimer;

    private void Start()
    {
        if (lanePoints == null || lanePoints.Length == 0)
        {
            lanePoints = new Transform[3];
            for (int i = 0; i < 3; i++)
            {
                GameObject lane = new GameObject("Lane_" + i);
                lane.transform.SetParent(transform);
                lane.transform.localPosition = new Vector3((i - 1) * 3f, 0f, 0f);
                lanePoints[i] = lane.transform;
            }
        }
    }

    private void Update()
    {
        MoveForward();
        SmoothLaneMovement();
        ApplyJumpAndGravity();
        UpdateSlideState();
    }

    private void MoveForward()
    {
        transform.position += Vector3.forward * baseForwardSpeed * Time.deltaTime;
    }

    private void SmoothLaneMovement()
    {
        if (lanePoints == null || lanePoints.Length == 0) return;

        Vector3 target = new Vector3(
            lanePoints[laneIndex].position.x,
            transform.position.y,
            transform.position.z
        );

        transform.position = Vector3.Lerp(
            transform.position,
            target,
            laneMoveSpeed * Time.deltaTime
        );
    }

    private void ApplyJumpAndGravity()
    {
        if (!isGrounded)
        {
            verticalVelocity += gravity * Time.deltaTime;
            transform.position += Vector3.up * verticalVelocity * Time.deltaTime;
        }

        if (transform.position.y <= 0f)
        {
            transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
            verticalVelocity = 0f;
            isGrounded = true;
        }
    }

    private void UpdateSlideState()
    {
        if (isSliding)
        {
            slideTimer -= Time.deltaTime;
            if (slideTimer <= 0f)
            {
                isSliding = false;
                if (normalCollider != null)
                    normalCollider.enabled = true;
                if (slideCollider != null)
                    slideCollider.enabled = false;
            }
        }
    }

    public void MoveLeft()
    {
        if (laneIndex > 0)
            laneIndex--;
    }

    public void MoveRight()
    {
        if (laneIndex < lanePoints.Length - 1)
            laneIndex++;
    }

    public void Jump()
    {
        if (!isGrounded) return;
        verticalVelocity = jumpForce;
        isGrounded = false;
    }

    public void Slide()
    {
        if (isSliding || !isGrounded) return;

        isSliding = true;
        slideTimer = slideDuration;

        if (normalCollider != null)
            normalCollider.enabled = false;
        if (slideCollider != null)
            slideCollider.enabled = true;
    }
}
