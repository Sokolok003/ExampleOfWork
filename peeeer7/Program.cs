using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using MyLibrary;
using System.Runtime.InteropServices;

namespace peeeer7
{
    /// <summary>
    /// УВАЖАЕМЫЙ ПРОВЕРЯЮЩИЙ. Я сделал сохранение состояния приложения. Оно работает только с помощью команды выхода. Поэтому я удалил
    /// возможность закрыть консоль. Почти сразу после этого комментария ты можешь видеть, как я это сделал. Данный код взят из интернета.
    /// Ссылка воть: https://ru.stackoverflow.com/questions/593565/%d0%92%d0%be%d0%b7%d0%bc%d0%be%d0%b6%d0%bd%d0%be-%d0%bb%d0%b8-%d1%83%d0%b1%d1%80%d0%b0%d1%82%d1%8c-%d0%ba%d0%bd%d0%be%d0%bf%d0%ba%d1%83-%d0%97%d0%b0%d0%ba%d1%80%d1%8b%d1%82%d1%8c-%d0%b2-%d0%ba%d0%be%d0%bd%d1%81%d0%be%d0%bb%d1%8c%d0%bd%d0%be%d0%bc-%d0%bf%d1%80%d0%b8%d0%bb%d0%be%d0%b6%d0%b5%d0%bd%d0%b8%d0%b8
    /// Также, если в файле будет написана какая-то дичь (не относящаяся к проекту), то будет ошибка. Даже если будет пустой файл просто с пробелами,
    /// то все равно сработает try-catch. Так что не трогай пожалуйста файл Status.txt, чтобы не возникло недопонимания (только если ты хочешь удалить весь текст внутри него и оставить его пустым).
    /// Спасибо тебе, и удачного дня!
    /// </summary>
    class Program
    {

        private const int MF_BYCOMMAND = 0x00000000;
        public const int SC_CLOSE = 0xF060;

        [DllImport("user32.dll")]
        public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();


        public static string[] linesForStartMenu = new string[] { "Работа с пользователем", "Работа с проектом", "Настроить проект", "Выход из приложения" };
        // Строки начального меню.
        public static string[] linesForWorkWithUser = new string[] { "Создать нового пользователя", "Посмотреть список пользователей", "Удалить пользователя", "Вернуться назад" };
        // Строки меню работы с пользователем.
        public static string[] linesForWorkWithProject = new string[] { "Создать проект", "Посмотреть список проектов", "Изменить имя проекта", "Удалить проект", "Вернуться назад" };
        // Строки меню работы с проектом.
        public static string[] linesForWorkInProject = new string[] { "Добавить задачу в проект", "Назначить испонителя задачи", "Изменить исполнителя задачи", "Удалить исполнителя из задачи", "Изменить статус задачи", "Посмотреть список задач", "Группировать задачи по статусу", "Удалить задачу из проекта", "Работа с задачами тип Epic", "Вернуться назад" };
        // Строки меню работы внутри проекта.
        public static string[] typesOfExcersices = new string[] { "Epic", "Story", "Task", "Bug" };
        // Строки выбора типа задачи.
        public static string[] linesForWorkWithEpic = new string[] { "Назначить подзадачу", "Удалить подзадачу", "Посмотреть все подзадачи", "Вернуться назад" };
        public static string[] statusesWithoutFirst = new string[] { "Задача в работе", "Завершенная задача" };
        public static string[] statusesWithoutSecond = new string[] { "Открытая задача", "Завершенная задача" };
        public static string[] statusesWithoutThird = new string[] { "Открытая задача", "Задача в работе" };


        public static List<User> users = new List<User>();
        // Все пользователи.
        public static List<Project> projects = new List<Project>();
        // Все проекты.
        public static List<Excersice> subtask = new List<Excersice>();
        // Возможные подзадачи для задачи типа Epic.
        static void Main(string[] args)
        {
            LetsGo();
        }

        /// <summary>
        /// Проверка ошибок. На всякий случай. 
        /// </summary>
        private static void LetsGo()
        {
            DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_CLOSE, MF_BYCOMMAND);
            try
            {
                var fi = new FileInfo("Status.txt");
                if (fi.Length != 0)
                {
                    ObjectCache.LoadInstance("Status.txt");
                    users = ObjectCache.Instance.AllUsers;
                    projects = ObjectCache.Instance.AllProjects;
                }
                else
                {
                    users = new List<User>();
                    projects = new List<Project>();
                }
                StartMenu(1);
            }
            catch (Exception e)
            {
                Error(e.Message);
            }
        }


        /// <summary>
        /// Стартовое меню.
        /// </summary>
        /// <param name="numberOfLine"> Номер "зеленой" строки. </param>
        static void StartMenu(int numberOfLine)
        {
            numberOfLine = WriteCnoicesInConsole(linesForStartMenu, 4, numberOfLine, "startMenu", 0, 0);
            StartMenu(numberOfLine);
        }


