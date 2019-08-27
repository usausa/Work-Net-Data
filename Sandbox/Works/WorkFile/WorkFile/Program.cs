using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using WorkFile.Dao;
using WorkFile.Dao.Entry;
using WorkFile.Dao.Master;

namespace WorkFile
{
    class Program
    {
        static void Main(string[] args)
        {
            var loader = new FileLoader(args[0], args[1], args[2]);
            Test(loader, typeof(IRootDao).GetMethod(nameof(IRootDao.Execute)));
            Test(loader, typeof(IExecuteDao).GetMethod(nameof(IExecuteDao.Query)));
            Test(loader, typeof(IOtherDao).GetMethod(nameof(IOtherDao.Execute)));
            Test(loader, typeof(IGenericDao).GetMethod(nameof(IGenericDao.Execute)));
            Test(loader, typeof(IEntryDao).GetMethod(nameof(IEntryDao.QueryList)));
            Test(loader, typeof(Outer.IInnerDao).GetMethod(nameof(Outer.IInnerDao.Execute)));
        }

        static void Test(FileLoader loader, MethodInfo mi)
        {
            Debug.WriteLine($"{mi.DeclaringType}.{mi.Name} : {loader.ReadFile(mi)}");
        }
    }

    public class FileLoader
    {
        private readonly string rootDirectory;

        private readonly string rootNamespace;

        private readonly string subDirectory;

        public FileLoader(string rootDirectory, string rootNamespace, string subDirectory)
        {
            this.rootDirectory = rootDirectory;
            this.rootNamespace = rootNamespace;
            this.subDirectory = subDirectory;
        }

        public string ReadFile(MethodInfo method)
        {
            var type = method.DeclaringType;
            var ns = type.Namespace;

            // Dir
            var dir = rootDirectory;
            if (ns.StartsWith(rootNamespace + "."))
            {
                dir = Path.Combine(rootDirectory, ns.Substring(rootNamespace.Length + 1).Replace('.', Path.DirectorySeparatorChar));
            }
            else if (ns != rootNamespace)
            {
                return null;
            }

            if (!String.IsNullOrEmpty(subDirectory))
            {
                dir = Path.Combine(dir, subDirectory);
            }

            // File
            var index = type.FullName.LastIndexOf('.');
            var name = index >= 0 ? type.FullName.Substring(index + 1) : type.FullName;
            var filename = $"{name.Replace('+', '.')}.{method.Name}.sql";
            var path = Path.Combine(dir, filename);
            if (!File.Exists(path))
            {
                return null;
            }

            // dummy
            return filename;
        }
    }
}
