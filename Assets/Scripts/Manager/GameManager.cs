using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Neural Network Values")]
    [SerializeField] private int _brainsCount;
    [SerializeField] private int _bestCount;
    [SerializeField] private int _inputLayerNodes;
    [SerializeField] private int _outputLayerNodes;
    [SerializeField] private int[] _hiddenLayerNodes;

    private List<Game> _activeGames;
    [SerializeField] private Game _gamePrefab;

    private Brain[] _neuralNetworks;
    private float[] _lifetimes;

    private int _completedEncounters;

    private int _generationCount;

    private void Awake()
    {
        _generationCount = 0;
        _completedEncounters = 0;
        
        _activeGames = new List<Game>();
        _neuralNetworks = new Brain[_brainsCount];
        _lifetimes = new float[_brainsCount];

        for (int brainIndex = 0; brainIndex < _brainsCount; brainIndex++)
        {
            Brain brain = new Brain(_inputLayerNodes, _outputLayerNodes, _hiddenLayerNodes);
            _neuralNetworks[brainIndex] = brain;

            Game newGame = Instantiate(_gamePrefab, new Vector3(brainIndex * 100, 0, 0), Quaternion.identity, transform);
            newGame.Initialize(brainIndex, CompleteEncounter);
            _activeGames.Add(newGame);
        }

        StartGames();
    }

    private void StartGames()
    {
        for (int gameIndex = 0; gameIndex < _activeGames.Count; gameIndex++)
        {
            _activeGames[gameIndex].StartGame(_neuralNetworks[gameIndex]);
        }

        _generationCount++;
        Debug.Log(_generationCount);
    }

    private void CompleteEncounter(Game game)
    {
        int gameID = game.ID;
        _lifetimes[gameID] = game.GameTime;
        _completedEncounters++;

        UIManager.Instance.SetLifeTime(game.GameTime);

        if (_completedEncounters < _activeGames.Count) return;

        _completedEncounters = 0;
        NewGenerations();
        StartGames();
    }

    private void NewGenerations()
    {
        SortBrainsAndLifetimes();

        float lifetimeSum = _lifetimes.Sum();

        List<Brain> newGeneration = new List<Brain>();
        List<Brain> roulette = new List<Brain>();

        for (int i = 0; i < _neuralNetworks.Length; i++)
        {
            if (i < _bestCount) newGeneration.Add(_neuralNetworks[i]);
            int coeff = (int)(_lifetimes[i] / lifetimeSum * 100);

            for (int j = 0; j < coeff; j++)
            {
                roulette.Add(_neuralNetworks[i]);
            }
        }

        for (int i = 0; i < _brainsCount - _bestCount; i++)
        {
            int randIndexF = Random.Range(0, roulette.Count);
            int randIndexS = Random.Range(0, roulette.Count);

            Brain crossover = Brain.Crossover(roulette[randIndexF], roulette[randIndexS]);
            newGeneration.Add(crossover);
        }

        for (int i = _bestCount; i < newGeneration.Count; i++)
        {
            Brain gen = newGeneration[i];
            gen.Mutate();
            _neuralNetworks[i] = gen;
        }
    }

    private void SortBrainsAndLifetimes()
    {
        for (int i = 0; i < _lifetimes.Length; i++)
        {
            for (int j = i + 1; j < _lifetimes.Length; j++)
            {
                if (_lifetimes[i] > _lifetimes[j]) continue;

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
