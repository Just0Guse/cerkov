using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace организация_церковных_мероприятий
{
    public partial class App : Application
    {
        public static DatabaseHelper.UserRole CurrentRole { get; set; }
    }
}