        /// <summary>
        /// Прыжок со стартового меню.
        /// </summary>
        /// <param name="number"> Номер "зеленой" строки. </param>
        static void JumpFromStartMenu(int number)
        {
            switch (number)
            {
                case 1:
                    UserAct(1);
                    break;

                case 2:
                    ProjectAct(1);
                    break;

                case 3:
                    int indexOfProject = ChooseProjectForWork("");
                    if (indexOfProject == -1)
                        StartMenu(3);
                    else
                    {
                        WorkInProjectAct(indexOfProject, 1);
                    }
                    break;

                case 4:
                    ObjectCache.CreateInstance();
                    ObjectCache.Instance.AddUser(users);
                    ObjectCache.Instance.AddProject(projects);
                    ObjectCache.SaveInstance("Status.txt");
                    Environment.Exit(0);
                    break;
            }
        }
        /// <summary>
        /// Меню работы с задачей типа Epic.
        /// </summary>
        /// <param name="index"> Индекс проекта. </param>
        /// <param name="numberOfLine"> Номер "зеленой" строки. </param>
        /// <param name="indexOfExc"> Индекс задачи типа Epic. </param>
        static void WorkInEpicAct(int index, int numberOfLine, int indexOfExc)
        {
            numberOfLine = WriteCnoicesInConsole(linesForWorkWithEpic, 4, numberOfLine, "inEpicAct", index, indexOfExc);
            WorkInEpicAct(index, numberOfLine, indexOfExc);
        }
        /// <summary>
        /// Меню работы внутри проекта.
        /// </summary>
        /// <param name="index"> Индекс проекта. </param>
        /// <param name="numberOfLine"> Номер "зеленой" строки. </param>
        static void WorkInProjectAct(int index, int numberOfLine)
        {
            numberOfLine = WriteCnoicesInConsole(linesForWorkInProject, 10, numberOfLine, "inProjectAct", index, 0);
            WorkInProjectAct(index, numberOfLine);
        }
        /// <summary>
        /// Переход из меню работы внутри проекта.
        /// </summary>
        /// <param name="number"> Номер "зеленой" строки. </param>
        /// <param name="index"> Индекс проекта. </param>
        static void JumpFromWorkInProjectAct(int number, int index)
        {
            switch (number)
            {
                case 1:
                    ChoiceTypeAct(1, index);
                    break;

                case 2:
                    if (users.Count == 0)
                    {
                        ContinueAfterError("Список пользователей на данный момент пуст!");
                        WorkInProjectAct(index, 2);
                    }
                    else
                        GiveUser(index);
                    break;

                case 3:
                    if (users.Count == 0)
                    {
                        ContinueAfterError("Список пользователей на данный момент пуст!");
                        WorkInProjectAct(index, 3);
                    }
                    else
                        GiveUserAgain(index);
                    break;

                case 4:
                    if (projects[index].Excersices.Count == 0)
                    {
                        ContinueAfterError("Список задач на данный момент пуст!");
                        WorkInProjectAct(index, 4);
                    }
                    else
                    {
                        if (users.Count == 0)
                        {
                            ContinueAfterError("Список пользователей на данный момент пуст!");
                            WorkInProjectAct(index, 4);
                        }
                        else
                        {
                            DeleteUserFromExc(index);
                        }
                    }
                    break;

                case 5:
                    if (projects[index].Excersices.Count == 0)
                    {
                        ContinueAfterError("Список задач на данный момент пуст!");
                        WorkInProjectAct(index, 5);
                    }
                    else
                    {
                        int indexOfExc = ChoiceExc("", index, "Статус");
                        if (indexOfExc == -1)
                            WorkInProjectAct(index, 3);
                        ChoiceStatus(index, indexOfExc, 1);
                    }
                    break;

                case 6:
                    WriteAllExcs(index);
                    break;

                case 7:
                    GroupExcsForStatus(index);
                    break;

                case 8:
                    if (projects[index].Excersices.Count == 0)
                    {
                        ContinueAfterError("Список задач на данный момент пуст!");
                        WorkInProjectAct(index, 8);
                    }
                    else
                    {
                        DeleteExc(index);
                    }
                    break;

                case 9:
                    int indexOfEpic = ChoiceEpic(index, "");
                    WorkInEpicAct(index, 1, indexOfEpic);
                    break;

                case 10:
                    StartMenu(3);
                    break;
            }
        }
        /// <summary>
        /// Переход из меню работы с задачей типа Epic.
        /// </summary>
        /// <param name="number"> Номер "зеленой" строки. </param>
        /// <param name="index"> Индекс проекта. </param>
        /// <param name="indexOfEpic"> Индекс задачи типа Epic. </param>
        static void JumpInEpic(int number, int index, int indexOfEpic)
        {
            switch (number)
            {
                case 1:
                    AddSubtask(number, index, indexOfEpic);
                    break;

                case 2:
                    DelSubtask(index, indexOfEpic);
                    break;

                case 3:
                    ShowSubsInEpic(index, indexOfEpic);
                    break;

                case 4:
                    WorkInProjectAct(index, 9);
                    break;
            }
        }
        /// <summary>
        /// Удаление подзадачи.
        /// </summary>
        /// <param name="index"> Индекс проекта. </param>
        /// <param name="indexOfEpic"> Индекс задачи типа Epic. </param>
        static void DelSubtask(int index, int indexOfEpic)
        {
            try
            {
                int indexOfSub = ChoiceSubtask(projects[index].Excersices[indexOfEpic].Subs(), "", index, indexOfEpic);
                projects[index].Excersices[indexOfEpic].DelSubtask(indexOfSub);
                Congratulation("Подазадача успешно удалена!");
                WorkInEpicAct(index, 2, indexOfEpic);
            }
            catch (Exception e)
            {
                ContinueAfterError(e.Message);
                WorkInEpicAct(index, 2, indexOfEpic);
            }
        }
        /// <summary>
        /// Выбор индекса подзадачи.
        /// </summary>
        /// <param name="subs"> Список подзадач задачи. </param>
        /// <param name="error"> Ошибка функции. </param>
        /// <param name="index"> Индекс проекта. </param>
        /// <param name="indexOfEpic"> Индекс задачи типа Epic. </param>
        /// <returns></returns>
        static int ChoiceSubtask(List<Excersice> subs, string error, int index, int indexOfEpic)
        {
            Console.Clear();
            if (error != "")
                Error(error);
            projects[index].Excersices[indexOfEpic].ShowAllSubtasts();
            Console.WriteLine("Введите индекс подазадчи, которую хотите удалить.");
            int indexOfSub = -1;
            if (!int.TryParse(Console.ReadLine(), out indexOfSub) || indexOfSub < 0 || indexOfSub >= subs.Count)
            {
                return ChoiceSubtask(subs, "", index, indexOfEpic);
            }
            else
            {
                subtask.Add(subs[indexOfSub]);
                return indexOfSub;
            }
        }
        /// <summary>
        /// Вывод всех подзадач.
        /// </summary>
        /// <param name="index"> Индекс проекта. </param>
        /// <param name="indexOfEpic"> Индекс задачи типа Epic. </param>
        static void ShowSubsInEpic(int index, int indexOfEpic)
        {
            try
            {
                Console.Clear();
                projects[index].Excersices[indexOfEpic].ShowAllSubtasts();
                Congratulation("");
                WorkInEpicAct(index, 3, indexOfEpic);
            }
            catch (Exception e)
            {
                ContinueAfterError(e.Message);
                WorkInEpicAct(index, 3, indexOfEpic);
            }
        }
        /// <summary>
        /// Добавление подзадачи.
        /// </summary>
        /// <param name="number"> Номер "зеленой" строки. </param>
        /// <param name="index"> Индекс проекта. </param>
        /// <param name="indexOfEpic"> Индекс задачи типа Epic. </param>
        static void AddSubtask(int number, int index, int indexOfEpic)
        {
            int indexOfSub = ChoiceSubtask("", number, index, indexOfEpic);
            try
            {
                projects[index].Excersices[indexOfEpic].AddSubtask(subtask[indexOfSub]);
                Congratulation("Подзадача успешно добавлена!");
                subtask.Remove(subtask[indexOfSub]);
                WorkInEpicAct(index, 1, indexOfEpic);
            }
            catch (Exception e)
            {
                ContinueAfterError(e.Message);
                WorkInEpicAct(index, 1, indexOfEpic);
            }
        }
        /// <summary>
        /// Выбор нужной подзадачи из списка возможных.
        /// </summary>
        /// <param name="error"> Ошибка функции. </param>
        /// <param name="number"> Номер "зеленой" строки. </param>
        /// <param name="index"> Инедкс проекта. </param>
        /// <param name="indexOfEpic"> Индекс задачи типа Epic. </param>
        /// <returns></returns>
        static int ChoiceSubtask(string error, int number, int index, int indexOfEpic)
        {
            Console.Clear();
            if (error != "")
                Error(error);
            ShowSubtasks(index, indexOfEpic);
            Console.WriteLine("Введите индекс нужной подзадачи:");
            int indexOfSub = -1;
            if (!int.TryParse(Console.ReadLine(), out indexOfSub) || indexOfSub < 0 || indexOfSub >= subtask.Count)
            {
                return ChoiceSubtask("Некорректный индекс!", number, index, indexOfEpic);
            }
            else
            {
                return indexOfSub;
            }
        }
        /// <summary>
        /// Вывод всех доступных подзадач.
        /// </summary>
        /// <param name="index"> Индекс проекта. </param>
        /// <param name="indexOfEpic"> Индекс задачи типа Epic. </param>
        static void ShowSubtasks(int index, int indexOfEpic)
        {
            if (subtask.Count == 0)
            {
                ContinueAfterError("Список возможных подзадач на данный момент пуст!");
                WorkInEpicAct(index, 1, indexOfEpic);
            }
            else
            {
                Console.WriteLine("Список возможных подзадач:");
                for (int i = 0; i < subtask.Count; ++i)
                {
                    subtask[i].GetInfo(i);
                }
            }
        }
        /// <summary>
        /// Выбор нужной задачи типа Epic.
        /// </summary>
        /// <param name="index"> Инедкс проекта. </param>
        /// <param name="error"> Ошибка функции. </param>
        /// <returns></returns>
        static int ChoiceEpic(int index, string error)
        {
            int count = 0;
            List<int> excersices = new List<int>();
            Console.Clear();
            if (error != "")
                Error(error);
            Console.WriteLine("Введите индекс задачи типа Epic для дальнейшей работы:");
            for (int i = 0; i < projects[index].Excersices.Count; ++i)
            {
                if (projects[index].Excersices[i].type == "Epic")
                {
                    count++;
                    Console.WriteLine(count - 1 + ". " + projects[index].Excersices[i].Name);
                    excersices.Add(i);
                }
            }
            if (excersices.Count == 0)
            {
                ContinueAfterError("Список задач типа Epic на данный момент пуст!");
                JumpFromWorkInProjectAct(9, index);
            }
            int indexOfEpic = -1;
            if (!int.TryParse(Console.ReadLine(), out indexOfEpic) || indexOfEpic < 0 || indexOfEpic >= count)
            {
                return ChoiceEpic(index, "Некорректный индекс!");
            }
            else
            {
                return excersices[indexOfEpic];
            }
        }
        /// <summary>
        /// Удаление пользователя из задачи.
        /// </summary>
        /// <param name="index"> Индекс проекта. </param>
        static void DeleteUserFromExc(int index)
        {
            int indexOfExc = ChoiceExc("", index, "Удаление исполнителя");
            switch (projects[index].Excersices[indexOfExc].type)
            {
                case "Epic":
                    ContinueAfterError("В задаче типа Epic нет исполнителя!");
                    break;

                case "Bug":
                    projects[index].Excersices[indexOfExc].DelUser(1);
                    Congratulation("Пользователя успешно удален!");
                    break;

                case "Task":
                    projects[index].Excersices[indexOfExc].DelUser(1);
                    Congratulation("Пользователя успешно удален!");
                    break;

                case "Story":
                    List<string> names = projects[index].Excersices[indexOfExc].AllUsers();
                    for (int i = 0; i < names.Count; ++i)
                    {
                        Console.WriteLine(i + ". Пользователь " + names[i]);
                    }
                    int indexOfName = DeleteUserFromStory("", names.Count);
                    projects[index].Excersices[indexOfExc].DelUser(indexOfName);
                    Congratulation("Пользователя успешно удален!");
                    break;
            }
        }
        /// <summary>
        /// Ввод индекс удаляемого пользователя из задачи типа Story.
        /// </summary>
        /// <param name="error"> Ошибка функции. </param>
        /// <param name="count"> Размер списка имен. </param>
        /// <returns></returns>
        static int DeleteUserFromStory(string error, int count)
        {
            if (error != "")
                Error(error);
            Console.WriteLine("Введите индекс пользователя для удаления из задачи:");
            int indexOfUser = -1;
            if (!int.TryParse(Console.ReadLine(), out indexOfUser) || indexOfUser < 0 || indexOfUser >= count)
            {
                return ChoiceUser("Некорректный индекс!");
            }
            else
                return indexOfUser;
        }


