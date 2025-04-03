using System.Diagnostics;

namespace CryptoSearch
{
    public class ConsoleUI
    {
        private readonly BitArray256 _currentPrivateKey;
        private readonly AddressValidator _validator;
        private int _foundAddresses = 0;
        private readonly List<string> _foundAddressList = [];

        public ConsoleUI()
        {
            _currentPrivateKey = new BitArray256();
            _validator = new AddressValidator();
        }

        public async Task RunAsync()
        {
            DisplayWelcomeScreen();
            await MainMenuAsync();
        }

        private void DisplayWelcomeScreen()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(@"
   _____                  _        _____                     _     
  / ____|                | |      / ____|                   | |    
 | |     _ __ _   _ _ __ | |_ ___| (___   ___  __ _ _ __ ___| |__  
 | |    | '__| | | | '_ \| __/ _ \\___ \ / _ \/ _` | '__/ __| '_ \ 
 | |____| |  | |_| | |_) | || (_) |___) |  __/ (_| | | | (__| | | |
  \_____|_|   \__, | .__/ \__\___/_____/ \___|\__,_|_|  \___|_| |_|
               __/ | |                                             
              |___/|_|                                             
");
            Console.WriteLine("Новая эра поиска существующих Bitcoin адресов\n");
            Console.ResetColor();
            Console.WriteLine("Загрузка базы данных адресов...");
            _validator.LoadAddressDatabase();

            Console.WriteLine("База данных загружена успешно!");
            Console.WriteLine("\nНажмите любую клавишу, чтобы продолжить...");
            Console.ReadKey(true);
        }

        private async Task MainMenuAsync()
        {
            bool exit = false;

            while (!exit)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("===== ГЛАВНОЕ МЕНЮ =====");
                Console.ResetColor();
                Console.WriteLine("1. Редактировать биты приватного ключа");
                Console.WriteLine("2. Запустить поиск");
                Console.WriteLine("3. Показать найденные адреса");
                Console.WriteLine("4. Сохранить найденные адреса");
                Console.WriteLine("5. Загрузить базу адресов из файла");
                Console.WriteLine("6. Расширенный поиск");
                Console.WriteLine("7. Выход");

                Console.Write("\nВыберите опцию: ");
                string choice = Console.ReadLine() ?? "";

                switch (choice)
                {
                    case "1":
                        EditPrivateKeyBits();
                        break;
                    case "2":
                        await SearchAddressesAsync();
                        break;
                    case "3":
                        ShowFoundAddresses();
                        break;
                    case "4":
                        SaveFoundAddresses();
                        break;
                    case "5":
                        LoadAddressDatabase();
                        break;
                    case "6":
                        await AdvancedSearchMenuAsync();
                        break;
                    case "7":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Некорректный выбор. Нажмите любую клавишу...");
                        Console.ReadKey(true);
                        break;
                }
            }
        }

        private void EditPrivateKeyBits()
        {
            bool doneEditing = false;

            while (!doneEditing)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("===== РЕДАКТОР ПРИВАТНОГО КЛЮЧА =====");
                Console.ResetColor();

                DisplayPrivateKeyBits();

                Console.WriteLine("\nКоманды:");
                Console.WriteLine("set <ячейка> <значение> - установить значение ячейки (0-255)");
                Console.WriteLine("toggle <ячейка> - изменить статус фиксации ячейки");
                Console.WriteLine("random - сгенерировать случайные значения для нефиксированных ячеек");
                Console.WriteLine("reset - сбросить все ячейки");
                Console.WriteLine("done - вернуться в главное меню");

                Console.Write("\nВведите команду: ");
                string command = Console.ReadLine() ?? "";
                string[] parts = command.Split(' ');

                if (parts.Length == 0) continue;

                switch (parts[0].ToLower())
                {
                    case "set":
                        if (parts.Length == 3 && int.TryParse(parts[1], out int cellIndex) &&
                            byte.TryParse(parts[2], out byte value) && cellIndex >= 0 && cellIndex < 32)
                        {
                            _currentPrivateKey.SetCellValue(cellIndex, value);
                        }
                        else
                        {
                            Console.WriteLine("Некорректные параметры команды 'set'");
                            Console.ReadKey(true);
                        }
                        break;

                    case "toggle":
                        if (parts.Length == 2 && int.TryParse(parts[1], out int toggleIndex) &&
                            toggleIndex >= 0 && toggleIndex < 32)
                        {
                            _currentPrivateKey.ToggleCellFixed(toggleIndex);
                        }
                        else
                        {
                            Console.WriteLine("Некорректные параметры команды 'toggle'");
                            Console.ReadKey(true);
                        }
                        break;

                    case "random":
                        _currentPrivateKey.RandomizeNonFixedCells();
                        break;

                    case "reset":
                        _currentPrivateKey.Reset();
                        break;

                    case "done":
                        doneEditing = true;
                        break;

                    default:
                        Console.WriteLine("Неизвестная команда");
                        Console.ReadKey(true);
                        break;
                }
            }
        }

        private void DisplayPrivateKeyBits()
        {
            Console.WriteLine("\nТекущий приватный ключ (256 бит, разбитый на 32 ячейки по 8 бит):");
            Console.WriteLine("Индекс\tЗначение\tФиксирован\tДвоичное представление");
            Console.WriteLine(new string('-', 70));

            for (int i = 0; i < 32; i++)
            {
                byte value = _currentPrivateKey.GetCellValue(i);
                bool isFixed = _currentPrivateKey.IsCellFixed(i);
                string binaryStr = Convert.ToString(value, 2).PadLeft(8, '0');

                Console.ForegroundColor = isFixed ? ConsoleColor.Green : ConsoleColor.White;
                Console.WriteLine($"{i,2}\t{value,3}\t\t{(isFixed ? "Да" : "Нет"),-10}\t{binaryStr}");
                Console.ResetColor();
            }
        }

        private async Task SearchAddressesAsync()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("===== ПОИСК АДРЕСОВ =====");
            Console.ResetColor();

            Console.Write("Введите количество адресов для генерации: ");
            if (!int.TryParse(Console.ReadLine(), out int count) || count <= 0)
            {
                Console.WriteLine("Некорректное число. Нажмите любую клавишу для продолжения...");
                Console.ReadKey(true);
                return;
            }

            Console.WriteLine("\nНачинаем поиск...");

            int initialFound = _foundAddresses;
            Stopwatch sw = Stopwatch.StartNew();

            await Task.Run(() =>
            {
                for (int i = 0; i < count; i++)
                {
                    BitArray256 testKey = _currentPrivateKey.Clone();
                    testKey.RandomizeNonFixedCells();

                    string privKey = testKey.ToHexString();
                    string address = BitcoinAddressGenerator.GenerateAddressFromPrivateKey(privKey);

                    if (_validator.IsAddressExists(address))
                    {
                        _foundAddresses++;
                        string wifKey = BitcoinAddressGenerator.PrivateKeyToWIF(privKey);
                        _foundAddressList.Add($"{address},{wifKey}");

                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Найден адрес: {address}");
                        Console.ResetColor();
                    }

                    if (i % 1000 == 0)
                    {
                        Console.Write($"\rПрогресс: {i}/{count} ({i * 100.0 / count:F2}%) " +
                                     $"Найдено: {_foundAddresses - initialFound}");
                    }
                }
            });

            sw.Stop();

            Console.WriteLine($"\n\nПоиск завершен!");
            Console.WriteLine($"Время выполнения: {sw.Elapsed}");
            Console.WriteLine($"Проверено адресов: {count}");
            Console.WriteLine($"Найдено новых адресов: {_foundAddresses - initialFound}");
            Console.WriteLine($"Всего найдено адресов: {_foundAddresses}");
            Console.WriteLine($"Скорость: {count / sw.Elapsed.TotalSeconds:F0} адресов/сек");

            Console.WriteLine("\nНажмите любую клавишу для продолжения...");
            Console.ReadKey(true);
        }

        private void ShowFoundAddresses()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("===== НАЙДЕННЫЕ АДРЕСА =====");
            Console.ResetColor();

            if (_foundAddressList.Count == 0)
            {
                Console.WriteLine("Адреса еще не найдены.");
            }
            else
            {
                Console.WriteLine("Адрес                                                   | Приватный ключ (WIF)");
                Console.WriteLine(new string('-', 98));

                foreach (string entry in _foundAddressList)
                {
                    string[] parts = entry.Split(',');
                    Console.WriteLine($"{parts[0]} | {parts[1]}");
                }

                Console.WriteLine($"\nВсего адресов: {_foundAddressList.Count}");
            }

            Console.WriteLine("\nНажмите любую клавишу для продолжения...");
            Console.ReadKey(true);
        }

        private void SaveFoundAddresses()
        {
            if (_foundAddressList.Count == 0)
            {
                Console.WriteLine("Нет адресов для сохранения.");
                Console.ReadKey(true);
                return;
            }

            Console.Write("Введите имя файла для сохранения: ");
            string filename = Console.ReadLine() ?? "found_addresses.txt";

            if (string.IsNullOrWhiteSpace(filename))
                filename = "found_addresses.txt";

            try
            {
                File.WriteAllLines(filename, _foundAddressList.Select(entry =>
                {
                    string[] parts = entry.Split(',');
                    return $"{parts[0]},{parts[1]}";
                }));

                Console.WriteLine($"Адреса успешно сохранены в файл {filename}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при сохранении адресов: {ex.Message}");
            }

            Console.ReadKey(true);
        }

        private void LoadAddressDatabase()
        {
            Console.Clear();
            Console.WriteLine("===== ЗАГРУЗКА БАЗЫ АДРЕСОВ =====");

            Console.Write("Введите путь к файлу с адресами: ");
            string filePath = Console.ReadLine() ?? "";

            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            {
                Console.WriteLine("Файл не найден.");
                Console.ReadKey(true);
                return;
            }

            try
            {
                Console.WriteLine("Загрузка базы данных...");
                int count = _validator.LoadAddressDatabaseFromFile(filePath);
                Console.WriteLine($"Загружено {count} адресов.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке базы данных: {ex.Message}");
            }

            Console.WriteLine("Нажмите любую клавишу для продолжения...");
            Console.ReadKey(true);
        }

        private async Task AdvancedSearchMenuAsync()
        {
            bool back = false;

            while (!back)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("===== РАСШИРЕННЫЙ ПОИСК =====");
                Console.ResetColor();
                Console.WriteLine("1. Поиск с фильтрацией HEX-значений");
                Console.WriteLine("2. Поиск по алгоритму secp256k1 (по публичным ключам)");
                Console.WriteLine("3. Настройка паттернов из найденных ключей");
                Console.WriteLine("4. Назад в главное меню");

                Console.Write("\nВыберите опцию: ");
                string choice = Console.ReadLine() ?? "";

                switch (choice)
                {
                    case "1":
                        await SearchWithHexFilterAsync();
                        break;
                    case "2":
                        await SearchWithSecp256k1Async();
                        break;
                    case "3":
                        SetupPatternsFromKnownKeys();
                        break;
                    case "4":
                        back = true;
                        break;
                    default:
                        Console.WriteLine("Некорректный выбор. Нажмите любую клавишу...");
                        Console.ReadKey(true);
                        break;
                }
            }
        }

        private async Task SearchWithHexFilterAsync()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("===== ПОИСК С ФИЛЬТРАЦИЕЙ HEX-ЗНАЧЕНИЙ =====");
            Console.ResetColor();

            Console.WriteLine("Этот режим игнорирует сгенерированные приватные ключи с четными HEX-значениями,");
            Console.WriteLine("основываясь на наблюдении о редкости таких значений в реальных ключах.");

            Console.Write("\nВведите количество адресов для генерации: ");
            if (!int.TryParse(Console.ReadLine(), out int count) || count <= 0)
            {
                Console.WriteLine("Некорректное число. Нажмите любую клавишу для продолжения...");
                Console.ReadKey(true);
                return;
            }

            Console.WriteLine("\nНачинаем поиск с фильтрацией HEX-значений...");

            int initialFound = _foundAddresses;
            Stopwatch sw = Stopwatch.StartNew();

            await Task.Run(() =>
            {
                for (int i = 0; i < count; i++)
                {
                    BitArray256 testKey = _currentPrivateKey.Clone();
                    testKey.RandomizeNonFixedCellsWithFilter();

                    string privKey = testKey.ToHexString();
                    string address = BitcoinAddressGenerator.GenerateAddressFromPrivateKey(privKey);

                    if (_validator.IsAddressExists(address))
                    {
                        _foundAddresses++;
                        string wifKey = BitcoinAddressGenerator.PrivateKeyToWIF(privKey);
                        _foundAddressList.Add($"{address},{wifKey}");

                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Найден адрес: {address}");
                        Console.ResetColor();
                    }

                    if (i % 1000 == 0)
                    {
                        Console.Write($"\rПрогресс: {i}/{count} ({i * 100.0 / count:F2}%) " +
                                     $"Найдено: {_foundAddresses - initialFound}");
                    }
                }
            });

            sw.Stop();

            Console.WriteLine($"\n\nПоиск завершен!");
            Console.WriteLine($"Время выполнения: {sw.Elapsed}");
            Console.WriteLine($"Проверено адресов: {count}");
            Console.WriteLine($"Найдено новых адресов: {_foundAddresses - initialFound}");
            Console.WriteLine($"Всего найдено адресов: {_foundAddresses}");
            Console.WriteLine($"Скорость: {count / sw.Elapsed.TotalSeconds:F0} адресов/сек");

            Console.WriteLine("\nНажмите любую клавишу для продолжения...");
            Console.ReadKey(true);
        }

        private async Task SearchWithSecp256k1Async()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("===== ПОИСК ПО АЛГОРИТМУ SECP256K1 =====");
            Console.ResetColor();

            Console.WriteLine("Этот режим работает напрямую с публичными ключами в формате secp256k1,");
            Console.WriteLine("что позволяет значительно ускорить поиск и проверять ключи из разных блокчейнов.");

            Console.WriteLine("\nПроверка базы публичных ключей...");

            string pubKeyDbPath = "pubkeys.db";
            bool dbExists = File.Exists(pubKeyDbPath);

            if (!dbExists)
            {
                Console.WriteLine("База публичных ключей не найдена. Для использования этого режима необходимо");
                Console.WriteLine("создать файл публичных ключей из блокчейна по инструкции ниже:");
                Console.WriteLine("\n1. Импортировать все транзакции из блокчейна");
                Console.WriteLine("2. Извлечь публичные ключи из транзакций");
                Console.WriteLine("3. Сохранить их в файл 'pubkeys.db' в формате [33 байта ключа][1 байт типа сети]");
                Console.WriteLine("\nДля демонстрации будет создана тестовая база с несколькими ключами.");

                Console.WriteLine("Создание тестовой базы публичных ключей...");
                CreateDemoPubKeyDatabase(pubKeyDbPath);
                Console.WriteLine("Тестовая база создана успешно!");
            }

            Secp256k1KeyFinder keyFinder = new Secp256k1KeyFinder();
            int keyCount = keyFinder.LoadPublicKeyDatabase(pubKeyDbPath);

            Console.WriteLine($"База публичных ключей загружена: {keyCount} ключей.");

            Console.Write("\nВведите количество попыток для генерации: ");
            if (!int.TryParse(Console.ReadLine(), out int attempts) || attempts <= 0)
            {
                Console.WriteLine("Некорректное число. Нажмите любую клавишу для продолжения...");
                Console.ReadKey(true);
                return;
            }

            Console.WriteLine("\nНачинаем поиск по алгоритму secp256k1...");

            var progress = new Progress<string>(message =>
            {
                if (message.StartsWith("Прогресс:"))
                {
                    Console.Write($"\r{message}");
                }
                else
                {
                    Console.WriteLine(message);
                }
            });

            Stopwatch sw = Stopwatch.StartNew();

            int matchesFound = await keyFinder.SearchMatchingKeysAsync(_currentPrivateKey, attempts, progress);

            sw.Stop();

            Console.WriteLine($"\n\nПоиск завершен!");
            Console.WriteLine($"Время выполнения: {sw.Elapsed}");
            Console.WriteLine($"Проверено ключей: {attempts}");
            Console.WriteLine($"Найдено совпадений: {matchesFound}");
            Console.WriteLine($"Скорость: {attempts / sw.Elapsed.TotalSeconds:F0} ключей/сек");

            if (matchesFound > 0)
            {
                Console.WriteLine("\nНайденные совпадения:");

                var matches = keyFinder.GetMatches();
                foreach (var match in matches)
                {
                    Console.WriteLine($"Адрес: {match.Address}");
                    Console.WriteLine($"Приватный ключ: {match.PrivateKey}");
                    Console.WriteLine($"Публичный ключ: {match.PublicKeyCompressed}");
                    Console.WriteLine($"Сеть: {match.NetworkType}");
                    Console.WriteLine();

                    _foundAddresses++;
                    string wifKey = BitcoinAddressGenerator.PrivateKeyToWIF(match.PrivateKey);
                    _foundAddressList.Add($"{match.Address},{wifKey}");
                }
            }

            Console.WriteLine("\nНажмите любую клавишу для продолжения...");
            Console.ReadKey(true);
        }

        private void SetupPatternsFromKnownKeys()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("===== НАСТРОЙКА ПАТТЕРНОВ ИЗ ИЗВЕСТНЫХ КЛЮЧЕЙ =====");
            Console.ResetColor();

            Console.WriteLine("Этот режим позволяет настроить шаблон приватного ключа на основе");
            Console.WriteLine("известных паттернов из реальных ключей.");

            Console.WriteLine("\nВыберите паттерн:");
            Console.WriteLine("1. Стандартный WIF-ключ из BIP (начинается с 5HpHagT65TZzG...)");
            Console.WriteLine("2. Паттерн сдвига битов (00000001, 00000010, 00000100...)");
            Console.WriteLine("3. Паттерн нулевых бит в начале");
            Console.WriteLine("4. Ввести свой паттерн");
            Console.WriteLine("5. Назад");

            Console.Write("\nВыберите опцию: ");
            string choice = Console.ReadLine() ?? "";

            switch (choice)
            {
                case "1":
                    SetupWIFPattern();
                    break;
                case "2":
                    SetupBitshiftPattern();
                    break;
                case "3":
                    SetupZeroBitsPattern();
                    break;
                case "4":
                    SetupCustomPattern();
                    break;
                case "5":
                    break;
                default:
                    Console.WriteLine("Некорректный выбор. Нажмите любую клавишу...");
                    Console.ReadKey(true);
                    break;
            }
        }

        private void SetupWIFPattern()
        {
            _currentPrivateKey.Reset();

            Console.WriteLine("Настройка шаблона WIF-ключа...");

            byte[] pattern = new byte[]
            {
                0x80, 0x76, 0x87,
                0x45, 0x23, 0x98,
                0x23, 0x45, 0x67,
                0x89, 0x01, 0x23,
                0x45, 0x67, 0x89,
                0x01, 0x23, 0x45,
                0x67, 0x89, 0x01,
                0x23, 0x45, 0x67,
                0x89, 0x01, 0x23,
                0x45, 0x67, 0x89
            };

            for (int i = 0; i < 30; i++)
            {
                _currentPrivateKey.SetCellValue(i, pattern[i]);
                _currentPrivateKey.SetCellFixed(i, true);
            }

            Console.WriteLine("Шаблон WIF-ключа успешно настроен!");
            Console.WriteLine("\nНажмите любую клавишу для продолжения...");
            Console.ReadKey(true);
        }

        private void SetupBitshiftPattern()
        {
            _currentPrivateKey.Reset();

            Console.WriteLine("Настройка шаблона сдвига битов...");

            byte[] pattern = new byte[] { 1, 2, 4, 8, 16, 32, 64, 128 };

            for (int i = 0; i < 8; i++)
            {
                _currentPrivateKey.SetCellValue(i, pattern[i]);
                _currentPrivateKey.SetCellFixed(i, true);
            }

            Console.WriteLine("Шаблон сдвига битов успешно настроен!");
            Console.WriteLine("\nНажмите любую клавишу для продолжения...");
            Console.ReadKey(true);
        }

        private void SetupZeroBitsPattern()
        {
            _currentPrivateKey.Reset();

            Console.WriteLine("Настройка шаблона нулевых бит в начале...");

            for (int i = 0; i < 16; i++)
            {
                _currentPrivateKey.SetCellValue(i, 0);
                _currentPrivateKey.SetCellFixed(i, true);
            }

            Console.WriteLine("Шаблон нулевых бит успешно настроен!");
            Console.WriteLine("\nНажмите любую клавишу для продолжения...");
            Console.ReadKey(true);
        }

        private void SetupCustomPattern()
        {
            Console.Clear();
            Console.WriteLine("===== НАСТРОЙКА СВОЕГО ПАТТЕРНА =====");

            Console.WriteLine("Введите последовательность команд 'set' и 'toggle' для настройки шаблона.");
            Console.WriteLine("Например: 'set 0 123' установит значение 123 для ячейки 0.");
            Console.WriteLine("Команда 'toggle 0' зафиксирует значение ячейки 0.");
            Console.WriteLine("Введите 'done' когда закончите настройку.");

            bool done = false;
            while (!done)
            {
                Console.Write("\nВведите команду: ");
                string cmd = Console.ReadLine() ?? "";

                if (cmd.ToLower() == "done")
                {
                    done = true;
                    continue;
                }

                string[] parts = cmd.Split(' ');

                if (parts.Length >= 3 && parts[0].ToLower() == "set")
                {
                    if (int.TryParse(parts[1], out int cellIndex) && byte.TryParse(parts[2], out byte value))
                    {
                        if (cellIndex >= 0 && cellIndex < 32)
                        {
                            _currentPrivateKey.SetCellValue(cellIndex, value);
                            Console.WriteLine($"Ячейка {cellIndex} установлена в значение {value}");
                        }
                        else
                        {
                            Console.WriteLine("Индекс ячейки должен быть от 0 до 31");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Некорректный формат команды 'set'");
                    }
                }
                else if (parts.Length >= 2 && parts[0].ToLower() == "toggle")
                {
                    if (int.TryParse(parts[1], out int cellIndex))
                    {
                        if (cellIndex >= 0 && cellIndex < 32)
                        {
                            _currentPrivateKey.ToggleCellFixed(cellIndex);
                            Console.WriteLine($"Статус фиксации ячейки {cellIndex} изменен на '{(_currentPrivateKey.IsCellFixed(cellIndex) ? "фиксирован" : "не фиксирован")}'");
                        }
                        else
                        {
                            Console.WriteLine("Индекс ячейки должен быть от 0 до 31");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Некорректный формат команды 'toggle'");
                    }
                }
                else
                {
                    Console.WriteLine("Неизвестная команда. Используйте 'set', 'toggle' или 'done'");
                }
            }

            Console.WriteLine("\nШаблон успешно настроен!");
            Console.WriteLine("\nНажмите любую клавишу для продолжения...");
            Console.ReadKey(true);
        }

        private void CreateDemoPubKeyDatabase(string filePath)
        {
            using FileStream fs = new FileStream(filePath, FileMode.Create);
            using BinaryWriter writer = new BinaryWriter(fs);

            Random rnd = new Random();
            for (int i = 0; i < 10; i++)
            {
                byte[] pubKey = new byte[33];
                rnd.NextBytes(pubKey);

                pubKey[0] = (byte)(rnd.Next(2) == 0 ? 0x02 : 0x03);

                writer.Write(pubKey);
                writer.Write((byte)rnd.Next(5));
            }
        }
    }
}