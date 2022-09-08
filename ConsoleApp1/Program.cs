using BenchmarkDotNet.Running;
using ConsoleApp1;
using System.Diagnostics;

var st = new Stopwatch();
var t = new WcfClientSpeedTester();
var iterationCount = 1000;
var loopCount = 1;

await t.WarmUp();
Console.WriteLine($"Warmed up. Running {loopCount} times {iterationCount} concurrent calls...");



for (int i = 0; i < loopCount; i++)
{

    Console.Write($"Starting iteration {i}...");
    st.Restart();
    await t.NewChannelEveryTime(iterationCount);
    st.Stop();
    Console.WriteLine($"NewChannelEveryTime {iterationCount} calls took {st.ElapsedMilliseconds:N0}ms");

}



for (int i = 0; i < loopCount; i++)
{

    Console.Write($"Starting iteration {i}...");
    st.Restart();
    await t.UseOpenChannel(iterationCount);
    st.Stop();
    Console.WriteLine($"UseOpenChannel {iterationCount} calls took {st.ElapsedMilliseconds:N0}ms");

}





