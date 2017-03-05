using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Introspection.Analysis.Models.Introspection.Analysis.Models
{
    public class AssemblyResolver : IDisposable
    {
        /// <summary>
        ///     Exécute les tâches définies par l'application associées à la libération ou à la redéfinition des ressources non
        ///     managées.
        /// </summary>
        public void Dispose()
        {
        }

        public IEnumerable<AssemblyFileInfo> Resolve(string pathToAssembly)
        {
            var assemblies = new List<AssemblyFileInfo>
                             {
                                 new AssemblyFileInfo
                                 {
                                     Path = pathToAssembly,
                                     Name = Path.GetFileNameWithoutExtension(pathToAssembly),
                                     Version = FileVersionInfo.GetVersionInfo(pathToAssembly).FileVersion
                                 }
                             };

            var folder = Path.GetDirectoryName(pathToAssembly);

            if (folder != null)
            {
                var assembly = Assembly.LoadFile(pathToAssembly);

                assemblies.AddRange(from references in assembly.GetReferencedAssemblies()
                    select Path.Combine(folder, $"{references.Name}.dll")
                    into dll
                    where File.Exists(dll)
                    select new AssemblyFileInfo
                           {
                               Path = dll,
                               Name = Path.GetFileNameWithoutExtension(dll),
                               Version = FileVersionInfo.GetVersionInfo(dll).FileVersion
                           });
            }

            return assemblies;
        }
    }
}