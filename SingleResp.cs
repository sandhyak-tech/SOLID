using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace SOLID
{
    class Journal
    {
        //Purpose of Journal class is to manage entries (Add/Remove) (Single responsibility)
        private readonly List<string> _entries = new List<string>();
        private static int count = 0;
        public int AddEntry(string entry)
        {
            _entries.Add($"{++count} : {entry}");
            return count;
        }
        public void RemoveEntry(int index)
        {
            _entries.RemoveAt(index);
        }
        public override string ToString()
        {
            return string.Join(Environment.NewLine, _entries);
        }

        //following methods breaks single responsibility principle
        public void Save(string filename, bool overwrite = false)
        {
            File.WriteAllText(filename, ToString());
        }
        public void Load(string filename)
        {

        }

        public void Load(Uri uri)
        {

        }
    }
    //So Create a separate class to Save
    class Persistence
    {
        public void SaveToFile(Journal journal, string filename, bool overwrite = false)
    {
      if (overwrite || !File.Exists(filename))
      {
        File.WriteAllText(filename, journal.ToString());
      }
    }
    }
    public partial class Demo
  {
    public static void RunSRP()
    {
      var j = new Journal();
      j.AddEntry("I cried today.");
      j.AddEntry("I ate a bug.");
      Console.WriteLine(j);

      var p = new Persistence();
      var filename = @"C:\Personal\OOD\SOLID\journal.txt";
      p.SaveToFile(j, filename);
      //Process.Start(filename);
    }
  }
}