using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace OnurBankWCF
{
    [ServiceContract]
    public interface IOnurBankService
    {

        [OperationContract]
        string GetData(int value);


        [OperationContract]
        string GetDataButEmulateServerAbort(int value);
    }


  
}
