using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using MessageBox = System.Windows.MessageBox;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;
using NPOI.SS.UserModel;
using System.IO;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using System.ComponentModel;

namespace PKAD_STEGANOGRAPHY_SECURITY_REPORT
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Renderer renderer = null;
        private int currentChartIndex = 0;
        private string exportFolderPath = "";
        public MainWindow()
        {
            renderer = null;
            InitializeComponent();
        }
        private object getCellValue(ICell cell)
        {
            object cValue = string.Empty;
            switch (cell.CellType)
            {
                case (CellType.Unknown | CellType.Formula | CellType.Blank):
                    cValue = cell.ToString();
                    break;
                case CellType.Numeric:
                    cValue = cell.NumericCellValue;
                    break;
                case CellType.String:
                    cValue = cell.StringCellValue;
                    break;
                case CellType.Boolean:
                    cValue = cell.BooleanCellValue;
                    break;
                case CellType.Error:
                    cValue = cell.ErrorCellValue;
                    break;
                default:
                    cValue = string.Empty;
                    break;
            }
            return cValue;
        }
        private void btnImportCSV_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "xlsx files (*.xlsx)|*.xlsx|All files (*.*)|*.*";

            List<BallotData> data = new List<BallotData>();

            string errMsg = "";

            if ( openFileDialog.ShowDialog() == true)
            {
                string batchID = "";
                try
                {
                    IWorkbook workbook = null;
                    string fileName = openFileDialog.FileName;

                    using ( FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read) )
                    {
                        if (fileName.IndexOf(".xlsx") > 0)
                            workbook = new XSSFWorkbook(fs);
                        else if (fileName.IndexOf(".xls") > 0)
                            workbook = new HSSFWorkbook(fs);

                    }

                    ISheet sheet = workbook.GetSheetAt(0);

                    if (sheet != null)
                    {
                        int rowCount = sheet.LastRowNum;
                        for (int i = 0; i <= rowCount; i++)
                        {
                            IRow curRow = sheet.GetRow(i);
                            if (curRow == null)
                            {
                                rowCount = i - 1;
                                break;
                            }
                            if (curRow.Cells.Count == 1)
                            {
                                batchID = (getCellValue(curRow.GetCell(0)) is string) ? curRow.GetCell(0).StringCellValue : "";
                            }
                            else if (curRow.Cells.Count == 12)
                            {
                                //If header, then continue;
                                if (i == 1) continue;
                                data.Add(new BallotData()
                                {
                                    filepath = (getCellValue(curRow.GetCell(0)) is string) ? curRow.GetCell(0).StringCellValue.Trim() : "",
                                    filename = (getCellValue(curRow.GetCell(1)) is string) ? curRow.GetCell(1).StringCellValue.Trim() : "",
                                    is_color = (getCellValue(curRow.GetCell(2)) is string) ? curRow.GetCell(2).StringCellValue.Trim() : "",
                                    mic = (getCellValue(curRow.GetCell(3)) is string) ? curRow.GetCell(3).StringCellValue.Trim() : "",
                                    ooc = (getCellValue(curRow.GetCell(4)) is string) ? curRow.GetCell(4).StringCellValue.Trim() : "",
                                    code = (getCellValue(curRow.GetCell(5)) is string) ? curRow.GetCell(5).StringCellValue.Trim() : "",
                                    color = (getCellValue(curRow.GetCell(6)) is string) ? curRow.GetCell(6).StringCellValue.Trim() : "",
                                    type = (getCellValue(curRow.GetCell(7)) is string) ? curRow.GetCell(7).StringCellValue.Trim() : "",
                                    precinct = (getCellValue(curRow.GetCell(8)) is string) ? curRow.GetCell(8).StringCellValue.Trim() : "",
                                    flag = (getCellValue(curRow.GetCell(9)) is string) ? curRow.GetCell(9).StringCellValue.Trim() : "",
                                    fed = (getCellValue(curRow.GetCell(10)) is string) ? curRow.GetCell(10).StringCellValue.Trim() : "",
                                    letter = (getCellValue(curRow.GetCell(11)) is string) ? curRow.GetCell(11).StringCellValue.Trim() : "",
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    string msg = ex.GetType().FullName;
                    if (msg == "System.IO.IOException")
                        MessageBox.Show("The file is open by another process", "Error");
                    else
                    {
                        //MessageBox.Show(errMsg,  "Error");
                    }
                }

                if ( data.Count > 0 )
                {
                    Dictionary<string, int> precinct_map = new Dictionary<string, int>();
                    foreach (var item in data)
                    {
                        item.precinct_key = "";
                        item.precinct_name = "";
                        if (item.precinct.Length >= 4)
                        {
                            item.precinct_key = item.precinct.Substring(0, 4);
                            item.precinct_name = item.precinct.Substring(4).Trim();
                        }
                        else
                        {
                            item.precinct_name = item.precinct;
                        }

                        if (precinct_map.ContainsKey(item.precinct_name))
                            precinct_map[item.precinct_name]++;
                        else precinct_map[item.precinct_name] = 1;
                    }

                    renderer.setChatData(data, precinct_map,  batchID);
                    currentChartIndex = 0;
                    seeNext.Visibility = Visibility.Hidden;
                    seePrevious.Visibility = Visibility.Hidden;
                    Render();
                }
            }
        }
        void Render()
        {
            if (renderer == null)
                renderer = new Renderer((int)myCanvas.ActualWidth, (int)myCanvas.ActualHeight);
            if (renderer.getDataCount() > 0)
            {
                renderer.setRenderSize((int)myCanvas.ActualWidth, (int)myCanvas.ActualHeight);
                if (renderer.getPrecinctCount() > 64)
                {
                    seeNext.Visibility = Visibility.Visible;
                    seePrevious.Visibility = Visibility.Visible;
                    seePrevious.IsEnabled = false;
                }
                renderer.draw(currentChartIndex);
                myImage.Source = BmpImageFromBmp(renderer.getBmp());
            }
        }
        private void myCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            Render();
        }
        private void myCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //Render();
        }
        private void btnExportCurrentChart_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Image file (*.png)|*.png";
            //saveFileDialog.Filter = "Image file (*.png)|*.png|PDF file (*.pdf)|*.pdf";
            if (saveFileDialog.ShowDialog() == true)
            {
                SaveControlImage(PrecinctChart, saveFileDialog.FileName);
            }
        }
        private void SaveControlImage(FrameworkElement control, string filename)
        {
            RenderTargetBitmap rtb = (RenderTargetBitmap)CreateBitmapFromControl(control);
            // Make a PNG encoder.
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(rtb));

            // Save the file.
            using (FileStream fs = new FileStream(filename,
                FileMode.Create, FileAccess.Write, FileShare.None))
            {
                encoder.Save(fs);
            }
        }
        public BitmapSource CreateBitmapFromControl(FrameworkElement element)
        {
            // Get the size of the Visual and its descendants.
            Rect rect = VisualTreeHelper.GetDescendantBounds(element);

            // Make a DrawingVisual to make a screen
            // representation of the control.
            DrawingVisual dv = new DrawingVisual();

            // Fill a rectangle the same size as the control
            // with a brush containing images of the control.
            using (DrawingContext ctx = dv.RenderOpen())
            {
                VisualBrush brush = new VisualBrush(element);
                ctx.DrawRectangle(brush, null, new Rect(rect.Size));
            }

            // Make a bitmap and draw on it.
            int width = (int)element.ActualWidth;
            int height = (int)element.ActualHeight;
            RenderTargetBitmap rtb = new RenderTargetBitmap(
                width, height, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(dv);
            return rtb;
        }
        private BitmapImage BmpImageFromBmp(Bitmap bmp)
        {
            using (var memory = new System.IO.MemoryStream())
            {
                bmp.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
        }

        private int getChartCount()
        {
            int precinctCount = renderer.getPrecinctCount();
            int chartCount = 1;
            chartCount = chartCount + precinctCount / 64;
            if (precinctCount % 64 == 0) chartCount--;
            return chartCount;
        }
        private void drawPreviousChart(object sender, RoutedEventArgs e)
        {
            int chartCount = getChartCount();

            currentChartIndex--;

            if (currentChartIndex == 0)
            {
                seePrevious.IsEnabled = false;
                seeNext.IsEnabled = true;
            }
            else if (currentChartIndex == chartCount - 1)
            {
                seePrevious.IsEnabled = true;
                seeNext.IsEnabled = false;
            }
            else
            {
                seePrevious.IsEnabled = true;
                seeNext.IsEnabled = true;
            }
            renderer.draw(currentChartIndex);
            myImage.Source = BmpImageFromBmp(renderer.getBmp());

        }

        private void drawNextChart(object sender, RoutedEventArgs e)
        {
            int chartCount = getChartCount();
            currentChartIndex++;

            if (currentChartIndex == 0)
            {
                seePrevious.IsEnabled = false;
                seeNext.IsEnabled = true;
            }
            else if (currentChartIndex == chartCount - 1)
            {
                seePrevious.IsEnabled = true;
                seeNext.IsEnabled = false;
            }
            else
            {
                seePrevious.IsEnabled = true;
                seeNext.IsEnabled = true;
            }
            renderer.draw(currentChartIndex);
            myImage.Source = BmpImageFromBmp(renderer.getBmp());
        }
        private void SaveBitmapImagetoFile(BitmapImage image, string filePath)
        {
            //PngBitmapEncoder encoder1 = new PngBitmapEncoder();
            //encoder1.Frames.Add(BitmapFrame.Create(image));

            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));

            try
            {
                using (var fileStream = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
                {
                    encoder.Save(fileStream);
                }
            }
            catch (Exception ex)
            {

            }


        }
        private void btnExportChart_Click(object sender, RoutedEventArgs e)
        {
            if (renderer == null)
            {
                renderer = new Renderer((int)myCanvas.ActualWidth, (int)myCanvas.ActualHeight);
            } 
            if (renderer.getPrecinctCount() > 0 )
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Image file (*.png)|*.png";
                //saveFileDialog.Filter = "Image file (*.png)|*.png|PDF file (*.pdf)|*.pdf";
                if (saveFileDialog.ShowDialog() == true)
                {
                    string filename = saveFileDialog.FileName;
                    exportFolderPath = saveFileDialog.FileName;

                    BackgroundWorker worker = new BackgroundWorker();
                    worker.WorkerReportsProgress = true;
                    worker.DoWork += worker_DoExport;
                    worker.ProgressChanged += worker_ProgressChanged;
                    worker.RunWorkerAsync();
                    worker.RunWorkerCompleted += worker_CompletedWork;

                }
            }
        }

        void worker_DoExport(object sender, DoWorkEventArgs e)
        {
            int chartCount = getChartCount();
            for (int index = 0; index < chartCount; index++)
            {
                renderer.draw(index);

                string filename = exportFolderPath.Substring(0, exportFolderPath.Length - 4) + "-" + index.ToString() + ".png";
                SaveBitmapImagetoFile(BmpImageFromBmp(renderer.getBmp()), filename);
            }

        }
        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
        }
        void worker_CompletedWork(object sender, RunWorkerCompletedEventArgs e)
        {
            string msg = "Exporting has been done\n";
            MessageBox.Show(msg);
        }
    }
}
