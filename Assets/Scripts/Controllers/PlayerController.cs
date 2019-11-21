using UnityEngine;

public class PlayerController : PhysicsBehaviour
{
    private void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            forceVector.z += force;
        }

        if (Input.GetKey(KeyCode.S))
        {
            forceVector.z -= force;
        }

        if (Input.GetKey(KeyCode.D))
        {
            forceVector.x += force;
        }

        if (Input.GetKey(KeyCode.A))
        {
            forceVector.x -= force;
        }

        RigidBody.AddForce(forceVector);
        forceVector = Vector3.zero;

        ClampVelocity();
    }
}
