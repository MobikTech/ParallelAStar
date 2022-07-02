using System;
using System.Diagnostics;

namespace TesterAdditionalLibrary
{
    public class TimeWatcher
    {
        private readonly Stopwatch _stopwatch;

        public TimeWatcher()
        {
            _stopwatch = new Stopwatch();
        }

        public long TestSync(Action action)
        {
            _stopwatch.Restart();
            action();
            _stopwatch.Stop();
            return _stopwatch.ElapsedMilliseconds;
        }
        
        public long TestSync<T1, TResult>(Func<T1, TResult> action, T1 input1, out TResult output)
        {
            _stopwatch.Restart();
            output = action(input1);
            _stopwatch.Stop();
            return _stopwatch.ElapsedMilliseconds;
        }
        
        public long TestSync<T1, T2, TResult>(Func<T1, T2, TResult> action, T1 input1, T2 input2, out TResult output)
        {
            _stopwatch.Restart();
            output = action(input1, input2);
            _stopwatch.Stop();
            return _stopwatch.ElapsedMilliseconds;
        }

        public long TestSync<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> action, T1 input1, T2 input2, T3 input3, out TResult output)
        {
            _stopwatch.Restart();
            output = action(input1, input2, input3);
            _stopwatch.Stop();
            return _stopwatch.ElapsedMilliseconds;
        }
        
        public long TestSync<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> action, T1 input1, T2 input2, T3 input3, T4 input4, out TResult output)
        {
            _stopwatch.Restart();
            output = action(input1, input2, input3, input4);
            _stopwatch.Stop();
            return _stopwatch.ElapsedMilliseconds;
        }
    }
}