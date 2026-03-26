using System;
using System.Collections.Generic;
using System.Text;

namespace LiveAuction.Core.Entites.BaseEntity
{
    public interface IEntity
    {
        Guid Id { get; set; }
    }
}
