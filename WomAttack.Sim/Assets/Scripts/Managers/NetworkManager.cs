using System.Collections;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    private SimulationPhase currentPhase = SimulationPhase.Scan;
    private bool isProcessing;
    private bool simulationComplete;

    void Awake()
    {
        ServiceLocator.Register(this);
    }

    IEnumerator Start()
    {
        var nodeManager = ServiceLocator.Get<NodeManager>();

        NetworkConfig config = NetworkConfigBuilder.BuildDefault();
        nodeManager.BuildNetwork(config);

        yield return StartCoroutine(nodeManager.AnimateTreeAppearance());

        nodeManager.RandomizePCColors();

        var uiManager = ServiceLocator.Get<UIManager>();
        uiManager.SetButtonText("SCAN");
        uiManager.SetButtonInteractable(true);
    }

    public void OnButtonClicked()
    {
        if (isProcessing || simulationComplete) return;

        switch (currentPhase)
        {
            case SimulationPhase.Scan:
                StartCoroutine(ExecuteScan());
                break;
            case SimulationPhase.Exploit:
                StartCoroutine(ExecuteExploit());
                break;
            case SimulationPhase.Propagate:
                StartCoroutine(ExecutePropagate());
                break;
        }
    }

    private IEnumerator ExecuteScan()
    {
        isProcessing = true;
        var uiManager = ServiceLocator.Get<UIManager>();
        uiManager.SetButtonInteractable(false);

        var scanManager = ServiceLocator.Get<ScanManager>();
        yield return StartCoroutine(scanManager.ExecuteScan());

        currentPhase = SimulationPhase.Exploit;
        uiManager.SetButtonText("EXPLOIT");
        uiManager.SetButtonInteractable(true);
        isProcessing = false;
    }

    private IEnumerator ExecuteExploit()
    {
        isProcessing = true;
        var uiManager = ServiceLocator.Get<UIManager>();
        uiManager.SetButtonInteractable(false);

        var exploitManager = ServiceLocator.Get<ExploitManager>();
        yield return StartCoroutine(exploitManager.ExecuteExploit());

        currentPhase = SimulationPhase.Propagate;
        uiManager.SetButtonText("PROPAGATE AND REPLICATE");
        uiManager.SetButtonInteractable(true);
        isProcessing = false;
    }

    private IEnumerator ExecutePropagate()
    {
        isProcessing = true;
        var uiManager = ServiceLocator.Get<UIManager>();
        uiManager.SetButtonInteractable(false);

        var propagationManager = ServiceLocator.Get<PropagationManager>();
        yield return StartCoroutine(propagationManager.ExecutePropagate());

        var nodeManager = ServiceLocator.Get<NodeManager>();
        if (nodeManager.AreAllPCsBlack())
        {
            simulationComplete = true;
            uiManager.SetButtonText("SIMULATION COMPLETE");
            uiManager.SetButtonInteractable(false);
        }
        else
        {
            currentPhase = SimulationPhase.Scan;
            uiManager.SetButtonText("SCAN");
            uiManager.SetButtonInteractable(true);
        }

        isProcessing = false;
    }
}
