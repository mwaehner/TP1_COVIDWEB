using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TP1_ARQWEB.Areas.Identity.Data;
using TP1_ARQWEB.Data;
using TP1_ARQWEB.Models;

namespace TP1_ARQWEB.Services
{
    public class Time
    {
        public static DateTime Now()
        {
            return DateTime.UtcNow.AddHours(-3);
        }

    }
}
