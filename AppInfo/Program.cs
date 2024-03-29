﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;      // основная используемая сборка
using System.IO;               // Пространство имен используется для записи результатов в .txt файл


// Приложение AppInfo_х64 получает PID процесса и выводит информацию о процессе разрядности х64

namespace AppInfo_x64
{
    class Program
    {      
        static void Main(string[] args)
        {            
            Console.WindowWidth = 165;            
            Console.WriteLine("\t****** Добро пожаловать в приложение AppInfo_х64! ******\n");
            Console.WriteLine("Приложение AppInfo отображает различные данные о интересующем x64 процессе");
            do
            {
                Console.WriteLine("\nХотите ли Вы получить данные о процессе? [да / нет]");
                string answear = Console.ReadLine();
                if (string.Equals(answear, "нет", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Cпасибо за использование данной программы!");
                    break;
                }
                if (!answear.Equals("нет", StringComparison.OrdinalIgnoreCase)
                    && !answear.Equals("да", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Введены некорректные данные, пожалуйста повторите ввод");
                    continue; 
                }
                bool isCorrect = false;
                do
                {
                    Console.Write("Введите идентификатор(PID) процесса: ");
                    try
                    {
                        int PID = Convert.ToInt32(Console.ReadLine());
                        GetProccessInfo(PID);
                        isCorrect = true;
                    } 
                    catch
                    {
                        Console.WriteLine("Введены некорректные данные, пожалуйста повторите ввод");
                    }                    
                } while (isCorrect == false);
            } while (true); 
        }
        static void GetProccessInfo(int pid)
        {
            // Получаем путь текущего проекта
            string thisPath = Directory.GetCurrentDirectory();
            thisPath += "\\Modules.txt";
            try
            {
                // Создание файла Modules.txt в папке Debug
                using (StreamWriter sw = new StreamWriter(thisPath))
                {
                    Process proc = Process.GetProcessById(pid);
                    bool is32Bit = false;

                    Console.WriteLine($"\nПроцесс {proc.ProcessName} стартовал {proc.StartTime}");
                    sw.WriteLine($"\nПроцесс {proc.ProcessName} стартовал {proc.StartTime}");           // Запись в файл

                    Console.WriteLine($"Максимальный объем потребляемой памяти в момент времени: {(proc.PeakWorkingSet64 / 1024) / 1024} Мбайт");   // байт -> килобайт -> мегабайт
                    sw.WriteLine($"Максимальный объем потребляемой памяти в момент времени: {(proc.PeakWorkingSet64 / 1024) / 1024} Мбайт");
                                                                                              
                    // C помощью LINQ cортируем по убыванию исходя из потребляемых ресурсов модулей 
                    var modules = from ProcessModule module in proc.Modules
                                       orderby module.ModuleMemorySize descending
                                       select module;
                    
                    Console.WriteLine("Использует следующие ресурсы(модули):\n");
                    sw.WriteLine("Использует следующие ресурсы(модули):\n");

                    Console.WriteLine("№  Модуль" + "Путь".PadLeft(22) + "Описание".PadLeft(49) +"Память KB".PadLeft(46));
                    sw.WriteLine("№  Модуль" + "Путь".PadLeft(22) + "Описание".PadLeft(49) + "Память KB".PadLeft(46));
                    int count = 0;
                    int memory = 0;
                    foreach (ProcessModule pm in modules)
                    {
                        count++;
                        if (string.Equals(pm.FileVersionInfo.FileDescription, "win32 Emulation on NT64",
                            StringComparison.OrdinalIgnoreCase))
                        {
                            Console.WriteLine("\n********************************************************************************************************************");
                            Console.WriteLine($"Процесс {proc.ProcessName} является 32-ух разрядным," +
                                $" пожалуйста используйете 32-ух разрядную версию приложения AppInfo");
                            Console.WriteLine("********************************************************************************************************************");
                            is32Bit = true;
                            break;
                        }
                        Console.WriteLine($"{count}  {pm.ModuleName}".PadRight(27) + $"{pm.FileName}".PadRight(45) +
                                    $"{pm.FileVersionInfo.FileDescription}".PadRight(45) + $"{pm.ModuleMemorySize / 1024}"); // получим килобайты
                        sw.WriteLine($"{count}  {pm.ModuleName}".PadRight(27) + $"{pm.FileName}".PadRight(45) +
                                    $"{pm.FileVersionInfo.FileDescription}".PadRight(45) + $"{pm.ModuleMemorySize / 1024}");
                        memory += pm.ModuleMemorySize / 1024;   // подсчитываем количество сумарной потребляемой памяти всеми модулями
                        if (is32Bit)
                            return;
                    }
                    Console.WriteLine("\nВсего памяти используется суммарно всеми модулями: {0} KB", memory);
                    sw.WriteLine("\nВсего памяти используется суммарно всеми модулями: {0} KB", memory);
                }            
                Console.WriteLine();
            }
            catch(Exception ex)
            {
                Console.WriteLine("\n**************** АХТУНГ! ЕРРОРР!!!!11 *******************");
                Console.WriteLine("Описание ошибки: {0}", ex.Message);
                Console.WriteLine("Стек вызовов: {0}", ex.StackTrace);
                Console.WriteLine("Метод вызвавший исключение: {0}", ex.TargetSite);
                Console.WriteLine("Источник исключения: {0}", ex.GetBaseException().GetType().Name);
                Console.WriteLine("***********************************************************");
            }
        }        
    }
}