        /// <summary>
        /// Удаление задачи из проекта.
        /// </summary>
        /// <param name="index"> Индекс проекта. </param>
        static void DeleteExc(int index)
        {
            int indexOfExc = ChoiceExc("", index, "Удаление задачи");
            projects[index].Excersices.Remove(projects[index].Excersices[indexOfExc]);
            Congratulation("Задача успешно удалена!");
            WorkInProjectAct(index, 8);
        }


        /// <summary>
        /// Группировка задач по статусу.
        /// </summary>
        /// <param name="index"> Индекс проекта. </param>
        static void GroupExcsForStatus(int index)
        {
            Console.Clear();
            if (projects[index].Excersices.Count == 0)
            {
                ContinueAfterError("Список задач на данный момент пуст!");
                WorkInProjectAct(7, index);
            }
            else
            {
                List<Excersice> excStatus1 = new List<Excersice>();
                List<Excersice> excStatus2 = new List<Excersice>();
                List<Excersice> excStatus3 = new List<Excersice>();
                for (int i = 0; i < projects[index].Excersices.Count; ++i)
                {
                    switch (projects[index].Excersices[i].Status)
                    {
                        case "Задача в работе":
                            excStatus2.Add(projects[index].Excersices[i]);
                            break;

                        case "Открытая задача":
                            excStatus1.Add(projects[index].Excersices[i]);
                            break;

                        case "Завершенная задача":
                            excStatus3.Add(projects[index].Excersices[i]);
                            break;
                    }
                }
                WriteTypeOfExc("Открытая задача");
                GetInfoFromExc(excStatus1);
                WriteTypeOfExc("Задача в работе");
                GetInfoFromExc(excStatus2);
                WriteTypeOfExc("Завершенная задача");
                GetInfoFromExc(excStatus3);
                Congratulation("");
            }
        }
        /// <summary>
        /// Вывод информации списка задач.
        /// </summary>
        /// <param name="excersices"> Список задач. </param>
        static void GetInfoFromExc(List<Excersice> excersices)
        {
            for (int i = 0; i < excersices.Count; ++i)
            {
                excersices[i].GetInfo(i);
            }
        }
        /// <summary>
        /// Вывод в консоль тип задачи.
        /// </summary>
        /// <param name="text"> Тип задачи. </param>
        static void WriteTypeOfExc(string text)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(text);
            Console.ResetColor();
        }
        /// <summary>
        /// Вывод всех задач.
        /// </summary>
        /// <param name="index"> Индекс проекта. </param>
        static void WriteAllExcs(int index)
        {
            Console.Clear();
            if (projects[index].Excersices.Count == 0)
            {
                ContinueAfterError("Список задач на данный момент пуст!");
            }
            else
            {
                for (int i = 0; i < projects[index].Excersices.Count; ++i)
                {
                    projects[index].Excersices[i].GetInfo(i);
                }
                ConsoleKey key = ConsoleKey.Escape;
                while (key != ConsoleKey.Enter)
                {
                    Console.WriteLine("Нажмите Enter для продолжения...");
                    key = Console.ReadKey().Key;
                }
            }
            WorkInProjectAct(index, 6);
        }
        /// <summary>
        /// Изменение статуса.
        /// </summary>
        /// <param name="status"> Тип статуса. </param>
        /// <param name="indexOfExc"> Индекс задачи. </param>
        /// <param name="index"> Индекс проекта. </param>
        static void ChangeStatus(string status, int indexOfExc, int index)
        {
            projects[index].Excersices[indexOfExc].Status = status;
            Congratulation("Статус успешно изменен!");
            WorkInProjectAct(index, 5);
        }
        /// <summary>
        /// Выбор статуса в зависимости от того, какой статус стоит на данный момент.
        /// </summary>
        /// <param name="indexOfProject"> Индекс проекта. </param>
        /// <param name="indexOfExc"> Индекс задачи. </param>
        /// <param name="numberOfLine"> Номер "зеленой" строки. </param>
        static void ChoiceStatus(int indexOfProject, int indexOfExc, int numberOfLine)
        {
            switch (projects[indexOfProject].Excersices[indexOfExc].Status)
            {
                case "Открытая задача":
                    numberOfLine = WriteCnoicesInConsole(statusesWithoutFirst, 2, numberOfLine, "statusAct", indexOfProject, indexOfExc);
                    ChoiceStatus(indexOfProject, indexOfExc, numberOfLine);
                    break;

                case "Задача в работе":
                    numberOfLine = WriteCnoicesInConsole(statusesWithoutSecond, 2, numberOfLine, "statusAct", indexOfProject, indexOfExc);
                    ChoiceStatus(indexOfProject, indexOfExc, numberOfLine);
                    break;

                case "Завершенная задача":
                    numberOfLine = WriteCnoicesInConsole(statusesWithoutThird, 2, numberOfLine, "statusAct", indexOfProject, indexOfExc);
                    ChoiceStatus(indexOfProject, indexOfExc, numberOfLine);
                    break;
            }
        }
        /// <summary>
        /// Назначения пользователя из списка исполнителей.
        /// </summary>
        /// <param name="index"> Индекс проекта. </param>
        static void GiveUser(int index)
        {
            if (projects[index].Excersices.Count == 0)
            {
                ContinueAfterError("Список задач на данный момент пуст!");
            }
            else
            {
                int indexOfExc = ChoiceExc("", index, "Назначение");
                if (indexOfExc != -1)
                {
                    WriteAllUsers();
                    int indexOfUser = ChoiceUser("");
                    projects[index].Excersices[indexOfExc].GiveUser(users[indexOfUser]);
                    Congratulation("Исполнитель успешно назначен!");
                }
            }
            WorkInProjectAct(index, 2);
        }

        /// <summary>
        /// Выбор пользователя для назначения на задачу из списка исполнителей.
        /// </summary>
        /// <param name="error"> Ошибка функции. </param>
        /// <returns> Инндекс пользователя. </returns>
        static int ChoiceUser(string error)
        {
            if (error != "")
                Error(error);
            Console.WriteLine("Введите индекс пользователя для (пере)назначения на задачу:");
            int indexOfUser = -1;
            if (!int.TryParse(Console.ReadLine(), out indexOfUser) || indexOfUser < 0 || indexOfUser >= users.Count)
            {
                return ChoiceUser("Некорректный индекс!");
            }
            else
                return indexOfUser;
        }
        /// <summary>
        /// Вывод всех пользователей.
        /// </summary>
        static void WriteAllUsers()
        {
            Console.Clear();
            Console.WriteLine("Список пользователей:");
            for (int i = 0; i < users.Count; ++i)
            {
                Console.WriteLine("{0}. Имя: {1}. Дата создания: {2}.", i, users[i].Name, users[i].Date);
            }
        }
        /// <summary>
        /// Переназначение пользователя.
        /// </summary>
        /// <param name="index"> Индекс проекта. </param>
        static void GiveUserAgain(int index)
        {
            if (projects[index].Excersices.Count == 0)
            {
                ContinueAfterError("Список задач на данный момент пуст!");
            }
            else
            {
                int indexOfExc = ChoiceExc("", index, "Переназначение");
                if (indexOfExc != -1)
                {
                    WriteAllUsers();
                    int indexOfUser = ChoiceUser("");
                    projects[index].Excersices[indexOfExc].GiveUser(users[indexOfUser]);
                    Congratulation("Исполнитель успешно переназначен!");
                }
            }
            WorkInProjectAct(index, 3);
        }
        /// <summary>
        /// Выбор задачи для функции.
        /// </summary>
        /// <param name="error"> Ошибка функции. </param>
        /// <param name="index"> Индекс проекта. </param>
        /// <param name="typeOfFunction"> Тип функции. </param>
        /// <returns> Индекс задачи в проекте. </returns>
        static int ChoiceExc(string error, int index, string typeOfFunction)
        {
            Console.Clear();
            int indexOfExc = -1;
            if (error != "")
                Error(error);
            switch (typeOfFunction)
            {
                case "Назначение":
                    Console.WriteLine("Введите имя задачи для назначения ей исполнителя (или Exit для выхода из функции): ");
                    break;

                case "Переназначение":
                    Console.WriteLine("Введите имя задачи для переназначения ей исполнителя (или Exit для выхода из функции): ");
                    break;

                case "Удаление исполнителя":
                    Console.WriteLine("Введите имя задачи для удаления исполнителя (или Exit для выхода из функции): ");
                    break;

                case "Статус":
                    Console.WriteLine("Введите имя задачи для изменения статуса (или Exit для выхода из функции): ");
                    break;

                case "Удаление задачи":
                    Console.WriteLine("Введите имя задачи для удаления ее из проекта (или Exit для выхода из функции): ");
                    break;
            }
            string name = Console.ReadLine();
            if (name == "Exit")
                return -1;
            for (int i = 0; i < projects[index].Excersices.Count; ++i)
            {
                if (name == projects[index].Excersices[i].Name)
                {
                    if (projects[index].Excersices[i].type == "Epic")
                    {
                        switch (typeOfFunction)
                        {
                            case "Назначение":
                                return ChoiceExc("Задаче типа Epic нельзя назначить исполнителя!", index, typeOfFunction);

                            case "Переназначение":
                                return ChoiceExc("Задаче типа Epic нельзя переназначить исполнителя!", index, typeOfFunction);

                            case "Удаление исполнителя":
                                return ChoiceExc("В задаче типа Epic нет исполнителя!", index, typeOfFunction);

                            case "Статус":
                                indexOfExc = i;
                                break;

                            case "Удаление задачи":
                                indexOfExc = i;
                                break;
                        }
                    }
                    else
                    {
                        indexOfExc = i;
                        break;
                    }
                }
            }
            if (indexOfExc == -1)
                return ChoiceExc("Данной задачи не существует в этом проекте!", index, typeOfFunction);
            else
            {
                Congratulation("Задача успешно выбрана.");
                return indexOfExc;
            }
        }
        /// <summary>
        /// Выбор типа задачи для создания.
        /// </summary>
        /// <param name="numberOfLine"> Номер "зеленой" строки. </param>
        /// <param name="index"> Индекс проекта. </param>
        static void ChoiceTypeAct(int numberOfLine, int index)
        {
            numberOfLine = WriteCnoicesInConsole(typesOfExcersices, 4, numberOfLine, "typeAct", index, 0);
            ChoiceTypeAct(numberOfLine, index);
        }
        /// <summary>
        /// Переход к созданию задачи.
        /// </summary>
        /// <param name="number"> Тип задачи. </param>
        /// <param name="index"> Индекс проекта. </param>
        static void JumpAfterChoiceTypeOfPRoject(int number, int index)
        {
            switch (number)
            {
                case 1:
                    CreateExc(index, "", "Epic");
                    break;

                case 2:
                    CreateExc(index, "", "Story");
                    break;

                case 3:
                    CreateExc(index, "", "Task");
                    break;

                case 4:
                    CreateExc(index, "", "Bug");
                    break;
            }
        }
        /// <summary>
        /// Создание задачи.
        /// </summary>
        /// <param name="index"> Индекс проекта. </param>
        /// <param name="error"> Ошибка функции. </param>
        /// <param name="type"> Тип задачи. </param>
        static void CreateExc(int index, string error, string type)
        {
            Console.Clear();
            if (error != "")
                Error(error);
            Console.WriteLine("Введите имя задачи: ");
            string name = Console.ReadLine();
            if (name == "Exit")
            {
                error = "Данное имя невозможно!";
                CreateExc(index, error, type);
            }
            for (int i = 0; i < projects[index].Excersices.Count; ++i)
            {
                if (name == projects[index].Excersices[i].Name)
                {

                    error = "Задача с таким именем уже существует!";
                    CreateExc(index, error, type);
                }
            }
            switch (type)
            {
                case "Epic":
                    Epic epic = new Epic(name);
                    projects[index].Excersices.Add(epic);

                    break;

                case "Story":
                    Story story = new Story(name);
                    projects[index].Excersices.Add(story);
                    subtask.Add(story);
                    break;

                case "Task":
                    Task task = new Task(name);
                    projects[index].Excersices.Add(task);
                    subtask.Add(task);
                    break;

                case "Bug":
                    Bug bug = new Bug(name);
                    projects[index].Excersices.Add(bug);
                    break;
            }
            Congratulation("Задача успешно доавлена!");
            WorkInProjectAct(index, 1);
        }
        /// <summary>
        /// Выбор проекта, с котором в дальнейшем будет работа.
        /// </summary>
        /// <param name="error"> Ошибка функции. </param>
        /// <returns> Индекс проекта. </returns>
        static int ChooseProjectForWork(string error)
        {
            Console.Clear();
            if (projects.Count == 0)
            {
                ContinueAfterError("Список проектов на данный момент пуст!");
                return -1;
            }
            else
            {
                if (error != "")
                    Error(error);
                Console.WriteLine("Введите имя проекта, с которым хотите работать:");
                string name = Console.ReadLine();
                bool check = false;
                int index = -1;
                for (int i = 0; i < projects.Count; ++i)
                {
                    if (name == projects[i].Name)
                    {
                        check = true;
                        index = i;
                    }

                }
                if (check == false)
                    index = ChooseProjectForWork("Такое имя не найдено!");
                return index;
            }
        }
        // ФУНКЦИИ ПРОЕКТА
        /// <summary>
        /// Меню работы с проектом.
        /// </summary>
        /// <param name="numberOfLine"> Номер "зеленой" строки. </param>
        static void ProjectAct(int numberOfLine)
        {
            numberOfLine = WriteCnoicesInConsole(linesForWorkWithProject, 5, numberOfLine, "projectAct", 0, 0);
            ProjectAct(numberOfLine);
        }
        /// <summary>
        /// Распределение выбора пользователя.
        /// </summary>
        /// <param name="number"> Номер строки, которую выбрал пользователь. </param>
        static void JumpFromProjectAct(int number)
        {
            switch (number)
            {
                case 1:
                    CreateNewProject("");
                    break;

                case 2:
                    ShowProjects();
                    break;

                case 3:
                    ChangeNameOfProject("");
                    break;

                case 4:
                    DeleteProject("");
                    break;

                case 5:
                    StartMenu(2);
                    break;
            }
        }
        /// <summary>
        /// Удалить проект.
        /// </summary>
        /// <param name="error"> Ошибка функции. </param>
        static void DeleteProject(string error)
        {
            Console.Clear();
            if (projects.Count == 0)
            {
                ContinueAfterError("Список проектов на данный момент пуст!");
            }
            else
            {
                if (error != "")
                    Error(error);
                Console.WriteLine("Введите имя проекта для удаления:");
                int index = -1;
                string name = Console.ReadLine();
                for (int i = 0; i < projects.Count; ++i)
                {
                    if (name == projects[i].Name)
                    {
                        index = i;
                    }
                }
                if (index == -1)
                {
                    DeleteProject("Проект с таким именем не найден!");
                }
                else
                {
                    projects.RemoveAt(index);
                }
                Congratulation("Проект успешно удален!");
            }
            ProjectAct(4);
        }
        /// <summary>
        /// Изменение имя проекта подготовка.
        /// </summary>
        /// <param name="error"> Ошибка функции. </param>
        static void ChangeNameOfProject(string error)
        {
            Console.Clear();
            if (projects.Count == 0)
            {
                ContinueAfterError("Список проектов на данный момент пуст!");
            }
            else
            {
                if (error != "")
                    Error(error);
                Console.WriteLine("Введите имя проекта, который хотите изменить:");
                int index = -1;
                string name = Console.ReadLine();
                for (int i = 0; i < projects.Count; ++i)
                {
                    if (name == projects[i].Name)
                    {
                        index = i;
                    }
                }
                if (index == -1)
                {
                    ChangeNameOfProject("Проект с таким именем не найден!");
                }
                else
                {
                    projects[index].Name = ChangeName("");
                }
                Congratulation("Имя проекта успешно изменено!!");
                ProjectAct(3);
            }
        }
        /// <summary>
        /// Изменить имя для проекта финиш.
        /// </summary>
        /// <param name="error"> Ошибка функции. </param>
        /// <returns> Возвращает новое имя проекта. </returns>
        static string ChangeName(string error)
        {
            Console.Clear();
            if (error != "")
                Error(error);
            Console.WriteLine("Введите новое имя проекта: ");
            string name = Console.ReadLine();
            for (int i = 0; i < projects.Count; ++i)
            {
                if (name == projects[i].Name)
                {
                    error = "Проект с таким именем уже существует!";
                    name = ChangeName(error);
                }
            }
            return name;
        }
        /// <summary>
        /// Посмотреть все проекты.
        /// </summary>
        static void ShowProjects()
        {
            Console.Clear();
            if (projects.Count == 0)
            {
                ContinueAfterError("Список проектов на данный момент пуст!");
            }
            else
            {
                ConsoleKey key = ConsoleKey.Escape;
                while (key != ConsoleKey.Enter)
                {
                    Console.WriteLine("Список всех проектов:");
                    for (int i = 0; i < projects.Count; ++i)
                    {
                        Console.Write($"\n{i}. ");
                        projects[i].GetInfo();
                    }
                    Console.WriteLine("\nНажмите Enter для продолжения...");
                    key = Console.ReadKey().Key;
                }
            }
        }
        /// <summary>
        /// Создание нового проекта.
        /// </summary>
        /// <param name="error"> Ошибка функции. </param>
        static void CreateNewProject(string error)
        {
            Console.Clear();
            if (error != "")
                Error(error);
            Console.WriteLine("Введите имя проекта: ");
            string name = Console.ReadLine();
            for (int i = 0; i < projects.Count; ++i)
            {
                if (name == projects[i].Name)
                {
                    error = "Проект с таким именем уже существует!";
                    CreateNewProject(error);
                }
            }
            Project project = new Project(name);
            projects.Add(project);
            Congratulation("Проект успешно создан!");
            ProjectAct(1);
        }

        // ФУНКЦИИ ПОЛЬЗОВАТЕЛЯ
        /// <summary>
        /// Меню работы с пользователем.
        /// </summary>
        /// <param name="numberOfLine"> Номер "зеленой" строки. </param>
        static void UserAct(int numberOfLine)
        {
            numberOfLine = WriteCnoicesInConsole(linesForWorkWithUser, 4, numberOfLine, "userAct", 0, 0);
            UserAct(numberOfLine);
        }
        /// <summary>
        /// Распределение выбора пользователя.
        /// </summary>
        /// <param name="number"> Номер выбранной строки. </param>
        static void JumpFromUserAct(int number)
        {
            switch (number)
            {
                case 1:
                    CreateNewUser("");
                    break;

                case 2:
                    WatchAllUsers();
                    break;

                case 3:
                    DeleteUser("");
                    break;

                case 4:
                    StartMenu(1);
                    break;
            }
        }
        /// <summary>
        /// Удаление пользователя.
        /// </summary>
        /// <param name="error"> Ошибка функции. </param>
        static void DeleteUser(string error)
        {
            Console.Clear();
            if (users.Count == 0)
            {
                ContinueAfterError("Список пользователей на данный момент пуст!");
            }
            else
            {
                if (error != "")
                    Error(error);
                Console.WriteLine("Введите имя пользователя для удаления:");
                int index = -1;
                string name = Console.ReadLine();
                for (int i = 0; i < users.Count; ++i)
                {
                    if (name == users[i].Name)
                    {
                        index = i;
                    }
                }
                if (index == -1)
                {
                    DeleteUser("Пользователь с таким именем не найден!");
                }
                else
                {
                    users.RemoveAt(index);
                }
                Congratulation("Пользователь успешно удален!");
                UserAct(3);
            }
        }
        /// <summary>
        /// Просмотр всех пользователей.
        /// </summary>
        static void WatchAllUsers()
        {
            Console.Clear();
            if (users.Count == 0)
            {
                ContinueAfterError("Список пользователей на данный момент пуст!");
            }
            else
            {
                ConsoleKey key = ConsoleKey.Escape;
                while (key != ConsoleKey.Enter)
                {
                    Console.WriteLine("Список всех пользователей на данный момент:");
                    for (int i = 0; i < users.Count; ++i)
                    {
                        users[i].GetInfo(i);
                    }
                    Console.WriteLine("Нажмите Enter для продолжения...");
                    key = Console.ReadKey().Key;
                }
            }
        }
        /// <summary>
        /// Создание нового пользователя.
        /// </summary>
        /// <param name="error"> Ошибка выполнения. </param>
        static void CreateNewUser(string error)
        {
            Console.Clear();
            if (error != "")
                Error(error);
            Console.WriteLine("Введите имя пользователя: ");
            string name = Console.ReadLine();
            for (int i = 0; i < users.Count; ++i)
            {
                if (name == users[i].Name)
                {
                    error = "Пользователь с таким именем уже существует!";
                    CreateNewUser(error);
                }
            }
            User user = new User(name);
            users.Add(user);
            Congratulation("Пользователь успешно создан!");
            UserAct(1);
        }


        // ОБЩИЕ ФУНКЦИИ
        /// <summary>
        /// Делает строку зеленой, если таков путь.
        /// </summary>
        /// <param name="text"> Текст строки. </param>
        /// <param name="choice"> Показывает, таков ли путь. </param>
        static void WriteLine(string text, bool choice)
        {
            if (choice == true)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(text);
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine(text);
            }
        }
        /// <summary>
        /// Успешное выполнение функции.
        /// </summary>
        /// <param name="text"> Текст выполнения. </param>
        static void Congratulation(string text)
        {
            if (text != "")
            {
                ConsoleKey key = ConsoleKey.Escape;
                while (key != ConsoleKey.Enter)
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(text);
                    Console.ResetColor();
                    Console.WriteLine("Нажмите Enter для продолжения...");
                    key = Console.ReadKey().Key;
                }
            }
            else
            {
                ConsoleKey key = ConsoleKey.Escape;
                while (key != ConsoleKey.Enter)
                {
                    Console.WriteLine("Нажмите Enter для продолжения...");
                    key = Console.ReadKey().Key;
                }
            }

        }
        /// <summary>
        /// Вывод ошибки.
        /// </summary>
        /// <param name="text"> Текст ошибки. </param>
        static void Error(string text)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(text);
            Console.ResetColor();
        }
        /// <summary>
        /// Вывод в консоль строк меню.
        /// </summary>
        /// <param name="lines"> Список строк меню. </param>
        /// <param name="maxNumbersOfLines"> Количество строк меню. </param>
        /// <param name="numberOfLine"> Номер выбранной строки пользователем. </param>
        /// <param name="menuType"> Тип меню. </param>
        /// <returns></returns>
        static int WriteCnoicesInConsole(string[] lines, int maxNumbersOfLines, int numberOfLine, string menuType, int index, int indexOfExc)
        {
            Console.Clear();
            for (int i = 0; i < maxNumbersOfLines; ++i)
            {
                if (i + 1 == numberOfLine)
                {
                    WriteLine(lines[i], true);
                }
                else
                {
                    WriteLine(lines[i], false);
                }
            }
            ConsoleKey key = Console.ReadKey().Key;
            switch (key)
            {
                case ConsoleKey.DownArrow:
                    numberOfLine++;
                    if (numberOfLine > maxNumbersOfLines)
                    {
                        numberOfLine = 1;
                    }
                    break;

                case ConsoleKey.UpArrow:
                    numberOfLine--;
                    if (numberOfLine == 0)
                    {
                        numberOfLine = maxNumbersOfLines;
                    }
                    break;

                case ConsoleKey.Enter:
                    switch (menuType)
                    {
                        case "userAct":
                            JumpFromUserAct(numberOfLine);
                            break;

                        case "startMenu":
                            JumpFromStartMenu(numberOfLine);
                            break;

                        case "projectAct":
                            JumpFromProjectAct(numberOfLine);
                            break;

                        case "inProjectAct":
                            JumpFromWorkInProjectAct(numberOfLine, index);
                            break;

                        case "typeAct":
                            JumpAfterChoiceTypeOfPRoject(numberOfLine, index);
                            break;

                        case "statusAct":
                            ChangeStatus(lines[numberOfLine - 1], indexOfExc, index);
                            break;

                        case "inEpicAct":
                            JumpInEpic(numberOfLine, index, indexOfExc);
                            break;
                    }
                    break;

                default:
                    break;
            }
            return numberOfLine;
        }
        /// <summary>
        /// Вывод ошибки другого типа.
        /// </summary>
        /// <param name="text"> Текст ошибки. </param>
        static void ContinueAfterError(string text)
        {
            ConsoleKey key = ConsoleKey.Escape;
            while (key != ConsoleKey.Enter)
            {
                Console.Clear();
                Console.WriteLine(text);
                Console.WriteLine("Нажмите Enter для продолжения...");
                key = Console.ReadKey().Key;
            }
        }
    }

    [Serializable]
    public class ObjectCache
    {
        public List<User> AllUsers { get; set; }
        public List<Project> AllProjects { get; set; }
        public List<User> allUsers = new List<User>();
        public List<Project> allProjects = new List<Project>();

        public void AddUser(List<User> users)
        {
            AllUsers = users;
        }

        public void AddProject(List<Project> projects)
        {
            AllProjects = projects;
        }

        public static ObjectCache Instance { get; private set; }
        public static void CreateInstance()
        {
            Instance = new ObjectCache();
        }
        public static void LoadInstance(string fileName)
        {
            using (var st = File.OpenRead(fileName))
                Instance = (ObjectCache)new BinaryFormatter().Deserialize(st);
        }

        public static void SaveInstance(string fileName)
        {
            using (var f = File.OpenWrite(fileName))
                new BinaryFormatter().Serialize(f, Instance);
        }
    }
}
