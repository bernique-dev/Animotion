using System;
using UnityEngine;

public class FrameParser {

    public static ParsedFrame GetParsedFrame(float frames) {
        ParsedFrame parsedTime = new ParsedFrame(frames);
        return parsedTime;
    }

    public static string GetParsedString(float frames, FrameFormat format = FrameFormat.FORCED_MINUTE) {
        return GetParsedFrame(frames).ToString(format);
    }
    public static ParsedFrame GetParsedFrameFromTime(float time) {
        ParsedFrame parsedTime = new ParsedFrame(GetTotalFrames(time));
        return parsedTime;
    }

    public static string GetParsedStringFromTime(float time, FrameFormat format = FrameFormat.FORCED_MINUTE) {
        return GetParsedFrameFromTime(time).ToString(format);
    }

    public static float GetTotalFrames(float s) {
        return GetTotalFrames(TimeParser.GetParsedTime(s));
    }

    public static float GetTotalFrames(TimeParser.ParsedTime pt) {
        return pt.hours * 3600 * 60 + pt.minutes * 3600 + pt.seconds * 60 + (((float)pt.milliseconds) * 60/1000);
    }

    public class ParsedFrame {
        public int minutes;
        public int seconds;
        public int frames;

        public ParsedFrame(int m, int s, int f) {
            minutes = m;
            seconds = s;
            frames = f;
        }

        public ParsedFrame(float s) : this(Mathf.FloorToInt(s / 3600), Mathf.FloorToInt(s / 60), Mathf.RoundToInt((float)Math.Round(s, 2) % 60)) { }


        public string ToString(FrameFormat format) {
            string result = "";

            switch (format) {
                case FrameFormat.OPTIONAL_MINUTE_OPTIONAL_EXTRA_ZERO_SECOND:
                    result = (minutes > 0 ? minutes + ":" : "") + (minutes > 0 ? AddExtraZeros(seconds, 1) : seconds) + ":" + AddExtraZeros(frames, 1);
                    break;
                case FrameFormat.FORCED_MINUTE:
                    result = minutes + ":" + AddExtraZeros(seconds, 1) + ":" + AddExtraZeros(frames, 1);
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
    public enum FrameFormat {
        OPTIONAL_MINUTE_OPTIONAL_EXTRA_ZERO_SECOND, FORCED_MINUTE
    }
}