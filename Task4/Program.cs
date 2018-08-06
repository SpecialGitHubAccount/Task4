using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Task4.Configuration;

namespace Task4
{
    class Program
    {
        private static List<FileSystemWatcher> watchersLst = null;

        private static List<string> langs = new List<string> { "english", "russian" };

        private static Cultures currentCulture = Cultures.English;

        static void Main(string[] args)
        {
            // Инициализация начальных значений из файла конфигурации.    
            Init();
            string cultureName = string.Empty;
            ConsoleKeyInfo key;
            bool isExit = false;
            do
            {
                Console.WriteLine(DialogStrings.Menu);
                key = Console.ReadKey();
                switch (key.KeyChar)
                {
                    case '1':
                        InputValidCultureName(out cultureName);
                        switch (cultureName)
                        {
                            case "english":
                                UpdateCulture(Cultures.English);
                                currentCulture = Cultures.English;
                                break;
                            case "russian":
                                UpdateCulture(Cultures.Russian);
                                currentCulture = Cultures.Russian;
                                break;
                            default:
                                break;
                        }
                        break;
                    case '2':
                        string extension = InputExtension();

                        Console.WriteLine(DialogStrings.DateDialog);
                        bool isAppendDate = InputYesOrNo();

                        Console.WriteLine(DialogStrings.CounterDialog);
                        bool isAutoInc = InputYesOrNo();

                        string folderName = string.Empty;
                        InputValidFolderName(out folderName);

                        AddRule(CreateRule(extension, isAppendDate, isAutoInc, folderName));
                        break;
                    case '3':
                        string directory = string.Empty;
                        InputValidDirectory(out directory);
                        AddDirectory(new DirectoryElement { Path = directory });
                        break;
                    default:

                        break;
                }
            }
            while (!isExit);
        }

        private static void Init()
        {
            CustomConfigSection customConfig = (CustomConfigSection)ConfigurationManager.GetSection("CustomConfigSection");

            using (CustomConfigSectionManager customConfigSectionManager = new CustomConfigSectionManager())
            {
                CultureElement culture = customConfigSectionManager.GetCurrentCulture();

                Thread.CurrentThread.CurrentUICulture = new CultureInfo(Convert.ToInt32(culture.Name.ToString("D")));

                Id = customConfigSectionManager.GetCurrentCount();

                Rules = customConfigSectionManager.GetRules();

                watchersLst = new List<FileSystemWatcher>();

                foreach (DirectoryElement item in customConfigSectionManager.GetDirectories())
                {
                    watchersLst.Add(CreateWatcher(item.Path, Watcher_Changed));
                }
            }
        }        

        private static void InputValidCultureName(out string cultureName)
        {
            bool isValid = false;

            do
            {
                Console.WriteLine(DialogStrings.CultureChoise);
                cultureName = Console.ReadLine();

                if (langs.Contains(cultureName))
                {
                    isValid = true;
                }
            }
            while (!isValid);
        }

        private static void InputValidFolderName(out string directory)
        {

            bool isValid = false;
            do
            {
                Console.WriteLine(DialogStrings.inputFolderName);
                directory = Console.ReadLine();

                if (Regex.IsMatch(directory, @"[^\/:*?""<>|]"))
                {
                    isValid = true;
                }
            }
            while (!isValid);
        }

        private static void InputValidDirectory(out string directory)
        {
            bool isValid = false;
            do
            {
                Console.WriteLine(DialogStrings.inputFolderName);
                directory = Console.ReadLine();

                if (Directory.Exists(directory))
                {
                    isValid = true;
                }
            }
            while (!isValid);
        }

        private static string InputExtension()
        {
            bool isValid = false;
            string fileExtension = string.Empty;
            do
            {
                Console.WriteLine(DialogStrings.InputFileExtension);
                fileExtension = Console.ReadLine();
                if (Regex.IsMatch(fileExtension, @"^\.[a-zA-Z0-9]+$"))
                {
                    isValid = true;
                };
            }
            while (!isValid);

            return fileExtension.ToLower();
        }

        private static bool InputYesOrNo()
        {
            bool result = false;
            bool isValid = false;
            do
            {
                Console.WriteLine(DialogStrings.inputYesOrNo);
                string input = Console.ReadLine();
                if (input.ToLower() == "yes")
                {
                    result = true;
                    isValid = true; ;
                }
                if (input.ToLower() == "no")
                {
                    result = false;
                    isValid = true;
                }

            } while (!isValid);

            return result;
        }

        private static RuleElement CreateRule(string extension, bool isAppendDate, bool isAutoIncrementId, string destinationFolder = "Other")
        {
            return new RuleElement
            {
                RegexPattern = ".*" + extension,
                IsAppendDate = isAppendDate,
                IsAutoIncrementId = isAutoIncrementId,
                DestinationFolder = destinationFolder
            };
        }

        private static void UpdateCulture(Cultures name)
        {
            using (CustomConfigSectionManager customConfigManager = new CustomConfigSectionManager())
            {
                customConfigManager.UpdateCulture(new CultureElement { Name = name });
            }

            CultureInfo culture = new CultureInfo(Convert.ToInt32(name.ToString("d")));
            Thread.CurrentThread.CurrentUICulture = culture;
        }

