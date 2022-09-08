using BenchmarkDotNet.Attributes;
using DasBankReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace ConsoleApp1
{
    [MemoryDiagnoser]
    public class WcfClientSpeedTester
    {
        // private const string dasEndpointAddress = "http://localhost:2937/DasOnurBankService.svc";
        private const string dasEndpointAddress = "http://onurbankwcf.azurewebsites.net/dasonurbankservice.svc";

        internal async Task WarmUp()
        {
            var binding = new BasicHttpBinding();
            var address = new EndpointAddress(dasEndpointAddress);
            var factory = new ChannelFactory<IOnurBankServiceChannel>(binding, address);

            var chan = factory.CreateChannel();
            chan.Open();
            await chan.GetDataAsync(7);
            chan.Close();
            factory.Close();
        }

        [Benchmark]
        public async Task<string> NewChannelEveryTime(int iteration)
        {
            var binding = new BasicHttpBinding();
            var address = new EndpointAddress(dasEndpointAddress);
            var factory = new ChannelFactory<IOnurBankServiceChannel>(binding, address);

            

            List<Task> tasks = new();
            TaskFactory tf = new();            

            for (int i = 0; i < iteration; i++)
            {                
                var t = await tf.StartNew(async () =>
                {
                    using var chan = factory.CreateChannel();                    
                    try
                    {
                        var resp = await chan.GetDataAsync(7);
                    }
                    catch (CommunicationException e)
                    {
                        Console.WriteLine("communication exception");
                        chan.Abort();
                    }
                    catch (TimeoutException e)
                    {
                        Console.WriteLine("timeout exception");
                        chan.Abort();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Generic exception " + e.Message);
                        chan.Abort();
                        throw;
                    }
                });
                tasks.Add(t);
            }
            await Task.WhenAll(tasks.ToArray());
            factory.Close();
            return "";
        }

 

        [Benchmark]
        public async Task<string> UseOpenChannel(int iteration)
        {
            var binding = new BasicHttpBinding();
            var address = new EndpointAddress(dasEndpointAddress);
            var factory = new ChannelFactory<IOnurBankServiceChannel>(binding, address);

            var chan = factory.CreateChannel();
            chan.Open();

            List<Task> tasks = new();
            TaskFactory tf = new();

            for (int i = 0; i < iteration; i++)
            {
                var t = await tf.StartNew(async () =>
                {
                    try
                    {
                        await chan.GetDataAsync(7);
                    }
                    catch (CommunicationException e)
                    {
                        Console.WriteLine("communication exception");
                        chan.Abort();
                    }
                    catch (TimeoutException e)
                    {
                        Console.WriteLine("timeout exception");
                        chan.Abort();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Generic exception " + e.Message);
                        chan.Abort();
                        throw;
                    }
                });
                tasks.Add(t);

                // tasks.Add(chan.GetDataAsync(7));

            }
            await Task.WhenAll(tasks);
            chan.Close();
            factory.Close();
            return "";
        }
    }
}
