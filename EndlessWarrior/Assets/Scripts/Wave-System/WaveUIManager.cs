using UnityEngine;
using TMPro;

public class WaveUIManager : MonoBehaviour
{
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private TMP_Text waveCounter;

    private void OnEnable()
    {
        if (waveManager != null)
            waveManager.OnWaveChanged += UpdateWaveUI;
        else
            Debug.LogError("WaveManager reference is missing in WaveUIManager!");
    }

    private void OnDisable()
    {
        if (waveManager != null)
            waveManager.OnWaveChanged -= UpdateWaveUI;
    }

    private void Start()
    {
        if (waveManager != null)
            UpdateWaveUI(waveManager.Wave);
        else
            Debug.LogError("WaveManager reference is missing!");
    }

    private void UpdateWaveUI(int currentWave)
    {
        waveCounter.text = $"Runda: {currentWave}";
    }

    public void UpdateTimerText(float timeLeft)
    {
        waveCounter.text = $"Colectează banii: {timeLeft:0.0}s";
    }

    public void ResetWaveText(int currentWave)
    {
        waveCounter.text = $"Runda: {currentWave}";
    }
}
