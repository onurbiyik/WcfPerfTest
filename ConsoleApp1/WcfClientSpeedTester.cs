using BenchmarkDotNet.Attributes;
using DasBankReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;

namespace ConsoleApp1
{
    public class WcfClientSpeedTester
    {
        private Binding DasBinding
        {
            get
            {
                var binding = new BasicHttpBinding();
                //binding.OpenTimeout = new TimeSpan(0, 10, 0);
                //binding.CloseTimeout = new TimeSpan(0, 10, 0);
                binding.SendTimeout = new TimeSpan(10, 0, 0);
                binding.ReceiveTimeout = new TimeSpan(10, 0, 0);
                return binding;
            }
        }

        private EndpointAddress DasAddress
        {
            get
            {        
                // var dasEndpointAddress = "http://localhost:2937/DasOnurBankService.svc";
                var dasEndpointAddress = "http://onurbankwcf.azurewebsites.net/dasonurbankservice.svc";
                return new EndpointAddress(dasEndpointAddress);
            }
        }

        internal async Task WarmUp()
        {
            var factory = new ChannelFactory<IOnurBankServiceChannel>(DasBinding, DasAddress);

            var chan = factory.CreateChannel();
            chan.Open();
            var resp = await chan.GetDataAsync(7);            
            chan.Close();
            factory.Close();
        }

        private async Task CallIt(ChannelFactory<IOnurBankServiceChannel> factory)
        {
            var chan = factory.CreateChannel();
            try
            {
                var resp = await chan.GetDataAsync(7);
            }
            catch (CommunicationException e)
            {
                Console.Write("comm ex" + e.Message);
                chan.Abort();
            }
            catch (TimeoutException e)
            {
                Console.WriteLine("timeout exception" + e.Message);
                chan.Abort();
            }
            catch (Exception e)
            {
                Console.WriteLine("Generic exception " + e.Message);
                chan.Abort();
                throw;
            }
        }

        public async Task<string> NewChannelEveryTime(int iteration)
        {
            var factory = new ChannelFactory<IOnurBankServiceChannel>(DasBinding, DasAddress);

            var tasks = new Task[iteration];
            
            for (int i = 0; i < iteration; i++)
            {
                await Task.Delay(1);
                var t = CallIt(factory);
                tasks[i] = t;
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);
            factory.Close();
            return "";
        }

        private async Task CallIt(IOnurBankServiceChannel chan)
        {
            try
            {
                await chan.GetDataAsync(7);
            }
            catch (CommunicationException e)
            {
                Console.WriteLine("communication exception" + e.Message);
                chan.Abort();
            }
            catch (TimeoutException e)
            {
                Console.WriteLine("timeout exception" + e.Message);
                chan.Abort();
            }
            catch (Exception e)
            {
                Console.WriteLine("Generic exception " + e.Message);
                chan.Abort();
                // throw;
            }
        }


        public async Task<string> UseOpenChannel(int iteration)
        {
            var factory = new ChannelFactory<IOnurBankServiceChannel>(DasBinding, DasAddress);

            var chan = factory.CreateChannel();
            chan.Open();

            var tasks = new Task[iteration];
            
            for (int i = 0; i < iteration; i++)
            {                
                var t = CallIt(chan);
                tasks[i] = t;
            }
            await Task.WhenAll(tasks).ConfigureAwait(false);
            chan.Close();
            factory.Close();
            return "";
        }
    }
}
