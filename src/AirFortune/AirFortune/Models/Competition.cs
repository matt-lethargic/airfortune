using System.Diagnostics;

namespace AirFortune.Models
{
    public class Competition
    {
        public CompetitionEntry[] Entries { get; }

        public CompetitionEntry? Winner { get; protected set; }

        public bool Running { get; protected set; }
        public int CurrentNumberIndex { get; protected set; }


        public event Func<Task>? OnStartAsync;
        public event Func<Task>? OnFinishAsync;
        public event Func<Task>? OnNumberChangedAsync;

        public Competition(CompetitionEntry[] entries)
        {
            Entries = entries;
        }

        public async Task RollTheBallAsync()
        {
            Winner = null;
            Running = true;

            if (OnStartAsync != null)
            {
                await OnStartAsync.Invoke();
            }

            var running = true;
            var random = new Random();
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var lengthOfSpin = new TimeSpan(0, 0, random.Next(5, 15));

            random = new Random();
            var speed = new TimeSpan(random.Next(30000, 40000));

            while (running)
            {
                CurrentNumberIndex += 1;

                if (CurrentNumberIndex > Entries.GetUpperBound(0))
                {
                    CurrentNumberIndex = 0;
                }
                if (OnNumberChangedAsync != null)
                {
                    await OnNumberChangedAsync.Invoke();
                }

                await Task.Delay(speed);

                if (stopwatch.Elapsed.TotalSeconds > lengthOfSpin.TotalSeconds - 5)
                {
                    random = new Random();
                    speed = new TimeSpan(random.Next(100000, 200000));
                }
                if (stopwatch.Elapsed.TotalSeconds > lengthOfSpin.TotalSeconds - 2)
                {
                    random = new Random();
                    speed = new TimeSpan(random.Next(500000, 700000));
                }
                if (stopwatch.Elapsed.TotalSeconds > lengthOfSpin.TotalSeconds)
                {
                    running = false;
                }
            }

            Winner = Entries[CurrentNumberIndex];

            Running = false;

            if (OnFinishAsync != null)
            {
                await OnFinishAsync.Invoke();
            }
        }

        public async Task Reset()
        {
            Winner = null;
            CurrentNumberIndex = -1;

            if (OnNumberChangedAsync != null)
            {
                await OnNumberChangedAsync.Invoke();
            }
        }
    }
}
