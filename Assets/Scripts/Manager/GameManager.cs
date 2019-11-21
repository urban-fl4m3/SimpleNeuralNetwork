using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform _fistPlace;
    [SerializeField] private Transform _secondPlace;

    [SerializeField] private PhysicsBehaviour _player;
    [SerializeField] private PhysicsBehaviour _devourer;

    [Header("Neural Network Values")]
    [SerializeField] private int _inputLayerNodes;
    [SerializeField] private int _outputLayerNodes;
    [SerializeField] private int[] _hiddenLayerNodes;

    private Brain _neuralNetwork;
    private float[] _tempValues;

    private void Awake()
    {
        _devourer.GetComponent<DevourerController>().SetOnEatAction(StartGame);
        
        _tempValues = new float[_inputLayerNodes];
        _neuralNetwork = new Brain(_inputLayerNodes, _outputLayerNodes, _hiddenLayerNodes);
    }

    private void Update()
    {
        var playerVelocity = _player.RigidBody.velocity;
        _tempValues[0] = playerVelocity.x;
        _tempValues[1] = playerVelocity.y;
        _tempValues[2] = playerVelocity.z;

        var enemyVelocity = _devourer.RigidBody.velocity;
        _tempValues[3] = enemyVelocity.x;
        _tempValues[4] = enemyVelocity.y;
        _tempValues[5] = enemyVelocity.z;

        var playerPosition = _player.transform.position;
        _tempValues[6] = playerPosition.x;
        _tempValues[7] = playerPosition.y;
        _tempValues[8] = playerPosition.z;

        var enemyPosition = _devourer.transform;
        _tempValues[9] = enemyPosition.position.x;
        _tempValues[10] = enemyPosition.position.y;
        _tempValues[11] = enemyPosition.position.z;
    }

    private void StartGame()
    {
        _player.transform.position = _fistPlace.position;
        _devourer.transform.position = _secondPlace.position;
        ResetStates();
    }

    private void ResetStates()
    {
        _player.ResetVelocity();
        _devourer.ResetVelocity();
    }
}
