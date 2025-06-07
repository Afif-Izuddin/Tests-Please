using UnityEngine;

public class GradingLogic : MonoBehaviour
{
        //Checks if what the player does is the correct one
       public bool ShouldPaperPass(PaperData paper, int currentDay)
    {
        bool meetsRules = false;

        switch (currentDay)
        {
            case 0: // Day 1: Only check if answer is correct
                meetsRules = paper.isCorrect;
                break;
            case 1: // Day 2: Answer correct AND Student ID exists
                meetsRules = paper.isCorrect && !string.IsNullOrEmpty(paper.studentID);
                break;
            case 2: // Day 3: Answer correct AND Student ID ends with a number
                meetsRules = paper.isCorrect &&
                             !string.IsNullOrEmpty(paper.studentID) &&
                             paper.studentID.Length > 0 &&
                             char.IsDigit(paper.studentID[paper.studentID.Length - 1]);
                break;
            case 3: // Day 4: Answer correct AND Date is present
                meetsRules = paper.isCorrect && !string.IsNullOrEmpty(paper.date);
                break;
            case 4: // Day 5: All previous rules AND UM Logo must be present
                meetsRules = paper.isCorrect &&
                             !string.IsNullOrEmpty(paper.studentID) &&
                             paper.studentID.Length > 0 &&
                             char.IsDigit(paper.studentID[paper.studentID.Length - 1]) &&
                             !string.IsNullOrEmpty(paper.date) &&
                             paper.hasLogo;
                break;
            default:
                meetsRules = paper.isCorrect;
                Debug.LogWarning($"No specific grading rule for day {currentDay}. Defaulting to 'isCorrect'.");
                break;
        }

        return meetsRules;
    }


    // Provides the descriptive rule text for the current day.
    public string GetRuleTextForDay(int currentDay)
    {
        string rule = $"Day {currentDay + 1} Rule: ";

        switch (currentDay)
        {
            case 0:
                rule += "Check if answer is correct.";
                break;
            case 1:
                rule += "Check if answer is correct AND Student ID exists.";
                break;
            case 2:
                rule += "Check if answer is correct AND Student ID ends with a number.";
                break;
            case 3:
                rule += "Check if answer is correct AND Date is present.";
                break;
            case 4:
                rule += "Check ALL previous rules AND UM Logo must be present.";
                break;
            default:
                rule += "No specific rules for this day. Check if answer is correct.";
                break;
        }
        return rule;
    }
}