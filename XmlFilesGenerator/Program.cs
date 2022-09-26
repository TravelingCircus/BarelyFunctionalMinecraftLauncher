using CommonData.Models;
using TCPFileServer.DataAccess;

namespace XmlFilesGenerator
{
    internal class Program
    {
        static void Main()
        {
            Console.WriteLine("Enter file path:");
            string path = Console.ReadLine();

            Console.WriteLine("Enter xml file type (0 - Version, 1 - LaunchConfig):");
            XmlFile xmlFile = (XmlFile)Convert.ToInt32(Console.ReadLine());

            switch (xmlFile)
            {
                case XmlFile.Version:
                    CreateVersionXml(path);
                    break;
                
                case XmlFile.LaunchConfig:
                    CreateLaunchConfigXml(path);
                    break;
            }
        }

        private static void CreateVersionXml(string path)
        {
            Console.WriteLine("Enter major version:");
            int majorVersion = Convert.ToInt32(Console.ReadLine());
                    
            Console.WriteLine("Enter minor version:");
            int minorVersion = Convert.ToInt32(Console.ReadLine());
                    
            Console.WriteLine("Enter changelog:");
            string changeLog = Console.ReadLine();

            ConfigurationVersion version = new ConfigurationVersion(
                majorVersion, minorVersion, changeLog);
            
            using FileStream fileStream = new FileStream(path, FileMode.Create);

            DataSerializer.ConfigVersionToXml(version, fileStream);
        }
        
        private static void CreateLaunchConfigXml(string path)
        {
            Console.WriteLine("Enter vanilla version:");
            string vanillaVersion = Console.ReadLine();
                    
            Console.WriteLine("Enter forge version:");
            string forgeVersion = Console.ReadLine();
                    
            Console.WriteLine("Enter modsChecksum:");
            string modsChecksum = Console.ReadLine();

            LaunchConfiguration launchConfig = new LaunchConfiguration(vanillaVersion, forgeVersion, modsChecksum);
            
            using FileStream fileStream = new FileStream(path, FileMode.Create);

            DataSerializer.LaunchConfigToXml(launchConfig, fileStream);
        }
    }

    enum XmlFile
    {
        Version = 0,
        LaunchConfig = 1
    }
}