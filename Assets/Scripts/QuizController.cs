using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class QuizController : MonoBehaviour
{
    [SerializeField] InputField inputField;
    [SerializeField] Text responseText;
    [SerializeField] private Text scoreText;
    [SerializeField] string[] answers;
    [SerializeField] private int[] scores;
    [SerializeField] UIManager uiManager;
    private int Score;

    private TouchScreenKeyboard keyboard;

    void Start()
    {
        OpenKeyboard();
        Score = 0;
        responseText.enabled = false;
        UpdateScore();
    }

    public void OpenKeyboard()
    {
        inputField.ActivateInputField();
        keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
    }

    public void CloseKeyboard()
    {
        inputField.DeactivateInputField();
        if (keyboard != null)
        {
            keyboard.active = false;
            keyboard = null;
        }
    }

    void Update()
    {
        if (keyboard != null)
        {
            if (keyboard.status == TouchScreenKeyboard.Status.Done ||
                keyboard.status == TouchScreenKeyboard.Status.Canceled ||
                keyboard.status == TouchScreenKeyboard.Status.LostFocus)
            {
                Debug.Log("Keyboard closed. Input: " + keyboard.text);
                CloseKeyboard();
            }
        }
    }

    public void EnterBtn()
    {
        FormatInputText();
        string input = inputField.text;
        for (int i = 0; i < answers.Length; i++)
        {
            if (!string.IsNullOrEmpty(answers[i]) && input.Contains(answers[i]))
            {
                Debug.Log("Match found: " + answers[i]);
                responseText.enabled = true;
                responseText.text = answers[i] + "  " + scores[i];
                Score += scores[i];
                UpdateScore();
                if (Score == 100)
                {
                    uiManager.TriggerGameWon();
                }
                input = input.Replace(answers[i], "");
                answers[i] = null;
                break;
            }
            else
            {
                Debug.Log("Match not found.");
                responseText.enabled = true;
                responseText.text = "Wrong Answer";
            }
        }

        inputField.text = "";
    }
    void FormatInputText()
    {
        string input = inputField.text.Trim();

        if (!string.IsNullOrEmpty(input))
        {
            string formatted = char.ToUpper(input[0]) + input.Substring(1).ToLower();
            inputField.text = formatted;
            Debug.Log("Formatted Input: " + formatted);
        }
    }

    void UpdateScore()
    {
        scoreText.text = "Score: " + Score.ToString();
    }
}
