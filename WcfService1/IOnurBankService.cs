using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace OnurBankWCF
{
    [ServiceContract]
    public interface IOnurBankService
    {

        [OperationContract]
        Task<string> GetData(int value);


        [OperationContract]
        string GetDataButEmulateServerAbort(int value);
    }


  
}
