﻿using DotOPDS.Utils;
using System;
using System.IO;
using System.Text;

namespace DotOPDS.Commands
{
    class FixtureCommand : ICommand
    {
        public int Run(SharedOptions options)
        {
            var opts = (FixtureOptions)options;
            if (!File.Exists(opts.Input))
            {
                Console.WriteLine("Error: Input file {0} not found.", opts.Input);
                return 1;
            }

            var parser = new InpxParser(opts.Input);
            parser.OnNewEntry += Parser_OnNewEntry;

            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine(@"using DotOPDS.Utils;
using System;
using System.Collections.Generic;

namespace DotOPDS.Tests
{
    class InpxDemoFixture
    {
        public static List<Book> Result = new List<Book>
        {");

            parser.Parse().Wait(); // perform in a sync way

            Console.WriteLine(@"        };
    }
}");

            return 0;
        }

        public void Dispose()
        {
            // do nothing
        }

        private void Parser_OnNewEntry(object sender, NewEntryEventArgs e)
        {
            Dump(e.Book);
        }

        private void Dump(Book e)
        {
            Func<string, string> strornull = (s) => s != null ? "\"" + s + "\"" : "null";
            Console.WriteLine("            new Book {");
            Console.WriteLine("                Authors = new Author[] {");
            foreach (var author in e.Authors)
                Console.WriteLine(
                    "                    new Author {{FirstName = {0}, MiddleName = {1}, LastName = {2}}},",
                    strornull(author.FirstName), strornull(author.MiddleName), strornull(author.LastName));
            Console.WriteLine("                },");

            Console.WriteLine("                Genres = new string[] {");
            foreach (var genre in e.Genres)
                Console.WriteLine("                    \"{0}\",", genre);
            Console.WriteLine("                },");

            Console.WriteLine("                Title = \"{0}\",", e.Title);
            Console.WriteLine("                Series = {0},", strornull(e.Series));
            Console.WriteLine("                SeriesNo = {0},", e.SeriesNo);
            Console.WriteLine("                File = \"{0}\",", e.File);
            Console.WriteLine("                Size = {0},", e.Size);
            Console.WriteLine("                LibId = {0},", e.LibId);
            Console.WriteLine("                Del = {0},", e.Del ? "true" : "false");
            Console.WriteLine("                Ext = \"{0}\",", e.Ext);
            Console.WriteLine("                Date = DateTime.Parse(\"{0}\"),", e.Date);
            Console.WriteLine("                Archive = \"{0}\",", e.Archive);

            Console.WriteLine("            },");
        }
    }
}
