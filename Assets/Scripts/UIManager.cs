using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Barra de vida")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TMP_Text healthText;

    [Header("Temporizador")]
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private float matchDuration = 180f;

    [Header("Panel END")]
    [SerializeField] private GameObject endPanel;
    [SerializeField] private TMP_Text endTitleText;
    [SerializeField] private TMP_Text endKillsCurrentText;  // "Esta partida: X"
    [SerializeField] private TMP_Text endKillsRecordText;   // "Récord: X"

    private const string RecordKey = "KillRecord";

    private float timeRemaining;
    private bool gameActive = false;

    private int totalEnemies  = 0;
    private int killedEnemies = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        timeRemaining = matchDuration;
    }

    private void Update()
    {
        if (!gameActive) return;

        timeRemaining -= Time.deltaTime;
        UpdateTimerText();

        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            TriggerVictory();
        }
    }

    // ── Vida ─────────────────────────────────────────────────────────────────

    public void InitHealthBar(float maxHealth)
    {
        healthSlider.minValue = 0f;
        healthSlider.maxValue = maxHealth;
        healthSlider.value    = maxHealth;
        UpdateHealthText(maxHealth, maxHealth);
        gameActive = true;
    }

    public void UpdateHealth(float current, float max)
    {
        healthSlider.value = current;
        UpdateHealthText(current, max);
    }

    // ── Enemigos ─────────────────────────────────────────────────────────────

    public void RegisterEnemy()  => totalEnemies++;
    public void RegisterEnemyKill() => killedEnemies++;

    // ── Panel END ────────────────────────────────────────────────────────────

    public void ShowEndPanel(string title)
    {
        gameActive = false;
        Time.timeScale = 0f;

        // Guardar récord si se supera
        int previousRecord = PlayerPrefs.GetInt(RecordKey, 0);
        int newRecord      = Mathf.Max(previousRecord, killedEnemies);
        PlayerPrefs.SetInt(RecordKey, newRecord);
        PlayerPrefs.Save();

        endPanel.SetActive(true);
        endTitleText.text = title;

        endKillsCurrentText.text = $"Esta partida: {killedEnemies}";
        endKillsRecordText.text  = previousRecord < killedEnemies
            ? $"¡Nuevo récord!: {newRecord}"
            : $"Récord: {previousRecord}";

        Cursor.visible   = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void TriggerVictory() => ShowEndPanel("VICTORIA");

    public void OnRestartButtonPressed()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private void UpdateHealthText(float current, float max)
    {
        if (healthText != null)
            healthText.text = $"{(int)current} / {(int)max}";
    }

    private void UpdateTimerText()
    {
        if (timerText == null) return;
        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);
        timerText.text = $"{minutes:0}:{seconds:00}";
    }
}