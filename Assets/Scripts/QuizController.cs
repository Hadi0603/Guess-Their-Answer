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
    [Header("Toaster Settings")]
    [SerializeField] private GameObject toasterPrefab;
    [SerializeField] private Transform toasterParent;
    [SerializeField] private float toasterDuration = 3f;
    [SerializeField] private float bounceScale = 1.2f;
    [SerializeField] private float bounceSpeed = 0.1f;
    private GameObject currentToaster;
    private string resposnseStr = "";
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
                    //responseText.text = "Already answered!";
                    resposnseStr="Already answered!";
                    ShowToaster(resposnseStr);
                }
                else
                {
                    Debug.Log("Match found: " + answerData[i]);
                    responseText.enabled = true;
                    //responseText.text = answerData[i] + "  " + scoreData[i];
                    resposnseStr = answerData[i] + "  " + scoreData[i];
                    ShowToaster(resposnseStr);
                    Score += scoreData[i];
                    AudienceManager.Instance.MoveAudienceToPlayer(scoreData[i]);
                    UpdateScore();
                    answered[i] = true;
                    CheckGameWon();

                }

                matched = true;
                break;
            }
        }

        if (!matched)
        {
            Debug.Log("Match not found.");
            responseText.enabled = true;
            //responseText.text = "Wrong Answer";
            resposnseStr="Wrong Answer";
            ShowToaster(resposnseStr);
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
    private void ShowToaster(string message)
    {
        if (currentToaster != null)
        {
            Destroy(currentToaster);
        }

        if (toasterPrefab != null && toasterParent != null)
        {
            GameObject toasterInstance = Instantiate(
                toasterPrefab,
                toasterPrefab.transform.position,
                toasterPrefab.transform.rotation,
                toasterParent
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
