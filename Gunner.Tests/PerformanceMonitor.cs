using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Gunner.Tests
{

    public class Measurement
    {
        public double RequestsPerSecond { get; set; }
    }


    public class PerformanceMonitor
    {
        //private List<Measurement> _Measurements;
        private PerformanceCounter _requestsPerSecond;

        public PerformanceMonitor()
        {
            //_Measurements = new List<Measurement>();
            var _requestsPerSecond = new PerformanceCounter("ASP.NET Applications", "Requests/Sec", "__Total__");
            //var _executionTime  = new PerformanceCounter("ASP.NET Applications", "Request Execution Time", "__Total__");
            //_rex = new PerformanceCounter("ASP.NET Applications", "Requests Executing", "__Total__");
            //_cpu = new PerformanceCounter("Processor", "% Processor Time", "_Total")
            //_counters.Add(_executionTime );
        }

        public float Read_RequestsPersecond()
        {
            return _requestsPerSecond.NextValue();
        }

        //// start collecting requests per second
        //// need a way to update a UI with the current measurement, an IObservable
        //public static Task StartMonitoring(int takeMeasurementEveryMs)
        //{
        //    var task = Task.Run(() =>
        //        {
        //            var 
        //        });
        //}

        //public void Stop()
        //{
            
        //}

        //public List<PerformanceCounter> GetCounters()
        //{
            
        //}
    }
}
