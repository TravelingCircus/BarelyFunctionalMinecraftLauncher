using Common.Models;
using TCPFileServer.DataAccess;

namespace XmlFilesGenerator;

internal class Program
{
    private static void Main()
    {
        Console.WriteLine("Enter file path:");
        string path = Console.ReadLine();

        Console.WriteLine("Enter xml file type (0 - Version, 1 - LaunchConfig):");
        XmlFile xmlFile = (XmlFile)Convert.ToInt32(Console.ReadLine());

        switch (xmlFile)
        {
            case XmlFile.LaunchConfig:
                CreateLaunchConfigXml(path);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
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

internal enum XmlFile
{
    LaunchConfig = 0
}