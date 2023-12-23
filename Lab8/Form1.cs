using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using OfficeOpenXml;
using LicenseContext = OfficeOpenXml.LicenseContext;

namespace Lab8
{
    public partial class Form1 : Form
    {
        private string fileName = string.Empty;
        private DataTableCollection tableCollection = null;
        double[] ArrayX = null;
        double[] ArrayY = null;
        double[] DefultArrayX = { 1, 2, 3 };
        double[] DefultArrayY = { 10, 11, 12 };
        private bool ArrayUnic(double[] Array)
        {
            for (int i = Array.Length - 1; i > 0; --i)
            {
                for (int j = i-1; j >= 0; --j)
                {
                    if (Array[i] == Array[j])
                    {
                        return false;
                    }
                }
            }
            return true;   
        }
        private void button4_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            this.chart1.Series[0].Points.Clear();
            ArrayX = DefultArrayX;
            ArrayY = DefultArrayY;
            GetArrayInGrid();
            PrintPointsChart();
        }            
        
            
        
        private void GetArrayInGrid() 
        {
            dataGridView1.Rows.Clear();
            try
            {             
                for (int index = 0; index < ArrayX.Length; ++index)
                {
                    dataGridView1.Rows.Add(Convert.ToString(ArrayX[index]), Convert.ToString(ArrayY[index]));
                }                
            }
            catch
            {
                MessageBox.Show("Ошибка при заполнении сетки!", "Ошибка конвертации!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        private void GetArrayFromGrid() // получение чисел из gridViev в NumberArray
        {
            try
            {
                bool Correct = true;
                bool NotNull = true;
                ArrayX = new double[dataGridView1.Rows.Count - 1];
                ArrayY = new double[dataGridView1.Rows.Count - 1];
 
                for (int index = 0; index < dataGridView1.RowCount - 1; ++index)
                {
                    if (dataGridView1[0, index].Value == null)
                    {
                        NotNull = false;
                    }
                    double ItemX;
                    Correct = double.TryParse(Convert.ToString(dataGridView1[0, index].Value), out ItemX);
                    ArrayX[index] = ItemX;
                }
                for (int index = 0; index < dataGridView1.RowCount - 1; ++index)
                {
                    if (dataGridView1[1, index].Value == null)
                    {
                        NotNull = false;
                    }
                    double ItemY;
                    Correct = double.TryParse(Convert.ToString(dataGridView1[1, index].Value), out ItemY);
                    ArrayY[index] = ItemY;                   
                }
                if (!NotNull)
                {
                    throw new Exception("В множестве отстутсвуют данные, либо столбцы X и Y неравны!");
                }
                if (!Correct) 
                {                   
                    throw new Exception("Некорректный ввод данных!");
                }
                if (ArrayUnic(ArrayX) == false)
                {
                    throw new Exception("Данные в столбце X не должны повторяться!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        private void button2_Click(object sender, EventArgs e)
        {
            // Обработка события нажатия кнопки "Загрузить из Excel"
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Выберите файл Excel",
                Filter = "Файлы Excel (*.xlsx)|*.xlsx|Все файлы (*.*)|*.*",
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;

                try
                {
                    LoadDataFromExcel(filePath);
                    //UpdateDataGridView(data);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке данных из Excel: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }         
        private void LoadDataFromExcel(string filePath)
        {

            // Устанавливаем LicenseContext перед использованием EPPlus
            ExcelPackage.LicenseContext = LicenseContext.Commercial;

            //data.Clear(); // Очищаем старые данные при загрузке новых

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                if (worksheet != null)
                {
                    dataGridView1.Rows.Clear();                   

                    for (int row = 1; row <= worksheet.Dimension.End.Row; row++)
                    {
                        string cellValueX = worksheet.Cells[row, 1].Text;
                        string cellValueY = worksheet.Cells[row, 2].Text;

                        // Проверяем, если ячейка начинается с минуса, добавляем пробел перед минусом
                        //if (cellValue.StartsWith("-"))
                        //{
                        //    cellValue = " " + cellValue;
                        //}                                          
                        if ((double.TryParse(cellValueX, out double ValueX)) &
                            (double.TryParse(cellValueY, out double ValueY)))
                        {
                            dataGridView1.Rows.Add(Convert.ToString(ValueX), Convert.ToString(ValueY));
                        }
                        else
                        {
                            MessageBox.Show($"Некорректное значение в {row} строке", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }
            }
        }
        private void button3_Click_1(object sender, EventArgs e)
        {
            try 
            {
                if (Int32.TryParse(SizeGenerateArray.Text, out int Size) == false) 
                {
                    throw new Exception("Некорректный ввод размера!");
                }
                if (Convert.ToInt32(SizeGenerateArray.Text) <= 0)
                {
                    throw new Exception("Размер столбца должен быть больше нуля!");
                }
                ArrayX = new double[Size];
                ArrayY = new double[Size];
                Random random = new Random();
                for (int i = 0; i < ArrayX.Length; ++i)
                {
                    ArrayX[i] = i + 1;
                }
                if (Convert.ToInt32(StartGenerate.Text) > Convert.ToInt32(FinishGenerate.Text))
                {
                    throw new Exception("Минимальная граница генерации должна быть меньше максимальной");
                }
                for (int i = 0; i < ArrayY.Length; ++i)
                {
                    ArrayY[i] = random.Next(Convert.ToInt32(StartGenerate.Text), Convert.ToInt32(FinishGenerate.Text));
                }
                GetArrayInGrid();                          
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }
        void PrintPointsChart()
        {
            this.chart1.Series[1].Points.Clear();
            for (int index = 0; index < ArrayX.Length; ++index)
            {
                chart1.Series[1].Points.AddXY(ArrayX[index], ArrayY[index]); 
            }
            
        }

        public void PrintLineChart(double[] Array, double a, double b)
        {
            double start = Array.Min() - 1;
            double finish = Array.Max() + 1;
            double h = 0.1, x, y;
            this.chart1.Series[0].Points.Clear();
            x = start;
            while (x <= finish)
            {
                y = a * x + b;
                this.chart1.Series[0].Points.AddXY(x, y);
                x += h;
            }
        }
        public void PrintParabolaChart(double[] Array, double a, double b, double c)
        {
            double start = Array.Min() - 0.1;
            double finish = Array.Max() + 0.1;
            double h = 0.1, x, y;
            this.chart1.Series[0].Points.Clear();
            x = start;
            while (x <= finish)
            {
                y = a * (x*x) + b * x + c;
                this.chart1.Series[0].Points.AddXY(x, y);
                x += h;
            }
        }
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }
        double LineA;
        double LineB;
        double ParabolaA;
        double ParabolaB;
        double ParabolaC;
        private void button1_Click(object sender, EventArgs e)
        {
            InaccuracyLine.Text = String.Empty;
            InaccuracyParabola.Text = String.Empty;
            LineFun.Text = "a * x + b";
            ParabolaFun.Text = "a * x ^ 2 + b * x + c";
            bool Continue = true;
            if ((LineCheck.Checked == false) &
                (ParabolaCheck.Checked == false))
            {
                MessageBox.Show("Выберите функцию!");
                Continue = false;
            }           
            if (Continue == true)
            {
                if (LineCheck.Checked == true)
                {
                    try
                    {
                        GetArrayFromGrid();
                        PrintPointsChart();
                        if ((ArrayX == null) &
                            (ArrayX == null))
                        {
                            MessageBox.Show("Какое множество?!");
                            Continue = false;
                        }
                        LineA = FindLineA();
                        LineB = FindLineB();
                        LineFun.Text = Convert.ToString(LineA) + " * x + " + Convert.ToString(LineB);
                        PrintLineChart(ArrayX, LineA, LineB);
                        InaccuracyLine.Text = Convert.ToString(LineInaccuracy(LineA, LineB));
                    } catch
                    {
                        MessageBox.Show($"Ошибка", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                if (ParabolaCheck.Checked == true)
                {
                    try
                    {
                        GetArrayFromGrid();
                        PrintPointsChart();
                        if ((ArrayX == null) &
                            (ArrayX == null))
                        {
                            MessageBox.Show("Какое множество?!");
                            Continue = false;
                        }
                        double[,] matrix = new double[3,3] {
                            {ArrayX.Length, SumArray(ArrayX), SumDegree2Array(ArrayX) },
                            {SumArray(ArrayX), SumDegree2Array(ArrayX), SumDegree3Array(ArrayX) },
                            {SumDegree2Array(ArrayX), SumDegree3Array(ArrayX), SumDegree4Array(ArrayX) }
                        };
                        double[] res = new double[3] { SumArray(ArrayY), SumMultiplicationArray(ArrayX, ArrayY), SumYX2Array(ArrayY, ArrayX) };
                        double[] coefficients = MethodGauss(matrix, res, 3);
                        ParabolaA = coefficients[2];
                        ParabolaB = coefficients[1];
                        ParabolaC = coefficients[0];
                        ParabolaFun.Text = Convert.ToString(ParabolaA) + " * x ^ 2 + " + Convert.ToString(ParabolaB) + " * x + " + Convert.ToString(ParabolaC);
                        PrintParabolaChart(ArrayX, ParabolaA, ParabolaB, ParabolaC);
                        InaccuracyParabola.Text = Convert.ToString(ParabolaInaccuracy(ParabolaA, ParabolaB, ParabolaC));
                    }
                    catch
                    {
                        MessageBox.Show($"Ошибка", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        private void button5_Click(object sender, EventArgs e)
        {
            
            if (LineCheck.Checked == true)
            {
                try
                {
                    if (LineFun.Text == "a * x + b")                   
                    {
                        throw new Exception("Сначала найдите коэффициенты");
                    }                  
                    PrognosisY.Text = Convert.ToString(LineA * Convert.ToDouble(PrognosisX.Text) + LineB);
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            if (ParabolaCheck.Checked == true)
            {
                try
                {
                    if (ParabolaFun.Text == "a * x ^ 2 + b * x + c")
                    {
                        throw new Exception("Сначала найдите коэффициенты");
                    }
                    PrognosisY.Text = Convert.ToString(ParabolaA * Math.Pow(Convert.ToDouble(PrognosisX.Text), 2) + ParabolaB * Convert.ToDouble(PrognosisX.Text) + ParabolaC);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private double LineInaccuracy(double a, double b)
        {
            double Result = 0;
            for (int index = 0; index < ArrayX.Length; ++index)
            {
                Result += Math.Pow(((a * ArrayX[index] + b) - ArrayY[index]), 2);
            }
            return Result;
        }
        private double ParabolaInaccuracy(double a, double b, double c)
        {
            double Result = 0;
            for (int index = 0; index < ArrayX.Length; ++index)
            {
                Result += Math.Pow(((a * Math.Pow(ArrayX[index], 2) + b * ArrayX[index] + c) - ArrayY[index]), 2);
            }
            return Result;
        }
        private double FindLineA()
        {
            double n = ArrayX.Length;
            double a;
            a = ((n * SumMultiplicationArray(ArrayX, ArrayY)) - (SumArray(ArrayX) * SumArray(ArrayY))) / ((n * SumDegree2Array(ArrayX)) - Math.Pow(SumArray(ArrayX), 2));
            return a;          
        }
        private double FindLineB()
        {
            double n = ArrayX.Length;
            double a = FindLineA();
            double b;
            b = (SumArray(ArrayY) - a * SumArray(ArrayX)) / n ;
            return b;
        }

        private double SumMultiplicationArray(double[] Array1, double[] Array2)
        {
            double Result = 0;
            for (int index = 0; index < Array1.Length; ++index)
            {
                Result += (Array1[index] * Array2[index]);
            }
            return Result;
        }
        private double SumArray(double[] Array)
        {
            double Result = 0;
            int n = Array.Length;
            for (int index = 0; index < n; ++index)
            {
                Result += Array[index];
            }
            return Result;
        }
        private double SumDegree2Array(double[] Array)
        {
            double Result = 0;
            for (int index = 0; index < Array.Length; ++index)
            {
                Result += Math.Pow(Array[index], 2);
            }
            return Result;
        }
        private double SumDegree3Array(double[] Array)
        {
            double Result = 0;
            for (int index = 0; index < Array.Length; ++index)
            {
                Result += Math.Pow(Array[index], 3);
            }
            return Result;
        }
        private double SumDegree4Array(double[] Array)
        {
            double Result = 0;
            for (int index = 0; index < Array.Length; ++index)
            {
                Result += Math.Pow(Array[index], 4);
            }
            return Result;
        }
        private double SumYX2Array(double[] Array1, double[] Array2)
        {
            double Result = 0;
            for (int index = 0; index < Array1.Length; ++index)
            {
                Result += (Array1[index] * Math.Pow(Array2[index], 2));
            }
            return Result;
        }
        private double[] MethodGauss(double[,] matrixParam, double[] resSlau, int countParam)
        {
            double[][] resultX = new double[3][];
            resultX[1] = new double[countParam];

            double[,] matrix = matrixParam.Clone() as double[,];
            double[] res = resSlau.Clone() as double[];
            double[] det = new double[countParam];
            double delta = Determinant(matrix, countParam);

            if (delta == 0)
            {
                throw new Exception("Нет решения!");
            }

            for (int indexRow = 1; indexRow < countParam; ++indexRow)
            {
                if (matrix[indexRow - 1, indexRow - 1] != 1)
                {      // превращение опорного элемента в единицу
                    double del = matrix[indexRow - 1, indexRow - 1];

                    for (int index = 0; index < countParam; ++index)
                    {
                        matrix[indexRow - 1, index] /= del;
                    }

                    res[indexRow - 1] /= del;
                }

                for (int index = 0; index < indexRow - 1; ++index)
                {
                    double element = matrix[indexRow, index];

                    for (int indexColumn = 0; indexColumn < countParam; ++indexColumn)
                    {
                        matrix[indexRow, indexColumn] = matrix[index, indexColumn] * (-element) + matrix[indexRow, indexColumn];
                    }

                    res[indexRow] = res[index] * (-element) + res[indexRow];
                }

                double el = matrix[indexRow, indexRow - 1];

                for (int index = indexRow - 1; index < countParam; ++index)
                {
                    matrix[indexRow, index] = matrix[indexRow - 1, index] * (-el) + matrix[indexRow, index];
                }

                res[indexRow] = res[indexRow - 1] * (-el) + res[indexRow];
            }

            resultX[1][countParam - 1] = res[countParam - 1] / matrix[countParam - 1, countParam - 1];

            for (int indexRow = countParam - 2; indexRow >= 0; --indexRow)
            {
                double sum = 0;

                for (int indexRes = countParam - 1; indexRes >= indexRow + 1; --indexRes)
                {
                    sum += matrix[indexRow, indexRes] * resultX[1][indexRes];
                }

                resultX[1][indexRow] = (res[indexRow] - sum) / matrix[indexRow, indexRow];
            }

            return resultX[1];
        }

        private double Determinant(double[,] matrix, int matrixSide)
        {
            if (matrixSide == 2)
            {
                return (matrix[0, 0] * matrix[1, 1] -
                  matrix[0, 1] * matrix[1, 0]);
            }
            else if (matrixSide == 1)
            {
                return matrix[0, 0];
            }
            else if (matrixSide >= 3)
            {
                double[,] MatrixForDeterminant = new double[matrixSide - 1, matrixSide - 1];
                double MatrixDeterminant = default;

                int MatrixForDeterminantRowIndex, MatrixForDeterminantColumnIndex;

                for (int RowElementsIndex = 0; RowElementsIndex < matrixSide; ++RowElementsIndex)
                {
                    MatrixForDeterminantRowIndex = default;

                    for (int RowIndex = 1; RowIndex < matrixSide; ++RowIndex)
                    {
                        MatrixForDeterminantColumnIndex = default;

                        for (int ColumnIndex = 0; ColumnIndex < matrixSide; ++ColumnIndex)
                        {
                            if (ColumnIndex != RowElementsIndex)
                            {
                                MatrixForDeterminant[MatrixForDeterminantRowIndex, MatrixForDeterminantColumnIndex] =
                                  matrix[RowIndex, ColumnIndex];
                                ++MatrixForDeterminantColumnIndex;
                            }
                        }

                        ++MatrixForDeterminantRowIndex;
                    }

                    MatrixDeterminant += Math.Pow(-1, RowElementsIndex + 2) * matrix[0, RowElementsIndex]
                      * Determinant(MatrixForDeterminant, matrixSide - 1);
                }

                return MatrixDeterminant;
            }
            else
            {
                throw new Exception("Неверное значение стороны!");
            }
        }
        private void LineA_Click(object sender, EventArgs e)
        {

        }

        
    }
}
