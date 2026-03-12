using System;
using System.Collections.Generic;

[Serializable]
public class NodeConfig
{
    public string id;
    public NodeType type;
    public List<NodeConfig> children = new List<NodeConfig>();
}
