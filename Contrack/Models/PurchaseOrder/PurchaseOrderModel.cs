using System;
using System.Collections.Generic;

namespace Contrack
{
    public class PurchaseOrderModel
    {
        public ContainerPODTO ContainerPO { get; set; } = new ContainerPODTO();
        public Result result { get; set; } = new Result();
    }
}