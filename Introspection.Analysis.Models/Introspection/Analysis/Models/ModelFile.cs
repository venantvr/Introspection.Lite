using System;
using System.Collections.Generic;
using System.IO;
using Mono.Cecil.Cil;

namespace Introspection.Analysis.Models.Introspection.Analysis.Models
{
    [Serializable]
    public class ModelFile
    {
        [NonSerialized] private readonly string[] _paths;

        public ModelFile(Document document)
        {
            var path = document.Url;

            var list = new List<string> { "" };

            if (File.Exists(path))
            {
                using (var streamReader = new StreamReader(path))
                {
                    while (!streamReader.EndOfStream)
                    {
                        list.Add(streamReader.ReadLine());
                    }

                    _paths = list.ToArray();
                }
            }
        }

        internal bool method_4(SequencePoint sequencePoint)
        {
            return (sequencePoint.StartLine != sequencePoint.EndLine || sequencePoint.EndColumn - sequencePoint.StartColumn != 1 || _paths.Length <= sequencePoint.StartLine
                ? 1
                : (_paths[sequencePoint.StartLine].Length <= sequencePoint.StartColumn - 1 ? 1 : 0)) == 0 && _paths[sequencePoint.StartLine].Substring(sequencePoint.StartColumn - 1, 1) == "{";
        }
    }
}