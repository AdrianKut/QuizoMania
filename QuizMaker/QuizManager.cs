using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace QuizMaker
{
    class QuizManager
    {

        readonly Dictionary<RadioButton, TextBox> dictionaryRadioButtonsTextboxesAtoD;

        readonly Label labelTitle;
        readonly Label labelCounterNumberOfQuestion;
        readonly Label labelCounterCorrectAnswer;
        readonly Label labelCounterWrongAnswer;

        readonly Label labelLeftTime;

        readonly Button buttonConfirm;
        readonly Button buttonDraw;
        readonly Timer timer;

        private string currentCorrectAnswer;


        public QuizManager(Dictionary<RadioButton, TextBox> dictionaryRadioButtonsTextboxesAtoD, Timer timer, Label labelTitle,
            Label labelCounterNumberOfQuestion, Label labelCounterCorrectAnswer, Label labelCounterWrongAnswer,
            Label labelLeftTime, Button buttonConfirm, Button buttonDraw)
        {

            this.dictionaryRadioButtonsTextboxesAtoD = dictionaryRadioButtonsTextboxesAtoD;
            this.timer = timer;
            this.labelTitle = labelTitle;
            this.labelCounterNumberOfQuestion = labelCounterNumberOfQuestion;
            this.labelCounterCorrectAnswer = labelCounterCorrectAnswer;
            this.labelCounterWrongAnswer = labelCounterWrongAnswer;
            this.labelLeftTime = labelLeftTime;
            this.buttonConfirm = buttonConfirm;
            this.buttonDraw = buttonDraw;

            timer.Tick += Timer_Tick;
            timer.Start();

        }


        readonly private SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\AiXen\Documents\DB_Server.mdf;Integrated Security=True;Connect Timeout=30");
        readonly private Random random = new Random();

        readonly private List<int> listIDofLeftQuestions = new List<int>();
        readonly private List<int> listIDofAllQuestions = new List<int>();

        private void AssignNumberOfQuestionsToList()
        {
            con.Open();
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select [IdQuestion] from [QuestionTable] ";
            cmd.ExecuteNonQuery();

            DataTable dataTable = new DataTable();
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(cmd);
            sqlDataAdapter.Fill(dataTable);

            foreach (DataRow row in dataTable.Rows)
            {
                listIDofAllQuestions.Add(int.Parse(row["IdQuestion"].ToString()));
            }
            con.Close();
        }
        private bool IsGameOver()
        {
            if (listIDofAllQuestions.Count == listIDofLeftQuestions.Count)
            {
                StopAndResetTime();
                buttonConfirm.Enabled = false;
                buttonDraw.Enabled = false;

                Results results = new Results(correctAnswer, wrongAnswer);
                results.ShowDialog();
                return true;
            }
            else
            {
                return false;
            }

        }
        private byte numberOfQuestion = 0;
        private void IncreaseNumberOfQuestion() => numberOfQuestion++;
        private void ShowNumberOfLeftQuestion() => labelCounterNumberOfQuestion.Text = numberOfQuestion + "/" + listIDofAllQuestions.Count;

        private int currentIDQuestion = 0;
        private int DrawQuestion()
        {
            buttonDraw.Enabled = false;
            do
            {
                currentIDQuestion = random.Next(listIDofAllQuestions.Count);

            } while (listIDofLeftQuestions.Contains(currentIDQuestion));

            listIDofLeftQuestions.Add(currentIDQuestion);
            IncreaseNumberOfQuestion();

            return listIDofAllQuestions[currentIDQuestion];
        }

        private void AssignTitleAndAnswers(DataTable dataTable)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                labelTitle.Text = row["Title"].ToString();

                dictionaryRadioButtonsTextboxesAtoD.ElementAt(0).Value.Text = row["AnswerA"].ToString();
                dictionaryRadioButtonsTextboxesAtoD.ElementAt(1).Value.Text = row["AnswerB"].ToString();
                dictionaryRadioButtonsTextboxesAtoD.ElementAt(2).Value.Text = row["AnswerC"].ToString();
                dictionaryRadioButtonsTextboxesAtoD.ElementAt(3).Value.Text = row["AnswerD"].ToString();

                currentCorrectAnswer = row["CorrectAnswer"].ToString();
            }
        }

        private void ResetColor()
        {
            foreach (KeyValuePair<RadioButton, TextBox> item in dictionaryRadioButtonsTextboxesAtoD)
            {
                item.Value.BackColor = Color.White;
            }
        }

        public void ShowOneQuestion()
        {
            timer.Start();
            ResetColor();
            buttonConfirm.Enabled = true;

            con.Open();
            SqlCommand cmd = con.CreateCommand();

            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select * from [QuestionTable] where IdQuestion = " + DrawQuestion();
            cmd.ExecuteNonQuery();

            DataTable dataTable = new DataTable();
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(cmd);
            sqlDataAdapter.Fill(dataTable);

            ShowNumberOfLeftQuestion();
            AssignTitleAndAnswers(dataTable);

            con.Close();
        }

        private byte correctAnswer = 0, wrongAnswer = 0;
        private void IncreaseAndShowCorrectAnswer()
        {
            correctAnswer++;
            labelCounterCorrectAnswer.Text = "" + correctAnswer;
        }
        private void IncreaseAndShowWrongAnswer()
        {
            wrongAnswer++;
            labelCounterWrongAnswer.Text = "" + wrongAnswer;
        }


        private void CheckAnswer(KeyValuePair<RadioButton, TextBox> answer)
        {

            StopAndResetTime();
            buttonConfirm.Enabled = false;

            if (answer.Key.Text == currentCorrectAnswer)
            {
                IncreaseAndShowCorrectAnswer();
                ShowCorrectGreenTextBox(answer);
            }
            else
            {
                IncreaseAndShowWrongAnswer();
                ShowWrongTextBox(answer);
            }

            answer.Key.Checked = false;
        }

        private void ShowWrongTextBox(KeyValuePair<RadioButton, TextBox> answer)
        {
            foreach (KeyValuePair<RadioButton, TextBox> eachAnswer in dictionaryRadioButtonsTextboxesAtoD)
            {
                if (eachAnswer.Key.Text == currentCorrectAnswer)
                {
                    ShowCorrectGreenTextBox(eachAnswer);
                }
            }

            answer.Value.BackColor = Color.IndianRed;
        }

        private void ShowCorrectGreenTextBox(KeyValuePair<RadioButton, TextBox> answer)
        {
            answer.Value.BackColor = Color.DarkSeaGreen;
        }
        public void ConfirmAnswer()
        {
            bool isAnyChecked = false;

            foreach (KeyValuePair<RadioButton, TextBox> answer in dictionaryRadioButtonsTextboxesAtoD)
            {
                if (answer.Key.Checked && !isAnyChecked)
                {
                    buttonDraw.Enabled = true;

                    CheckAnswer(answer);
                    isAnyChecked = true;

                    ShowNumberOfLeftQuestion();
                    IsGameOver();
                }
            }

            if (!isAnyChecked)
            {
                MessageBox.Show("Nie zaznaczono odpowiedzi", "Komunikat", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }




        public void Start()
        {
            AssignNumberOfQuestionsToList();
            ShowOneQuestion();

            buttonDraw.Enabled = false;
        }

        private int counter = 21;
        private void Timer_Tick(object sender, EventArgs e)
        {
            counter--;
            labelLeftTime.Text = "Pozostało: " + counter;
            CheckTime();
        }

        private void StopAndResetTime()
        {
            counter = 21;
            timer.Stop();
        }

        private void CheckTime()
        {
            if (counter <= 0)
            {
                IncreaseAndShowWrongAnswer();
                StopAndResetTime();
                MessageBox.Show("Koniec czasu!", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                buttonConfirm.Enabled = false;
                buttonDraw.Enabled = true;

                foreach (KeyValuePair<RadioButton, TextBox> eachAnswer in dictionaryRadioButtonsTextboxesAtoD)
                {
                    if (eachAnswer.Key.Text == currentCorrectAnswer)
                    {
                        ShowCorrectGreenTextBox(eachAnswer);
                    }
                }

                IsGameOver();
            }
        }

    }
}
