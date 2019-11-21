using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

/// <summary> Representation of neural network system. </summary>
public class Brain
{
    private Dictionary<string, float> _edges;
    private List<float> _edgeValues;

    private readonly int[] _nodeCounts;
    private List<Node>[] _layers;

    private readonly int _layersCount;

    public Brain(int inputCount, int outputCount, IReadOnlyList<int> hiddenCounts)
    {
        int _hiddenLayersCount = hiddenCounts.Count;
        _layersCount = _hiddenLayersCount + 2;

        _nodeCounts = new int[_layersCount];
        _nodeCounts[0] = inputCount;

        for (int i = 0; i < _hiddenLayersCount; i++)
        {
            _nodeCounts[i + 1] = hiddenCounts[i];
        }

        _nodeCounts[_layersCount - 1] = outputCount;

        _edgeValues = new List<float>();
        CreateLayersAndEdges();
    }

    /// <summary> Optimized method for layers/edge creating. </summary>
    private void CreateLayersAndEdges()
    {
        _layers = new List<Node>[_layersCount];
        _edges = new Dictionary<string, float>();

        for (int layerIndex = 0; layerIndex < _layersCount; layerIndex++)
        {
            _layers[layerIndex] = new List<Node>();

            bool createEdges = layerIndex > 0;
            List<Node> lastLayer = null;
            if (createEdges) lastLayer = _layers[layerIndex - 1];

            for (int nodeIndex = 0; nodeIndex < _nodeCounts[layerIndex]; nodeIndex++)
            {
                Node layerNode = new Node(layerIndex, nodeIndex);
                _layers[layerIndex].Add(layerNode);

                if (!createEdges) continue;
                foreach (Node lastNode in lastLayer)
                {
                    string edgeID = GenerateEdgeID(lastNode.Info, layerNode.Info);
                    _edges.Add(edgeID, Random.Range(-1.0f, 1.0f));
                }
            }
        }
    }

    /// <summary> This method is creating layers (1 input, 1 output, 1 hidden) and fill them with new nodes.
    /// (LEGACY) </summary>
    // ReSharper disable once UnusedMember.Local
    private void CreateLayers()
    {
        _layers = new List<Node>[_layersCount];

        for (int layerIndex = 0; layerIndex < _layersCount; layerIndex++)
        {
            _layers[layerIndex] = new List<Node>();
            for (int nodeIndex = 0; nodeIndex < _nodeCounts[layerIndex]; nodeIndex++)
            {
                Node layerNode = new Node(layerIndex, nodeIndex);
                _layers[layerIndex].Add(layerNode);
            }
        }
    }

    /// <summary> This method is creating edges for every node which doesn't belong to input layers. Basically it means that
    /// layer index starts with 1.
    /// (LEGACY) </summary>
    // ReSharper disable once UnusedMember.Local
    private void CreateEdges()
    {
        _edges = new Dictionary<string, float>();

        for (int layerIndex = 1; layerIndex < _layersCount; layerIndex++)
        {
            List<Node> currentLayer = _layers[layerIndex];
            List<Node> lastLayer = _layers[layerIndex - 1];

            foreach (Node currentNode in currentLayer)
            {
                foreach (Node lastNode in lastLayer)
                {
                    string edgeID = GenerateEdgeID(lastNode.Info, currentNode.Info);
                    _edges.Add(edgeID, Random.Range(-1.0f, 1.0f));
                }
            }
        }
    }

    public float[] Process(float[] inputValues)
    {
        if (inputValues.Length != _layers[0].Count) throw new ArgumentException();
        float[] result = new float[_nodeCounts[_layersCount - 1]];

        for (int layerIndex = 0; layerIndex < _layersCount; layerIndex++)
        {
            List<Node> currentLayer = _layers[layerIndex];
            List<Node> lastLayer = _layers[layerIndex - 1];

            for (int curNodeIndex = 0; curNodeIndex < currentLayer.Count; curNodeIndex++)
            {
                Node currentNode = currentLayer[curNodeIndex];

                if (layerIndex == 0)
                {
                    currentNode.SetValue(inputValues[curNodeIndex]);
                }
                else
                {
                    float value = 0;
                    foreach (Node lastNode in lastLayer)
                    {
                        string edgeName = GenerateEdgeID(lastNode.Info, currentNode.Info);
                        value += _edges[edgeName] * lastNode.Value;
                    }

                    value = ActivateValue(value);
                    currentNode.SetValue(value);

                    if (layerIndex == _layersCount - 1)
                    {
                        result[curNodeIndex] = value;
                    }
                }
            }
        }
        return result;
    }

    private List<float> GetEdges()
    {
        _edgeValues = new List<float>();
        foreach (KeyValuePair<string, float> edge in _edges)
        {
            _edgeValues.Add(edge.Value);
        }

        return _edgeValues;
    }

    private void SetEdges(IReadOnlyList<float> newValues)
    {
        int index = 0;
        foreach (var key in _edges.Keys.ToList())
        {
            _edges[key] = newValues[index];
            index++;
        }
    }

    private static float ActivateValue(float value) => value < 0 ? value * 0.01f : value;

    /// <summary> Serializing node infos to string format: x:y:z, where
    /// x - layer index of past node,
    /// y - node index in past layer,
    /// z - node index in current layer </summary>
    private static string GenerateEdgeID(NodeInfo from, NodeInfo to)
    {
        string result = $"{from.LayerIndex}:{from.NodeIndex}:{to.NodeIndex}";
        return result;
    }

    /// <summary> Cross to brains in another one with new edges (parts from parameter brains)
    /// Also parse list on node counts and set all to new brain system. </summary>
    public static Brain Crossover(Brain f, Brain s)
    {
        var firstEdges = f.GetEdges();
        var secondEdges = s.GetEdges();

        int edgesCount = firstEdges.Count;
        int separator = Random.Range(0, edgesCount);

        List<float> newEdgeValues = new List<float>();
        for (int edgeIndex = 0; edgeIndex < edgesCount; edgeIndex++)
        {
            float newValue = edgeIndex > separator ? secondEdges[edgeIndex] : firstEdges[edgeIndex];
            newEdgeValues.Add(newValue);
        }

        int[] counts = f._nodeCounts;
        int inputCount = counts[0];
        int outputCount = counts[counts.Length - 1];
        List<int> hiddenCounts = new List<int>();

        int hiddenLayerCounts = counts.Length - 2;

        for (int index = 0; index < hiddenLayerCounts; index++)
        {
            hiddenCounts.Add(counts[index + 1]);
        }

        Brain crossover = new Brain(inputCount, outputCount, hiddenCounts);
        crossover.SetEdges(newEdgeValues);
        return crossover;
    }
}
