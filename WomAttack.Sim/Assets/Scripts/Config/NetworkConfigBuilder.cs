using UnityEngine;

public static class NetworkConfigBuilder
{
    static NodeConfig Node(string id, NodeType type) =>
        new NodeConfig { id = id, type = type };

    static NodeConfig PC(int index) =>
        Node($"PC{index:D2}", NodeType.PC);

    public static NetworkConfig BuildDefault()
    {
        var internet = Node("Internet", NodeType.Internet);

        var router1 = Node("Router 1", NodeType.Router);
        internet.children.Add(router1);

        var router2 = Node("Router 2", NodeType.Router);
        var switch1 = Node("Switch 1", NodeType.Switch);
        var switch2 = Node("Switch 2", NodeType.Switch);
        router1.children.Add(router2);
        router1.children.Add(switch1);
        router1.children.Add(switch2);

        for (int i = 1; i <= 5; i++)  switch1.children.Add(PC(i));
        for (int i = 6; i <= 9; i++)  switch2.children.Add(PC(i));

        var switch3 = Node("Switch 3", NodeType.Switch);
        router2.children.Add(switch3);

        for (int i = 10; i <= 17; i++) switch3.children.Add(PC(i));

        var config = ScriptableObject.CreateInstance<NetworkConfig>();
        config.root = internet;
        return config;
    }
}
