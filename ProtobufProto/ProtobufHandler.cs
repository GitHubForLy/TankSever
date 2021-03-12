using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;
using ServerCommon.Protocol;
using ProtobufProto.Model;

namespace ProtobufProto
{
    public class ProtobufHandler : IProtocolHandler
    {
        public void DataHandle(byte[] data, IActionExecuter actionExecuter)
        {
            Request request = Request.Parser.ParseFrom(data);

            string ControllerName;
            string ActionName;

            ControllerName = request.Controller;
            ActionName = request.Action;

            /************子参数解析*****************/



            /***************************************/


            actionExecuter.ExecuteAction(ControllerName, ActionName);
        }
    }
}