        private static void AddRule(RuleElement rule)
        {
            using (CustomConfigSectionManager customConfigManager = new CustomConfigSectionManager())
            {
                customConfigManager.AddRule(rule);
            }

            if (Rules == null)
            {
                Rules = new RuleElementCollection();
            }
            Rules.Add(rule);
        }

        private static void AddDirectory(DirectoryElement directory)
        {
            using (CustomConfigSectionManager customConfigManager = new CustomConfigSectionManager())
            {
                customConfigManager.AddDirectory(directory);
            }

            if (watchersLst == null)
            {
                watchersLst = new List<FileSystemWatcher>();
            }

            watchersLst.Add(CreateWatcher(directory.Path, Watcher_Changed));
        }

        private static FileSystemWatcher CreateWatcher(string path, FileSystemEventHandler handler)
        {
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = path;
            watcher.Created += handler;
            watcher.EnableRaisingEvents = true;
            return watcher;
        }

        private static void Watcher_Changed(object sender, FileSystemEventArgs args)
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(Convert.ToInt32(currentCulture.ToString("D")));

            // Игнорирование папок.
            if (Directory.Exists(args.FullPath))
            {
                return;
            }
            bool hasMoved = false;
            string folderName = "Other";
            if (args.ChangeType == WatcherChangeTypes.Created)
            {
                foreach (RuleElement item in Rules)
                {
                    if (Regex.IsMatch(args.Name, item.RegexPattern))
                    {
                        folderName = item.DestinationFolder;
                        MoveTo(args.FullPath, folderName, item.IsAutoIncrementId, item.IsAppendDate);
                        hasMoved = true;
                        break;
                    }
                }
            }
            if (!hasMoved)
            {
                MoveTo(args.FullPath, folderName, false, false);
            }
            // задание 3
            StringBuilder logMessage = new StringBuilder(DialogStrings.LogMessage);
            logMessage
                .Replace("{fileName}", args.Name)
                .Replace("{datetime}", DateTime.Now.ToString())
                .Replace("{folderName}", folderName);

            Console.WriteLine("id: " + Id.ToString() + " " + logMessage.ToString());

        }

        private static void MoveTo(string fullPath, string destinationFolderName, bool isAppendNumber, bool isAppendDate)
        {
            StringBuilder fileNameWithouExtensionBuilder = new StringBuilder(Path.GetFileNameWithoutExtension(fullPath));

            string destinationFolderFullName = Path.Combine(Path.GetDirectoryName(fullPath), destinationFolderName);
            DirectoryInfo destinationFolder = null;
            if (Directory.Exists(destinationFolderFullName))
            {
                destinationFolder = new DirectoryInfo(destinationFolderFullName);
            }
            else
            {
                destinationFolder = Directory.CreateDirectory(destinationFolderFullName);
            }

            if (isAppendNumber)
            {
                fileNameWithouExtensionBuilder.Append("Id");
                Id++;
                using (CustomConfigSectionManager customConfigManager = new CustomConfigSectionManager())
                {
                    customConfigManager.UpdateCounter(Id);
                }
                fileNameWithouExtensionBuilder.Append(Id);
            }

            if (isAppendDate)
            {
                fileNameWithouExtensionBuilder.Append("Date");
                fileNameWithouExtensionBuilder.Append(DateTime.Now.Date.ToLongDateString());
            }

            SafeMoveTo(fullPath, destinationFolderFullName, fileNameWithouExtensionBuilder.ToString(), Path.GetExtension(fullPath));
        }

        /// <summary>
        /// Вспомогательный метод, перемещающий файл без перезаписи.
        /// </summary>
        /// <param name="source">Type: string. Путь исходного файла.</param>
        /// <param name="destinationFolderFullName">Type: string. Путь папки назначения.</param>
        /// <param name="destinationFileFullName">Type: string. Имя файла в папке назначения.</param>
        /// <param name="extension">Type: string. Расширение файла.</param>
        private static void SafeMoveTo(string source, string destinationFolderFullName, string destinationFileFullName, string extension)
        {
            string uniqueFileName = IncrementIfHasTheSame(destinationFolderFullName, destinationFileFullName, extension);

            bool isMoved = false;
            do
            {
                try
                {
                    File.Move(source, uniqueFileName);
                    isMoved = true;
                }
                catch (IOException)
                {
                    isMoved = false;
                }
            } while (!isMoved);
        }
        /// <summary>
        /// Метод, увеличивающий номер файла, если такой уже имеется.
        /// </summary>
        /// <param name="directory">Type: string. Путь папки назначения.</param>
        /// <param name="file">Type: string. Имя файла в папке назначения.</param>
        /// <param name="fileExtension">Type: string. Расширение файла.</param>
        /// <returns>Type: string. Файл с уникальным именем в заданом катаологе.</returns>
        private static string IncrementIfHasTheSame(string directory, string file, string fileExtension)
        {
            int i = 0;
            StringBuilder destinationFileName = new StringBuilder(file + fileExtension);

            if (File.Exists(Path.Combine(directory, destinationFileName.ToString())))
            {
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(destinationFileName.ToString());
                do
                {
                    destinationFileName.Clear();
                    destinationFileName.Append(fileNameWithoutExtension + (++i).ToString() + fileExtension.ToString());
                }
                while (File.Exists(Path.Combine(directory, destinationFileName.ToString())));
            }
            return Path.Combine(directory, destinationFileName.ToString());
        }

        private static RuleElementCollection Rules;
        private static int Id { get; set; }
    }
}
