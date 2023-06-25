using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLA_Project
{
    class CFG
    {
        public List<Var> Grammer = new List<Var>();
        int Tcount = 0;
        private bool is_nullable(HashSet<String> V_N, Rule R)
        {
            foreach (string s in R.Rlist)
            {
                if (!V_N.Contains(s))
                {
                    return false;
                }
            }
            return true;
        }
        private bool is_notAccessible(HashSet<String> V_Removed, Rule R)
        {
            foreach (string s in R.Rlist)
            {
                if (V_Removed.Contains(s))
                {
                    return true;
                }
            }
            return false;
        }

        public void Remove_LR()
        {
            for (int i = 0; i < Grammer.Count; i++)
            {
                if (Grammer[i].is_LR() == true)
                {
                    Var T = new Var();
                    Var Replaced = new Var();
                    Tcount++;
                    T.V = "T" + Tcount.ToString();
                    Replaced.V = Grammer[i].V;
                    foreach (Rule Selected_Rule in Grammer[i].Rules)
                    {
                        Selected_Rule.updateR();
                        if (Selected_Rule.Rlist[0] == Grammer[i].V)
                        {
                            Rule MyRule1 = new Rule();
                            Rule MyRule2 = new Rule();
                            MyRule1.Rlist.Add(Selected_Rule.R.Substring(Grammer[i].V.Length, Selected_Rule.R.Length - Grammer[i].V.Length));
                            MyRule1.Rlist.Add("T" + Tcount.ToString());
                            T.Rules.Add(MyRule1);
                            MyRule2.Rlist.Add(Selected_Rule.R.Substring(Grammer[i].V.Length, Selected_Rule.R.Length - Grammer[i].V.Length));
                            T.Rules.Add(MyRule2);
                        }
                        else
                        {
                            Rule MyRule1 = new Rule();
                            Rule MyRule2 = new Rule();
                            foreach (string s in Selected_Rule.Rlist)
                            {
                                MyRule1.Rlist.Add(s);
                            }
                            MyRule1.Rlist.Add("T" + Tcount.ToString());
                            Replaced.Rules.Add(MyRule1);

                            foreach (string s in Selected_Rule.Rlist)
                            {
                                MyRule2.Rlist.Add(s);
                            }

                            Replaced.Rules.Add(MyRule2);

                        }
                    }

                    if (Replaced.Rules.Count == 0)
                    {
                        Rule MyRule1 = new Rule();
                        MyRule1.Rlist.Add("T" + Tcount.ToString());
                        Replaced.Rules.Add(MyRule1);
                    }
                    Grammer[i] = Replaced;
                    Grammer.Add(T);
                }
            }
        }
        public void Remove_Landa()
        {
            HashSet<String> V_N = new HashSet<String>();
            foreach (Var v in Grammer)
            {
                foreach (Rule r in v.Rules)
                {
                    if (r.Rlist[0] == "λ")
                    {
                        V_N.Add(v.V);
                    }
                }
            }

            bool flag = true;
            while (true)
            {
                int count = V_N.Count();
                foreach (Var v in Grammer)
                {
                    foreach (Rule r in v.Rules)
                    {
                        if (is_nullable(V_N, r))
                        {
                            V_N.Add(v.V);
                        }
                    }
                }
                if (V_N.Count == count)
                    break;
            }
            // hazf landa ha
            foreach (Var v in Grammer)
            {
                for (int i = 0; i < v.Rules.Count; i++) // Check kardane yek Khat
                {
                    if (v.Rules[i].Rlist[0] == "λ")
                    {
                        v.Rules.RemoveAt(i);
                        break;
                    }

                }
            }


            // EZAFE KARDANE TARKIBAT BA VN
            while (flag)
            {
                flag = false;
                foreach (Var v in Grammer)
                {
                    int count = v.Rules.Count();
                    for (int i = 0; i < v.Rules.Count; i++) // Check kardane yek Khat
                    {
                        for (int j = 0; j < v.Rules[i].Rlist.Count; j++) // Check kardane 1 rule
                        {
                            Rule Temp = new Rule();

                            if (V_N.Contains(v.Rules[i].Rlist[j])) // AGAR J JOZE VN BASHAD
                            {
                                int tekrar = 0;
                                foreach (string s in v.Rules[i].Rlist)
                                {
                                    if (v.Rules[i].Rlist[j] != s)
                                    {
                                        Temp.Rlist.Add(s);
                                    }
                                    else
                                    {
                                        tekrar++;
                                        if (tekrar > 1)
                                        {
                                            Temp.Rlist.Add(s);
                                        }

                                    }
                                }
                            }

                            if (Temp.Rlist.Count > 0)
                            {
                                v.Add_Rule(Temp);
                            }

                        }
                    }
                    if (count < v.Rules.Count())
                    {
                        flag = true;
                    }
                }

            }
        }

        public void Remove_Unit()
        {
            // Find Units
            List<Var> V_Units = new List<Var>();
            foreach (Var v in Grammer)
            {
                foreach (Rule r in v.Rules)
                {
                    if (r.Rlist.Count == 1)
                    {
                        foreach (Var v2 in Grammer)
                        {
                            if (v2.V == r.Rlist[0])
                            {
                                r.is_Unit = true;
                                V_Units.Add(v);

                                if (v.V != v2.V)
                                    v.Going_To.Add(v2);

                            }

                        }

                    }
                }
            }
            // Find SUB UNITS
            while (true)
            {
                int count = V_Units.Count;
                foreach (Var v in Grammer)
                {
                    foreach (Rule r in v.Rules)
                    {
                        if (r.Rlist.Count == 1)
                        {
                            foreach (Var v2 in V_Units)
                            {
                                if (v2.V == r.Rlist[0])
                                {
                                    foreach (Var v3 in v2.Going_To)
                                    {
                                        if (v.V != v3.V)
                                        {
                                            v.Going_To.Add(v3);
                                        }

                                    }

                                }

                            }

                        }

                    }
                }

                if (V_Units.Count == count)
                    break;
            }


            // Remove Units

            foreach (Var v in Grammer)
            {
                for (int i = 0; i < v.Rules.Count; i++)
                {
                    if (i == v.Rules.Count)
                        break;
                    if (v.Rules[i].is_Unit == true)
                    {
                        v.Rules.RemoveAt(i);
                        i--;
                    }
                }
            }


            // ADD GOING TO GRAMMERS TO DELETED UNIT

            foreach (Var v in Grammer)
            {
                foreach (Var vunits in v.Going_To)
                {
                    foreach (Rule r in vunits.Rules)
                    {
                        v.Add_Rule(r);
                    }
                }
            }


        }

        public void Remove_UseLess()
        {
            HashSet<String> G = new HashSet<String>();
            foreach (Var v in Grammer)
            {
                G.Add(v.V);
            }
            HashSet<String> V_1 = new HashSet<String>();
            foreach (Var v in Grammer)
            {
                foreach (Rule r in v.Rules)
                {
                    if (r.Rlist[0] == "λ")
                    {
                        V_1.Add(v.V);
                    }
                    else
                    {
                        bool flag = true;
                        foreach (string s in r.Rlist)
                        {
                            if (G.Contains(s))
                            {
                                flag = false;
                            }
                        }
                        if (flag == true)
                        {
                            V_1.Add(v.V);
                        }

                    }
                }
            }
            // Add Subset to V1
            while (true)
            {
                int count = V_1.Count;
                foreach (Var v in Grammer)
                {
                    foreach (Rule r in v.Rules)
                    {
                        foreach (string s in r.Rlist)
                        {
                            if (V_1.Contains(s))
                            {
                                V_1.Add(v.V);
                                break;

                            }
                        }

                    }
                }

                if (V_1.Count == count)
                    break;

            }

            HashSet<String> Removed_V = new HashSet<String>();

            // Remove them from Grammer
            for (int i = 0; i < Grammer.Count; i++)
            {

                if (i == Grammer.Count)
                    break;
                if (!V_1.Contains(Grammer[i].V))
                {
                    Removed_V.Add(Grammer[i].V);
                    Grammer.RemoveAt(i);
                    i--;
                }

            }

            foreach (Var v in Grammer)
            {
                for (int i = 0; i < v.Rules.Count; i++)
                {
                    if (i == v.Rules.Count)
                        break;
                    if (is_notAccessible(Removed_V, v.Rules[i]))
                    {
                        v.Rules.RemoveAt(i);
                        i--;
                    }
                }
            }

            // Remove Not ACCESABLE FROM START
            HashSet<Var> Accessible = new HashSet<Var>();
            HashSet<String> Accessible_V = new HashSet<String>();
            Accessible.Add(Grammer[0]);
            Accessible_V.Add(Grammer[0].V);
            while (true)
            {
                int count = Accessible.Count;
                for (int i = 0; i < Accessible.Count; i++)
                {
                    foreach (Rule r in Accessible.ElementAt(i).Rules)
                    {
                        foreach (string s in r.Rlist)
                        {

                            foreach (Var v2 in Grammer)
                            {
                                if (v2.V == s)
                                {
                                    Accessible.Add(v2);
                                    Accessible_V.Add(s);
                                }

                            }
                        }

                    }
                }

                if (Accessible.Count == count)
                    break;

            }

            // Remove them from Grammer
            for (int i = 0; i < Grammer.Count; i++)
            {

                if (i == Grammer.Count)
                    break;
                if (!Accessible_V.Contains(Grammer[i].V))
                {
                    Removed_V.Add(Grammer[i].V);
                    Grammer.RemoveAt(i);
                    i--;
                }
            }




        }

        private bool is_Rule_Chomsky(HashSet<String> V_1, Rule R)
        {
            if (R.Rlist.Count > 2)
            {
                return false;
            }
            int V_count = 0;
            foreach (string s in R.Rlist)
            {
                if (V_1.Contains(s))
                {
                    V_count++;
                }
            }
            if (V_count == 2 || V_count == 0)
            {
                return true;
            }
            return false;
        }
        public void Chomsky()
        {
            HashSet<String> G = new HashSet<String>();
            List<Var> New_Grammer = new List<Var>();
            foreach (Var v in Grammer)
            {
                G.Add(v.V);
            }
            foreach (Var v in Grammer)
            {
                foreach (Rule r in v.Rules)
                {
                    if (!is_Rule_Chomsky(G, r)) // The Rule is not on Chomsky form
                    {
                        for (int i = 0; i < r.Rlist.Count; i++)
                        {
                            if (!G.Contains(r.Rlist[i]))
                            {
                                Tcount++;
                                Var myVar = new Var();
                                Rule myRule = new Rule();
                                myVar.V = "T" + Tcount.ToString();
                                myRule.Rlist.Add(r.Rlist[i]);
                                myVar.Add_Rule(myRule);

                                r.Rlist[i] = "T" + Tcount.ToString();
                                New_Grammer.Add(myVar);

                            }
                        }
                    }
                }
            }
            //Add T TO MAIN GRAMMER
            foreach (Var v in New_Grammer)
            {
                Grammer.Add(v);
            }
            // Convert to Chomskey


            while (true)
            {
                int count = Grammer.Count;
                for (int i = 0; i < Grammer.Count; i++)
                {
                    for (int j = 0; j < Grammer[i].Rules.Count; j++)
                    {
                        if (Grammer[i].Rules[j].Rlist.Count > 2)
                        {
                            Tcount++;
                            Var myVar = new Var();
                            Rule myRule = new Rule();
                            myVar.V = "T" + Tcount.ToString();
                            myRule.Rlist.Add(Grammer[i].Rules[j].Rlist[Grammer[i].Rules[j].Rlist.Count - 2]);
                            myRule.Rlist.Add(Grammer[i].Rules[j].Rlist[Grammer[i].Rules[j].Rlist.Count - 1]);
                            Grammer[i].Rules[j].Rlist.RemoveAt(Grammer[i].Rules[j].Rlist.Count - 1);
                            myVar.Add_Rule(myRule);

                            Grammer[i].Rules[j].Rlist[Grammer[i].Rules[j].Rlist.Count - 1] = "T" + Tcount.ToString();

                            Grammer.Add(myVar);
                        }
                    }
                }
                if (count == Grammer.Count)
                    break;
            }



        }

    }
}
