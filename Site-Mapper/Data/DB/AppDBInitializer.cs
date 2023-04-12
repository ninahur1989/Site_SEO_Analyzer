using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Site_Mapper.Data.DB
{
    static internal class AppDBInitializer
    {
        public static void Initialize()
        {
            using (var context = new SiteContext())
                context.Database.EnsureCreated();
        }
    }
}
