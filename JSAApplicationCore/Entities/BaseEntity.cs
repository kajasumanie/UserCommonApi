using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace JSAApplicationCore.Entities
{
     public class BaseEntity
    {
        public int Id { get; set; }
        public string CreatedBy { get; set; }
    }
}
