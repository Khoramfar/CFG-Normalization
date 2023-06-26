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
                            //    MyRule1.Rlist.Add(Selected_Rule.R.Substring(Grammer[i].V.Length, Selected_Rule.R.Length - Grammer[i].V.Length));
                            MyRule1.Rlist.AddRange(Selected_Rule.Rlist.Skip(1));
                            MyRule1.Rlist.Add("T" + Tcount.ToString());
                            T.Rules.Add(MyRule1);
                            //  MyRule2.Rlist.Add(Selected_Rule.R.Substring(Grammer[i].V.Length, Selected_Rule.R.Length - Grammer[i].V.Length));
                            MyRule2.Rlist.AddRange(Selected_Rule.Rlist.Skip(1));
                            T.Rules.Add(MyRule2);
                        }
                        else
                        {
                            Rule MyRule1 = new Rule();
                            Rule MyRule2 = new Rule();
                            foreach (string s in Selected_Rule.Rlist)
                            {
                                if (s != "λ")
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
            bool flag = true;
            while (flag)
            {
                flag = false;
                foreach (Var v in Grammer)
                {
                    foreach (Var vunits in v.Going_To)
                    {
                        foreach (Rule r in vunits.Rules)
                        {
                            if (!v.Rules.Contains(r))
                            {
                                v.Add_Rule(r);
                                flag = true;
                            }
                        }
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
                                bool not_duplicates = true;
                                String Temp = "T" + Tcount.ToString();

                                Rule myRule = new Rule();
                                myRule.Rlist.Add(r.Rlist[i]);

                                Var myVar = new Var();
                                myVar.V = Temp;
                                myVar.Add_Rule(myRule);


                                foreach (Var vv in New_Grammer)
                                {
                                    if (vv.Rules.Count == 1 && vv.Rules[0].Rlist.SequenceEqual(myVar.Rules[0].Rlist))
                                    {
                                        Tcount--;
                                        Temp = vv.V;
                                        not_duplicates = false;
                                        break;
                                    }
                                }

                                r.Rlist[i] = Temp;

                                if (not_duplicates)
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
            bool flag = true;
            while (flag)
            {
                flag = false;
                int count = Grammer.Count;
                for (int i = 0; i < Grammer.Count; i++)
                {
                    for (int j = 0; j < Grammer[i].Rules.Count; j++)
                    {
                        if (Grammer[i].Rules[j].Rlist.Count > 2)
                        {
                            Tcount++;
                            bool not_duplicates = true;
                            String Temp = "T" + Tcount.ToString();

                            Var myVar = new Var();
                            Rule myRule = new Rule();
                            myVar.V = Temp;
                            myRule.Rlist.Add(Grammer[i].Rules[j].Rlist[Grammer[i].Rules[j].Rlist.Count - 2]);
                            myRule.Rlist.Add(Grammer[i].Rules[j].Rlist[Grammer[i].Rules[j].Rlist.Count - 1]);
                            Grammer[i].Rules[j].Rlist.RemoveAt(Grammer[i].Rules[j].Rlist.Count - 1);
                            myVar.Add_Rule(myRule);

                            foreach (Var vv in Grammer)
                            {
                                if ( vv.Rules[0].Rlist.SequenceEqual(myVar.Rules[0].Rlist))
                                {
                                    Tcount--;
                                    Temp = vv.V;
                                    not_duplicates = false;
                                    break;
                                }
                            }

                            Grammer[i].Rules[j].Rlist[Grammer[i].Rules[j].Rlist.Count - 1] = Temp;
                            flag = true;
                            if (not_duplicates)
                                Grammer.Add(myVar);
                        }
                    }
                }
            }

        }

        private bool is_Rule_Geribach(HashSet<String> V_1, Rule R)
        {
            if (R.Rlist.Count == 1 && V_1.Contains(R.Rlist[0]))
            {
                return false;
            }

            for (int i = 1; i < R.Rlist.Count; i++)
            {
                if (!V_1.Contains(R.Rlist[i]))
                {
                    return false;
                }
            }

            return true;
        }

        private int Sort_Point(HashSet<String> V_1, String Vi)
        {
            for (int i = 0; i < V_1.Count; i++)
            {
                if (V_1.ElementAt(i) == Vi)
                {
                    return i;
                }
            }
            return -1;
        }
        public void TartibIJ()
        {
            HashSet<String> G = new HashSet<String>();
            List<Var> New_Grammer = new List<Var>();
            foreach (Var v in Grammer)
            {
                G.Add(v.V);
            }
            // Tartibe Ai Aj
            bool flag = true;
            while (flag)
            {
                flag = false;
                foreach (Var v in Grammer)
                {

                    for (int i = 0; i < v.Rules.Count; i++)
                    {

                        if (G.Contains(v.Rules[i].Rlist[0])) // terminal nabashe
                        {
                            if (Sort_Point(G, v.V) > Sort_Point(G, v.Rules[i].Rlist[0]))
                            {
                                foreach (Var jaygozin in Grammer)
                                {
                                    if (jaygozin.V == v.Rules[i].Rlist[0])
                                    {
                                        foreach (Rule r in jaygozin.Rules)
                                        {
                                            Rule temp = new Rule();
                                            temp.Rlist = r.Rlist.ToList();
                                            temp.Rlist.AddRange(v.Rules[i].Rlist.Skip(1));
                                            v.Add_Rule(temp);
                                        }
                                        v.Rules.RemoveAt(i);
                                        flag = false;
                                        break;
                                    }
                                }

                            }
                        }


                    }
                }
            }
        }
        public void Geribach()
        {
            HashSet<String> G = new HashSet<String>();
            List<Var> New_Grammer = new List<Var>();
            foreach (Var v in Grammer)
            {
                G.Add(v.V);
            }

            // Hazfe payane haye moshkel dar 
            foreach (Var v in Grammer)
            {
                foreach (Rule r in v.Rules)
                {
                    if (!is_Rule_Geribach(G, r)) // The Rule is not on Geribach form
                    {
                        int i = 0;
                        if (!G.Contains(r.Rlist[0]))
                        {
                            i = 1;
                        }
                        for (; i < r.Rlist.Count; i++)
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

            G = new HashSet<String>();
            foreach (Var v in Grammer)
            {
                G.Add(v.V);
            }

            // Jaygozini
            bool flag = true;
            while (flag)
            {
                flag = false;
                foreach (Var v in Grammer)
                {
                    for (int i = 0; i < v.Rules.Count; i++)
                    {
                        if (G.Contains(v.Rules[i].Rlist[0]))
                        {
                            foreach (Var jaygozin in Grammer)
                            {
                                if (jaygozin.V == v.Rules[i].Rlist[0])
                                {
                                    foreach (Rule r in jaygozin.Rules)
                                    {
                                        Rule temp = new Rule();
                                        temp.Rlist = r.Rlist.ToList();
                                        temp.Rlist.AddRange(v.Rules[i].Rlist.Skip(1));
                                        v.Add_Rule(temp);
                                    }
                                    v.Rules.RemoveAt(i);
                                    flag = true;
                                }
                            }
                        }
                    }
                }

            }


        }


        public bool CYK(String Word)
        {
            int n = Word.Length;
            int r = Grammer.Count;
            bool[,,] T = new bool[n, n, r];
            int i, j, k, x, y, Z;

            // Jadval Avalie baraye Pooya
            for (i = 0; i < n; i++)
                for (j = 0; j < r; j++)
                    if (Grammer[j].TerminalCheck(Word[i])) // Agar terminal dar grammer vojood dasht yani radif aval hame word gharar migirad
                        T[i, 0, j] = true;


            for (i = 1; i < n; i++) // satr haye jadval
                for (j = 0; j < n - i; j++) // sotoon haye jadval ta ghotr
                    for (k = 0; k < i; k++) // hame balayi ha ( yeki yeki radif mirim payin ) 
                        for (x = 0; x < r; x++) // Baresi tak tak Var ha ( bode 3vom)
                            for (y = 0; y < r; y++) //  Baresi tak tak Var ha ( bod 3vom ) 
                                if (T[j, k, x] && T[j + k + 1, i - k - 1, y]) // ta x tajzie shodem, ta y ham tajzie shode
                                    for (Z = 0; Z < r; Z++) // agar Z -> XY bebinim mishe xy ro ham tajzie kard
                                        if (Grammer[Z].VarCheck(Grammer[x], Grammer[y])) // Agar Z vojood dasht pas ta Z tajzie mishavad
                                            T[j, i, Z] = true;


            for (i = 0; i < r; i++)
                if (T[0, n - 1, i] && Grammer[i].V == Grammer[0].V) // Hame anha az S moshtagh mishavad yani hamishe Z = S boode
                {
                    return true;
                }

            return false;
        }

    }
}
