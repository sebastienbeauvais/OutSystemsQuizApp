using System;

class Program
{
    static void Main(string[] args)
    {
        var quizManager = new QuizManager();
        quizManager.EnsureDataFolderExists();
        quizManager.LoadQuestions();
        quizManager.LoadHistory();

        while (true)
        {
            Console.WriteLine("\nOutSystems O11 Practice Quiz\n1. Take Practice Quiz\n2. View Section Performance\n3. Exit\nChoose an option:");
            var choice = Console.ReadLine();
            switch (choice)
            {
                case "1": quizManager.TakeQuiz(); break;
                case "2": quizManager.ShowSectionPerformance(); break;
                case "3": quizManager.SaveHistory(); return;
                default: Console.WriteLine("Invalid choice."); break;
            }
        }
    }
}