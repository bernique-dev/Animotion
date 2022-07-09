using System.Collections;
using UnityEngine;


public static class TimeParser {

    public static ParsedTime GetParsedTime(float time) {

        ParsedTime parsedTime = new ParsedTime(time);

        return parsedTime;
    }

    public static string GetParsedString(float time, TimeFormat format = TimeFormat.OPTIONAL_HOUR) {
        return GetParsedTime(time).ToString(format);
    }

    public class ParsedTime {
        public int hours;
        public int minutes;
        public int seconds;
        public int milliseconds;

        public ParsedTime(int h, int m, int s, int ms) {
            hours = h;
            minutes = m;
            seconds = s;
            milliseconds = ms;
        }

        public ParsedTime(float s) : this(Mathf.FloorToInt(s / 3600), Mathf.FloorToInt(s / 60), Mathf.FloorToInt(s % 60), Mathf.RoundToInt((s % 1) * 1000)) { }


        public string ToString(TimeFormat format) {
            string result = "";

            switch (format) {
                case TimeFormat.OPTIONAL_HOUR:
                    result = (hours > 0 ? (AddExtraZeros(hours, 1) + ":") : "") + AddExtraZeros(minutes, 1) + ":" + AddExtraZeros(seconds, 1) + ":" + AddExtraZeros(milliseconds, 2);
                    break;
                case TimeFormat.FORCED_HOUR:
                    result = AddExtraZeros(hours, 1) + ":" + AddExtraZeros(minutes, 1) + ":" + AddExtraZeros(seconds, 1) + ":" + AddExtraZeros(milliseconds, 2);
                    break;
                case TimeFormat.OPTIONAL_HOUR_OPTIONAL_EXTRA_ZERO_MINUTE_NO_MS:
                    result = (hours > 0 ? (AddExtraZeros(hours, 1) + ":") : "") + (hours > 0 ? AddExtraZeros(minutes, 1) : minutes) + ":" + AddExtraZeros(seconds, 1);
                    break;
            }

            return result;
        }

        private string AddExtraZeros(int value, int maxZeroNb) {
            string extraZeros = "";

            for (int pow = 1; pow <= maxZeroNb; pow++) {
                if (value / Mathf.Pow(10, pow) < 1) {
                    extraZeros += "0";
                } 
            }

            return extraZeros + value;
        }
    }

    public enum TimeFormat {
        OPTIONAL_HOUR, FORCED_HOUR, OPTIONAL_HOUR_OPTIONAL_EXTRA_ZERO_MINUTE_NO_MS
    }
}