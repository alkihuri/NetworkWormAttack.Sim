using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PropagationManager : MonoBehaviour
{
    void Awake()
    {
        ServiceLocator.Register(this);
    }

    public IEnumerator ExecutePropagate()
    {
        var nodeManager = ServiceLocator.Get<NodeManager>();
        var allPCs = nodeManager.GetAllPCs();

        // Collect color transitions before applying
        var transitions = new List<(NetworkNodeController node, NodeColor newColor)>();

        foreach (var pc in allPCs)
        {
            switch (pc.currentColor)
            {
                case NodeColor.Green:
                    transitions.Add((pc, NodeColor.Yellow));
                    break;
                case NodeColor.Yellow:
                    transitions.Add((pc, NodeColor.Red));
                    break;
                case NodeColor.Red:
                    transitions.Add((pc, NodeColor.Black));
                    break;
                case NodeColor.Black:
                    // Already Black, no change
                    break;
            }
        }

        // Apply transitions sequentially with animation
        foreach (var (node, newColor) in transitions)
        {
            yield return StartCoroutine(node.ChangeColor(newColor));
        }

        // After propagation, check parent nodes for blackout
        yield return StartCoroutine(CheckAllParentNodes(nodeManager));
    }

    private IEnumerator CheckAllParentNodes(NodeManager nodeManager)
    {
        var allNodes = nodeManager.GetAllNodes();

        // Check from bottom up - find nodes whose children are all Black
        for (int i = allNodes.Count - 1; i >= 0; i--)
        {
            var node = allNodes[i];
            if (node.type == NodeType.PC) continue;
            if (node.currentColor == NodeColor.Black) continue;
            if (node.children.Count == 0) continue;

            bool allChildrenBlack = node.children.All(child => child.currentColor == NodeColor.Black);

            if (allChildrenBlack)
            {
                yield return StartCoroutine(node.ChangeColor(NodeColor.Black));
            }
        }
    }
}
