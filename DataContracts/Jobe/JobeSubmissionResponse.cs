using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataContracts.Jobe
{
    public class JobeSubmissionResponse
    {
        public int Outcome { get; set; }

        public List<string> Output { get; set; }
    }
}
