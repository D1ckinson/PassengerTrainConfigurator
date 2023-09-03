using System;
using System.Collections.Generic;
using System.Linq;

namespace Конфигуратор_пассажирских_поездов
{
    internal class Program
    {
        static void Main()
        {
            const char CreatePathCommand = '1';
            const char SkipTimeCommand = '2';
            const char ExitProgramCommand = '3';

            char userKey;
            bool isRunning = true;
            string input;

            Dictionary<char, string> commandsDescriptions = new Dictionary<char, string>
            {
                { CreatePathCommand, "Создать путь." },
                { SkipTimeCommand, "Пропустить время." },
                { ExitProgramCommand, "Выход." }
            };

            RailwayDepot railwayDepot = new RailwayDepot();

            while (isRunning)
            {
                Console.Clear();

                railwayDepot.DrawTrainsInfo();

                foreach (var item in commandsDescriptions)
                    Console.WriteLine($"{item.Key} - {item.Value}");

                Console.Write("Введите команду:");

                input = Console.ReadLine();

                if (input == "")
                    userKey = ' ';
                else
                    userKey = input[0];

                switch (userKey)
                {
                    case CreatePathCommand:
                        railwayDepot.CreatePath();
                        break;

                    case SkipTimeCommand:
                        railwayDepot.SkipTime();
                        break;

                    case ExitProgramCommand:
                        isRunning = false;
                        break;
                }
            }
        }
    }

    class RailwayDepot
    {
        private readonly int _shortDistance = 1;
        private readonly int _mediumDistance = 3;
        private readonly int _longDistance = 4;

        private readonly int _minPassengersQuantity = 70;
        private readonly int _maxPassengersQuantity = 300;

        private Random _random = new Random();

        private List<Train> _trains = new List<Train>();

        private readonly string[] _cities = { "Санкт-Петербург", "Москва", "Казань", "Екатеринбург", "Челябинск", "Тюмень" };

        public void CreatePath()
        {
            List<string> cities = _cities.ToList();

            bool isDirectionOfMovementToRight = false;
            string departureCity = cities[ChooseCity("отправления", cities)];

            cities.Remove(departureCity);

            string arrivalCity = cities[ChooseCity("прибытия", cities)];
            int stopsQuantity = Array.FindIndex(_cities, city => city == departureCity) - Array.FindIndex(_cities, city => city == arrivalCity);
            int passengersQuantity = _random.Next(_minPassengersQuantity, _maxPassengersQuantity);

            if (stopsQuantity < 0)
            {
                stopsQuantity *= -1;
                isDirectionOfMovementToRight = true;
            }

            List<Railcar> railcars = CreateTrainComposition(passengersQuantity, stopsQuantity);

            _trains.Add(new Train(departureCity, arrivalCity, _cities, railcars, isDirectionOfMovementToRight));
        }

        private List<Railcar> CreateTrainComposition(int passengersQuantity, int stopsQuantity)
        {
            if (stopsQuantity >= _longDistance)
                return FillTrainLongDistance(passengersQuantity, passengersQuantity / new CompartmentRailcar().Seats);

            if (stopsQuantity >= _mediumDistance)
                return FillTrainMediumDistance(passengersQuantity, passengersQuantity / new ReservedRailcar().Seats);

            if (stopsQuantity >= _shortDistance)
                return FillTrainShortDistance(passengersQuantity, passengersQuantity / new SittingRailcar().Seats);

            return null;
        }

        private List<Railcar> FillTrainLongDistance(int passengersQuantity, int railcarQuantity)
        {
            List<Railcar> trainComposition = new List<Railcar>();

            for (int i = 0; i < railcarQuantity; i++)
            {
                CompartmentRailcar railcar = new CompartmentRailcar();
                railcar.Passengers = railcar.Seats;

                trainComposition.Add(railcar);
            }

            CompartmentRailcar lastRailcar = new CompartmentRailcar();
            lastRailcar.Passengers = passengersQuantity - lastRailcar.Seats * railcarQuantity;

            trainComposition.Add(lastRailcar);

            return trainComposition;
        }

