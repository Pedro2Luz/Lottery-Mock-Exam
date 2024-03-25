using System;
using System.Collections.Generic;
using System.Linq;

namespace Lottery_ticket
{
    public class LotteryPanel
    {
        public const int MaxNumbers = 6;

        private const int MinNumber = 1;
        private const int MaxNumber = 50;
        private List<int> _numbers;

        public LotteryPanel()
        {
            _numbers = new List<int>();
        }

        public LotteryPanel(IEnumerable<int> numbers)
        {
            ValidateNumbers(numbers);
            _numbers = new List<int>(numbers);
        }

        private void ValidateNumbers(IEnumerable<int> numbers)
        {
            if (numbers == null)
            {
                throw new ArgumentNullException(nameof(numbers));
            }

            if (!numbers.All(n => MinNumber <= n && n <= MaxNumber))
            {
                throw new ArgumentOutOfRangeException(nameof(numbers), "Numbers have to be between 1 and 50");
            }

            if (numbers.Distinct().Count() != numbers.Count())
            {
                throw new ArgumentException("Numbers cannot contain duplicates");
            }

            if (numbers.Count() > MaxNumbers)
            {
                throw new ArgumentOutOfRangeException(nameof(numbers), $"Maximum of {MaxNumbers} numbers allowed per panel");
            }
        }

        public void SelectQuickPick()
        {
            _numbers = Enumerable.Range(MinNumber, MaxNumber).OrderBy(i => Guid.NewGuid()).Take(MaxNumbers).ToList();
        }

        public void SelectUserNumbers(IEnumerable<int> numbers)
        {
            ValidateNumbers(numbers);
            _numbers = new List<int>(numbers);
        }

        public bool HasWinningNumbers(IEnumerable<int> winningNumbers)
        {
            return _numbers.Any(n => winningNumbers.Contains(n));
        }
    }

    public class LotteryTicket
    {
        private const int MinPanels = 2;
        private const int MaxPanels = 8;
        private const int CostPerPanel = 2;
        private List<LotteryPanel> _panels;

        public LotteryTicket()
        {
            _panels = new List<LotteryPanel>();
        }

        public void AddPanel(LotteryPanel panel, bool quickPick = false)
        {
            if (_panels.Count >= MaxPanels)
            {
                throw new InvalidOperationException("Maximum number of panels reached");
            }

            if (quickPick)
            {
                panel.SelectQuickPick();
            }

            _panels.Add(panel);
        }

        public bool HasWinningPanel(IEnumerable<int> winningNumbers)
        {
            return _panels.Any(panel => panel.HasWinningNumbers(winningNumbers));
        }

        public int GetCost()
        {
            return _panels.Count * CostPerPanel;
        }
    }

    public static class RandomNumberGenerator
    {
        private const int MinNumber = 1;
        private const int MaxNumber = 50;

        public static List<int> GenerateRandomNumbers(int count = 6)
        {
            if (count <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Count must be positive");
            }

            var random = new Random();
            return Enumerable.Range(MinNumber, MaxNumber).OrderBy(i => random.Next()).Take(count).ToList();
        }
    }

    public static class UserInputSimulator
    {
        private const int MinNumber = 1;
        private const int MaxNumber = 50;

        public static List<int> SimulateUserNumbers(int panelNumber)
        {
            while (true)
            {
                Console.WriteLine($"Panel {panelNumber + 1} (Enter numbers separated by spaces or 'quick pick'): ");
                var input = Console.ReadLine()?.Trim()?.ToLower();
                if (input == "quick pick")
                {
                    return null;
                }
                try
                {
                    var numbers = input.Split(' ').Select(int.Parse).ToList();
                    if (numbers.Any(n => n < MinNumber || n > MaxNumber))
                    {
                        Console.WriteLine($"Invalid input. Numbers must be between {MinNumber} and {MaxNumber}.");
                        continue;
                    }

                    if (numbers.Count > LotteryPanel.MaxNumbers) // Access MaxNumbers through LotteryPanel
                    {
                        Console.WriteLine($"Maximum of {LotteryPanel.MaxNumbers} numbers allowed per panel.");
                        continue;
                    }

                    return numbers;
                }
                catch (FormatException)
                {
                    Console.WriteLine("Invalid input. Please enter numbers separated by spaces or 'quick pick'.");
                }
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to the Lottery ");

            //  Create  a lotterry ticket 
            var ticket = new LotteryTicket();

            // Add  panels to the ticket
            var panel1 = new LotteryPanel();
            var panel2 = new LotteryPanel();
            ticket.AddPanel(panel1, quickPick: true);
            ticket.AddPanel(panel2);

            // Simulation of  winning numbers
            var winningNumbers = RandomNumberGenerator.GenerateRandomNumbers();

            // Checking  if the ticket has a winning panel
            bool hasWinningPanel = ticket.HasWinningPanel(winningNumbers);
            if (hasWinningPanel)
            {
                Console.WriteLine("Congratulations! You have a winning panel.");
            }
            else
            {
                Console.WriteLine("Better luck next time! No winning panels found.");
            }

            //  ticket cost 
            int cost = ticket.GetCost();
            Console.WriteLine($"The cost of the ticket is: {cost} Euro.");
        }
    }
}
