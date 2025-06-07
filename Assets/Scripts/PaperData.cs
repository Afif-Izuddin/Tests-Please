using UnityEngine;

[System.Serializable]
public class PaperData
{
    public string question;
    public string studentAnswer;
    public bool isCorrect;
    public string studentID;
    public string date;
    public bool hasLogo;

    public PaperData(string q, string sa, bool correct)
    {
        question = q;
        studentAnswer = sa;
        isCorrect = correct;
        studentID = "";
        date = "";
        hasLogo = false;
    }

    public PaperData(string q, string sa, bool correct, string id)
    {
        question = q;
        studentAnswer = sa;
        isCorrect = correct;
        studentID = id;
        date = "";
        hasLogo = false;
    }

    public PaperData(string q, string sa, bool correct, string id, string d)
    {
        question = q;
        studentAnswer = sa;
        isCorrect = correct;
        studentID = id;
        date = d;
        hasLogo = false;
    }

    public PaperData(string q, string sa, bool correct, string id, string d, bool logo)
    {
        question = q;
        studentAnswer = sa;
        isCorrect = correct;
        studentID = id;
        date = d;
        hasLogo = logo;
    }
}