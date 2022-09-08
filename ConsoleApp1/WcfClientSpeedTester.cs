using BenchmarkDotNet.Attributes;
using DasBankReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace ConsoleApp1
{
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


        public async Task<string> NewChannelEveryTime(int iteration)
        {
            var binding = new BasicHttpBinding();
            var address = new EndpointAddress(dasEndpointAddress);
            var factory = new ChannelFactory<IOnurBankServiceChannel>(binding, address);

            List<Task> tasks = new();

            for (int i = 0; i < iteration; i++)
            {                
                async Task CallIt() { 
                    using var chan = factory.CreateChannel();                    
                    try
                    {
                        var resp = await chan.GetDataAsync(7);
                    }
                    catch (CommunicationException e)
                    {
                        Console.Write("comm ex" + e.ToString());
                        chan.Abort();
                    }
                    catch (TimeoutException e)
                    {
                        Console.WriteLine("timeout exception" + e.ToString() );
                        chan.Abort();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Generic exception " + e.ToString());
                        chan.Abort();
                        throw;
                    }
                }

                var t = CallIt();
                tasks.Add(t);
            }
            await Task.WhenAll(tasks.ToArray());
            factory.Close();
            return "";
        }


        public async Task<string> UseOpenChannel(int iteration)
        {
            var binding = new BasicHttpBinding();
            var address = new EndpointAddress(dasEndpointAddress);
            var factory = new ChannelFactory<IOnurBankServiceChannel>(binding, address);

            var chan = factory.CreateChannel();
            chan.Open();

            List<Task> tasks = new();

            for (int i = 0; i < iteration; i++)
            {
                async Task CallIt()
                {
                    try
                    {
                        await chan.GetDataAsync(7);
                    }
                    catch (CommunicationException e)
                    {
                        Console.WriteLine("communication exception" + e.ToString());
                        chan.Abort();
                    }
                    catch (TimeoutException e)
                    {
                        Console.WriteLine("timeout exception" + e.ToString());
                        chan.Abort();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Generic exception " + e.ToString());
                        chan.Abort();
                        throw;
                    }
                }
                var t = CallIt();
                tasks.Add(t);

            }
            await Task.WhenAll(tasks);
            chan.Close();
            factory.Close();
            return "";
        }
    }
}
