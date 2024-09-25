using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    private Vector3 velocity;
    [SerializeField] private BoidManager boidManager;

    [SerializeField] private float speed;  // Speed of the boid
    [SerializeField] private float rotationSpeed;  // Rotation speed
    [SerializeField] private float neighborDistance;  // Distance to consider other boids as neighbors
    [SerializeField] private float avoidanceDistance;  // Minimum distance to avoid other boids
    [SerializeField] private float maxForce;  // Maximum steering force

    void Start()
    {
        // Set random initial velocity
        velocity = transform.forward * boidManager.speed;
        boidManager = FindObjectOfType<BoidManager>();  // Get reference to BoidManager
    }

    void Update()
    {
        speed = boidManager.speed;
        rotationSpeed = boidManager.rotationSpeed;
        neighborDistance = boidManager.neighborDistance;
        avoidanceDistance = boidManager.avoidanceDistance;
        maxForce = boidManager.maxForce;

        ApplyFlockingRules();
        Move();
    }

    // Move the boid according to its velocity
    void Move()
    {
        // Limit the speed
        velocity = Vector3.ClampMagnitude(velocity, speed);

        // Update position and rotation
        transform.position += velocity * Time.deltaTime;
        if (velocity != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(velocity);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
        }
    }

    // Apply flocking rules: Separation, Alignment, Cohesion
    void ApplyFlockingRules()
    {
        Vector3 separation = Vector3.zero;
        Vector3 alignment = Vector3.zero;
        Vector3 cohesion = Vector3.zero;
        int neighborCount = 0;

        foreach (Boid otherBoid in boidManager.boids)
        {
            if (otherBoid != this)
            {
                float distance = Vector3.Distance(otherBoid.transform.position, transform.position);

                // Separation - steer to avoid crowding local flockmates
                if (distance < avoidanceDistance)
                {
                    separation -= (otherBoid.transform.position - transform.position);
                }

                // Alignment and Cohesion
                if (distance < neighborDistance)
                {
                    alignment += otherBoid.velocity;
                    cohesion += otherBoid.transform.position;
                    neighborCount++;
                }
            }
        }

        if (neighborCount > 0)
        {
            alignment /= neighborCount;
            alignment = Vector3.ClampMagnitude(alignment, maxForce);

            cohesion /= neighborCount;
            cohesion = (cohesion - transform.position).normalized;
            cohesion = Vector3.ClampMagnitude(cohesion, maxForce);
        }

        // Apply the calculated steering forces
        velocity += (separation.normalized + alignment + cohesion) * Time.deltaTime;
    }

    //void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.green;
    //    Gizmos.DrawWireSphere(transform.position, neighborDistance);  // Neighbor detection radius

    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(transform.position, avoidanceDistance);  // Avoidance distance
    //}
}