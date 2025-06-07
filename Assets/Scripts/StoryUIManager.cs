using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class StoryUIManager : MonoBehaviour
{
    [Header("Story UI References")]
    public TextMeshProUGUI storyTextDisplay;
    public Button skipStoryButton;

    [Header("Story Settings")]
    public float textDisplaySpeed = 0.05f; // Time per character for typing effect
    public float timeBetweenStorySegments = 1.5f; // Pause after a segment finishes typing

    private string[] storySegments = new string[]
    {
        "You always wanted to do something important...",
        "As a child, you dreamed of greatness — a writer, a scientist, a hero.",
        "Reality had other plans.",
        "University of Malaya, Class of 2030. One step closer to the dream.",
        "Then came the bills.",
        "You took the only offer you had. A temporary job. No promises.",
        "Grade quickly. Grade correctly. Survive.",
        "Make it through your assignment period... or lose everything."
    };
    private int currentStorySegmentIndex = 0;
    private Coroutine storyDisplayCoroutine;

    void Awake()
    {
        if (skipStoryButton != null)
        {
            skipStoryButton.onClick.AddListener(OnSkipStoryClicked);
        }
    }

    void Start()
    {
        // Check if story has been skipped before
        if (SceneFlowManager.Instance != null && SceneFlowManager.Instance.GameSettingsInstance != null)
        {
            if (SceneFlowManager.Instance.GameSettingsInstance.HasSkippedStoryBefore)
            {
                Debug.Log("StoryUIManager: 'Always Skip Story' is ON. Skipping story immediately.");
                EndStory(true); // Call EndStory to skip the story and load GameScene
                return; 
            }
        }

        currentStorySegmentIndex = 0;
        // Start the first segment, which will then chain to the next automatically
        storyDisplayCoroutine = StartCoroutine(DisplayStorySegment(currentStorySegmentIndex));
    }

    private IEnumerator DisplayStorySegment(int index)
    {
        if (index < storySegments.Length)
        {
            storyTextDisplay.text = ""; // Clear current text
            string segment = storySegments[index];
            foreach (char c in segment)
            {
                storyTextDisplay.text += c; // Add character one by one
                yield return new WaitForSeconds(textDisplaySpeed);
            }
            yield return new WaitForSeconds(timeBetweenStorySegments); // Pause after typing

            // Trigger the next segment after the pause
            currentStorySegmentIndex++;
            if (currentStorySegmentIndex < storySegments.Length)
            {
                storyDisplayCoroutine = StartCoroutine(DisplayStorySegment(currentStorySegmentIndex)); 
            }
            else
            {
                EndStory(false); // All segments displayed, end the story naturally
            }
        }
        else
        {
            EndStory(false);
        }
    }

    private void OnSkipStoryClicked()
    {
        Debug.Log("StoryUIManager: Skip Story button clicked (manual skip).");
        EndStory(true); // Immediately end story and mark as skipped
    }

    private void EndStory(bool skipped)
    {
        StopCurrentStoryCoroutine(); // Ensure coroutine is stopped
        Debug.Log("StoryUIManager: Story sequence ended. Requesting GameScene load.");

        // Update persistent setting if story was skipped (either by manual skip or 'always skip' being on)
        if (SceneFlowManager.Instance != null && SceneFlowManager.Instance.GameSettingsInstance != null)
        {
            SceneFlowManager.Instance.GameSettingsInstance.SetStorySkippedStatus(skipped);
        }

        SceneFlowManager.Instance.LoadScene("GameScene");
    }

    private void StopCurrentStoryCoroutine()
    {
        if (storyDisplayCoroutine != null)
        {
            StopCoroutine(storyDisplayCoroutine);
            storyDisplayCoroutine = null;
        }
    }
}