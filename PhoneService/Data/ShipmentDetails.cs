//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PhoneService.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class ShipmentDetails
    {
        public int ShipmentDetailId { get; set; }
        public int ShipmentId { get; set; }
        public int PartId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    
        public virtual Parts Parts { get; set; }
        public virtual Shipments Shipments { get; set; }
    }
}