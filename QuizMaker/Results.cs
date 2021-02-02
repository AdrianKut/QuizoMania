using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuizMaker
{
    public partial class Results : Form
    {
        public Results(int correct, int wrong)
        {
            InitializeComponent();
            labelCounterCorrectAnswer.Text = "" + correct;
            labelCounterWrongAnswer.Text = "" + wrong;
            labelCounterQuestions.Text = correct + "/" + (wrong+correct);

           
        }


        private void buttonOK_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}

