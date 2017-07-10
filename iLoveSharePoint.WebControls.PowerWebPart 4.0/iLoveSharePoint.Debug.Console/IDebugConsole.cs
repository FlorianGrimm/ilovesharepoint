using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.Drawing;

namespace iLoveSharePoint.Debug
{
    [ServiceContract(Namespace = "http://iLoveSharePoint.com/debug/2009/04/07")]
    public interface IDebugConsole
    {
        [OperationContract(Name = "Write")]
        void Write(string text);

        [OperationContract(Name = "Write2")]
        void Write(string text, ConsoleColor foregoroundColor);

        [OperationContract(Name = "Write3")]
        void Write(string text, ConsoleColor foregoroundColor, ConsoleColor backgroundColor);

        [OperationContract]
        string ReadLine();

        [OperationContract]
        Size GetBufferSize();
    }
}
