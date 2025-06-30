using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup levelWonUI;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private CanvasGroup pauseUI;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private CanvasGroup levelLostUI;
    [SerializeField] private GameObject lostPanel;
    [SerializeField] private GameObject pauseBtn;
    [SerializeField] private Text timerText;
    [SerializeField] private GameObject keyBoard;
    [SerializeField] QuizController quizController;
    [SerializeField] private AiController aiController;
    [FormerlySerializedAs("aiController")] [SerializeField] GameObject AiController;
    [SerializeField] private float gameTime = 60f;

    private bool isBlinking = false;
    private bool isPaused = false;

    private void Awake()
    {
        levelWonUI.alpha = 0f;
        winPanel.transform.localPosition = new Vector2(0, +Screen.height);
        levelLostUI.alpha = 0f;
        lostPanel.transform.localPosition = new Vector2(0, +Screen.height);
        pauseUI.alpha = 0f;
        pausePanel.transform.localPosition = new Vector2(0, +Screen.height);
        StartCoroutine(TimerCountdown());
    }

    private IEnumerator TimerCountdown()
    {
        while (gameTime > 0)
        {
            if (!isPaused)
            {
                gameTime -= Time.deltaTime;
                UpdateTimerDisplay();

                if (gameTime <= 15f && !isBlinking)
                {
                    StartCoroutine(BlinkTimer());
                }
            }
            yield return null;
        }

        TriggerGameOver();
    }

    private void UpdateTimerDisplay()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(gameTime / 60);
            int seconds = Mathf.FloorToInt(gameTime % 60);
            timerText.text = $"{minutes:00}:{seconds:00}";
        }
    }

    private IEnumerator BlinkTimer()
    {
        isBlinking = true;
        Color originalColor = timerText.color;
    
        while (gameTime <= 15f && gameTime > 0)
        {
            timerText.color = (timerText.color == Color.red) ? Color.white : Color.red;
            yield return new WaitForSeconds(0.5f);
        }

        timerText.color = originalColor;
        isBlinking = false;
    }

    public void OpenPauseMenu()
    {
        pauseUI.gameObject.SetActive(true);
        pauseUI.LeanAlpha(1, 0.5f);
        pauseBtn.SetActive(false);
        pausePanel.LeanMoveLocalY(0, 0.5f).setEaseOutExpo().delay = 0.1f;
        isPaused = true;
        AiController.SetActive(false);
    }

    public void ClosePauseMenu()
    {
        pauseUI.LeanAlpha(0, 0.5f);
        pausePanel.LeanMoveLocalY(+Screen.height, 0.5f).setEaseInExpo();
        pauseBtn.SetActive(true);
        isPaused = false;
        AiController.SetActive(true);
        Invoke(nameof(DisablePauseUI), 0.5f);
    }

    private void DisablePauseUI()
    {
        pauseUI.gameObject.SetActive(false);
    }

    public void TriggerGameOver()
    {
        int playerScore = quizController.Score;
        int aiScore = aiController.aiScore;
        Animator playerAnimator = GameObject.Find("Player").GetComponent<Animator>();
        Animator aiAnimator = GameObject.Find("AI").GetComponent<Animator>();

        if (playerScore > aiScore)
        {
            playerAnimator.SetTrigger("Win");
            aiAnimator.SetTrigger("Lose");
            StartCoroutine(WinDelay());
        }
        else
        {
            playerAnimator.SetTrigger("Lose");
            aiAnimator.SetTrigger("Win");
            StartCoroutine(LostDelay());
        }

        StopCoroutine(TimerCountdown());
        Destroy(timerText);
    }


    IEnumerator WinDelay()
    {
        pauseBtn.SetActive(false);
        StopCoroutine(TimerCountdown());
        Destroy(timerText);
        AiController.SetActive(false);
        keyBoard.LeanMoveLocalY(-500, 0.5f).setEaseInExpo().delay = 0.1f;
        yield return new WaitForSeconds(2.5f);
        levelWonUI.gameObject.SetActive(true);
        levelWonUI.LeanAlpha(1, 0.5f);
        winPanel.LeanMoveLocalY(0, 0.5f).setEaseOutExpo().delay = 0.1f;
        if (GameController.levelToLoad < 5)
        {
            PlayerPrefs.SetInt("levelToLoad", ++GameController.levelToLoad);
        }
        PlayerPrefs.Save();
    }

    IEnumerator LostDelay()
    {
        pauseBtn.SetActive(false);
        timerText.enabled = false;
        AiController.SetActive(false);
        keyBoard.LeanMoveLocalY(-500, 0.5f).setEaseInExpo().delay = 0.1f;
        yield return new WaitForSeconds(2.5f);
        levelLostUI.gameObject.SetActive(true);
        levelLostUI.LeanAlpha(1, 0.5f);
        lostPanel.LeanMoveLocalY(0, 0.5f).setEaseOutExpo().delay = 0.1f;
    }

}
