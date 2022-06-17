using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    public class MusicFile
    {
        public string Path { get; set; }
        public string Name { get; set; }
        public string Album { get; set; }
        public string Length { get; set; }

        public override string ToString()
        {
            return Path;
        }
    }
}
