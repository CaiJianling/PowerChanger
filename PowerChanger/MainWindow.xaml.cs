using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PowerChanger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Initvalue();
        }

        private void Initvalue()
        {
            int currentMaxCpu = GetCurrentMaxCpuValue();
            GetOriginValue.Text = currentMaxCpu.ToString();
            InputChangeValue.Text = currentMaxCpu.ToString();
        }

        private void ChangeValue()
        {
            string input = InputChangeValue.Text;
            int maxCpuValue;
            if (int.TryParse(input, out maxCpuValue))
            {
                if (maxCpuValue > 0)
                {
                    if (maxCpuValue <= 10)
                    {
                        // 弹出确认框
                        MessageBoxResult result = MessageBox.Show("您设置的处理器最大电源值过小可能导致系统运行卡顿，确定要执行此操作吗？", "确认操作", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        // 根据用户的选择执行相应的操作
                        switch (result)
                        {
                            case MessageBoxResult.Yes:
                                // 用户选择了“是”
                                break;
                            case MessageBoxResult.No:
                                // 用户选择了“否”
                                return;
                        }
                    }
                }
                else
                {
                    // 弹出确认框
                    MessageBoxResult result = MessageBox.Show("不推荐将数值设置为 0 ，确定要执行此操作吗？", "确认操作", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    // 根据用户的选择执行相应的操作
                    switch (result)
                    {
                        case MessageBoxResult.Yes:
                            // 用户选择了“是”
                            break;
                        case MessageBoxResult.No:
                            // 用户选择了“否”
                            return;
                    }
                }
                SetMaxCpuValue(maxCpuValue);
                MessageBox.Show("设置成功！", "信息", MessageBoxButton.OK, MessageBoxImage.Information);
                int currentMaxCpu = GetCurrentMaxCpuValue();
                GetOriginValue.Text = currentMaxCpu.ToString();
            }
            else
            {
                MessageBox.Show("输入无效，请输入一个有效的数字。", "警告", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void ChangeButton_Click(object sender, RoutedEventArgs e)
        {
            ChangeValue();
        }

        static int GetCurrentMaxCpuValue()
        {
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = "/c powercfg -q SCHEME_CURRENT SUB_PROCESSOR PROCTHROTTLEMAX | find \"当前交流电源设置索引: \"";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            int startIndex = output.IndexOf("当前交流电源设置索引: ") + "当前交流电源设置索引: ".Length;
            string hexValue = output.Substring(startIndex).Trim();
            int decimalValue = Convert.ToInt32(hexValue, 16);
            return decimalValue;
        }

        static void SetMaxCpuValue(int value)
        {
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = $"/c powercfg -setacvalueindex SCHEME_CURRENT SUB_PROCESSOR PROCTHROTTLEMAX {value} && powercfg -s SCHEME_CURRENT";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
            process.WaitForExit();
        }

        private void InputChangeValue_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox? txt = sender as TextBox;

            // 过滤按键
            if ((e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9))
            {
                e.Handled = false;
            }
            else if (((e.Key >= Key.D0 && e.Key <= Key.D9)))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }

            if (e.Key == Key.Enter)
            {
                // 阻止默认行为（例如，输入框中的回车会导致换行）
                e.Handled = true;
                // 触发按钮的点击事件
                ChangeValue();
            }
        }

        private void InputChangeValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                string strNum = InputChangeValue.Text;
                if ("" == strNum || null == strNum)
                {
                    return;
                }
                int num = int.Parse(InputChangeValue.Text);
                InputChangeValue.Text = num.ToString();
                if (num <= 100)
                {
                    return;
                }
                else
                {
                    InputChangeValue.Text = InputChangeValue.Text.Remove(2);
                    InputChangeValue.SelectionStart = InputChangeValue.Text.Length;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void InputChangeValue_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
        }

        private void AddInputValue_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int inputValue = int.Parse(InputChangeValue.Text);
                if (inputValue < 100)
                {
                    inputValue++;
                    InputChangeValue.Text = inputValue.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void SubInputValue_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int inputValue = int.Parse(InputChangeValue.Text);
                if (inputValue > 0)
                {
                    inputValue--;
                    InputChangeValue.Text = inputValue.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}