using System;
using System.Collections.Generic;
using System.Text;

namespace MicroRabbit.Transfer.Data.Models
{
    public class TransferLog
    {
        public int Id { get; set; }
        public int AccountFrom { get; set; }
        public int ToAccount { get; set; }
        public decimal TransferAmount { get; set; }

    }
}
