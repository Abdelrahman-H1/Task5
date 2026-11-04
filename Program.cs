namespace Task5
{
    enum QuestionLevel
    {
        Easy = 1,
        Medium,
        Hard
    }
    abstract class Question
    {
        public string Header { get; set; }
        public int Marks { get; set; }
        public QuestionLevel Level { get; set; }

        public Question(string header, int marks, QuestionLevel level)
        {
            Header = header;
            Marks = marks;
            Level = level;
        }

        public abstract void Display();

        public abstract bool CheckAnswer(string answer);
    }
    class TrueFalseQuestion : Question
    {
        private bool CorrectAnswer;

        public TrueFalseQuestion(
            string header,
            int marks,
            QuestionLevel level,
            bool correctAnswer)
            : base(header, marks, level)
        {
            CorrectAnswer = correctAnswer;
        }

        public override void Display()
        {
            Console.WriteLine(Header);
            Console.WriteLine("1. True");
            Console.WriteLine("2. False");
        }

        public override bool CheckAnswer(string answer)
        {
            answer = answer.Trim().ToLower();

            if (answer == "1" || answer == "true")
                return CorrectAnswer;

            if (answer == "2" || answer == "false")
                return !CorrectAnswer;

            return false;
        }
    }
    class ChooseOneQuestion : Question
    {
        private List<string> Choices;
        private int CorrectChoice;

        public ChooseOneQuestion(
            string header,
            int marks,
            QuestionLevel level,
            List<string> choices,
            int correctChoice)
            : base(header, marks, level)
        {
            Choices = choices;
            CorrectChoice = correctChoice;
        }

        public override void Display()
        {
            Console.WriteLine(Header);

            for (int i = 0; i < Choices.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {Choices[i]}");
            }
        }

        public override bool CheckAnswer(string answer)
        {
            return int.TryParse(answer, out int selected)
                   && selected == CorrectChoice;
        }
    }
    class MultipleChoiceQuestion : Question
    {
        private List<string> Choices;
        private List<int> CorrectAnswers;

        public MultipleChoiceQuestion(
            string header,
            int marks,
            QuestionLevel level,
            List<string> choices,
            List<int> correctAnswers)
            : base(header, marks, level)
        {
            Choices = choices;
            CorrectAnswers = correctAnswers;
        }

        public override void Display()
        {
            Console.WriteLine(Header);

            for (int i = 0; i < Choices.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {Choices[i]}");
            }

            Console.WriteLine("Enter multiple answers separated by commas.");
        }

        public override bool CheckAnswer(string answer)
        {
            try
            {
                List<int> selectedAnswers = answer
                    .Split(',')
                    .Select(x => int.Parse(x.Trim()))
                    .Distinct()
                    .ToList();

                return selectedAnswers.OrderBy(x => x)
                    .SequenceEqual(CorrectAnswers.OrderBy(x => x));
            }
            catch
            {
                return false;
            }
        }
    }

    class Program
    {
        static List<Question> QuestionBank = new List<Question>();

        static void Main(string[] args)
        {
            bool running = true;

            while (running)
            {
                Console.WriteLine("\n===== EXAMINATION SYSTEM =====");
                Console.WriteLine("1. Doctor Mode");
                Console.WriteLine("2. Student Mode");
                Console.WriteLine("3. Exit");

                int choice = ReadInt("Choose: ", 1, 3);

                switch (choice)
                {
                    case 1:
                        DoctorMode();
                        break;

                    case 2:
                        StudentMode();
                        break;

                    case 3:
                        running = false;
                        Console.WriteLine("Goodbye!");
                        break;
                }
            }
        }
        static void DoctorMode()
        {
            Console.WriteLine("\n===== DOCTOR MODE =====");

            int number = ReadInt("How many questions? ", 1, 100);

            for (int i = 0; i < number; i++)
            {
                Console.WriteLine($"\nQuestion {i + 1}");

                Console.WriteLine("1. True / False");
                Console.WriteLine("2. Choose One");
                Console.WriteLine("3. Multiple Choice");

                int type = ReadInt("Choose type: ", 1, 3);

                QuestionLevel level = ReadLevel();

                Console.Write("Enter question header: ");
                string header = Console.ReadLine() ?? "";

                int marks = ReadInt("Enter marks: ", 1, 100);

                Question question;

                if (type == 1)
                {
                    Console.WriteLine("1. True");
                    Console.WriteLine("2. False");

                    int correct = ReadInt("Correct answer: ", 1, 2);

                    question = new TrueFalseQuestion(
                        header, marks, level, correct == 1);
                }
                else
                {
                    List<string> choices = new List<string>();

                    for (int j = 0; j < 4; j++)
                    {
                        Console.Write($"Enter choice {j + 1}: ");
                        choices.Add(Console.ReadLine() ?? "");
                    }

                    if (type == 2)
                    {
                        int correct = ReadInt("Correct choice: ", 1, 4);

                        question = new ChooseOneQuestion(
                            header, marks, level, choices, correct);
                    }
                    else
                    {
                        List<int> correctAnswers;

                        while (true)
                        {
                            Console.Write("Enter correct answers (example 1,3): ");

                            try
                            {
                                correctAnswers = Console.ReadLine()!
                                    .Split(',')
                                    .Select(x => int.Parse(x.Trim()))
                                    .Distinct()
                                    .ToList();

                                if (correctAnswers.Count > 0 &&
                                    correctAnswers.All(x => x >= 1 && x <= 4))
                                    break;
                            }
                            catch
                            {
                                // Invalid input
                            }

                            Console.WriteLine("Invalid answers. Try again.");
                        }

                        question = new MultipleChoiceQuestion(
                            header, marks, level, choices, correctAnswers);
                    }
                }

                QuestionBank.Add(question);

                Console.WriteLine("Question Added Successfully!");
            }
        }

        static void StudentMode()
        {
            Console.WriteLine("\n===== STUDENT MODE =====");

            if (QuestionBank.Count == 0)
            {
                Console.WriteLine("No questions available!");
                return;
            }

            Console.WriteLine("1. Practical Exam");
            Console.WriteLine("2. Final Exam");

            int examType = ReadInt("Choose exam type: ", 1, 2);

            QuestionLevel level = ReadLevel();

            List<Question> examQuestions = QuestionBank
                .Where(q => q.Level == level)
                .ToList();

            if (examQuestions.Count == 0)
            {
                Console.WriteLine("No questions found for this level!");
                return;
            }

            if (examType == 1)
            {
                int half = (examQuestions.Count + 1) / 2;

                Random random = new Random();

                examQuestions = examQuestions
                    .OrderBy(q => random.Next())
                    .Take(half)
                    .ToList();
            }

            int totalMarks = 0;
            int studentMarks = 0;

            Console.WriteLine("\n===== START EXAM =====");

            for (int i = 0; i < examQuestions.Count; i++)
            {
                Question current = examQuestions[i];

                Console.WriteLine($"\nQuestion {i + 1}");
                Console.WriteLine($"Marks: {current.Marks}");

                current.Display();

                Console.Write("Your Answer: ");
                string answer = Console.ReadLine() ?? "";

                totalMarks += current.Marks;

                if (current.CheckAnswer(answer))
                {
                    studentMarks += current.Marks;
                    Console.WriteLine("Correct!");
                }
                else
                {
                    Console.WriteLine("Wrong!");
                }
            }

            Console.WriteLine("\n===== EXAM RESULT =====");
            Console.WriteLine($"Your Result: {studentMarks} / {totalMarks}");
        }
        static QuestionLevel ReadLevel()
        {
            Console.WriteLine("\nChoose Level:");
            Console.WriteLine("1. Easy");
            Console.WriteLine("2. Medium");
            Console.WriteLine("3. Hard");

            int level = ReadInt("Enter level: ", 1, 3);

            return (QuestionLevel)level;
        }
        static int ReadInt(string message, int min, int max)
        {
            while (true)
            {
                Console.Write(message);

                if (int.TryParse(Console.ReadLine(), out int number)
                    && number >= min && number <= max)
                {
                    return number;
                }

                Console.WriteLine("Invalid input! Try again.");
            }
        }
    }
}
