using System;
using System.Collections.Generic;
using System.Data.Entity;
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
            // Log queries
            db.Database.Log = Console.WriteLine;

            // All vendors in CA, using link query syntax
            Console.WriteLine("\n\n\n*** All vendors in CA ***");
            List<Vendor> caVendors = 
                (from v in db.Vendors 
                 where v.VendorState == "CA" 
                 orderby v.VendorName 
                 select v).ToList();

            Console.WriteLine("Vendors in CA");
            foreach (Vendor currVendor in caVendors)
            {
                Console.WriteLine(currVendor.VendorName);
            }

            // return a single object from the db
            Console.WriteLine("\n\n\n*** Return a single object from the db ***");
            Vendor singleVendor = 
                (from vendor in db.Vendors 
                 where vendor.VendorName == "IBM" 
                 select vendor).SingleOrDefault();

            if(singleVendor != null)
            {
                Console.WriteLine(singleVendor.VendorName);
            }
            else
            {
                Console.WriteLine("Vendor not found");
            }

            // join, get all vendors and invoices
            // beware of ef lazy loading, https://docs.microsoft.com/en-us/ef/ef6/querying/related-data
            // join operator https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/ef/language-reference/method-based-query-syntax-examples-join-operators
            Console.WriteLine("\n\n\n*** Get all vendors and invoices ***");
            List<Vendor> vendorAndInvoices =
                (from v in db.Vendors
                 select v)
                        //.Include(nameof(Vendor.Invoices))
                        //.Include("Invoices");
                          .Include(v => v.Invoices)
                          .ToList();

            foreach (Vendor vendor in vendorAndInvoices)
            {
                Console.WriteLine(vendor.VendorName);
                foreach (Invoice inv in vendor.Invoices)
                {
                    Console.WriteLine("\t" + inv.InvoiceNumber);
                }
            }

            Console.WriteLine("\n\n\n*** Get all vendors and invoices using join ***");
            var vendorAndInvoices2 =
               (from v in db.Vendors
                join i in db.Invoices
                on v.VendorID equals i.VendorID
                select new 
                {
                    v.VendorName,           // using inferred names
                    Invoices = v.Invoices   // explicit names
                }).ToList();

            // Performs an inner join correctly, but display info incorrectly
            // don't want a vendor for each invoice
            foreach (var vendor in vendorAndInvoices2)
            {
                Console.WriteLine(vendor.VendorName);
                foreach (Invoice inv in vendor.Invoices)
                {
                    Console.WriteLine("\t" + inv.InvoiceNumber);
                }
            }


            // return object(s) but limit the columns (efficiently use bandwidth)
            Console.WriteLine("\n\n\n*** Get vendors and only location info ***");
            // SELECT VendorName AS Name, VendorCity AS City, ...
            List<VendorLoc> vendorLocations =
               (from v in db.Vendors
                select new VendorLoc
                {
                    Name = v.VendorName,
                    City = v.VendorCity,
                    State = v.VendorState
                }).ToList();

            foreach (VendorLoc venLocation in vendorLocations)
            {
                Console.WriteLine($"{venLocation.Name}; {venLocation.City}, {venLocation.State}.");
            }

            // get sum of invoice totals
            Console.WriteLine("\n\n\n*** Get sum of invoice totals ***");
            double totalInvoiceTotal =
               (double)(from inv in db.Invoices
                        select inv.InvoiceTotal).Sum();

            Console.WriteLine($"Invoice Totall: {totalInvoiceTotal}");

            Console.ReadKey();
        }
    }

    class VendorLoc
    {
        public string Name { get; set; }

        public string City { get; set; }

        public string State { get; set; }
    }
}
