using System;

namespace AutoUpdate.Core
{
    public class Options
    {
        public IChecker Checker { get; set; }

        public TimeSpan Interval { get; set; } = TimeSpan.Zero;

        public Options(IChecker checker)
        { 
            Checker = checker;
        }

        public Options(IChecker checker, TimeSpan interval):this(checker)
        {
            Interval = interval;
        }

        public Options(IChecker checker, CheckType checkType):this(checker)
        {
            switch (checkType)
            {
                case CheckType.EveryMinute:
                    Interval = TimeSpan.FromMinutes(1);
                    break;
                case CheckType.EveryDay:
                    Interval = TimeSpan.FromDays(1);
                    break;
                case CheckType.Weekly:
                    Interval = TimeSpan.FromDays(7);
                    break;
            }
        }
    }
}
