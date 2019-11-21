using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform _fistPlace;
    [SerializeField] private Transform _secondPlace;

    [SerializeField] private PlayerController _player;
    [SerializeField] private PhysicsBehaviour _devourer;

    [Header("Neural Network Values")]
    [SerializeField] private int _brainsCount;
    [SerializeField] private int _bestCount;
    [SerializeField] private int _inputLayerNodes;
    [SerializeField] private int _outputLayerNodes;
    [SerializeField] private int[] _hiddenLayerNodes;

    private Brain[] _neuralNetworks;
    private float[] _lifetimes;
    private int _currentIndex;

    private void Awake()
    {
        _devourer.GetComponent<DevourerController>().SetOnEatAction(StartGame);

        _currentIndex = 0;
        _neuralNetworks = new Brain[_brainsCount];
        _lifetimes = new float[_brainsCount];

        for (int brainIndex = 0; brainIndex < _brainsCount; brainIndex++)
        {
            _neuralNetworks[brainIndex] = new Brain(_inputLayerNodes, _outputLayerNodes, _hiddenLayerNodes);
        }

        StartGame();
    }

    private void StartGame()
    {
        ResetStates();
    }

    private void MovePermission(bool p)
    {
        PhysicsBehaviour player = _player;
        player.Stop = !p;
        _devourer.Stop = !p;
    }

    private void ResetStates()
    {
        _player.Initialized = false;

        if (_currentIndex != 0)
        {
            _lifetimes[_currentIndex - 1] = _player.LifeTime;
            //Debug.Log(_player.LifeTime);

            _player.ResetVelocity();
            _devourer.ResetVelocity();
        }

        _player.transform.position = _fistPlace.position;
        _devourer.transform.position = _secondPlace.position;

        if (_currentIndex < _brainsCount)
        {
            _player.InitializePlayer(_neuralNetworks[_currentIndex], _devourer);
            _currentIndex++;

            MovePermission(true);
        }
        else
        {
            MovePermission(false);
            NewGenerations();
        }
    }

    private void NewGenerations()
    {
        SortBrainsAndLifetimes();

        float lifetimeSum = _lifetimes.Sum();

        List<Brain> bestResults = new List<Brain>();

        for (int i = 0; i < _neuralNetworks.Length; i++)
        {
            if (i < _bestCount) bestResults.Add(_neuralNetworks[i]);
        }
    }

    private void SortBrainsAndLifetimes()
    {
        for (int i = 0; i < _lifetimes.Length; i++)
        {
            for (int j = i + 1; j < _lifetimes.Length; j++)
            {
                if (!(_lifetimes[i] < _lifetimes[j])) continue;

                float tempTime = _lifetimes[i];
                _lifetimes[i] = _lifetimes[j];
                _lifetimes[j] = tempTime;

                Brain tempBrain = _neuralNetworks[i];
                _neuralNetworks[i] = _neuralNetworks[j];
                _neuralNetworks[j] = tempBrain;
            }
        }
    }
}
