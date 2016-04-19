using System;

namespace GuideHoundEPG.Common.Model.MXF {
    public class ScheduleEntry {
        public ProgramInfo Program { get; set; }
        public Channel Channel { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime StopTime { get; set; }
        public String Duration { 
            get {
                return DurationInSeconds.ToString();
            } 
        }
        public double DurationInSeconds { 
            get {
                return StopTime.Subtract(StartTime).TotalSeconds;
            } 
        }
        public double DurationInMinutes { 
            get {
                return StopTime.Subtract(StartTime).TotalMinutes;
            } 
        }
        public bool IsCC { get; set; }
        public int AudioFormat { get; set; }
        public bool IsRepeat { get; set; }
        public bool IsLive { get; set; }
        public bool IsLiveSports { get; set; }
        public bool IsTape { get; set; }
        public bool IsDelay { get; set; }
        public bool IsSubtitled { get; set; }
        public bool IsPremiere { get; set; }
        public bool IsFinale { get; set; }
        public bool IsInProgress { get; set; }
        public bool IsSap { get; set; }
        public bool IsBlackout { get; set; }
        public bool IsEnhanced { get; set; }
        public bool Is3d { get; set; }
        public bool IsLetterbox { get; set; }
        public bool IsHdtv { get; set; }
        public bool IsDvs { get; set; }
        public int Part { get; set; }
        public int Parts { get; set; }
    }
}