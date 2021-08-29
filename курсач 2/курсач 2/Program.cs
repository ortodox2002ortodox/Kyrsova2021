using System;
using System.IO;
using System.Linq;

namespace WordGuessGame
{
    public class Program
    {
        /// <резюме>
        /// Инициализирует хранилище слов стандартным набором слов.
        /// Запускает главное меню и продолжает его работу, пока пользователь не выйдет.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            // установить путь к файлу внешнего хранилища слов
            string path = ("../../../wordBank.txt");
            // начальная настройка внешнего хранилища слов
            string[] words = { "университет", "книга", "зачёт", "армия", "война", "школа", "церква", "программирование", "отец", "дом", "солнце", "девушка" }; 
            OverwriteWordBank(path, words);

            bool runMainMenu = true;
            MainMenu(runMainMenu, path);
            while (runMainMenu)
            {
                runMainMenu = MainMenu(runMainMenu, path);
            }
        }

        /// <резюме>
        /// Отображает главное меню.
        /// Принимает, проверяет и направляет выбор пользователя.
        /// </summary>
        /// <param name="runMainMenu"> переключается на 'false', когда пользователь хочет завершить программу</param>
        /// <param name="path"> путь к файлу внешнего хранилища слов </param>
        /// <returns> win or lose </returns>
        static bool MainMenu(bool runMainMenu, string path)
        {
            // получать и подтверждать направление от пользователя
            string selected = "";
            try
            {
                while (selected != "1" && selected != "2" && selected != "3")
                {
                    // Меню
                    Console.Clear();
                    Console.WriteLine("Выберете пункт: ");
                    Console.WriteLine("(1) Начать игру");
                    Console.WriteLine("(2) Настройки для администратора");
                    Console.WriteLine("(3) Закрыть программу");
                    selected = Console.ReadLine();
                }
            }
            catch (Exception)
            {
                Console.WriteLine($"\nНеверный ввод. Нажмите ENTER, чтобы повторить попытку.");
                Console.ReadLine();
                throw;
            }
            // путь к выбору меню
            switch (selected)
            {
                case "1":
                    NewGame(path);
                    break;
                case "2":
                    Admin(path);
                    break;
                case "3":
                    runMainMenu = false;
                    break;
                default:
                    Console.WriteLine($"{selected} неверный выбор. Начать сначала.");
                    break;
            }

            return runMainMenu;
        }

        /// <резюме>
        /// Запускает новую игру.
        /// Выбирает слово из банка слов и вызывает игру, чтобы начать.
        /// </summary>
        /// <param name="path"> путь к файлу внешнего хранилища слов </param>
        static void NewGame(string path)
        {
            string selected = PickWord(path); // выберите слово из хранилища слов
            char[] letters = selected.ToCharArray(); // поместить буквы слова в массив
            char[] spaces = new Char[letters.Length]; // построить массив для хранения игрового набора

            Console.Clear();

            bool wonGame = PlayGame(letters, spaces);
            Console.WriteLine("(Нажмите ENTER, чтобы вернуться в главное меню.)");
            Console.ReadLine();
        }

        /// <резюме>
        /// Случайно выбирает слово из банка слов.
        /// </summary>
        /// <param name="path">  путь к файлу внешнего банка слов</param>
        /// <returns></returns>
        static string PickWord(string path)
        {
            Random random = new Random();
            string[] words = ReadWordBank(path);
            int selectedIndex = random.Next(words.Length);
            return words[selectedIndex];
        }

        ///<резюме>
        /// Устанавливает и отображает игровое поле в начале игры и после каждого предположения
        /// </summary>
        /// <param name="spaces"> массив состояний - содержит пробелы и все правильные предположения - используется здесь для печати доски каждый раунд </param>
        static void PrintGameBoard(char[] spaces)
        {
            Console.WriteLine("\n\n");
            Console.Write("     ");
            for (int i = 0; i < spaces.Length; i++)
            {
                if (spaces[i] == '\0')
                {
                    Console.Write(" ___ ");
                }
                else
                {
                    Console.Write($" {spaces[i]} ");
                }
            }
            Console.WriteLine("\n\n");
        }

