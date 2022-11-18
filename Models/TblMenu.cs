using System;
using System.Collections.Generic;

namespace ASPPRODUCT.Models
{
    public partial class TblMenu
    {
        public string Id { get; set; } = null!;
        public string? Name { get; set; }
        public string? LinkName { get; set; }
    }
}
