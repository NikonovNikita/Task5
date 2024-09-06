using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Xml.Linq;

namespace PracticTask1
{
    class Program
    {
        private static readonly ISet<string> _aviableWorkingDaysOfWeekWithoutWeekends = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday" };

        private static readonly int[] _vacationSteps = [7, 14];

        private static readonly int _vacationCountToAssign = 28;

        private static Dictionary<string, List<DateTime>> GetVacationDictionary()
        {
            return new Dictionary<string, List<DateTime>>()
            {
                ["Иванов Иван Иванович"] = new List<DateTime>(),
                ["Петров Петр Петрович"] = new List<DateTime>(),
                ["Юлина Юлия Юлиановна"] = new List<DateTime>(),
                ["Сидоров Сидор Сидорович"] = new List<DateTime>(),
                ["Павлов Павел Павлович"] = new List<DateTime>(),
                ["Георгиев Георг Георгиевич"] = new List<DateTime>()
            };
        }

        private static void ShowVacations(in Dictionary<string, List<DateTime>> vacationDictionary)
        {
            foreach (var vacations in vacationDictionary)
            {
                Console.WriteLine("Дни отпуска " + vacations.Key + ": \n" + string.Join('\n', vacations.Value));
            }
        }

        private static bool CanCreateVacation(
            List<DateTime> vacationsOverallList,
            List<DateTime> employeeVacationList,
            DateTime startDate,
            DateTime endDate)
        {
            if (CheckOverallList(vacationsOverallList, startDate, endDate))
            {
                return CheckEmployeeVacationList(employeeVacationList, startDate, endDate);
            }

            return false;
        }

        private static bool IsBackwardDistanceBroken(DateTime startDate, DateTime endDate, DateTime element)
        {
            return element.AddMonths(-1) <= startDate && element.AddMonths(-1) <= endDate;
        }

        private static bool IsForwardDistanceBroken(DateTime startDate, DateTime endDate, DateTime element)
        {
            return element.AddMonths(1) >= startDate && element.AddMonths(1) >= endDate;
        }

        private static bool CheckEmployeeVacationList(List<DateTime> employeeVacationList, DateTime startDate, DateTime endDate)
        {
            return !employeeVacationList.Any(element => IsForwardDistanceBroken(startDate, endDate, element))
                                                                || !employeeVacationList.Any(element => IsBackwardDistanceBroken(startDate, endDate, element));
        }

        private static bool CheckOverallList(List<DateTime> vacationsOverallList, DateTime startDate, DateTime endDate)
        {
            return !vacationsOverallList.Any(element => IsAnIntersection(startDate, endDate, element)
                                                                       && IsAnIntersection(startDate, endDate, element.AddDays(3)));
        }

        private static bool IsAnIntersection(DateTime startDate, DateTime endDate, DateTime element)
        {
            return element >= startDate && element <= endDate;
        }

        private static void HandleAnEmployee(
            List<DateTime> employeeVacationList,
            List<DateTime> vacationsOverallList,
            int vacationCount)
        {
            var gen = new Random();

            var startPoint = new DateTime(DateTime.Now.Year, 1, 1);
            var endPoint = new DateTime(DateTime.Today.Year, 12, 31);

            while (vacationCount > 0)
            {
                var range = (endPoint - startPoint).Days;
                var startDate = startPoint.AddDays(gen.Next(range));

                if (_aviableWorkingDaysOfWeekWithoutWeekends.Contains(startDate.DayOfWeek.ToString()))
                {
                    var vacIndex = gen.Next(_vacationSteps.Length);
                    var endDate = new DateTime(DateTime.Now.Year, 12, 31);
                    var difference = 0;

                    if (_vacationSteps[vacIndex] == 7)
                    {
                        endDate = startDate.AddDays(7);
                        difference = _vacationSteps[vacIndex];
                    }

                    if (_vacationSteps[vacIndex] == 14)
                    {
                        endDate = startDate.AddDays(14);
                        difference = _vacationSteps[vacIndex];
                    }

                    if (vacationCount <= 7)
                    {
                        endDate = startDate.AddDays(7);
                        difference = _vacationSteps.Min();
                    }

                    if (CanCreateVacation(vacationsOverallList, employeeVacationList, startDate, endDate))
                    {
                        for (var tempDt = startDate; tempDt < endDate; tempDt = tempDt.AddDays(1))
                        {
                            vacationsOverallList.Add(tempDt);
                            employeeVacationList.Add(tempDt);
                        }

                        vacationCount -= difference;
                    }
                }
            }
        }

        static void Main(string[] args)
        {
            var vacationDictionary = GetVacationDictionary();
            var vacationsOverallList = new List<DateTime>();

            foreach (var vacationList in vacationDictionary)
            {
                HandleAnEmployee(vacationList.Value, vacationsOverallList, _vacationCountToAssign);
            }

            ShowVacations(vacationDictionary);
            Console.ReadKey();
        }
    }
}