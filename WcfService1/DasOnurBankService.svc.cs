using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OnurBankWCF
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class DasOnurBankService : IOnurBankService
    {
        public async Task<string> GetData(int value)
        {
            var r = new Random();
            await Task.Delay(r.Next(2000));
            return string.Format("You entered: {0}", value);
        }



        public string GetDataButEmulateServerAbort(int value)
        {
            OperationContext.Current.Channel.Abort();
            Task.Delay(100).Wait() ;
            return $"bla bla {value}";
        }


    }
}
