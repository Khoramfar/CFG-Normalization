using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLA_Project
{
    class Rule
    {
        public String R;
        public List<String> Rlist = new List<String>();
        public bool is_Unit = false;
        public void updateR()
        {
            R = "";
            foreach(String s in Rlist)
            {
                R += s;
            }
        }
    }
}
