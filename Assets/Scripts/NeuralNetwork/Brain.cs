using System.Collections.Generic;
using UnityEngine;

/// <summary> Representation of neural network system. </summary>
public class Brain
{
    private Dictionary<string, float> _edges;

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

    private IEnumerable<float> Process(IEnumerable<float> inputValues)
    {
        List<float> result = new List<float>();

        return result;
    }

    /// <summary> Serializing node infos to string format: x:y:z, where
    /// x - layer index of past node,
    /// y - node index in past layer,
    /// z - node index in current layer </summary>
    private static string GenerateEdgeID(NodeInfo from, NodeInfo to)
    {
        string result = $"{from.LayerIndex}:{from.NodeIndex}:{to.NodeIndex}";
        return result;
    }
}
