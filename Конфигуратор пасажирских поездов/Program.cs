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

                userKey = Console.ReadLine()[0];

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

                    default:
                        Console.WriteLine("Такой команды я не знаю, попробуйте еще раз.");
                        break;
                }
            }
        }
    }

    class RailwayDepot
    {
        private readonly int _seatsQuantityInSittingRailcar = 68;
        private readonly int _seatsQuantityInReservedRailcar = 54;
        private readonly int _seatsQuantityInCompartmentRailcar = 32;

        private readonly int _stopsQuantityForSittingRailcar = 1;
        private readonly int _stopsQuantityForReservedRailcar = 3;
        private readonly int _stopsQuantityForCompartmentRailcar = 4;

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

            if (stopsQuantity < 0)
            {
                stopsQuantity *= -1;
                isDirectionOfMovementToRight = true;
            }

            int passengersQuantity = _random.Next(_minPassengersQuantity, _maxPassengersQuantity);

            List<Railcar> railcars = CreateTrainComposition(passengersQuantity, stopsQuantity);

            _trains.Add(new Train(departureCity, arrivalCity, _cities, railcars, isDirectionOfMovementToRight));
        }

        private List<Railcar> CreateTrainComposition(int passengersQuantity, int stopsQuantity)
        {
            if (stopsQuantity >= _stopsQuantityForCompartmentRailcar)
                return FillTrainComposition(passengersQuantity, new CompartmentRailcar(_seatsQuantityInCompartmentRailcar));

            if (stopsQuantity >= _stopsQuantityForReservedRailcar)
                return FillTrainComposition(passengersQuantity, new ReservedRailcar(_seatsQuantityInReservedRailcar));

            if (stopsQuantity >= _stopsQuantityForSittingRailcar)
                return FillTrainComposition(passengersQuantity, new SittingRailcar(_seatsQuantityInSittingRailcar));

            return null;
        }

        private List<Railcar> FillTrainComposition(int passengersQuantity, Railcar railcar)
        {
            List<Railcar> trainComposition = new List<Railcar>();

            while (passengersQuantity > 0)
            {
                trainComposition.Add(railcar);

                passengersQuantity -= railcar.SeatingAreas;
            }

            return trainComposition;
        }

        public void SkipTime()
        {
            List<Train> trainsToRemove = new List<Train>();

            foreach (Train train in _trains)
            {
                train.Move();

                if (train.HasTrainArrive())
                    trainsToRemove.Add(train);
            }

            if (trainsToRemove == null)
                return;

            trainsToRemove.ForEach(train => _trains.Remove(train));
        }

        public void DrawTrainsInfo() => _trains.ForEach((train) =>
            Console.WriteLine($"Поезд {train.DepartureCity} - {train.ArrivalCity}" +
                $" сейчас находится в {train.CurrentCity}, следующая остановка {train.TellNextCity()}"));

        private int ChooseCity(string info, List<string> cities)
        {
            Console.WriteLine($"\nВыберите город {info}:");

            for (int i = 0; i < cities.Count; i++)
                Console.WriteLine($"{i + 1} - {cities[i]}.");

            bool isIndexInListBorder = true;

            int userInput = 0;

            while (isIndexInListBorder)
            {
                userInput = ReadInt();

                userInput--;

                if (userInput >= 0 && userInput < cities.Count)
                    isIndexInListBorder = false;
                else
                    Console.WriteLine("Города с таким номером нет, попробуйте еще раз.");
            }

            return userInput;
        }

        private int ReadInt()
        {
            int number;

            Console.Write("\nВведите число:");

            string userInput = Console.ReadLine();

            while (int.TryParse(userInput, out number) == false)
            {
                Console.Write("\nВведите число:");

                userInput = Console.ReadLine();
            }

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

        public string TellNextCity()
        {
            int cityIndex = Array.IndexOf(_cities, CurrentCity);

            if (_isDirectionOfMovementToRight)
                return _cities[cityIndex + 1];

            return _cities[cityIndex - 1];
        }

        public void Move()
        {
            CurrentCity = TellNextCity();
        }

        public bool HasTrainArrive()
        {
            if (ArrivalCity == CurrentCity)
                return true;

            return false;
        }
    }

    abstract class Railcar
    {
        public readonly int SeatingAreas;

        public Railcar(int seatingAreas)
        {
            SeatingAreas = seatingAreas;
        }
    }

    class SittingRailcar : Railcar
    {
        public SittingRailcar(int seatingAreas) : base(seatingAreas) { }
    }

    class ReservedRailcar : Railcar
    {
        public ReservedRailcar(int seatingAreas) : base(seatingAreas) { }
    }

    class CompartmentRailcar : Railcar
    {
        public CompartmentRailcar(int seatingAreas) : base(seatingAreas) { }
    }
}
