using System;
using System.Diagnostics;

namespace Tester
{
    public class TimeTester
    {
        private readonly Stopwatch _stopwatch;

        public TimeTester()
        {
            _stopwatch = new Stopwatch();
        }

        public TimeSpan TestSync(Action action)
        {
            _stopwatch.Start();
            action();
            _stopwatch.Stop();
            return _stopwatch.Elapsed;
        }
        
        public TimeSpan TestSync<T1, TResult>(Func<T1, TResult> action, T1 input1, out TResult output)
        {
            _stopwatch.Start();
            output = action(input1);
            _stopwatch.Stop();
            return _stopwatch.Elapsed;
        }
    }
}