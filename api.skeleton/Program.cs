using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.skeleton
{
    class Program
    {
        static void Main(string[] args)
        {
            Information skeletonInfo = new Information();
            skeletonInfo.WriteProjectContent(@"C:\Dummy\Dummy.sln", "Dummy.Web.API");
        }
    }
}
