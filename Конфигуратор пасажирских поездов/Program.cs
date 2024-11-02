using System;
using System.Collections.Generic;
using System.Linq;

namespace PassengerTrainConfigurator
{
    public class Program
    {
        static void Main()
        {
            RailwayDepot railwayDepot = new RailwayDepot();

            railwayDepot.Work();
        }
    }

    class RailwayDepot
    {
        private List<Train> _trains = new List<Train>();

        private List<string> _cities = new List<string>
        {
            "Санкт-Петербург", "Тверь", "Москва",
            "Рязань", "Саранск", "Самара",
            "Уфа", "Челябинск", "Екатеринбург"
        };

        public void Work()
        {
            const string CreateTrainCommand = "1";
            const string ExitCommand = "2";

            bool isWork = true;

            while (isWork)
            {
                _trains.ForEach(train => train.WriteInfo());

                Console.WriteLine(
                        $"\nМеню:\n" +
                        $"{CreateTrainCommand} - Создать поезд\n" +
                        $"{ExitCommand} - Выход\n");

                switch (UserUtils.ReadString("Введите команду:"))
                {
                    case CreateTrainCommand:
                        CreateTrain();
                        break;

                    case ExitCommand:
                        isWork = false;
                        break;

                    default:
                        break;
                }

                Console.Clear();
            }
        }

        private void CreateTrain()
        {
            int passengersQuantity = RandomPassengers();

            Direction direction = CreateDirection();
            List<Railcar> railcars = CreateRailcars(passengersQuantity);

            _trains.Add(new Train(railcars, direction, passengersQuantity));
        }

        private int RandomPassengers()
        {
            int minPassengers = 100;
            int maxPassengers = 400;

            return UserUtils.GenerateRandomValue(minPassengers, maxPassengers);
        }

        private List<Railcar> CreateRailcars(int passengersQuantity)
        {
            List<Railcar> railcars = new List<Railcar>();

            while (passengersQuantity > 0)
            {
                Railcar railcar = new Railcar(RandomSeats());

                passengersQuantity -= railcar.Seats;
                railcars.Add(railcar);
            }

            return railcars;
        }

        private int RandomSeats()
        {
            int minSeats = 40;
            int maxSeats = 100;

            return UserUtils.GenerateRandomValue(minSeats, maxSeats);
        }

        private Direction CreateDirection()
        {
            string departureCity = ChoseCity(_cities);
            string arrivalCity = ChoseCity(_cities.Where(city => city != departureCity).ToList());

            return new Direction(departureCity, arrivalCity);
        }

        private string ChoseCity(List<string> cities)
        {
            int index;

            do
            {
                Console.WriteLine("Список городов:");

                for (int i = 0; i < cities.Count; i++)
                    Console.WriteLine($"{i + 1} - {cities[i]}");

                index = UserUtils.ReadInt("Выберите город отправления: ") - 1;

            } while (IsIndexInRange(index, cities, "Города с таким номером нет") == false);

            return _cities[index];
        }

        private bool IsIndexInRange(int index, List<string> enumerator, string errorText)
        {
            if (index < 0 || index >= enumerator.Count)
            {
                Console.WriteLine(errorText);

                return false;
            }

            return true;
        }
    }

    class Train
    {
        private List<Railcar> _railcars;
        private Direction _direction;
        private int _seatsQuantity;
        private int _passengersQuantity;

        public Train(List<Railcar> railcars, Direction direction, int passengersQuantity)
        {
            _railcars = railcars;
            _direction = direction;
            _passengersQuantity = passengersQuantity;
            _seatsQuantity = _railcars.Sum(railcar => railcar.Seats);
        }

        public void WriteInfo()
        {
            Console.WriteLine($"Поезд {_direction.DepartureCity} - {_direction.ArrivalCity} " +
                $"везет {_passengersQuantity} пассажиров и имеет {_railcars.Count} вагонов с {_seatsQuantity} сидений");
        }
    }

    class Railcar
    {
        public Railcar(int seats)
        {
            Seats = seats;
        }

        public int Seats { get; private set; }
    }
}

class Direction
{
    public Direction(string departureCity, string arrivalCity)
    {
        DepartureCity = departureCity;
        ArrivalCity = arrivalCity;
    }

    public string DepartureCity { get; private set; }
    public string ArrivalCity { get; private set; }
}

static class UserUtils
{
    private static Random s_random = new Random();

    public static int ReadInt(string text)
    {
        int number;

        while (int.TryParse(ReadString(text), out number) == false)
            Console.WriteLine("Некорректный ввод. Введите число.");

        return number;
    }

    public static string ReadString(string text)
    {
        Console.Write(text);

        return Console.ReadLine();
    }

    public static int GenerateRandomValue(int min, int max) =>
        s_random.Next(min, max);
}