        /// <резюме>
        /// Управляет состоянием игры и контролирует ход игры.
        /// Взаимодействует с пользователем во время игры.
        /// Определяет конец игры, отслеживает предположения, наблюдает за проверкой ввода и проверкой предположений, имеет дело с приемом ввода.
        /// </резюме>
        /// <param name="letters"> массив, содержащий буквы слова в игре </param>
        /// <param name="spaces"> массив состояний - содержит пробелы и все правильные предположения - используется здесь для фиксации изменений состояния </param>
        /// <returns> игра выиграна или проиграна </returns>
        static bool PlayGame(char[] letters, char[] spaces)
        {
            Console.WriteLine("Начнём игру!\n\n\n\n");
            bool wonGame = false;

            PrintGameBoard(spaces);

            string allGuesses = "";
            string guess = "";

            while (spaces.Contains('\0'))
            {
                Console.WriteLine("Выберите букву и нажмите ENTER (или нажмите ENTER, чтобы выйти из игры):");
                try
                {
                    guess = Console.ReadLine();
                }
                catch (Exception)
                {
                    Console.WriteLine("\n\n  ПРИМЕЧАНИЕ. Произошло исключение, но оно было обнаружено. Пожалуйста, продолжайте.\n\n");
                }
                if (guess == "")
                {
                    return wonGame;
                }
                if (ValidateNewWord(guess))
                {
                    allGuesses += guess + ", ";
                    char charGuess = guess.ToLower().ToCharArray()[0];
                    if (CheckGuess(letters, charGuess))
                    {
                        for (int i = 0; i < letters.Length; i++)
                        {
                            if (letters[i] == charGuess)
                            {
                                spaces[i] = letters[i];
                                letters[i] = '_';
                            }
                        }
                        if (spaces.Contains('\0'))
                        {
                            Console.Clear();
                            Console.WriteLine("Отличная догадка! Следущая буква? ");
                            PrintGameBoard(spaces);
                        }
                        else
                        {
                            Console.Clear();
                            Console.WriteLine("Молодец! Ты выиграл!");
                            wonGame = true;
                            PrintGameBoard(spaces);
                        }
                    }
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("Ой... очень плохо.Попробуйте снова.");
                        PrintGameBoard(spaces);
                    }
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Неверное предположение.Попробуйте снова.");
                    PrintGameBoard(spaces);
                }
            }
            Console.WriteLine($"Ваши догадки: {allGuesses}");
            return wonGame;
        }

        /// <резюме>
        /// Проверяет текущее предположение пользователя по буквам, которые еще не были угаданы
        /// </summary>
        /// <param name="letters">массив, содержащий буквы слова в игре</param>
        /// <param name="charGuess"> символьное представление текущего предположения пользователя </param>
        /// <returns> предположение правильное (истина) или нет (ложное) </returns>
        public static bool CheckGuess(char[] letters, char charGuess)
        {
            return letters.Contains(charGuess);
        }

        /// <резюме>
        /// Читает текущее содержимое внешнего хранилища слов
        /// </summary>
        /// <param name="path"> путь к файлу внешнего хранилища слов </param>
        /// <returns> список слов в хранилище слов </returns>
        static string[] ReadWordBank(string path)
        {
            try
            {
                string[] lines = File.ReadAllLines(path);
                return lines;
            }
            catch (Exception)
            {
                Console.WriteLine($"ОШИБКА: не удалось прочитать файл в {path}");
                Console.ReadLine();
                throw;
            }
        }

        /// <резюме>
        /// Отображает меню администратора
        /// Принимает, проверяет и направляет выбор пользователя.
        /// </summary>
        /// <param name="path"> путь к файлу внешнего хранилище слов </param>
        static void Admin(string path)
        {
            // получать и подтверждать направление от пользователя
            string selected = "";
            try
            {
                while (selected != "1" && selected != "2" && selected != "3" && selected != "4" && selected != "5")
                {
                    // меню дисплея
                    Console.Clear();
                    Console.WriteLine("Выберите пункт: ");
                    Console.WriteLine("(1) Просмотреть все доступные слова в хранилище");
                    Console.WriteLine("(2) Добавить новое слово в хранилище");
                    Console.WriteLine("(3) Удалить слово из хранилища слов");
                    Console.WriteLine("(4) Восстановите хранилище слов");
                    Console.WriteLine("(5) Вернуться в главное меню");
                    selected = Console.ReadLine();
                }
            }
            catch (Exception)
            {
                Console.WriteLine($"\nНеверная запись. Нажмите ENTER, чтобы повторить попытку.");
                Console.ReadLine();
                throw;
            }

            // путь к выбору меню
            switch (selected)
            {
                case "1": // просмотреть все слова
                    Console.Clear();
                    ViewWords(path);
                    Console.WriteLine("\nНажмите ENTER, чтобы вернуться в меню для администратора.");
                    Console.ReadLine();
                    Admin(path);
                    break;
                case "2": // дорбавить слово
                    Console.Clear();
                    Console.WriteLine("Введите слово, которое нужно добавить: ");
                    string addWord = Console.ReadLine();
                    AddWord(path, addWord);
                    Admin(path);
                    break;
                case "3": // удалить слово
                    Console.Clear();
                    DeleteWord(path);
                    Admin(path);
                    break;
                case "4": // удалить и восстановить файл хранилища слов
                    Console.Clear();
                    string[] words = { "университет", "книга", "зачёт", "армия", "война", "школа", "церква", "программирование", "отец", "дом", "солнце", "девушка" }; // seed file contents
                    OverwriteWordBank(path, words);
                    Console.WriteLine("\nХранилище слов изменено. Нажмите ENTER, чтобы вернуться в административное меню..");
                    Console.ReadLine();
                    Admin(path);
                    break;
                case "5":
                    MainMenu(true, path); // вернуться в меню
                    break;
                default: // не выявлено - все остальные случаи исключены условиями 'while'
                    break;
            }
        }

        /// /// <резюме>
        /// Добавляет слово во внешний банк слов по указанию пользователя (если слово проходит проверку)
        /// </summary>
        /// <param name="path"> путь к файлу внешнего хранилища слов </param>
        /// <param name="addWord"> слово, которое пользователь хочет добавить </param>
        /// <returns> слово добавлено (истина) или не добавлено (ложь) </returns>
        public static bool AddWord(string path, string addWord)
        {
            if (ValidateNewWord(addWord))
            {
                string[] newWord = { addWord.ToLower() };
                File.AppendAllLines(path, newWord);
                Console.WriteLine($"\nAdded {addWord} to the word bank.");
                return true;
            }
            else
            {
                Console.WriteLine("Извините, использование специальных символов или цифр не допускается.");
                return false;
            }
        }

        /// <резюме>
        /// Проверяет слово, которое пользователь хочет добавить в хранилище слов
        /// </резюме>
        /// <param name="addWord"> слово, которое пользователь хочет добавить </param>
        /// <returns> действительный (истина) или недействительный (ложный)</returns>
        static bool ValidateNewWord(string addWord)
        {
            char[] notAllowed = { ' ', ',', '.', ':', '\t',';','!','@','#','$','%','^','&'
            ,'*','(',')','-','_','/','|','[',']','{','}','<','>','?','1','2','3','4','5','6','7','8','9','0'};

            foreach (char badChar in notAllowed)
            {
                if (addWord.Contains(badChar))
                {
                    return false;
                }
            }
            return true;
        }

        /// <резюме>
        /// Удаляет слово из внешнего хранилища слов (если есть)
        /// Метод: считывает строки файла в массив; перезаписывает элемент, содержащий целевое слово, последним элементом; заменяет последний элемент пустой строкой; перестраивает файл с измененным массивом
        /// </резюме>
        /// <param name="path"> путь к файлу внешнего хранилище слов </param>
        static void DeleteWord(string path)
        {
            Console.WriteLine("Хранилище слов содержит эти слова:");
            string[] words = ViewWords(path);
            Console.WriteLine("\nКакое слово вы хотите удалить? ");
            string selected = Console.ReadLine();
            int indexToDelete = Array.IndexOf(words, selected.ToLower());
            if (indexToDelete == -1)
            {
                Console.Write($"{selected} нет в списке.");
            }
            else
            {
                words[indexToDelete] = words[words.Length - 1];
                words[words.Length - 1] = null;
                OverwriteWordBank(path, words);
                Console.WriteLine($"\nУдалено { selected} из хранилища. Нажмите ENTER, чтобы вернуться в административное меню..");
                Console.ReadLine();

            }
        }

        /// <резюме>
        /// Отображает все слова в хранилище слов
        /// </резюме>
        /// <param name="path"> путь к файлу внешнего хранилища слов </param>
        /// <returns> массив слов из хранилища слов (только для теста) </returns>
        public static string[] ViewWords(string path)
        {
            string[] words = ReadWordBank(path);

            Console.WriteLine("\nЭти слова есть в хранилище слов:");
            foreach (string word in words)
            {
                Console.WriteLine($"{word}");
            }
            return words;
        }

        /// <резюме>
        /// Заменяет текущее хранилище слов новым списком слов
        /// </summary>
        /// <param name="path"> путь к файлу внешнего хранилища слов </param>
        /// <param name="words"> слова для записи в хранилище слов </param>
        /// <returns> количество строк в хранилище слов после перезаписи </returns>
        public static int OverwriteWordBank(string path, string[] words)
        {
            try
            {
                using (StreamWriter streamWriter = new StreamWriter(path))
                {
                    foreach (string word in words)
                    {
                        if (word != null)
                        {
                            streamWriter.WriteLine(word);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            string[] linesInNewFile = ReadWordBank(path);
            return linesInNewFile.Length;

        }
    }
}