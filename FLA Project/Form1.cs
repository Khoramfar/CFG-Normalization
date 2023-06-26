using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FLA_Project
{

    public partial class Main : Form
    {
        CFG MainCFG = new CFG();

        public Main()
        {
            InitializeComponent();
        }
        private void updateListBox()
        {
            listBox1.Items.Clear();
            foreach (Var v in MainCFG.Grammer)
            {
                String S = v.V;
                S = S + " -> ";
                foreach (Rule r in v.Rules)
                {
                    r.updateR();
                    S = S + r.R + " | ";
                }
                S = S.Remove(S.Length - 2);
                listBox1.Items.Add(S);
            }
        }


        private void LogoutBtn_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            VarSelect.Items.Add(varTbox.Text.ToString());
            varTbox.Text = "";
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Var MyVar = new Var();
            MyVar.V = VarSelect.Text;
            Rule MyRule = new Rule();

            String Temp = RuleTbox.Text.ToString();
            foreach (char c in Temp)
            {
                MyRule.Rlist.Add(c.ToString());
            }

            MyVar.Add_Rule(MyRule);
            foreach (Var v in MainCFG.Grammer)
            {
                if (v.V == MyVar.V)
                {
                    v.Add_Rule(MyRule);
                    updateListBox();
                    return;
                }
            }
            MainCFG.Grammer.Add(MyVar);
            updateListBox();
        }

        private void RuleTbox_TextChanged(object sender, EventArgs e)
        {

        }

        private void Main_Load(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            RuleTbox.Text = "λ";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            MainCFG.Remove_LR();
            updateListBox();

        }

        private void button5_Click(object sender, EventArgs e)
        {
            MainCFG.Remove_Landa();
            updateListBox();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            MainCFG.Remove_Unit();
            updateListBox();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            MainCFG.Remove_UseLess();
            updateListBox();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            MainCFG.Remove_Landa();
            MainCFG.Remove_Unit();
            MainCFG.Chomsky();
            updateListBox();

        }

        private void button9_Click(object sender, EventArgs e)
        {
            MainCFG.Remove_Landa();
            MainCFG.Remove_Unit();
            MainCFG.Remove_LR();
            MainCFG.TartibIJ();
            MainCFG.Remove_LR();
            MainCFG.Geribach();

            updateListBox();

        }

        private void button10_Click(object sender, EventArgs e)
        {


        }

        private void ParsTbox_TextChanged(object sender, EventArgs e)
        {
            MainCFG.Remove_Landa();
            MainCFG.Remove_Unit();
            MainCFG.Chomsky();
            updateListBox();
            if (ParsTbox.Text == "")
            {
                ParsTbox.BackColor = System.Drawing.Color.White;
                return;
            }
            if (MainCFG.CYK(ParsTbox.Text))
            {
                ParsTbox.BackColor = System.Drawing.Color.Green;
            }
            else
            {
                ParsTbox.BackColor = System.Drawing.Color.Red;
            }
        }

        private void button10_Click_1(object sender, EventArgs e)
        {
            MainCFG = new CFG();
            VarSelect.Items.Clear();
            updateListBox();

        }
    }
}
