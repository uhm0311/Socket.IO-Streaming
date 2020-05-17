using System;
using System.Diagnostics;
using System.IO;

namespace StreamingServer
{
    class Program
    {
        private static readonly string npm = "npm";

        private static readonly string[] modules = new string[] { "express", "socket.io" };
        private static readonly string modulesPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).ToString() + @"\npm\node_modules";

        private static readonly string batchFilePath = @".\js\launch.bat";
        private static readonly string jsFilePath = @".\js\socket.io.server.js";

        private static readonly string masterClientPath = @".\Master Client\Master Client.exe";

        static void Main(string[] args)
        {
            bool hasModules = Directory.Exists(modulesPath);
            foreach (string module in modules)
            {
                if (!Directory.Exists(modulesPath + @"\" + module))
                    Process.Start(npm, "install " + module + " -g").WaitForExit();
            }

            if (!Directory.GetParent(batchFilePath).Exists)
                Directory.GetParent(batchFilePath).Create();
            File.WriteAllText(batchFilePath, Properties.Resources.launch);

            if (!Directory.GetParent(jsFilePath).Exists)
                Directory.GetParent(jsFilePath).Create();
            File.WriteAllText(jsFilePath, Properties.Resources.socket_io_server);

            ushort port = 9104;
            Console.Write("Input port number : ");
            if (!ushort.TryParse(Console.ReadLine(), out port))
                port = 0;

            string batchFileArgs = jsFilePath;
            string masterClientArgs = string.Empty;

            if (port > 0)
            {
                batchFileArgs += (" " + port);
                masterClientArgs = port.ToString();
            }

            Process.Start(batchFilePath, batchFileArgs);
            Process.Start(masterClientPath, masterClientArgs);
        }
    }
}
