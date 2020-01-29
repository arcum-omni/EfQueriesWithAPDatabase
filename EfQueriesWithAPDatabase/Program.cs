using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfQueriesWithAPDatabase
{
    class Program
    {
        static void Main(string[] args)
        {
            APContext db = new APContext();

            // All vendors in CA, using link query syntax
            List<Vendor> caVendors = (from v in db.Vendors where v.VendorState == "CA" select v).ToList();

            Console.WriteLine("Vendors in CA");
            foreach (Vendor currVendor in caVendors)
            {
                Console.WriteLine(currVendor.VendorName);
            }
        }
    }
}
