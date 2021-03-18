using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using ProtobufProto.Model;
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
        protected IActionResult StandResult(StandRespone standRespone)
        {
            return new StandResponeResult(standRespone);
        }
    }
}
