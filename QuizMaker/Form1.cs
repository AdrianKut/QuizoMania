using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace QuizMaker
{
    public partial class Form1 : Form
    {

        readonly QuizManager quizManager;
        readonly Dictionary<RadioButton, TextBox> dicRadtioButtonsTextBoxes = new Dictionary<RadioButton, TextBox>();

        private void InitializeDictionaryRadioButtonsTextBoxesAtoB()
        {
            dicRadtioButtonsTextBoxes.Add(radioButtonA, textBoxA);
            dicRadtioButtonsTextBoxes.Add(radioButtonB, textBoxB);
            dicRadtioButtonsTextBoxes.Add(radioButtonC, textBoxC);
            dicRadtioButtonsTextBoxes.Add(radioButtonD, textBoxD);
        }

        public Form1()
        {
            InitializeComponent();
            InitializeDictionaryRadioButtonsTextBoxesAtoB();

            quizManager = new QuizManager(dicRadtioButtonsTextBoxes,timerMain,
                labelTitle, labelCounterQuestions, labelCounterCorrectAnswer, labelCounterWrongAnswer,
                labelLeftTime, buttonConfirm, buttonDraw);

            quizManager.Start();
        }


        private void buttonConfirm_Click(object sender, EventArgs e)
        {
            quizManager.ConfirmAnswer();
        }

        private void buttonDraw_Click(object sender, EventArgs e)
        {
            quizManager.ShowOneQuestion();
        }

    }
}
