using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudienceManager : MonoBehaviour
{
    [Header("Audience Parent")]
    [SerializeField] private Transform audienceParent;

    [Header("Dugout Parents")]
    [SerializeField] private Transform playerDugoutParent;
    [SerializeField] private Transform aiDugoutParent;

    [SerializeField] private float moveSpeed = 3f;

    private List<GameObject> audienceList = new List<GameObject>();
    private List<Transform> playerDugoutPositions = new List<Transform>();
    private List<Transform> aiDugoutPositions = new List<Transform>();

    private int currentAudienceIndex = 0;
    private int currentPlayerIndex = 0;
    private int currentAiIndex = 0;

    public static AudienceManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        // Get first 50 audience members
        int audienceCount = Mathf.Min(50, audienceParent.childCount);
        for (int i = 0; i < audienceCount; i++)
        {
            audienceList.Add(audienceParent.GetChild(i).gameObject);
        }

        // Get dugout positions
        for (int i = 0; i < playerDugoutParent.childCount; i++)
            playerDugoutPositions.Add(playerDugoutParent.GetChild(i));

        for (int i = 0; i < aiDugoutParent.childCount; i++)
            aiDugoutPositions.Add(aiDugoutParent.GetChild(i));
    }

    public void MoveAudienceToPlayer(int score)
    {
        int count = Mathf.Min(score / 2, audienceList.Count - currentAudienceIndex, playerDugoutPositions.Count - currentPlayerIndex);
        StartCoroutine(MoveAudience(count, playerDugoutPositions, isPlayer: true));
    }

    public void MoveAudienceToAI(int score)
    {
        int count = Mathf.Min(score / 2, audienceList.Count - currentAudienceIndex, aiDugoutPositions.Count - currentAiIndex);
        StartCoroutine(MoveAudience(count, aiDugoutPositions, isPlayer: false));
    }

    private IEnumerator MoveAudience(int count, List<Transform> dugoutTargets, bool isPlayer)
    {
        for (int i = 0; i < count; i++)
        {
            if (currentAudienceIndex >= audienceList.Count)
                yield break;

            GameObject audienceMember = audienceList[currentAudienceIndex++];
            Animator animator = audienceMember.GetComponent<Animator>();

            if (animator != null)
                animator.Play("Running");

            Transform targetPosition = isPlayer ? playerDugoutPositions[currentPlayerIndex++] : aiDugoutPositions[currentAiIndex++];
            StartCoroutine(MoveToPosition(audienceMember, targetPosition.position, animator));
            yield return new WaitForSeconds(0.1f); // slight delay between each audience
        }
    }

    private IEnumerator MoveToPosition(GameObject obj, Vector3 targetPos, Animator animator)
    {
        while (true)
        {
            float distance = Vector3.Distance(obj.transform.position, targetPos);

            // Keep moving and ensure animation stays as Running
            if (distance > 0.05f)
            {
                obj.transform.position = Vector3.MoveTowards(obj.transform.position, targetPos, moveSpeed * Time.deltaTime);
                yield return null;
            }
            else
            {
                obj.transform.position = targetPos; // Snap exactly to position
                if (animator != null)
                    animator.Play("Idle");
                break;
            }
        }
    }
}
