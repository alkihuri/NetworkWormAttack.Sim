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

        SimLog.Write("Initializing network topology...");
        NetworkConfig config = NetworkConfigBuilder.BuildDefault();
        nodeManager.BuildNetwork(config);

        yield return StartCoroutine(nodeManager.AnimateTreeAppearance());

        nodeManager.RandomizePCColors();
        SimLog.Write("Network ready. Awaiting operator command.");

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

        SimLog.Write("--- PHASE: SCAN ---");
        SimLog.Write("Scanning network for infected nodes...");

        var scanManager = ServiceLocator.Get<ScanManager>();
        yield return StartCoroutine(scanManager.ExecuteScan());

        SimLog.Write("Scan complete. Infected node located.");
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

        SimLog.Write("--- PHASE: EXPLOIT ---");
        SimLog.Write("Exploiting vulnerability on infected node...");

        var exploitManager = ServiceLocator.Get<ExploitManager>();
        yield return StartCoroutine(exploitManager.ExecuteExploit());

        SimLog.Write("Exploit complete. Propagation ready.");
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

        SimLog.Write("--- PHASE: PROPAGATE ---");
        SimLog.Write("Worm propagating through network...");

        var propagationManager = ServiceLocator.Get<PropagationManager>();
        yield return StartCoroutine(propagationManager.ExecutePropagate());

        var nodeManager = ServiceLocator.Get<NodeManager>();
        if (nodeManager.AreAllPCsBlack())
        {
            SimLog.Write("All nodes compromised. Network fully infected.");
            SimLog.Write("=== SIMULATION COMPLETE ===");
            simulationComplete = true;
            uiManager.SetButtonText("SIMULATION COMPLETE");
            uiManager.SetButtonInteractable(false);
        }
        else
        {
            SimLog.Write("Propagation wave done. Restarting scan cycle.");
            currentPhase = SimulationPhase.Scan;
            uiManager.SetButtonText("SCAN");
            uiManager.SetButtonInteractable(true);
        }

        isProcessing = false;
    }
}
