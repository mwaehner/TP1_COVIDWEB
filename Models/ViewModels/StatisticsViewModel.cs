using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TP1_ARQWEB.Areas.Identity.Data;

namespace TP1_ARQWEB.Models.ViewModels
{



    public class KeyValuePair
    {
        public string Key { get; set; }
        public int Value { get; set; }
    }


    public class StatisticsViewModel
    {




        public StatisticsViewModel (int usersAmount, int locationsAmount,int checkinsAmount, int infectionReportsAmount, int negativeTestsAmount)
        {
            locations = new KeyValuePair { Key = "Ubicaciones", Value = locationsAmount };
            users = new KeyValuePair { Key = "Usuarios", Value = usersAmount };
            checkins = new KeyValuePair { Key = "Checkins", Value = checkinsAmount };
            infectionReports = new KeyValuePair { Key = "Reportes de contagio", Value = infectionReportsAmount };
            negativeTests = new KeyValuePair { Key = "Tests negativos", Value = negativeTestsAmount };


        }

        public Dictionary<String, int> mockData { get; set; }
        public KeyValuePair locations;
        public KeyValuePair users;
        public KeyValuePair checkins;
        public KeyValuePair infectionReports;
        public KeyValuePair negativeTests;


    }
}
