﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Xml.Serialization;

public class QuizManager
{
    private List<Question> _questions = new();
    private ScoreHistory _history;
    private string baseDirectory = AppContext.BaseDirectory;
    private const string QuestionPath = "data/questions.json";
    private const string HistoryPath = "data\\history.json";
    private const string InitialLoadQuestionsPath = "Questions\\questions.json";

    public void EnsureDataFolderExists()
    {
        var folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            Console.WriteLine("[Info] Created /data folder.");
        }
    }
    public void LoadQuestions()
    {
        if (File.Exists(QuestionPath))
        {
            ReadQuestionBank();
        }
        else
        {
            Console.WriteLine("[Info] Transferring Question Bank to Root");
            Console.WriteLine("[Info] If you want to add additional questions, please update the questions.json file.");
            Console.WriteLine("[Info] This can be found under " + AppDomain.CurrentDomain.BaseDirectory + "data");
            // Copy questions to root
            string projectRoot = Directory.GetParent(baseDirectory)!.Parent!.Parent!.Parent!.FullName;
            string firstLoadQuestionsPath = Path.Combine(projectRoot, InitialLoadQuestionsPath);
            string questionsCopyPath = Path.Combine(baseDirectory, "data", "questions.json");
            File.Copy(firstLoadQuestionsPath, questionsCopyPath, true);
            ReadQuestionBank();
        }
    }
    private void ReadQuestionBank()
    {
        var json = File.ReadAllText(QuestionPath);
        _questions = JsonSerializer.Deserialize<List<Question>>(json);
    }

    public void LoadHistory()
    {
        var historyFullPath = Path.Combine(baseDirectory, HistoryPath);
        if (File.Exists(historyFullPath))
        {
            var json = File.ReadAllText(Path.Combine(baseDirectory, HistoryPath));
            _history = JsonSerializer.Deserialize<ScoreHistory>(json);
        }
        else
        {
            _history = new ScoreHistory();
        }
    }

    public void SaveHistory()
    {
        var json = JsonSerializer.Serialize(_history, new JsonSerializerOptions { WriteIndented = true });
        var historyFullPath = Path.Combine(baseDirectory, HistoryPath);
        Console.WriteLine("[Info] Saving history to " + historyFullPath);
        File.WriteAllText(historyFullPath, json);
    }

    public void TakeQuiz()
    {
        var rand = new Random();
        var quizQuestions = _questions.OrderBy(x => rand.Next()).Take(20).ToList();
        var sectionResults = new Dictionary<string, (int Correct, int Total)>();

        foreach (var q in quizQuestions)
        {
            Console.WriteLine($"\n{q.Text}");
            for (int i = 0; i < q.Options.Length; i++)
                Console.WriteLine($"{i + 1}. {q.Options[i]}");

            Console.Write("Your answer: ");
            if (!int.TryParse(Console.ReadLine(), out int ans)) ans = -1;

            bool correct = (ans - 1) == q.CorrectOption;
            if (!sectionResults.ContainsKey(q.Category))
                sectionResults[q.Category] = (0, 0);

            var (c, t) = sectionResults[q.Category];
            sectionResults[q.Category] = (c + (correct ? 1 : 0), t + 1);
        }

        foreach (var sec in sectionResults)
        {
            Console.WriteLine($"{sec.Key}: {sec.Value.Correct}/{sec.Value.Total} = {(sec.Value.Correct * 100 / sec.Value.Total)}%");

            if (!_history.SectionScore.ContainsKey(sec.Key))
            {
                _history.SectionScore[sec.Key] = new SectionScore { Correct = 0, Total = 0 };
            }

            var oldScore = _history.SectionScore[sec.Key];
            oldScore.Correct += sec.Value.Correct;
            oldScore.Total += sec.Value.Total;
            _history.SectionScore[sec.Key] = oldScore;
        }
    }

    public void ShowSectionPerformance()
    {
        Console.WriteLine("\nHistorical Performance:");
        foreach (var sec in _history.SectionScore)
        {
            if (sec.Value.Total == 0) continue;
            Console.WriteLine($"{sec.Key}: {sec.Value.Correct}/{sec.Value.Total} = {(sec.Value.Correct * 100 / sec.Value.Total)}%");
        }
    }
}