using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Api.Entities
{
    //ทำให้ Id Role เป็น int
    public class Role : IdentityRole<int>
    {
        
    }
}