using UnityEngine;

public class PlayerController : PhysicsBehaviour
{
    private Brain _currentBrain;
    private DevourerController _devourer;
    private float[] _tempValues;

    public bool Initialized { private get; set; }

    [SerializeField] private Transform _wallLeft;
    [SerializeField] private Transform _wallRight;
    [SerializeField] private Transform _wallTop;
    [SerializeField] private Transform _wallBot;

    public void InitializePlayer(Brain brain, DevourerController devourer)
    {
        _devourer = devourer;
        _currentBrain = brain;
        _tempValues = new float[brain[0]];

        Initialized = true;
    }

    private void Update()
    {
        if (!Initialized || Stop) return;

        NeuronsMove();
        //KeyMove();
        ClampVelocity();
    }

    private void NeuronsMove()
    {
        var playerVelocity = RigidBody.velocity;
        _tempValues[0] = playerVelocity.x;
        _tempValues[1] = playerVelocity.z;

        var enemyVelocity = _devourer.RigidBody.velocity;
        _tempValues[2] = enemyVelocity.x;
        _tempValues[3] = enemyVelocity.z;

        var playerPosition = transform.position;
        _tempValues[4] = playerPosition.x;
        _tempValues[5] = playerPosition.z;

        var enemyPosition = _devourer.transform.position;
        _tempValues[6] = enemyPosition.x;
        _tempValues[7] = enemyPosition.z;

        var leftDistance = playerPosition.x - _wallLeft.position.x;
        var rightDistance = _wallRight.position.x - playerPosition.x;
        var topDistance = _wallTop.position.z - playerPosition.z;
        var botDistance = playerPosition.z - _wallBot.position.z;
        var enemyDistance = Vector3.Distance(playerPosition, enemyPosition) - _devourer.EatDistance;

        _tempValues[8] = leftDistance;
        _tempValues[9] = rightDistance;
        _tempValues[10] = topDistance;
        _tempValues[11] = botDistance;
        _tempValues[12] = enemyDistance;

        float[] result = _currentBrain.Process(_tempValues);
        float forceX = (result[0] * 2 - 1) * MaxVelocity;
        float forceY = (result[1] * 2 - 1) * MaxVelocity;
        float forceZ = (result[2] * 2 - 1) * MaxVelocity;

        Vector3 newForce = new Vector3(forceX, forceY, forceZ);
        RigidBody.velocity = newForce;
    }

    /// <summary> Control player with keyboard for physics test.
    /// (LEGACY!) </summary>
    // ReSharper disable once UnusedMember.Local
    private void KeyMove()
    {
        if (Input.GetKey(KeyCode.W))
        {
            forceVector.z += MaxVelocity;
        }

        if (Input.GetKey(KeyCode.S))
        {
            forceVector.z -= MaxVelocity;
        }

        if (Input.GetKey(KeyCode.D))
        {
            forceVector.x += MaxVelocity;
        }

        if (Input.GetKey(KeyCode.A))
        {
            forceVector.x -= MaxVelocity;
        }

        RigidBody.velocity = forceVector;
        forceVector = Vector3.zero;
    }
}
