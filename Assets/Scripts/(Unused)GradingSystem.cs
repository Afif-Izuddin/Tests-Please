using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GradingSystem : MonoBehaviour
{
    public TextMeshProUGUI essayText;
    public TextMeshProUGUI ruleText;
    public TextMeshProUGUI scoreText;
    public Button passButton;
    public Button failButton;

    private List<List<PaperData>> allDays = new List<List<PaperData>>();
    private int currentDay = 0;
    private int currentIndex = 0;
    private int correctGradingCount = 0;
    private int totalPaperCount = 0;

    void Start()
    {
        SetupAllDays();
        CountTotalPapers();

        passButton.onClick.AddListener(() => Grade(true));
        failButton.onClick.AddListener(() => Grade(false));

        ShowPaper();
        UpdateScoreText();
    }

    void SetupAllDays()
    {
        allDays.Add(new List<PaperData> {
            new PaperData("What is 2 + 2?", "4", true),
            new PaperData("What is 5 * 3?", "15", true),
            new PaperData("What is 9 - 4?", "5", true),
            new PaperData("What is 6 / 2?", "3", true),
            new PaperData("What is 3 * 3?", "6", false),
            new PaperData("What is 10 + 5?", "15", true),
            new PaperData("What is 8 * 0?", "0", true),
            new PaperData("What is 12 / 4?", "3", true),
            new PaperData("What is 14 - 7?", "8", false),
            new PaperData("What is 11 + 11?", "22", true)
        });

        allDays.Add(new List<PaperData> {
            new PaperData("What is 4 + 1?", "5", true, "UM1234"),
            new PaperData("What is 6 - 2?", "4", true, ""),
            new PaperData("What is 3 * 5?", "15", true, "UM9999"),
            new PaperData("What is 8 / 2?", "4", true, "UM0012"),
            new PaperData("What is 7 - 3?", "4", true, "UM3241"),
            new PaperData("What is 12 + 1?", "13", true, ""),
            new PaperData("What is 3 + 3?", "6", true, "UM5678"),
            new PaperData("What is 4 * 2?", "8", true, ""),
            new PaperData("What is 6 * 6?", "36", true, "UM1357"),
            new PaperData("What is 5 + 9?", "14", true, "UM2468")
        });

        allDays.Add(new List<PaperData> {
            new PaperData("What is 1 + 1?", "2", true, "UM1234"),
            new PaperData("What is 2 * 2?", "4", true, "UMID"),
            new PaperData("What is 3 + 3?", "6", true, "UM999X"),
            new PaperData("What is 10 - 5?", "5", true, "UM77"),
            new PaperData("What is 8 / 2?", "4", true, "UMXYZ"),
            new PaperData("What is 6 + 6?", "12", true, "UM43"),
            new PaperData("What is 5 * 2?", "10", true, "STUDENT1"),
            new PaperData("What is 7 + 3?", "10", true, ""),
            new PaperData("What is 9 - 2?", "7", true, "UM11"),
            new PaperData("What is 3 * 4?", "12", true, "UMAB")
        });

        allDays.Add(new List<PaperData> {
            new PaperData("What is 2 + 1?", "3", true, "UM123", "1"),
            new PaperData("What is 5 + 2?", "7", true, "UM456", ""),
            new PaperData("What is 10 / 2?", "5", true, "UM789", "3"),
            new PaperData("What is 4 * 2?", "8", true, "UM321", ""),
            new PaperData("What is 6 - 1?", "5", true, "UM654", "4"),
            new PaperData("What is 7 + 2?", "9", true, "UM987", ""),
            new PaperData("What is 8 - 3?", "5", true, "UM741", "2"),
            new PaperData("What is 9 + 0?", "9", true, "UM852", ""),
            new PaperData("What is 3 * 3?", "9", true, "UM963", "5"),
            new PaperData("What is 12 - 6?", "6", true, "UM147", "")
        });

        allDays.Add(new List<PaperData> {
            new PaperData("What is 1 + 3?", "4", true, "UM159", "5", true),
            new PaperData("What is 7 - 4?", "3", true, "UM753", "5", false),
            new PaperData("What is 4 * 3?", "12", true, "UM357", "5", true),
            new PaperData("What is 6 / 3?", "2", true, "UM951", "5", true),
            new PaperData("What is 8 + 1?", "9", true, "UM852", "5", false),
            new PaperData("What is 10 - 2?", "8", true, "UM456", "5", true),
            new PaperData("What is 5 + 2?", "7", true, "UM654", "5", true),
            new PaperData("What is 3 * 4?", "12", true, "UM123", "5", false),
            new PaperData("What is 6 + 3?", "9", true, "UM321", "5", true),
            new PaperData("What is 9 / 3?", "3", true, "UM741", "5", true)
        });
    }

    void CountTotalPapers()
    {
        foreach (var day in allDays)
        {
            totalPaperCount += day.Count;
        }
    }

    void ShowPaper()
    {
        if (currentDay >= allDays.Count)
        {
            EndGame(true);
            return;
        }

        var dayPapers = allDays[currentDay];

        if (currentIndex >= dayPapers.Count)
        {
            currentDay++;
            currentIndex = 0;
            ShowPaper();
            return;
        }

        PaperData paper = dayPapers[currentIndex];

        string display = $"Question: {paper.question}\nAnswer: {paper.studentAnswer}";

        if (!string.IsNullOrEmpty(paper.studentID))
            display += $"\nStudent ID: {paper.studentID}";
        if (!string.IsNullOrEmpty(paper.date))
            display += $"\nDate: {paper.date}";
        display += $"\nUM Logo: {(paper.hasLogo ? "Present" : "Missing")}";

        essayText.text = display;
        ruleText.text = $"Day {currentDay + 1} Rule: ";

        switch (currentDay)
        {
            case 0:
                ruleText.text += "Check if answer is correct.";
                break;
            case 1:
                ruleText.text += "Check if answer is correct AND Student ID exists.";
                break;
            case 2:
                ruleText.text += "Check if answer is correct AND Student ID ends with number.";
                break;
            case 3:
                ruleText.text += "Check if answer is correct AND Date is present.";
                break;
            case 4:
                ruleText.text += "Check all previous rules AND UM Logo must be present.";
                break;
        }

        UpdateScoreText(); // NEW
    }

    void Grade(bool playerSaidPass)
    {
        PaperData paper = allDays[currentDay][currentIndex];
        bool correct = true;

        switch (currentDay)
        {
            case 0:
                correct = paper.isCorrect;
                break;
            case 1:
                correct = paper.isCorrect && !string.IsNullOrEmpty(paper.studentID);
                break;
            case 2:
                correct = paper.isCorrect && !string.IsNullOrEmpty(paper.studentID) &&
                          char.IsDigit(paper.studentID[paper.studentID.Length - 1]);
                break;
            case 3:
                correct = paper.isCorrect && !string.IsNullOrEmpty(paper.date);
                break;
            case 4:
                correct = paper.isCorrect &&
                          !string.IsNullOrEmpty(paper.studentID) &&
                          char.IsDigit(paper.studentID[paper.studentID.Length - 1]) &&
                          !string.IsNullOrEmpty(paper.date) &&
                          paper.hasLogo;
                break;
        }

        if (playerSaidPass == correct)
        {
            correctGradingCount++;
        }

        currentIndex++;
        ShowPaper();
    }

    void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = $"{correctGradingCount}/{allDays.Count * 10}";
        }
        else
        {
            Debug.LogWarning("scoreText is not assigned in the Inspector.");
        }
    }

    void EndGame(bool success)
    {
        essayText.text = $"All Days Complete!\nTotal Correct: {correctGradingCount} / {totalPaperCount}";
        ruleText.text = success ? "You finished the grading contract!" : "You failed the contract.";
        passButton.gameObject.SetActive(false);
        failButton.gameObject.SetActive(false);
        UpdateScoreText();
    }
}


   