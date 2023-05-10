﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Data.DbEntities
{
    public class ReturnHandReceipt : BaseEntity
    {
        public ReturnHandReceipt()
        {
            ReceiptItems = new List<ReceiptItem>();
        }

        public int Id { get; set; }
        public int HandReceiptId { get; set; }
        public HandReceipt HandReceipt { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public DateTime Date { get; set; }
        public List<ReceiptItem> ReceiptItems { get; set; }
    }
}