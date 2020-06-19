﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Model.Models
{
    public class Phone
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Company { get; set; }
        public decimal Price { get; set; }
    }
}
