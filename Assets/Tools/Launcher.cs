using System;
using System.IO;
using System.Diagnostics;
using System.Reflection;

class Program {
    static void Main(string[] args) {
        try {
            if (args.Length != 0) {
                string executable = args[2];
                Process.Start(executable);
            }
        }
        catch (Exception e) {
            Console.WriteLine(args[2]);
            Console.WriteLine(e.Message);
            Console.ReadLine();
        }
    }
}
