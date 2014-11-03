<Query Kind="Statements" />

var pc = new PerformanceCounterCategory("Web Service").GetCounters("_Total")
	.Where (n => n.CounterName.ToLower().Contains("total"))
	.Dump();