using System.Collections.Generic;
using UnityEngine;

public class PaperGenerator : MonoBehaviour
{
    // Can add more for the mathquestion and the answer, actually please do help 
    // make sure that the index is correct. eg. mathQuestions[0] the answer should be mathAnswers[0]
    private string[] mathQuestions = {
        "What is 2 + 2?", "What is 5 * 3?", "What is 9 - 4?", "What is 6 / 2?",
        "What is 3 * 3?", "What is 10 + 5?", "What is 8 * 0?", "What is 12 / 4?",
        "What is 14 - 7?", "What is 11 + 11?", "What is 4 + 1?", "What is 6 - 2?",
        "What is 3 * 5?", "What is 8 / 2?", "What is 7 - 3?"
    };

    private string[] mathAnswers = {
        "4", "15", "5", "3", "9", "15", "0", "3", "7", "22", "5", "4", "15", "4", "4"
    };

    // Generates a single paper for a specific day based on the game's rules.
    public PaperData GenerateSinglePaper(int dayIndex) 
    {
        string question;
        string studentAnswer;
        bool isCorrect;
        string studentID = "";
        string date = "";
        bool hasLogo = true;

        // Pick a random math question and determine correctness
        int questionIndex = Random.Range(0, mathQuestions.Length);
        question = mathQuestions[questionIndex];
        string correctAnswer = mathAnswers[questionIndex];

        // Introduce some incorrect answers
        if (Random.value < 0.2f) // 20% chance of incorrect answer
        {
            studentAnswer = (int.Parse(correctAnswer) + Random.Range(1, 5)).ToString(); // Slightly off answer
            isCorrect = false;
        }
        else
        {
            studentAnswer = correctAnswer;
            isCorrect = true;
        }

        // Apply day-specific rules for generating paper data
        switch (dayIndex)
        {
            case 0: // Day 1: Basic correctness
                break;
            case 1: // Day 2: Student ID exists
                studentID = (Random.value < 0.7f) ? "UM" + Random.Range(1000, 9999).ToString() : ""; // 70% chance of having an ID
                break;
            case 2: // Day 3: Student ID ends with a number
                // Ensure ID is often numerical at the end, but sometimes not
                if (Random.value < 0.6f) // 60% chance of ending with digit
                {
                    studentID = "UM" + Random.Range(100, 999).ToString();
                }
                else // 40% chance of not ending with digit
                {
                    studentID = "UMID" + (char)('A' + Random.Range(0, 26));
                }
                break;
            case 3: // Day 4: Date is present
                date = (Random.value < 0.7f) ? $"{Random.Range(1, 13)}/{Random.Range(1, 29)}/2025" : ""; // 70% chance of having a date
                
                // Still generate Student ID
                if (Random.value < 0.6f)
                {
                    studentID = "UM" + Random.Range(100, 999).ToString();
                }
                else
                {
                    studentID = "UMID" + (char)('A' + Random.Range(0, 26));
                }
                break;
            case 4: // Day 5: All previous rules AND UM Logo must be present
                // Combine everything and add the UM Logo
                studentID = (Random.value < 0.7f) ? "UM" + Random.Range(1000, 9999).ToString() : "";
                if (!string.IsNullOrEmpty(studentID) && Random.value < 0.2f) 
                {
                     studentID = studentID.Substring(0, studentID.Length - 1) + (char)('A' + Random.Range(0, 26));
                }

                date = (Random.value < 0.7f) ? $"{Random.Range(1, 13)}/{Random.Range(1, 29)}/2025" : "";
                hasLogo = (Random.value < 0.7f); // 70% chance of having a logo
                break;
        }
        
        return new PaperData(question, studentAnswer, isCorrect, studentID, date, hasLogo); 
    }
}