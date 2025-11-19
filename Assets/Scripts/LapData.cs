using System;

namespace F1DirtDrift
{
    /// <summary>
    /// Struct that stores timing data for a single lap
    /// </summary>
    [Serializable]
    public struct LapData
    {
        public int lapNumber;           // Which lap (1-3)
        public float lapTime;           // Time taken for this lap in seconds
        public float totalTimeElapsed;  // Cumulative time from race start
        public bool isCompleted;        // Whether this lap was finished

        public LapData(int lapNum, float time, float totalTime)
        {
            lapNumber = lapNum;
            lapTime = time;
            totalTimeElapsed = totalTime;
            isCompleted = true;
        }

        /// <summary>
        /// Formats lap time as MM:SS.mm
        /// </summary>
        public string GetFormattedLapTime()
        {
            int minutes = (int)(lapTime / 60f);
            float seconds = lapTime % 60f;
            return $"{minutes:00}:{seconds:00.00}";
        }

        /// <summary>
        /// Formats total time as MM:SS.mm
        /// </summary>
        public string GetFormattedTotalTime()
        {
            int minutes = (int)(totalTimeElapsed / 60f);
            float seconds = totalTimeElapsed % 60f;
            return $"{minutes:00}:{seconds:00.00}";
        }
    }

    /// <summary>
    /// Stores complete race results for one car
    /// </summary>
    [Serializable]
    public class RaceResult
    {
        public string carName;              // Name of the car/driver
        public LapData[] laps;              // Array of 3 lap data
        public float bestLapTime;           // Fastest single lap
        public float totalRaceTime;         // Sum of all 3 laps
        public int finalPosition;           // Race finish position (1st, 2nd, 3rd)
        public bool isPlayer;               // Is this the player's result?

        public RaceResult(string name, bool player)
        {
            carName = name;
            isPlayer = player;
            laps = new LapData[3];
            bestLapTime = float.MaxValue;
            totalRaceTime = 0f;
            finalPosition = 0;
        }

        /// <summary>
        /// Adds a completed lap to the results
        /// </summary>
        public void AddLap(int lapNumber, float lapTime, float totalTime)
        {
            if (lapNumber >= 1 && lapNumber <= 3)
            {
                laps[lapNumber - 1] = new LapData(lapNumber, lapTime, totalTime);
                
                // Update best lap if this one is faster
                if (lapTime < bestLapTime)
                {
                    bestLapTime = lapTime;
                }
                
                totalRaceTime = totalTime;
            }
        }

        /// <summary>
        /// Gets formatted best lap time
        /// </summary>
        public string GetFormattedBestLap()
        {
            if (bestLapTime == float.MaxValue) return "--:--.--";
            int minutes = (int)(bestLapTime / 60f);
            float seconds = bestLapTime % 60f;
            return $"{minutes:00}:{seconds:00.00}";
        }

        /// <summary>
        /// Gets formatted total race time
        /// </summary>
        public string GetFormattedTotalTime()
        {
            if (totalRaceTime == 0f) return "--:--.--";
            int minutes = (int)(totalRaceTime / 60f);
            float seconds = totalRaceTime % 60f;
            return $"{minutes:00}:{seconds:00.00}";
        }
    }
}
