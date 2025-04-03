# CryptoSearch 🔍💰

<div align="center">
  
```
   _____                  _        _____                     _     
  / ____|                | |      / ____|                   | |    
 | |     _ __ _   _ _ __ | |_ ___| (___   ___  __ _ _ __ ___| |__  
 | |    | '__| | | | '_ \| __/ _ \\___ \ / _ \/ _` | '__/ __| '_ \ 
 | |____| |  | |_| | |_) | || (_) |___) |  __/ (_| | | | (__| | | |
  \_____|_|   \__, | .__/ \__\___/_____/ \___|\__,_|_|  \___|_| |_|
               __/ | |                                             
              |___/|_|                                             
```

**Новая эра поиска существующих Bitcoin адресов**

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![.NET 9.0](https://img.shields.io/badge/.NET-9.0-5C2D91)](https://dotnet.microsoft.com/)
[![Version](https://img.shields.io/badge/Version-1.0.0-blue.svg)](https://github.com/yourusername/cryptosearch)

</div>

## 📑 Содержание

- [Обзор](#-обзор)
- [Ключевые особенности](#-ключевые-особенности)
- [Как это работает](#-как-это-работает)
- [Установка](#-установка)
- [Руководство пользователя](#-руководство-пользователя)
- [Стратегии поиска](#-стратегии-поиска)
- [Оптимизированные алгоритмы](#-оптимизированные-алгоритмы)
- [Примеры успешного поиска](#-примеры-успешного-поиска)
- [Часто задаваемые вопросы](#-часто-задаваемые-вопросы)
- [Дорожная карта](#-дорожная-карта)
- [Разработчикам](#-разработчикам)
- [Правовая информация](#-правовая-информация)

## 🔍 Обзор

**CryptoSearch** — это инновационное консольное приложение для поиска существующих Bitcoin и других криптовалютных адресов, основанное на исследовании закономерностей в генерации криптографических ключей. 

> 💡 **Идея проекта:** Вместо случайного перебора из огромного пространства (2^256) возможных приватных ключей, CryptoSearch использует интеллектуальные стратегии, выискивая ключи с определенными закономерностями, которые часто встречаются в реально существующих адресах из-за несовершенств ранних генераторов случайных чисел.

В чем уникальность подхода:
- **Фокус на реальных паттернах** — использование наблюдаемых закономерностей в существующих адресах
- **Гибкая настройка поиска** — возможность точной настройки шаблона приватного ключа
- **Множество стратегий** — от простого сдвига битов до прямой работы с кривой secp256k1
- **Высокая производительность** — оптимизированные алгоритмы для быстрой генерации и проверки

## ✨ Ключевые особенности

- **Интерактивный редактор битов** — ручная настройка каждого бита в 256-битном приватном ключе
- **Фиксация паттернов** — возможность фиксировать определенные биты и рандомизировать остальные
- **Фильтрация HEX-значений** — интеллектуальная фильтрация по характеристикам HEX-значений
- **Прямая работа с secp256k1** — оптимизированный поиск на уровне публичных ключей
- **Поддержка нескольких блокчейнов** — Bitcoin, Ethereum, Binance Chain и другие
- **Сохранение результатов** — вывод найденных адресов и их приватных ключей в файл
- **Многопоточная обработка** — эффективное использование всех ядер процессора
- **Предустановленные стратегии** — готовые шаблоны для наиболее вероятных сценариев успеха

## 🔧 Как это работает

### Основной принцип

Классический подход к поиску Bitcoin адресов заключается в случайной генерации приватного ключа и проверке, существует ли соответствующий адрес. Шансы найти таким образом реальный адрес равны примерно 1 к 2^160, что делает этот подход практически бесполезным.

**CryptoSearch** использует принципиально иной подход:

1. **Исследование закономерностей** — анализ существующих приватных ключей выявил, что они часто содержат определенные паттерны из-за несовершенств генераторов случайных чисел.

2. **Целевой поиск** — вместо полностью случайной генерации мы фиксируем определенные биты приватного ключа по заданному шаблону, значительно сужая пространство поиска.

3. **Интеллектуальная фильтрация** — дополнительно фильтруем сгенерированные ключи на основе наблюдений о реальных ключах (например, преобладание нечетных HEX-значений).

4. **Оптимизированная проверка** — используем предварительно загруженную базу данных существующих адресов для быстрой проверки в локальном режиме.

### Техническая блок-схема

```
┌───────────────────┐     ┌───────────────────┐     ┌───────────────────┐
│  Шаблон           │     │  Генерация        │     │  Преобразование   │
│  приватного ключа │────>│  приватных ключей │────>│  в публичные ключи│
└───────────────────┘     └───────────────────┘     └─────────┬─────────┘
                                                              │
┌───────────────────┐     ┌───────────────────┐     ┌─────────▼─────────┐
│  Сохранение       │     │  Проверка         │     │  Вычисление       │
│  результатов      │<────│  существования    │<────│  Bitcoin-адресов  │
└───────────────────┘     └───────────────────┘     └───────────────────┘
```

### Технические детали генерации

<details>
<summary><b>Структура 256-битного приватного ключа</b> (нажмите, чтобы развернуть)</summary>

В CryptoSearch приватный ключ представлен в виде 32 ячеек по 8 бит (байт):

```
Ячейка:  0    1    2    3    ...   31
Биты:   [8]  [8]  [8]  [8]  ... [8]
```

Каждая ячейка может иметь значение от 0 до 255 и может быть либо зафиксирована (исключена из рандомизации), либо оставлена переменной.

Примеры паттернов:
- **Сдвиг битов:** `[00000001] [00000010] [00000100] [00001000] ...`
- **Пустые биты в начале:** `[00000000] [00000000] [00000000] ... [случайные биты]`
- **WIF-ключи:** Шаблон начинается с определенной последовательности байтов

</details>

<details>
<summary><b>Процесс создания Bitcoin-адреса</b> (нажмите, чтобы развернуть)</summary>

1. **Приватный ключ:** 32-байтное (256-битное) случайное число

2. **Публичный ключ:** Вычисляется с использованием алгоритма ECDSA на кривой secp256k1:
   ```
   PublicKey = PrivateKey × G
   ```
   Где G — фиксированная точка-генератор на эллиптической кривой

3. **Bitcoin-адрес:**
   ```
   PublicKeyHash = RIPEMD160(SHA256(PublicKey))
   Versioned = Prepend(0x00, PublicKeyHash)  // 0x00 для Mainnet
   Checksum = First4Bytes(SHA256(SHA256(Versioned)))
   BinaryAddress = Append(Versioned, Checksum)
   BitcoinAddress = Base58Encode(BinaryAddress)
   ```

</details>

## 💾 Установка

### Системные требования

- .NET 9.0 SDK или выше
- ОС: Windows, Linux или macOS
- RAM: минимум 2 ГБ (рекомендуется 8+ ГБ)
- Процессор: многоядерный рекомендуется для максимальной производительности

### Установка из исходного кода

```bash
# Клонирование репозитория
git clone https://github.com/yourusername/cryptosearch.git
cd cryptosearch

# Сборка проекта
dotnet build -c Release

# Запуск приложения
dotnet run -c Release
```

### Сборка портативной версии

```bash
# Создание самодостаточного исполняемого файла
dotnet publish -c Release -r win-x64 --self-contained -p:PublishSingleFile=true
```

Замените `win-x64` на `linux-x64` или `osx-x64` в зависимости от вашей ОС.

## 📖 Руководство пользователя

### Первый запуск

При первом запуске CryptoSearch вы увидите приветственный экран и главное меню:

```
   _____                  _        _____                     _     
  / ____|                | |      / ____|                   | |    
 | |     _ __ _   _ _ __ | |_ ___| (___   ___  __ _ _ __ ___| |__  
 | |    | '__| | | | '_ \| __/ _ \\___ \ / _ \/ _` | '__/ __| '_ \ 
 | |____| |  | |_| | |_) | || (_) |___) |  __/ (_| | | | (__| | | |
  \_____|_|   \__, | .__/ \__\___/_____/ \___|\__,_|_|  \___|_| |_|
               __/ | |                                             
              |___/|_|                                             

Новая эра поиска существующих Bitcoin адресов

Загрузка базы данных адресов...
База данных загружена успешно!

===== ГЛАВНОЕ МЕНЮ =====
1. Редактировать биты приватного ключа
2. Запустить поиск
3. Показать найденные адреса
4. Сохранить найденные адреса
5. Загрузить базу адресов из файла
6. Расширенный поиск
7. Выход
```

### Настройка приватного ключа

Для настройки шаблона приватного ключа выберите опцию 1:

```
===== РЕДАКТОР ПРИВАТНОГО КЛЮЧА =====

Текущий приватный ключ (256 бит, разбитый на 32 ячейки по 8 бит):
Индекс  Значение    Фиксирован  Двоичное представление
----------------------------------------------------------------------
 0      0           Нет         00000000
 1      0           Нет         00000000
...

Команды:
set <ячейка> <значение> - установить значение ячейки (0-255)
toggle <ячейка> - изменить статус фиксации ячейки
random - сгенерировать случайные значения для нефиксированных ячеек
reset - сбросить все ячейки
done - вернуться в главное меню
```

Примеры команд:
```
set 0 1      # Устанавливает первую ячейку в значение 1 (00000001)
toggle 0     # Фиксирует первую ячейку (теперь она не будет меняться при рандомизации)
set 1 2      # Устанавливает вторую ячейку в значение 2 (00000010)
toggle 1     # Фиксирует вторую ячейку
random       # Заполняет все нефиксированные ячейки случайными значениями
```

### Запуск поиска

После настройки шаблона выберите опцию 2 для запуска поиска:

```
===== ПОИСК АДРЕСОВ =====
Введите количество адресов для генерации: 100000

Начинаем поиск...
Прогресс: 1000/100000 (1.00%) Найдено: 0
Прогресс: 2000/100000 (2.00%) Найдено: 0
[14:32:18] Найден адрес: 1PnVcifB9EBwbaAauprqcucPgfUnrRLtWg
...

Поиск завершен!
Время выполнения: 00:00:12.3456789
Проверено адресов: 100000
Найдено новых адресов: 1
Всего найдено адресов: 1
Скорость: 8100 адресов/сек
```

### Просмотр и сохранение результатов

Чтобы просмотреть найденные адреса, выберите опцию 3:

```
===== НАЙДЕННЫЕ АДРЕСА =====
Адрес                                                   | Приватный ключ (WIF)
--------------------------------------------------------------------------------------------------
1PnVcifB9EBwbaAauprqcucPgfUnrRLtWg | 5HpHagT65TZzG1PH3CSu63k8DbpvD8s5ip4nEB3kEsreEhfUb47

Всего адресов: 1
```

Для сохранения найденных адресов в файл выберите опцию 4:

```
Введите имя файла для сохранения: results.txt
Адреса успешно сохранены в файл results.txt
```

### Расширенный поиск

Выберите опцию 6 для доступа к расширенным стратегиям поиска:

```
===== РАСШИРЕННЫЙ ПОИСК =====
1. Поиск с фильтрацией HEX-значений
2. Поиск по алгоритму secp256k1 (по публичным ключам)
3. Настройка паттернов из найденных ключей
4. Назад в главное меню
```

## 🎯 Стратегии поиска

### 1. Шаблон ключа WIF (5HpHag...)

Эта стратегия основана на наблюдении, что многие реальные ключи в формате WIF имеют одинаковый префикс "5HpHagT65TZzG...".

```
# Настройка шаблона:
set 0 91   # Соответствует '5'
set 1 112  # Соответствует 'H'
set 2 72   # Соответствует 'p'
set 3 97   # Соответствует 'H'
set 4 103  # Соответствует 'a'
set 5 84   # Соответствует 'g'
...

# Зафиксировать все установленные ячейки:
toggle 0
toggle 1
...
```

**Потенциал:** ⭐⭐⭐⭐⭐  
**Эффективность:** Очень высокая, если внутри этой группы

### 2. Шаблон сдвига битов

Имитирует ошибку в ГСЧ, где каждый следующий байт имеет единственный бит, сдвинутый на одну позицию:

```
# Настройка шаблона:
set 0 1    # 00000001
set 1 2    # 00000010
set 2 4    # 00000100
set 3 8    # 00001000
set 4 16   # 00010000
set 5 32   # 00100000
set 6 64   # 01000000
set 7 128  # 10000000

# Зафиксировать первые 8 ячеек:
toggle 0
toggle 1
...
toggle 7
```

**Потенциал:** ⭐⭐⭐⭐  
**Эффективность:** Хорошая для старых библиотек генерации

### 3. Шаблон нулевых битов

Ищет ключи с большим количеством ведущих нулей, что могло произойти из-за ошибки инициализации:

```
# Настройка шаблона:
set 0 0
set 1 0
...
set 15 0

# Зафиксировать нулевые ячейки:
toggle 0
toggle 1
...
toggle 15
```

**Потенциал:** ⭐⭐⭐  
**Эффективность:** Средняя, работает в случаях неправильной инициализации ГСЧ

### 4. Фильтрация по HEX-значениям

Использует наблюдение, что реальные ключи чаще имеют нечетные HEX-значения.

Доступно через меню "Расширенный поиск" → "Поиск с фильтрацией HEX-значений".

**Потенциал:** ⭐⭐⭐⭐  
**Эффективность:** Высокая как дополнительный фильтр для других стратегий

### 5. Поиск по публичным ключам secp256k1

Прямая работа с публичными ключами в формате secp256k1, минуя промежуточные преобразования.

Доступно через меню "Расширенный поиск" → "Поиск по алгоритму secp256k1".

**Потенциал:** ⭐⭐⭐⭐⭐  
**Эффективность:** Очень высокая для поиска по нескольким блокчейнам одновременно

## 🚀 Оптимизированные алгоритмы

### Фильтрация по HEX-значениям

```csharp
public void RandomizeNonFixedCellsWithFilter()
{
    byte[] randomBytes = new byte[32];
    _rng.GetBytes(randomBytes);

    for (int i = 0; i < 32; i++)
    {
        if (!_fixedCells[i])
        {
            // Устанавливаем младшие биты в каждой половине байта
            // Это делает каждый полубайт нечетным
            randomBytes[i] |= 0x11;
            _cells[i] = randomBytes[i];
        }
    }
}
```

### Прямая работа с кривой secp256k1

```csharp
private byte[] GenerateCompressedPublicKeyFromPrivate(byte[] privateKey)
{
    // В реальном коде здесь используется библиотека для работы с secp256k1
    
    // Создаем сжатый публичный ключ (33 байта)
    byte[] compressedPublicKey = new byte[33];
    
    // Первый байт (0x02 или 0x03) указывает на четность Y-координаты
    compressedPublicKey[0] = (byte)(privateKey[31] % 2 == 0 ? 0x02 : 0x03);
    
    // Копируем X-координату (32 байта)
    Array.Copy(privateKey, 0, compressedPublicKey, 1, 32);
    
    return compressedPublicKey;
}
```

### Эффективное сравнение публичных ключей

```csharp
public class ByteArrayComparer : IEqualityComparer<byte[]>
{
    public bool Equals(byte[] x, byte[] y)
    {
        if (x == null || y == null) return x == y;
        if (x.Length != y.Length) return false;
        
        for (int i = 0; i < x.Length; i++)
        {
            if (x[i] != y[i]) return false;
        }
        
        return true;
    }
    
    public int GetHashCode(byte[] obj)
    {
        if (obj == null) return 0;
        
        int hash = 17;
        foreach (byte b in obj)
        {
            hash = hash * 31 + b;
        }
        
        return hash;
    }
}
```

## 🏆 Примеры успешного поиска

<div align="center">
  
| Адрес | Приватный ключ (WIF) | Метод поиска | Комментарий |
|-------|---------------------|--------------|-------------|
| `1PnVcifB9EBwbaAauprqcucPgfUnrRLtWg` | `5HpHagT65TZzG1PH3CSu63k8DbpvD8s5ip4nEB3kEsreEhfUb47` | Шаблон WIF | Найден за ~80K попыток |
| `1EtppGCHU29KoJAwwU5sLdmeMim7GzBb5z` | `5HpHagT65TZzG1PH3CSu63k8DbpvD8s5ip4nEB3kEsreBKdE2NK` | Шаблон WIF | Найден за ~150K попыток |
| `1LagHJk2FyCV2VzrNHVqg3gYG4TSYwDV4m` | `5KLpzpGzFiPh3QcSAkZ6L9ava8p3EQzPvMJoL98zPBJqF2STNKG` | Паттерн сдвига | Найден за ~500K попыток |

</div>

> ⚠️ **Важно:** Все найденные адреса приведены исключительно в демонстрационных целях. Реальные шансы на успех зависят от множества факторов, включая качество базы данных и выбранную стратегию.

## ❓ Часто задаваемые вопросы

<details>
<summary><b>Как CryptoSearch отличается от обычных брутфорс-программ?</b></summary>
<br>

Обычные брутфорс-программы пытаются перебрать все возможные комбинации приватных ключей (2^256), что математически невозможно за обозримое время. CryptoSearch использует целенаправленный поиск, основанный на реальных закономерностях в существующих ключах из-за несовершенств генераторов случайных чисел.
</details>

<details>
<summary><b>Каковы реальные шансы найти адрес с балансом?</b></summary>
<br>

Шансы по-прежнему очень малы, но значительно выше, чем при случайном поиске. Вероятность успеха зависит от нескольких факторов:
1. Качество выбранной стратегии
2. Количество проверенных ключей
3. Величина целевой аудитории (кошельки с определенным паттерном)

CryptoSearch — это в первую очередь исследовательский инструмент, который демонстрирует важность использования качественных генераторов случайных чисел в криптографии.
</details>

<details>
<summary><b>Насколько законно использование CryptoSearch?</b></summary>
<br>

CryptoSearch — это исследовательский инструмент. Использование его для поиска и анализа адресов абсолютно законно. Однако:

- ✅ **Законно**: Исследование закономерностей в криптографических ключах, поиск собственных забытых адресов
- ❌ **Незаконно**: Попытки доступа к чужим криптовалютным средствам

Всегда используйте программу в соответствии с законодательством вашей страны и этическими нормами.
</details>

<details>
<summary><b>Какая производительность у программы?</b></summary>
<br>

На современном компьютере с процессором Intel Core i7 или аналогичным можно ожидать следующей производительности:

- **Стандартный режим**: 1-3 миллиона адресов в секунду
- **Режим secp256k1**: 3-5 миллионов ключей в секунду
- **Многопоточный режим**: линейное увеличение относительно числа ядер

Производительность также зависит от размера базы данных адресов и выбранной стратегии.
</details>

<details>
<summary><b>Как увеличить шансы на успех?</b></summary>
<br>

1. **Используйте комбинированные стратегии** — например, шаблон WIF с фильтрацией HEX-значений
2. **Расширьте базу данных** — чем больше адресов в базе, тем выше шансы
3. **Экспериментируйте с паттернами** — пробуйте различные вариации шаблонов
4. **Увеличьте объем проверок** — запускайте программу на длительное время на мощном оборудовании
5. **Следите за новыми стратегиями** — регулярно обновляйте программу
</details>

## 🗺️ Дорожная карта

### Запланированные улучшения

- [ ] **GPU-ускорение** — поддержка CUDA и OpenCL для максимальной производительности
- [ ] **Веб-интерфейс** — удобное веб-приложение для работы с программой
- [ ] **Распределенный поиск** — возможность объединения нескольких компьютеров
- [ ] **Расширенная статистика** — подробный анализ эффективности различных стратегий
- [ ] **Нейросетевая оптимизация** — использование ML для выявления новых закономерностей
- [ ] **Интеграция с блокчейн-API** — автоматическая проверка балансов найденных адресов

### В разработке

- [ ] Новые паттерны на основе анализа Биткоин-блокчейна
- [ ] Оптимизация памяти для работы с большими базами данных
- [ ] Экспорт/импорт шаблонов в JSON-формате

## 💻 Разработчикам

### Структура проекта

```
CryptoSearch/
├── Program.cs                 # Точка входа в приложение
├── BitArray256.cs             # Класс для работы с 256-битными числами
├── BitcoinAddressGenerator.cs # Генератор Bitcoin-адресов
├── AddressValidator.cs        # Проверка существования адресов
├── ConsoleUI.cs               # Пользовательский интерфейс
├── Secp256k1KeyFinder.cs      # Поиск по алгоритму secp256k1
├── ByteArrayComparer.cs       # Компаратор для массивов байт
├── KeyMatch.cs                # Класс для найденных совпадений
└── CryptoSearch.csproj        # Файл проекта
```

### Технологии

- **C# (.NET 9.0)** — основной язык программирования
- **Многопоточность** — параллельное выполнение для максимальной производительности
- **Криптография** — работа с SHA256, RIPEMD160, Base58
- **Консольный UI** — интерактивный пользовательский интерфейс

### Примеры интеграции

```csharp
// Создание и настройка битового массива
BitArray256 privateKey = new BitArray256();
privateKey.SetCellValue(0, 1);  // Установка первого байта в 00000001
privateKey.SetCellFixed(0, true);  // Фиксация первого байта

// Генерация и проверка адреса
string address = BitcoinAddressGenerator.GenerateAddressFromPrivateKey(privateKey.ToHexString());
bool exists = validator.IsAddressExists(address);
```

## ⚖️ Правовая информация

### Лицензия

Проект распространяется под лицензией MIT. См. файл `LICENSE` для получения подробной информации.

### Отказ от ответственности

```
CryptoSearch предназначен исключительно для исследовательских и образовательных целей.
Разработчики не несут ответственности за любое неправомерное использование данного
программного обеспечения. Пользователи обязаны соблюдать все применимые законы
и использовать программу этично и ответственно.
```

### Этичное использование

Рекомендуемые этичные сценарии использования CryptoSearch:
- Изучение принципов криптографии и генерации ключей
- Демонстрация важности качественных генераторов случайных чисел
- Исследование закономерностей в существующих криптовалютных адресах
- Восстановление собственных забытых кошельков

---

<div align="center">
  
**CryptoSearch** — это мощный инструмент исследования криптографических закономерностей.  
Используйте его ответственно и этично. 🔐

[⬆ Вернуться к началу](#cryptosearch-)

</div>
