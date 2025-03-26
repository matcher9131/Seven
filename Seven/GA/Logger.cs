using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seven.GA
{
    public class Logger
    {
        public void Log(string content)
        {
            string fileName = $"{DateTime.Now:yyyyMMddHHmmss}.log";
            File.WriteAllText(fileName, content);
        }
    }
}
