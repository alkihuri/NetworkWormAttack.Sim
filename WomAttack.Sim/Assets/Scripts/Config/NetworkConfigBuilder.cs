using UnityEngine;

public static class NetworkConfigBuilder
{
    public static NetworkConfig BuildDefault()
    {
        var config = ScriptableObject.CreateInstance<NetworkConfig>();

        // Layer 0: Internet
        var internet = new NodeConfig { id = "Internet", type = NodeType.Internet };

        // Layer 1: Router 1
        var router1 = new NodeConfig { id = "Router1", type = NodeType.Router };
        internet.children.Add(router1);

        // Layer 2: Router 2, Switch 1, Switch 2
        var router2 = new NodeConfig { id = "Router2", type = NodeType.Router };
        var switch1 = new NodeConfig { id = "Switch1", type = NodeType.Switch };
        var switch2 = new NodeConfig { id = "Switch2", type = NodeType.Switch };
        router1.children.Add(router2);
        router1.children.Add(switch1);
        router1.children.Add(switch2);

        // Layer 3: Switch 1 -> 5 PCs
        for (int i = 1; i <= 5; i++)
            switch1.children.Add(new NodeConfig { id = $"PC{i:D2}", type = NodeType.PC });

        // Layer 3: Switch 2 -> 4 PCs
        for (int i = 6; i <= 9; i++)
            switch2.children.Add(new NodeConfig { id = $"PC{i:D2}", type = NodeType.PC });

        // Layer 3: Router 2 -> Switch 3
        var switch3 = new NodeConfig { id = "Switch3", type = NodeType.Switch };
        router2.children.Add(switch3);

        // Layer 4: Switch 3 -> 8 PCs
        for (int i = 10; i <= 17; i++)
            switch3.children.Add(new NodeConfig { id = $"PC{i:D2}", type = NodeType.PC });

        config.root = internet;
        return config;
    }
}
