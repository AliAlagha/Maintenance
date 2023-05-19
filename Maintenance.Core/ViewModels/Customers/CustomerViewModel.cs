﻿using Maintenance.Core.Enums;

namespace Maintenance.Core.ViewModels
{
    public class CustomerViewModel : IBaseViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public CustomerRate? CustomerRate { get; set; }
        public string? Notes { get; set; }
        public string CreatedAt { get; set; }
    }
}
