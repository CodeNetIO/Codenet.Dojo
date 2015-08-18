using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Codenet.Dojo.Contracts
{
    [ServiceContract]
    public interface IDojoService
    {
        [OperationContract]
        string ProcessSimple(string code, string tests);
    }
}
