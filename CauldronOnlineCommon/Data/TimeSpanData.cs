using System;

namespace CauldronOnlineCommon.Data
{
    [Serializable]
    public struct TimeSpanData
    {
        public int Days;
        public int Hours;
        public int Minutes;
        public int Seconds;
        public int Milliseconds;

        public TimeSpanData(TimeSpan timeSpan)
        {
            Days = timeSpan.Days;
            Hours = timeSpan.Hours;
            Minutes = timeSpan.Minutes;
            Seconds = timeSpan.Seconds;
            Milliseconds = timeSpan.Milliseconds;
        }

        public TimeSpan ToTimeSpan()
        {
            return new TimeSpan(Days, Hours, Minutes, Seconds, Milliseconds);
        }

        public override string ToString()
        {
            return $"{Days:N0)}:{Hours}:{Minutes}:{Seconds}:{Milliseconds}";
        }
    }
}