﻿using Maintenance.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Data.DbEntities
{
    public class ReceiptItem : BaseEntity
    {
        public ReceiptItem()
        {
            MaintenanceRequestStatus = MaintenanceRequestStatus.New;
        }

        public int Id { get; set; }
        public int? HandReceiptId { get; set; }
        public HandReceipt HandReceipt { get; set; }
        public int? ReturnHandReceiptId { get; set; }
        public ReturnHandReceipt ReturnHandHandReceipt { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public string Item { get; set; }
        public string Company { get; set; }
        public string? Color { get; set; }
        public string? Description { get; set; }
        public double? SpecifiedCost { get; set; }
        public bool NotifyCustomerOfTheCost { get; set; }
        public double? CostNotifiedToTheCustomer { get; set; }
        public double? CostFrom { get; set; }
        public double? CostTo { get; set; }
        public bool Urgent { get; set; }
        public string ItemBarcode { get; set; }
        public DateTime? WarrantyExpiryDate { get; set; }
        public double? CollectedAmount { get; set; }
        public DateTime? CollectionDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public MaintenanceRequestStatus MaintenanceRequestStatus { get; set; }
        public string? ReasonForRefusingMaintenance { get; set; }
        public string? MaintenanceSuspensionReason { get; set; }
        public string? ReturnReason { get; set; }
        public ReceiptItemType ReceiptItemType { get; set; }
        public string TechnicianId { get; set; }
        public User Technician { get; set; }
    }
}