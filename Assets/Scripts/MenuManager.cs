using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private Button actionButton;

    private bool isGameStarted = false;

    private void Start()
    {
        ShowMenu();
    }

    private void Update()
    {
        if (isGameStarted && Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.timeScale > 0f)
                ShowMenu();
            else
                ResumeGame();
        }
    }

    private void ShowMenu()
    {
        Time.timeScale = 0f;
        menuPanel.SetActive(true);
        // Mostramos el cursor del sistema para poder interactuar con el botón
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void OnActionButtonPressed()
    {
        isGameStarted = true;
        ResumeGame();
    }

    private void ResumeGame()
    {
        menuPanel.SetActive(false);
        Time.timeScale = 1f;
        // NO tocamos el cursor aquí: CrosshairFollow lo gestiona solo via OnEnable
    }
}