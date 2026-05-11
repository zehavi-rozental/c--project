using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

  namespace DalApi;
  using System.Xml.Linq;

    static class DalConfig
    {
        internal static string s_dalName;
        internal static Dictionary<string, string> s_dalPackages;

        static DalConfig()
        {
            string configPath = Path.Combine(AppContext.BaseDirectory, "xml", "dal-config.xml");
            if (!File.Exists(configPath))
            {
                configPath = Path.Combine(Directory.GetCurrentDirectory(), "xml", "dal-config.xml");
            }

            if (!File.Exists(configPath))
            {
                var directoryInfo = new DirectoryInfo(AppContext.BaseDirectory);
                while (directoryInfo != null)
                {
                    string candidatePath = Path.Combine(directoryInfo.FullName, "xml", "dal-config.xml");
                    if (File.Exists(candidatePath))
                    {
                        configPath = candidatePath;
                        break;
                    }
                    directoryInfo = directoryInfo.Parent;
                }
            }

            if (!File.Exists(configPath))
            {
                throw new DalConfigException("dal-config.xml file is not found");
            }

            XElement dalConfig = XElement.Load(configPath);

            s_dalName =
               dalConfig.Element("dal")?.Value ?? throw new DalConfigException("<dal> element is missing");

            var packages = dalConfig.Element("dal-packages")?.Elements() ??
      throw new DalConfigException("<dal-packages> element is missing");
            s_dalPackages = packages.ToDictionary(p => "" + p.Name, p => p.Value);
        }
    }

    [Serializable]
    public class DalConfigException : Exception
    {
        public DalConfigException(string msg) : base(msg) { }
        public DalConfigException(string msg, Exception ex) : base(msg, ex) { }
    }

