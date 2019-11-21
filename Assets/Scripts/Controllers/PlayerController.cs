using UnityEngine;

public class PlayerController : PhysicsBehaviour
{
    private Brain _currentBrain;
    private PhysicsBehaviour _devourer;
    private float[] _tempValues;

    public float LifeTime { get; private set; }

    public bool Initialized { get; set; }
    public void InitializePlayer(Brain brain, PhysicsBehaviour devourer)
    {
        _devourer = devourer;
        _currentBrain = brain;
        _tempValues = new float[brain[0]];
        LifeTime = 0;

        Initialized = true;
    }

    private void Update()
    {
        if (!Initialized || Stop) return;

        LifeTime += Time.deltaTime;
        var playerVelocity = RigidBody.velocity;
        _tempValues[0] = (playerVelocity.x / MaxVelocity + 1) / 2;
        _tempValues[1] = (playerVelocity.y / MaxVelocity + 1) / 2;
        _tempValues[2] = (playerVelocity.z / MaxVelocity + 1) / 2;

        var enemyVelocity = _devourer.RigidBody.velocity;
        _tempValues[3] = (enemyVelocity.x / _devourer.MaxVelocity + 1) / 2;
        _tempValues[4] = (enemyVelocity.y / _devourer.MaxVelocity + 1) / 2;
        _tempValues[5] = (enemyVelocity.z / _devourer.MaxVelocity + 1) / 2;

        var playerPosition = transform.position;
        _tempValues[6] = (playerPosition.x / MaxVelocity + 1) / 2;
        _tempValues[7] = (playerPosition.y / MaxVelocity + 1) / 2;
        _tempValues[8] = (playerPosition.z / MaxVelocity + 1) / 2;

        var enemyPosition = _devourer.transform;
        var position = enemyPosition.position;
        _tempValues[9] = (position.x / _devourer.MaxVelocity + 1) / 2;
        _tempValues[10] = (position.x / _devourer.MaxVelocity + 1) / 2;
        _tempValues[11] = (position.x / _devourer.MaxVelocity + 1) / 2;

        float[] result = _currentBrain.Process(_tempValues);
        float forceX = (result[0] * 2 - 1) * MaxVelocity;
        float forceY = (result[1] * 2 - 1) * MaxVelocity;
        float forceZ = (result[2] * 2 - 1) * MaxVelocity;

        Vector3 newForce = new Vector3(forceX, forceY, forceZ);

        ClampVelocity();
    }

    /// <summary> Control player with keyboard for physics test.
    /// (LEGACY!) </summary>
    // ReSharper disable once UnusedMember.Local
    private void KeyMove()
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
    }
}
