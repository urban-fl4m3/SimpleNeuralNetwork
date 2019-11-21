using UnityEngine;

public abstract class PhysicsBehaviour : MonoBehaviour
{
    public Rigidbody RigidBody { get; private set; }
    public float MaxVelocity => _velocityClamp;

    [SerializeField] protected float force;
    [SerializeField] private float _velocityClamp;
    protected Vector3 forceVector;
    
    public bool Stop { protected get; set; }

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