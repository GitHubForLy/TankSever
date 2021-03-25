using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using ProtobufProto.Model;
using Google.Protobuf;
using ServerCommon.Protocol;

namespace ProtobufProto
{

    public class StandResponeResult : IActionResult
    {
        private StandRespone standRespone;
        public StandResponeResult(StandRespone respone) => standRespone = respone;
        public Respone GetRespone()
        {
            return new Respone()
            {
                IsSuccess = standRespone.IsSuccess,
                Data = Any.Pack(DataTableToTable(standRespone.Data)),
                Message = standRespone.Message
            };
        }

        protected Table DataTableToTable(DataTable dataTable)
        {
            Table table = new Table();
            foreach (DataRow row in dataTable.Rows)
            {
                var newRow = new Row();

                foreach (DataColumn col in standRespone.Data.Columns)
                {
                    newRow.Cells.Add(new Row.Types.Cell()
                    {
                        Name = col.ColumnName,
                        Value = row[col.ColumnName].ToString()
                    });
                }
                table.Rows.Add(newRow);
            }
            return table;
        }
    }


    public class StandController:ControllerBase
    {
        /// <summary>
        /// 标准响应
        /// </summary>
        protected Respone StandResult(StandRespone standRespone)
        {
            return new StandResponeResult(standRespone).GetRespone();
        }

        protected void Broadcast(StandRespone respone)
        {
            var resp= new StandResponeResult(respone).GetRespone();
            Broadcast(resp);
        }

        /// <summary>
        /// 广播数据
        /// </summary>
        /// <param name="SubMessage">子消息</param>
        protected void Broadcast(IMessage SubMessage)
        {

            Respone res = new Respone
            {
                Controller = Context.Controller,
                Action = Context.Action,
                IsSuccess = false,
                Data = Any.Pack(SubMessage)
            };

            Task.Run(() =>
            {
                Context.NetServer.Broadcast(res.ToByteArray());
            });
        }
    }
}
