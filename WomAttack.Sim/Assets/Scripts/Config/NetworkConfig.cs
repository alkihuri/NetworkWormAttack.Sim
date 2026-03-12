using UnityEngine;

[CreateAssetMenu(fileName = "NetworkConfig", menuName = "Network/NetworkConfig")]
public class NetworkConfig : ScriptableObject
{
    [SerializeReference]
    public NodeConfig root;
}
