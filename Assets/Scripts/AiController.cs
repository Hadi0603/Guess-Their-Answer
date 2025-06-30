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
    [Header("Toaster Settings")]
    [SerializeField] private GameObject aiToasterPrefab;
    [SerializeField] private Transform aiToasterParent;
    [SerializeField] private float toasterDuration = 3f;
    [SerializeField] private float bounceScale = 1.2f;
    [SerializeField] private float bounceSpeed = 0.1f;
    private GameObject currentToaster;
    private string aiResposnseStr = "";
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
                AudienceManager.Instance.MoveAudienceToAI(QuizController.scores[i]);
                UpdateAiScoreUI();
                aiResponseText.enabled = true;
                aiResposnseStr = selectedAnswer + " " + QuizController.scores[i];
                ShowToaster(aiResposnseStr);
                Debug.Log("AI answered correctly: " + selectedAnswer);
                FindObjectOfType<QuizController>().SendMessage("CheckGameWon");
                return;
            }
        }
        aiResponseText.enabled = true;
        aiResposnseStr="Wrong Answer";
        ShowToaster(aiResposnseStr);
        Debug.Log("AI tried an already answered or invalid answer: " + selectedAnswer);
    }

    void UpdateAiScoreUI()
    {
        if (aiScoreText != null)
        {
            aiScoreText.text = "AI Score: " + aiScore;
        }
    }
    private void ShowToaster(string message)
    {
        if (currentToaster != null)
        {
            Destroy(currentToaster);
        }

        if (aiToasterPrefab != null && aiToasterParent != null)
        {
            GameObject toasterInstance = Instantiate(
                aiToasterPrefab,
                aiToasterPrefab.transform.position,
                aiToasterPrefab.transform.rotation,
                aiToasterParent
            );

            Text toasterText = toasterInstance.GetComponentInChildren<Text>();
            if (toasterText != null)
            {
                toasterText.text = message;
                StartCoroutine(AnimateToaster(toasterText.transform, toasterInstance));
            }

            currentToaster = toasterInstance;
            Destroy(toasterInstance, toasterDuration + (bounceSpeed * 4));
        }
        else
        {
            Debug.LogWarning("Toaster Prefab or Parent not assigned in the Inspector.");
        }
    }


    private IEnumerator AnimateToaster(Transform toasterTransform, GameObject toaster)
    {
        // Bounce In
        Vector3 originalScale = toasterTransform.localScale;
        float timer = 0f;
        while (timer < bounceSpeed * 2)
        {
            timer += Time.deltaTime;
            float scaleFactor = Mathf.Lerp(0f, bounceScale, timer / bounceSpeed);
            toasterTransform.localScale = originalScale * scaleFactor;
            yield return null;
        }
        toasterTransform.localScale = originalScale * bounceScale;

        timer = 0f;
        while (timer < bounceSpeed * 2)
        {
            timer += Time.deltaTime;
            float scaleFactor = Mathf.Lerp(bounceScale, 1f, timer / bounceSpeed);
            toasterTransform.localScale = originalScale * scaleFactor;
            yield return null;
        }
        toasterTransform.localScale = originalScale;

        // Wait for the duration
        yield return new WaitForSeconds(toasterDuration);

        // Bounce Out
        timer = 0f;
        while (timer < bounceSpeed * 2)
        {
            timer += Time.deltaTime;
            float scaleFactor = Mathf.Lerp(1f, bounceScale, timer / bounceSpeed);
            toasterTransform.localScale = originalScale * scaleFactor;
            yield return null;
        }
        toasterTransform.localScale = originalScale * bounceScale;

        timer = 0f;
        while (timer < bounceSpeed * 2)
        {
            timer += Time.deltaTime;
            float scaleFactor = Mathf.Lerp(bounceScale, 0f, timer / bounceSpeed);
            toasterTransform.localScale = originalScale * scaleFactor;
            yield return null;
        }
        toasterTransform.localScale = Vector3.zero; // Fully scale down before destroy
        if (toaster != null)
        {
            Destroy(toaster);
            currentToaster = null;
        }
    }
}