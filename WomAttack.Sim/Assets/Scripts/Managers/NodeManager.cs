using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class NodeManager : MonoBehaviour
{
    private List<NetworkNodeController> allNodes = new List<NetworkNodeController>();
    private List<ConnectionRenderer> allConnections = new List<ConnectionRenderer>();
    private List<List<NetworkNodeController>> layers = new List<List<NetworkNodeController>>();
    private NetworkNodeController rootNode;

    private const float LAYER_HEIGHT = 2.5f;
    private const float NODE_SPACING = 2.0f;

    void Awake()
    {
        ServiceLocator.Register(this);
    }

    public void BuildNetwork(NetworkConfig config)
    {
        rootNode = BuildNode(config.root, null, 0);
        PositionNodes();
        CreateAllConnections();
    }

    private NetworkNodeController BuildNode(NodeConfig config, NetworkNodeController parentController, int depth)
    {
        while (layers.Count <= depth)
            layers.Add(new List<NetworkNodeController>());

        GameObject prefab = LoadPrefab(config.type);
        GameObject obj;

        if (prefab != null)
            obj = Instantiate(prefab);
        else
            obj = CreateFallbackNode(config.type);

        obj.name = config.id;

        NetworkNodeController controller = AddController(obj, config.type);
        controller.id = config.id;
        controller.type = config.type;
        controller.parent = parentController;
        controller.SetLabel(config.id);

        if (parentController != null)
            parentController.children.Add(controller);

        allNodes.Add(controller);
        layers[depth].Add(controller);

        obj.transform.localScale = Vector3.zero;

        foreach (var childConfig in config.children)
        {
            BuildNode(childConfig, controller, depth + 1);
        }

        return controller;
    }

    private GameObject LoadPrefab(NodeType type)
    {
        string prefabName = type switch
        {
            NodeType.Internet => "internet",
            NodeType.Router => "router",
            NodeType.Switch => "switch",
            NodeType.PC => "pc",
            _ => null
        };

        if (prefabName == null) return null;
        return Resources.Load<GameObject>(prefabName);
    }

    private GameObject CreateFallbackNode(NodeType type)
    {
        GameObject obj;

        switch (type)
        {
            case NodeType.Internet:
                obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                obj.transform.localScale = Vector3.one * 1.2f;
                break;
            case NodeType.Router:
                obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                obj.transform.localScale = Vector3.one * 0.8f;
                break;
            case NodeType.Switch:
                obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                obj.transform.localScale = Vector3.one * 0.6f;
                break;
            case NodeType.PC:
                obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                obj.transform.localScale = Vector3.one * 0.4f;
                break;
            default:
                obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                break;
        }

        var renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            var shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null)
                shader = Shader.Find("Standard");
            renderer.material = new Material(shader);
        }

        return obj;
    }

    private NetworkNodeController AddController(GameObject obj, NodeType type)
    {
        switch (type)
        {
            case NodeType.Internet:
                return obj.AddComponent<InternetNodeController>();
            case NodeType.Router:
                return obj.AddComponent<RouterController>();
            case NodeType.Switch:
                return obj.AddComponent<SwitchController>();
            case NodeType.PC:
                return obj.AddComponent<PCController>();
            default:
                return obj.AddComponent<NetworkNodeController>();
        }
    }

    private void PositionNodes()
    {
        for (int layerIndex = 0; layerIndex < layers.Count; layerIndex++)
        {
            var layer = layers[layerIndex];
            float y = -layerIndex * LAYER_HEIGHT;
            float totalWidth = (layer.Count - 1) * NODE_SPACING;
            float startX = -totalWidth / 2f;

            for (int i = 0; i < layer.Count; i++)
            {
                float x = startX + i * NODE_SPACING;
                layer[i].transform.position = new Vector3(x, y, 0);
            }
        }
    }

    private void CreateAllConnections()
    {
        foreach (var node in allNodes)
        {
            if (node.parent != null)
            {
                CreateConnection(node.parent, node);
            }
        }
    }

    private void CreateConnection(NetworkNodeController parentNode, NetworkNodeController childNode)
    {
        var connectionObj = new GameObject($"Connection_{parentNode.id}_{childNode.id}");
        var connection = connectionObj.AddComponent<ConnectionRenderer>();
        connection.Initialize(parentNode.transform, childNode.transform);
        allConnections.Add(connection);
    }

    public IEnumerator AnimateTreeAppearance()
    {
        for (int layerIndex = 0; layerIndex < layers.Count; layerIndex++)
        {
            var layer = layers[layerIndex];
            Tween lastTween = null;

            foreach (var node in layer)
            {
                Vector3 targetScale = GetTargetScale(node.type);
                lastTween = node.transform.DOScale(targetScale, 0.5f).SetEase(Ease.OutBack);
            }

            if (lastTween != null)
                yield return lastTween.WaitForCompletion();
        }
    }

    private Vector3 GetTargetScale(NodeType type)
    {
        switch (type)
        {
            case NodeType.Internet: return Vector3.one * 1.2f;
            case NodeType.Router: return Vector3.one * 0.8f;
            case NodeType.Switch: return Vector3.one * 0.6f;
            case NodeType.PC: return Vector3.one * 0.4f;
            default: return Vector3.one;
        }
    }

    public void RandomizePCColors()
    {
        var pcs = GetAllPCs();
        if (pcs.Count == 0) return;

        foreach (var pc in pcs)
        {
            NodeColor color = Random.value > 0.5f ? NodeColor.Green : NodeColor.Yellow;
            pc.SetColorImmediate(color);
        }

        int infectedIndex = Random.Range(0, pcs.Count);
        pcs[infectedIndex].SetColorImmediate(NodeColor.Red);
    }

    public List<NetworkNodeController> GetAllPCs()
    {
        return allNodes.Where(n => n.type == NodeType.PC).ToList();
    }

    public List<NetworkNodeController> GetAllNodes()
    {
        return allNodes;
    }

    public List<ConnectionRenderer> GetAllConnections()
    {
        return allConnections;
    }

    public List<List<NetworkNodeController>> GetLayers()
    {
        return layers;
    }

    public NetworkNodeController GetRootNode()
    {
        return rootNode;
    }

    public NetworkNodeController FindInfectedPC()
    {
        return allNodes.FirstOrDefault(n => n.type == NodeType.PC && n.currentColor == NodeColor.Red);
    }

    public List<NetworkNodeController> FindAllInfectedPCs()
    {
        return allNodes.Where(n => n.type == NodeType.PC && n.currentColor == NodeColor.Red).ToList();
    }

    public bool AreAllPCsBlack()
    {
        return GetAllPCs().All(pc => pc.currentColor == NodeColor.Black);
    }
}