        private List<Railcar> FillTrainMediumDistance(int passengersQuantity, int railcarQuantity)
        {
            List<Railcar> trainComposition = new List<Railcar>();

            for (int i = 0; i < railcarQuantity; i++)
            {
                ReservedRailcar railcar = new ReservedRailcar();
                railcar.Passengers = railcar.Seats;

                trainComposition.Add(railcar);
            }

            ReservedRailcar lastRailcar = new ReservedRailcar();
            lastRailcar.Passengers = passengersQuantity - lastRailcar.Seats * railcarQuantity;

            trainComposition.Add(lastRailcar);

            return trainComposition;
        }

        private List<Railcar> FillTrainShortDistance(int passengersQuantity, int railcarQuantity)
        {
            List<Railcar> trainComposition = new List<Railcar>();

            for (int i = 0; i < railcarQuantity; i++)
            {
                SittingRailcar railcar = new SittingRailcar();
                railcar.Passengers = railcar.Seats;

                trainComposition.Add(railcar);
            }

            SittingRailcar lastRailcar = new SittingRailcar();
            lastRailcar.Passengers = passengersQuantity - lastRailcar.Seats * railcarQuantity;

            trainComposition.Add(lastRailcar);

            return trainComposition;
        }

        public void SkipTime()
        {
            List<Train> trainsToRemove = new List<Train>();

            _trains.ForEach(train =>
            {
                train.Move();

                if (train.HasTrainArrive())
                    trainsToRemove.Add(train);
            });

            trainsToRemove?.ForEach(train => _trains.Remove(train));
        }

        public void DrawTrainsInfo() => _trains.ForEach((train) =>
            Console.WriteLine($"Поезд {train.DepartureCity} - {train.ArrivalCity}" +
                $" сейчас находится в {train.CurrentCity}, следующая остановка {train.TellNextCity()}." +
                $"\nКоличество вагонов в поезде - {train.Size}. Количество пассажиров - {train.TellPassengersQuantity()}.\n"));

        private int ChooseCity(string info, List<string> cities)
        {
            Console.WriteLine($"\nВыберите город {info}:");

            for (int i = 0; i < cities.Count; i++)
                Console.WriteLine($"{i + 1} - {cities[i]}.");

            int userInput = 0;
            bool isIndexInListBorder = false;

            while (isIndexInListBorder == false)
            {
                userInput = ReadInt();

                userInput--;

                if (userInput >= 0 && userInput < cities.Count)
                    isIndexInListBorder = true;
                else
                    Console.WriteLine("Города с таким номером нет, попробуйте еще раз.");
            }

            return userInput;
        }

        private int ReadInt()
        {
            int number;

            while (int.TryParse(Console.ReadLine(), out number) == false)
                Console.Write("\nВведите число:");

            return number;
        }
    }

    class Train
    {
        private readonly List<Railcar> _railcars;

        private readonly bool _isDirectionOfMovementToRight;

        private readonly string[] _cities;

        public Train(string departureCity, string arrivalCity, string[] cities, List<Railcar> railcars, bool isDirectionOfMovementToRight)
        {
            _cities = cities;
            _railcars = railcars;
            _isDirectionOfMovementToRight = isDirectionOfMovementToRight;
            DepartureCity = departureCity;
            CurrentCity = departureCity;
            ArrivalCity = arrivalCity;
        }

        public string ArrivalCity { get; private set; }
        public string DepartureCity { get; private set; }
        public string CurrentCity { get; private set; }
        public int Size => _railcars.Count();

        public string TellNextCity()
        {
            int cityIndex = Array.IndexOf(_cities, CurrentCity);

            return _isDirectionOfMovementToRight ? _cities[cityIndex + 1] : _cities[cityIndex - 1];
        }

        public void Move() => CurrentCity = TellNextCity();

        public bool HasTrainArrive() => ArrivalCity == CurrentCity;

        public int TellPassengersQuantity()
        {
            int passengers = 0;

            _railcars.ForEach(railcar => passengers += railcar.Passengers);

            return passengers;
        }
    }

    abstract class Railcar
    {
        public Railcar(int seats)
        {
            Seats = seats;
        }

        public int Seats { get; private set; }
        public int Passengers { get; set; }
    }

    class SittingRailcar : Railcar
    {
        public SittingRailcar() : base(68) { }
    }

    class ReservedRailcar : Railcar
    {
        public ReservedRailcar() : base(54) { }
    }

    class CompartmentRailcar : Railcar
    {
        public CompartmentRailcar() : base(32) { }
    }
}
