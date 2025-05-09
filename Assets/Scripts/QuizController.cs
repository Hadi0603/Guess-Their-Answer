using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

public class QuizController : MonoBehaviour
{
    [Header("Serialized References")]
    [SerializeField] private Text inputText;
    [SerializeField] private Text responseText;
    [SerializeField] private Text scoreText;
    [FormerlySerializedAs("answers")] [SerializeField] private string[] answerData;
    [FormerlySerializedAs("scores")] [SerializeField] private int[] scoreData;
    [SerializeField] private UIManager uiManager;

    [Header("Public Static References")]
    public static string[] answers;
    public static int[] scores;
    public static bool[] answered;
    
    private string currentInput = "";
    [HideInInspector]
    public int Score = 0;

    void Start()
    {
        answers = answerData;
        scores = scoreData;
        answered = new bool[answers.Length];
        
        responseText.enabled = false;
        UpdateScore();
        inputText.text = "";

        answered = new bool[answerData.Length];
    }

    public void UpdateInputText(string newText)
    {
        inputText.text += newText;
    }

    public void EnterBtn()
    {
        FormatInputText();
        string input = inputText.text;

        bool matched = false;

        for (int i = 0; i < answerData.Length; i++)
        {
            if (!string.IsNullOrEmpty(answerData[i]) && input.Contains(answerData[i]))
            {
                if (answered[i])
                {
                    Debug.Log("Already answered: " + answerData[i]);
                    responseText.enabled = true;
                    responseText.text = "Already answered!";
                }
                else
                {
                    Debug.Log("Match found: " + answerData[i]);
                    responseText.enabled = true;
                    responseText.text = answerData[i] + "  " + scoreData[i];
                    Score += scoreData[i];
                    UpdateScore();
                    answered[i] = true;
                    CheckGameWon();
                    /*if (Score == 100)
                    {
                        uiManager.TriggerGameWon();
                    }*/

                }

                matched = true;
                break;
            }
        }

        if (!matched)
        {
            Debug.Log("Match not found.");
            responseText.enabled = true;
            responseText.text = "Wrong Answer";
        }

        currentInput = "";
        inputText.text = "";
    }

    void FormatInputText()
    {
        string input = inputText.text.Trim();

        if (!string.IsNullOrEmpty(input))
        {
            string formatted = char.ToUpper(input[0]) + input.Substring(1).ToLower();
            inputText.text = formatted;
            Debug.Log("Formatted Input: " + formatted);
        }
    }

    void UpdateScore()
    {
        scoreText.text = "Score: " + Score.ToString();
    }
    public void AddLetter(string letter)
    {
        currentInput += letter;
        inputText.text = currentInput;
    }

    public void DeleteLastLetter()
    {
        if (!string.IsNullOrEmpty(currentInput))
        {
            currentInput = currentInput.Substring(0, currentInput.Length - 1);
            inputText.text = currentInput;
        }
    }
    void CheckGameWon()
    {
        foreach (bool isAnswered in answered)
        {
            if (!isAnswered)
                return;
        }

        uiManager.TriggerGameOver();
    }
}
