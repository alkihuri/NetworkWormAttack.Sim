using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

    public class NetworkNodeController : MonoBehaviour, IColorable
    {
        public string id;
        public NodeType type;
        public NodeColor currentColor = NodeColor.Green;
        public List<NetworkNodeController> children = new List<NetworkNodeController>();
        public NetworkNodeController parent;

        protected List<Renderer> nodeRenderers;

        protected virtual void Awake()
        {
            nodeRenderers = new List<Renderer>(GetComponentsInChildren<Renderer>());
        }

        public virtual IEnumerator ChangeColor(NodeColor color)
        {
            currentColor = color;
            Color targetColor = GetUnityColor(color);

            foreach (var nodeRenderer in nodeRenderers)
            {
                if (nodeRenderer != null && nodeRenderer.material != null)
                {
                    yield return nodeRenderer.material.DOColor(targetColor, 0.5f).WaitForCompletion();
                }
            }
        }

        public void SetColorImmediate(NodeColor color)
        {
            currentColor = color;
            foreach (var nodeRenderer in nodeRenderers)
            {
                if (nodeRenderer != null && nodeRenderer.material != null)
                {
                    nodeRenderer.material.color = GetUnityColor(color);
                }
            }
        }

        public static Color GetUnityColor(NodeColor color)
        {
            switch (color)
            {
                case NodeColor.Green: return Color.green;
                case NodeColor.Yellow: return Color.yellow;
                case NodeColor.Red: return Color.red;
                case NodeColor.Black: return Color.black;
                default: return Color.white;
            }
        }
    }


