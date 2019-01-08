﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace TradingBot
{
    public class DataLoader
    {
        List<DataPoint> dataPoints;

        public DataLoader(string path)
        {
            // Create stopwatch to give estimate of data load time
            Stopwatch watch = new Stopwatch();
            watch.Start();

            // Load data from file
            string data = LoadData(path);
            // Deserialize data
            dataPoints = DeserializeJson(data);

            // Tell the user how long it took
            watch.Stop();
            Console.WriteLine("Loaded and deserialized " + 
                (dataPoints[dataPoints.Count - 1].openTime - dataPoints[0].openTime).TotalDays + " days of data in " + 
                watch.Elapsed.TotalSeconds.ToString() + " seconds.");
        }

        List<DataPoint> DeserializeJson(string data)
        {
            List<string[]> DataRaw = JsonConvert.DeserializeObject<List<string[]>>(data);

            List<DataPoint> output = new List<DataPoint>();

            for(int i = 0; i < DataRaw.Count; i++)
            //foreach(string[] Data in DataRaw)
            {
                string[] Data = DataRaw[i];
                DataPoint Point = new DataPoint();

                DateTime openTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                Point.openTime = openTime.AddMilliseconds(Convert.ToInt64(Data[0]));
                Point.open = Convert.ToDouble(Data[1]);
                Point.high = Convert.ToDouble(Data[2]);
                Point.low = Convert.ToDouble(Data[3]);
                Point.close = Convert.ToDouble(Data[4]);
                Point.volume = Convert.ToDouble(Data[5]);
                DateTime closeTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                Point.closeTime = closeTime.AddMilliseconds(Convert.ToInt64(Data[6]));
                Point.quoteVolume = Convert.ToDouble(Data[7]);
                Point.tradeCount = Convert.ToInt64(Data[8]);
                Point.buyBase = Convert.ToDouble(Data[9]);
                Point.buyQuote = Convert.ToDouble(Data[10]);

                output.Add(Point);
            }

            return output;
        }

        string LoadData(string path)
        {
            return System.IO.File.ReadAllText(path);
        }
    }

    public class DataPoint
    {
        public double open;
        public double high;
        public double low;
        public double close;
        public double volume;
        public double quoteVolume;
        public long tradeCount;
        public double buyBase;
        public double buyQuote;
        public DateTime openTime;
        public DateTime closeTime;
    }
}