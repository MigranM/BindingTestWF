using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace bindingtest
{
    public partial class Form1 : Form
    {

        List<someData> testList; //Данные для Датагрида

        DataSet ds; //датаСет чтобы хранить табличку что внизу;
        public DataTable MyDataTable; //Таблица в которую будем пихать все необходимые данные для привязки к Контролам (пока что тут будет только RowIndex для "страничек")
        
        public bool setPageSafety(string index) //безопасное переключение страниц, чтобы каждый раз не читать длину коллекции дата грида сделаешь проверку по нашей переменной с длиной даты
        {
            int newIndex;
            try
            {
                newIndex = Int32.Parse(index);
            }
            catch
            {
                return false;
            }
            if (dataGridView1.DataSource != null)
                if (newIndex >= 0 && newIndex < dataGridView1.Rows.Count) return true;
            return false;
        }

        public Form1()
        {
            InitializeComponent();

            testList = new List<someData>()
            {
                new someData(){column1 = "1", column2 =  "2", column3 = "3" },
                new someData(){column1 = "4", column2 =  "5", column3 = "6" },
                new someData(){column1 = "7", column2 =  "8", column3 = "9" }
            };//инициализация листа для датагрида

            dataGridView1.DataSource = testList; //привязка данных к датагриду
            CreateDataSet();

            //область привязки всех данных к необходимым контролам (вынести в отдельный метод)
            textBox1.DataBindings.Add(new Binding("Text", ds, "currentRow.rowID"));

        }

        public void CreateDataSet() //настройка всех необходимых для привязки данных, пока что только RowIndex для страничек
        {
            ds = new DataSet();
            MyDataTable = new DataTable("currentRow");
            DataColumn dc = new DataColumn("rowID", typeof(int));

            MyDataTable.Columns.Add(dc);
            ds.Tables.Add(MyDataTable);
            MyDataTable.ExtendedProperties[0] = 0;
            DataRow dRow;
            dRow = MyDataTable.NewRow();
            dRow["rowID"] = 1;
            MyDataTable.Rows.Add(dRow);

        }

        private void button1_Click(object sender, EventArgs e) //страница Назад
        {
            if (setPageSafety((Convert.ToInt32(MyDataTable.Rows[0]["rowID"]) - 1).ToString()))
            {
                MyDataTable.Rows[0]["rowID"] = Convert.ToInt32(MyDataTable.Rows[0]["rowID"]) - 1; //страничка назад (изменяем только RowIndex, текст в текстбокс сам подтянется от него при изменении так как под капотом там событие на сеттере)
                dataGridView1.Rows[Convert.ToInt32(MyDataTable.Rows[0]["rowID"])].Cells[0].Selected = true; //выбираем ячейку в соответствии с rowD
            }

        }

        private void button2_Click(object sender, EventArgs e) //страница Вперед
        {
            if (setPageSafety((Convert.ToInt32(MyDataTable.Rows[0]["rowID"]) + 1).ToString()))
            {
                MyDataTable.Rows[0]["rowID"] = Convert.ToInt32(MyDataTable.Rows[0]["rowID"]) + 1; //страничка назад (изменяем только RowIndex, текст в текстбокс сам подтянется от него при изменении так как под капотом там событие на сеттере)
                dataGridView1.Rows[Convert.ToInt32(MyDataTable.Rows[0]["rowID"])].Cells[0].Selected = true; //выбираем ячейку в соответствии с rowID
            }

        }

        private void button3_Click(object sender, EventArgs e) //выбрать введенную страницу
        {
            if (setPageSafety(textBox1.Text))
            {
                dataGridView1.Rows[Convert.ToInt32(Int32.Parse(textBox1.Text))].Cells[0].Selected = true; //выбираем ячейку в соответствии с textBox1 текстом, то бишь что ввел юзверь
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e) //при смене выбранной ячейки тычком мышки
        {
            if (dataGridView1.SelectedCells.Count != 0) //если что-либо выбрано вообще, то:
            {
                int newIndex = dataGridView1.SelectedCells[0].RowIndex;
                MyDataTable.Rows[0]["rowID"] = newIndex; //изменяем RowIndex 
                textBox1.Text = dataGridView1.SelectedCells[0].RowIndex.ToString(); /*в упор изменяем текст в textBox1 
                                                                                      (поток изменения контролов не находится в одном и том же потоке с формой,
                                                                                      поэтому событие в сеттере rowIndex не отрисует текст, пока это будет тут, 
                                                                                      всяко лучше чем вызывать перерисовку тысячу раз {надеюсь сможешь обойти это, будет просто шик}
                                                                                      скорее всего надо поиграть с Disposing, но я потом этим займусь уже, может ты найдешь быстрее решение)
                                                                                    */
            }

            //надеюсь ты это осилишь перенести в наш проект, тут нет исключений по границам

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (setPageSafety(textBox1.Text))
            {
                button3.Enabled = true;
            }
            else button3.Enabled = false;
        }
    }
    public class someData //данные для датаГрида (обязательно свойства а не поля иначе он будет игнорить некоторые спец символы в хедерах!)
    {
        public string column1 { get; set; }
        public string column2 { get; set; }
        public string column3 { get; set; }
    }
}
