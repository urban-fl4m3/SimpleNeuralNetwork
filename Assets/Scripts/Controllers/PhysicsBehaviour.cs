using UnityEngine;

public abstract class PhysicsBehaviour : MonoBehaviour
{
    public Rigidbody RigidBody { get; private set; }

    [SerializeField] protected float force;
    [SerializeField] private float _velocityClamp;
    protected Vector3 forceVector;

    protected virtual void Awake()
    {
        RigidBody = GetComponent<Rigidbody>();
        forceVector = new Vector3();
    }

    public void ResetVelocity() => RigidBody.velocity = Vector3.zero;
    public void ClampVelocity()
    {
        if (RigidBody.velocity.magnitude > _velocityClamp) RigidBody.velocity = RigidBody.velocity.normalized * _velocityClamp;
    }
}