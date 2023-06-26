using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLA_Project
{
    class Var
    {
        public String V;
        public List<Rule> Rules = new List<Rule>();
        public HashSet<Var> Going_To = new HashSet<Var>();
        public bool is_LR()
        {
            foreach (Rule S in Rules)
            {
                if (S.Rlist[0] == V)
                {
                    return true;
                }
            }
            return false;
        }
        public void Add_Rule(Rule r)
        {
            foreach (Rule s in Rules)
            {
                s.updateR();
                r.updateR();
                if (s.R == r.R)
                    return;
            }
            Rules.Add(r);
        }

        public bool TerminalCheck(char s)
        {
            String cmp = Convert.ToString(s);
            foreach (Rule r in Rules)
            {
                foreach (String str in r.Rlist)
                {
                    if (cmp == str)
                        return true;

                }
            }

            return false;
        }

        public bool VarCheck(Var v1, Var v2)
        {
            foreach (Rule r in Rules)
            {
                if(r.Rlist.Count == 2)
                if (r.Rlist[0] == v1.V && r.Rlist[1] == v2.V)
                    return true;
            }
            return false;

        }




    }
}
