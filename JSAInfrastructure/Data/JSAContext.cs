using JSAApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace JSAInfrastructure.Data
{
   public  class JSAContext : DbContext
    {
        //Add-Migration kajas -Project OVMInfrastructure
        //Update-Database
        //Remove-Migration
        public JSAContext(DbContextOptions<JSAContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }
}
