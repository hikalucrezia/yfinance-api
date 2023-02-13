# Usage
```
//Initialize the object
Yfp yfp = new Yfp();

//Fetch the date from yahoo finance api in the range of 1 month with the interval of 5 minutes between each data
List<YfpSymbol> symbols = yfp.GetSymbols("7203.T", DateRange._1mo, Interval._5m)
```
