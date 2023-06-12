using UnityEngine;

public class ConchyHelper : MonoBehaviour
{
    public GameObject depositPoint;
    public ParticleSystem particleSystemPrefab;
    public float speed = 5f;
    public float upSpeed = 2f;
    public float rotationSpeed = 5f; // Adjust the rotation speed here
    private Vector3 targetPosition;

    private bool isMovingUp = true;
    private bool isMovingY = true;
    private ParticleSystem particleSystemInstance;

    private void Start()
    {
        // Find the deposit point game object by tag
        depositPoint = GameObject.FindGameObjectWithTag("DepositPoint");
        targetPosition.y = depositPoint.transform.position.y;

        // Check if a deposit point was found
        if (depositPoint == null)
        {
            Debug.LogError("No deposit point found in the scene!");
        }
    }

    private void Update()
    {
        // Move up
        if (isMovingUp && isMovingY)
        {
            transform.Translate(Vector3.up * upSpeed * Time.deltaTime);

            // Check if reached desired height
            if (transform.position.y >= targetPosition.y)
            {
                isMovingUp = false;
            }
        }
        else
        {
            // Move towards deposit point
            if (depositPoint != null)
            {
                Vector3 depositPosition = depositPoint.transform.position;
                depositPosition.y = transform.position.y; // Lock the deposit position on the y-axis
                transform.position = Vector3.MoveTowards(transform.position, depositPosition, speed * Time.deltaTime);

                // Check if reached deposit point
                if (transform.position == depositPosition)
                {
                    // Detach child objects with the tag "Ingredient"
                    DetachChildObjectsWithTag("Ingredient");
                    DetachChildObjectsWithTag("Utensil");

                    // Instantiate the particle system
                    if (particleSystemPrefab != null)
                    {
                        particleSystemInstance = Instantiate(particleSystemPrefab, transform.position, Quaternion.identity);
                    }

                    // Stop moving
                    Destroy(gameObject);
                }
            }
        }

        // Rotate towards the deposit point
        RotateTowardsDepositPoint();
    }

    private void DetachChildObjectsWithTag(string tag)
    {
        Transform[] childObjects = GetComponentsInChildren<Transform>(true);

        foreach (Transform child in childObjects)
        {
            if (child.CompareTag(tag))
            {
                // Activate gravity for the child object's Rigidbody component
                Rigidbody childRigidbody = child.GetComponent<Rigidbody>();
                if (childRigidbody != null)
                {
                    childRigidbody.useGravity = true;
                    childRigidbody.isKinematic = false;
                }
                Collider childCollider = child.GetComponent<Collider>();
                if (childCollider != null)
                {
                    childCollider.enabled = true;
                }
                // Detach from parent
                child.parent = null;
            }
        }
    }

    private void RotateTowardsDepositPoint()
    {
        // Calculate the direction to the deposit point
        Vector3 direction = depositPoint.transform.position - transform.position;
        direction.y = 0f; // Ignore vertical movement

        // Check if there is any movement
        if (direction != Vector3.zero)
        {
            // Calculate the target rotation based on the movement direction
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            // Smoothly rotate towards the target rotation
            Quaternion newRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Apply the new rotation
            transform.rotation = newRotation;
        }
    }
}
