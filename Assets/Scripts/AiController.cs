using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AiController : MonoBehaviour
{
    [SerializeField] private string[] aiAnswers;
    [SerializeField] private float answerInterval = 5f;
    [SerializeField] private Text aiScoreText;
    [SerializeField] private Text aiResponseText;
    
    [HideInInspector]
    public int aiScore;

    void Start()
    {
        aiScore = 0;
        aiResponseText.enabled = false;
        StartCoroutine(AiAnsweringRoutine());
        UpdateAiScoreUI();
    }

    IEnumerator AiAnsweringRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(answerInterval);
            AttemptRandomAnswer();
        }
    }

    void AttemptRandomAnswer()
    {
        if (aiAnswers.Length == 0 || QuizController.answered == null) return;

        int randomIndex = Random.Range(0, aiAnswers.Length);
        string selectedAnswer = aiAnswers[randomIndex];

        for (int i = 0; i < QuizController.answered.Length; i++)
        {
            if (!QuizController.answered[i] && QuizController.answers[i] == selectedAnswer)
            {
                QuizController.answered[i] = true;
                aiScore += QuizController.scores[i];
                UpdateAiScoreUI();
                aiResponseText.enabled = true;
                aiResponseText.text = selectedAnswer + " " + QuizController.scores[i];
                Debug.Log("AI answered correctly: " + selectedAnswer);
                FindObjectOfType<QuizController>().SendMessage("CheckGameWon");
                return;
            }
        }
        aiResponseText.enabled = true;
        aiResponseText.text = "Wrong Answer";
        Debug.Log("AI tried an already answered or invalid answer: " + selectedAnswer);
    }

    void UpdateAiScoreUI()
    {
        if (aiScoreText != null)
        {
            aiScoreText.text = "AI Score: " + aiScore;
        }
    }
}