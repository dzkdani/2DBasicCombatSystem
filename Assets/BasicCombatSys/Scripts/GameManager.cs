using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public enum GameState
{
    Playing,
    Ended
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game Settings")]
    public Button restartButton; 
    public Button quittButton;
    public GameObject resultPanel; 
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI hitText;
    public TextMeshProUGUI parryText;
    public int hitsTaken = 0;
    public int parriesSuccess = 0;
    public int maxHits = 5;
    public int requiredParries = 3;

    public GameState currentState = GameState.Playing;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        GameSetup();
        UISetup();
    }

    void GameSetup()
    {
        hitsTaken = 0;
        parriesSuccess = 0;
    }

    void UISetup()
    {
        restartButton.onClick.RemoveAllListeners();
        restartButton.onClick.AddListener(RestartGame);
        quittButton.onClick.RemoveAllListeners();
        quittButton.onClick.AddListener(QuitGame);
        resultPanel.SetActive(false);
    }

    public void RegisterHit()
    {
        if (currentState != GameState.Playing) return;

        hitsTaken++;
        parriesSuccess = 0; // Reset parries on hit
        if (hitsTaken >= maxHits)
            EndGame(false);
        
        hitText.text = "Hits Taken : " + hitsTaken;
        parryText.text = "Parried : " + parriesSuccess;
    }

    public void RegisterParrySuccess()
    {
        if (currentState != GameState.Playing) return;

        parriesSuccess++;
        if (parriesSuccess >= requiredParries)
            EndGame(true);

        hitText.text = "Hits Taken : " + hitsTaken;
        parryText.text = "Parried : " + parriesSuccess;
    }

    void EndGame(bool win)
    {
        currentState = GameState.Ended;
        resultPanel.SetActive(true);

        if (resultText != null)
            resultText.text = win ? "Victory!" : "Game Over!";

        Time.timeScale = 0f; 
    }

    private void RestartGame()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void QuitGame()
    {
        Time.timeScale = 1f; 
        Application.Quit();
    }
}
