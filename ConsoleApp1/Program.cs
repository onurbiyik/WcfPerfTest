using ConsoleApp1;
using System.Diagnostics;


ThreadPool.GetMinThreads(out int workerThreads, out int complThreads);
Console.WriteLine($"WT min: {workerThreads} CT min:{complThreads}");
workerThreads *= 40;
complThreads *= 40;
ThreadPool.SetMinThreads(workerThreads, complThreads);
ThreadPool.GetMinThreads(out workerThreads, out complThreads);
Console.WriteLine($"WT min: {workerThreads} CT min:{complThreads}");


ThreadPool.GetMaxThreads(out workerThreads, out complThreads);
Console.WriteLine($"WT max: {workerThreads} CT max:{complThreads}");
workerThreads *= 40;
complThreads *= 40;
ThreadPool.SetMaxThreads(workerThreads, complThreads);
ThreadPool.GetMaxThreads(out workerThreads, out complThreads);
Console.WriteLine($"WT max: {workerThreads} CT max:{complThreads}");



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





