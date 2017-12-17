using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OopSoftwareMetrics
{
    class MetricsAnalizer
    {
        private Dictionary<string, ClassNode> _classes;

        private readonly ClassNode _root;

        private readonly Object _thisLock = new Object();

        private const string Pattern = @"class\s*(\w+)(?:<\s*\w+(?:\s*,\s*\w+)*>)?(?:\s*:\s*(\w+))?";

        private readonly Regex _regex = new Regex(Pattern);

        public MetricsAnalizer()
        {
            _root = new ClassNode(null);
            _classes = new Dictionary<string, ClassNode>();
        }

        public void GetModuleMetrics(string pathToModule)
        {
            var dirInfo = new DirectoryInfo(pathToModule);


            foreach (var file in Directory.GetFiles(dirInfo.FullName, "*.cs"))
                GetClassTree(file);

            foreach (var directory in Directory.GetDirectories(dirInfo.FullName))
                GetModuleMetrics(directory);

            //Parallel.ForEach(Directory.GetFiles(dirInfo.FullName, "*.cs"), GetClassTree);
            //Parallel.ForEach(Directory.GetDirectories(dirInfo.FullName), GetModuleMetrics);
        }

        private void GetClassTree(string pathToFile)
        {
            using (var streamReader = new StreamReader(pathToFile))
            {
                string file = streamReader.ReadToEnd();

                foreach (Match match in _regex.Matches(file))
                {
                    if (!String.IsNullOrEmpty(match.Groups[2].Value) && !match.Groups[2].Value.StartsWith("I"))
                    {
                        lock (_thisLock)
                        {
                            ClassNode classNode;
                            if ((classNode = _root.FindChild(match.Groups[2].Value)) != null)
                            {
                                if (classNode.ContainsChild(match.Groups[1].Value))
                                    continue;

                                classNode.AddChild(match.Groups[1].Value);
                            }
                            else if ((classNode = _root.FindChild(match.Groups[1].Value)) != null)
                            {
                                _root.AddChild(match.Groups[2].Value)
                                    .AddChild(classNode, match.Groups[1].Value);

                                _root.RemoveChild(match.Groups[1].Value);
                            }
                            else
                            {
                                _root.AddChild(match.Groups[2].Value).AddChild(match.Groups[1].Value);
                            }
                        }
                    }
                    else lock (_thisLock)
                    {
                        if (_root.FindChild(match.Groups[1].Value) == null)
                        {
                            _root.AddChild(match.Groups[1].Value);
                        }
                    }
                }
            }

        }
    }
}
