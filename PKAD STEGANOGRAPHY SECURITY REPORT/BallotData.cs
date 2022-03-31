using System;
using System.Collections.Generic;
using System.Text;

namespace PKAD_STEGANOGRAPHY_SECURITY_REPORT
{
    public class BallotData
    {
        public string filepath { get; set; }
        public string filename { get; set; }
        public string is_color { get; set; }
        public string mic { get; set; }
        public string ooc { get; set; }
        public string code { get; set; }
        public string color { get; set; }
        public string type { get; set; }
        public string precinct { get; set; }
        public string precinct_key { get; set; }
        public string precinct_name { get; set; }

        public string flag { get; set; }
        public string fed { get; set; }
        public string letter { get; set; }
        public int iooc { get; set; }
        

    }
}
