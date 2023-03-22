using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

/*
 
Код использует словарь `tables` для хранения информации о том, занят ли столик. Для синхронизации доступа к таблице используется объект `SemaphoreSlim`.

Метод `ReserveTable` занимает выбранный столик, если он свободен, и вызывает метод `NotifyClient` для отправки уведомления клиенту.

Метод `CancelReservation` освобождает выбранный столик, если он был забронирован.

Метод `NotifyClient` имитирует отправку уведомления с задержкой и выводит сообщение в консоль.

Метод `AutoCancelReservation` вызывается при запуске программы и каждые 20 секунд снимает бронь со всех забронированных столиков.

 */

class Program
{
    static Dictionary<int, bool> tables = new Dictionary<int, bool>(); // хранит информацию о занятых/свободных столиках
    static SemaphoreSlim semaphore = new SemaphoreSlim(1, 1); // используется для синхронизации доступа к таблице

    static void Main(string[] args)
    {
        // инициализируем столики как свободные
        for (int i = 1; i <= 10; i++)
        {
            tables[i] = false;
        }

        // запускаем бесконечный цикл для обработки команд пользователя
        while (true)
        {
            Console.WriteLine("Выберите действие: ");
            Console.WriteLine("1 - Забронировать столик");
            Console.WriteLine("2 - Снять бронь со столика");
            Console.WriteLine("3 - Выйти из программы");

            string input = Console.ReadLine();

            if (int.TryParse(input, out int action))
            {
                switch (action)
                {
                    case 1:
                        ReserveTable();
                        break;
                    case 2:
                        CancelReservation();
                        break;
                    case 3:
                        return;
                    default:
                        Console.WriteLine("Некорректный ввод");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Некорректный ввод");
            }
        }
    }

    static void ReserveTable()
    {
        Console.WriteLine("Введите номер столика (1-10): ");
        string input = Console.ReadLine();

        if (int.TryParse(input, out int tableNumber))
        {
            if (tableNumber < 1 || tableNumber > 10)
            {
                Console.WriteLine("Некорректный номер столика");
                return;
            }

            semaphore.Wait(); // ждем, пока станет доступна таблица
            if (!tables[tableNumber])
            {
                tables[tableNumber] = true; // занимаем столик
                Console.WriteLine($"Столик {tableNumber} успешно забронирован");
                NotifyClient(tableNumber); // отправляем уведомление клиенту
            }
            else
            {
                Console.WriteLine($"Столик {tableNumber} уже занят");
            }
            semaphore.Release(); // освобождаем таблицу
        }
        else
        {
            Console.WriteLine("Некорректный ввод");
        }
    }

    static void CancelReservation()
    {
        Console.WriteLine("Введите номер столика (1-10): ");
        string input = Console.ReadLine();

        if (int.TryParse(input, out int tableNumber))
        {
            if (tableNumber < 1 || tableNumber > 10)
            {
                Console.WriteLine("Некорректный номер столика");
                return;
            }

            semaphore.Wait(); // ждем, пока станет   доступна таблица

            if (tables[tableNumber])
            {
                tables[tableNumber] = false; // освобождаем столик
                Console.WriteLine($"Бронь на столик {tableNumber} успешно снята");
            }
            else
            {
                Console.WriteLine($"Столик {tableNumber} не забронирован");
            }
            semaphore.Release(); // освобождаем таблицу
        }
        else
        {
            Console.WriteLine("Некорректный ввод");
        }
    }

    static async void NotifyClient(int tableNumber)
    {
        // имитируем отправку уведомления с задержкой
        await Task.Delay(5000);

        Console.WriteLine($"\nУведомление: столик {tableNumber} забронирован");
    }

    static async void AutoCancelReservation()
    {
        while (true)
        {
            await Task.Delay(20000); // ждем 20 секунд

            semaphore.Wait(); // ждем, пока станет доступна таблица

            foreach (var table in tables)
            {
                if (table.Value)
                {
                    tables[table.Key] = false; // снимаем бронь со столика
                    Console.WriteLine($"Автоматическое снятие брони со столика {table.Key}");
                }
            }

            semaphore.Release(); // освобождаем таблицу
        }
    }
}