using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GameOfLife
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<TextBlock> buttons = new List<TextBlock>();
        TextBlock currentHover;
        bool canhover = true;
        bool isdead = false;
        int Size = App.Size;
        public MainWindow()
        {
            InitializeComponent();
            AreaSetter.Text = Size.ToString();
            MouseDown += A_MousePress;
            Draw();
        }
        public void Draw()
        {
            Area.Background = new SolidColorBrush(Colors.Black);
            for(int i = 0; i < Size; i++)
            {
                ColumnDefinition f = new ColumnDefinition();
                f.Width = new GridLength((Area.Width) / Size);
                Area.ColumnDefinitions.Add(f);
                RowDefinition r = new RowDefinition();
                r.Height = new GridLength((Area.Height) / Size);
                Area.RowDefinitions.Add(r);
            }
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    TextBlock t = new TextBlock
                    {
                        Width = (Area.Width) / Size,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        Height = (Area.Width) / Size,
                        Background = new SolidColorBrush(Colors.Black),

                        Tag = "0",
                    };
                    t.MouseEnter += A_MouseEnter;
                    t.MouseLeave += A_MouseLeave;
                    Area.Children.Add(t);
                    Grid.SetRow(t, i);
                    Grid.SetColumn(t, j);
                    buttons.Add(t);

                }
            }
        }
        private void A_MousePress(object sender, MouseEventArgs e)
        {
            if (canhover)
            {
                if(currentHover != null)
                {
                    if (currentHover.Tag.ToString() == "0")
                        currentHover.Tag = 1;
                    else currentHover.Tag = 0;
                }              
            }
            else return;
        }
        private void A_MouseEnter(object sender, MouseEventArgs e)
        {
            if (canhover)
            {


                (sender as TextBlock).Background = new SolidColorBrush(Colors.Gray);
                currentHover = sender as TextBlock;
            }
            else return;
        }
        private void A_MouseLeave(object sender, MouseEventArgs e)
        {
            if (canhover)
            {


                TextBlock hov = sender as TextBlock;
                if (hov.Tag.ToString() == "0")
                {
                    hov.Background = new SolidColorBrush(Colors.Black);
                }
                else hov.Background = new SolidColorBrush(Colors.White);
                currentHover = null;
            }
            else return;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            Button pressed = sender as Button;
            if (canhover)
            {
                canhover = false;
                pressed.Content = "STOP";
                pressed.Background = new SolidColorBrush(Colors.Red);
            }
            else
            {
                canhover = true;
                pressed.Content = "START";
                pressed.Background = new SolidColorBrush(Colors.LimeGreen);
            }
            CheckButton(sender);
        }
        async private void CheckButton(object sender)
        {
            int gen = 0;
            Button pressed = sender as Button;
            while (!canhover)
            {
                
                Play();
                gen++;
                Generation.Text = gen.ToString();
                if (isdead)
                {
                    gen--;
                    Generation.Text = gen.ToString();
                    isdead = false;
                    canhover = true;
                    pressed.Content = "START";
                    pressed.Background = new SolidColorBrush(Colors.LimeGreen);
                    return;
                }
                await Task.Delay(500);
                
            }
           
        }
        private void Play()
        {
            List<TextBlock> GoesDead = new List<TextBlock>();

            List<TextBlock> CheckDead = new List<TextBlock>();
            List<int> CheckDeadPos = new List<int>();

            List<TextBlock> NowAlive = new List<TextBlock>();
            List<int> NowAlivePos = new List<int>();

            List<TextBlock> NewAlive = new List<TextBlock>();

            SortedSet<int> neighbors = new SortedSet<int> {1,-1, Size + 1, Size - 1, 1 - Size, - 1 - Size , Size, -Size };
            for(int i = 0; i < buttons.Count; i++)
            {
                if (Convert.ToInt32(buttons[i].Tag.ToString()) == 1)
                {
                    NowAlive.Add(buttons[i]);
                    NowAlivePos.Add(i);
                }
            }
            if(NowAlive.Count == 0)
            {
                isdead = true;
                return;
            }
            for(int i = 0; i < NowAlive.Count; i++)
            {
                int neighborsalive = 0;
                if (NowAlivePos[i] % Size == 0)
                {
                    neighbors.Remove(-1 - Size);
                    neighbors.Remove(-1);
                    neighbors.Remove(-1 + Size);
                }
                if ((NowAlivePos[i] - (Size - 1)) % Size == 0)
                {
                    neighbors.Remove(1 - Size);
                    neighbors.Remove(1);
                    neighbors.Remove(1 + Size);
                }
                foreach (int m in neighbors)
                {
                    try
                    {
                        if (Convert.ToInt32(buttons[NowAlivePos[i] + m].Tag.ToString()) == 1)
                        {                           
                            neighborsalive++;
                        }
                        else
                        {
                            CheckDead.Add(buttons[NowAlivePos[i] + m]);
                            CheckDeadPos.Add(NowAlivePos[i] + m);
                        }                        
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
                if(neighborsalive > 3 || neighborsalive < 2)
                {
                    GoesDead.Add(NowAlive[i]);
                }
                else
                {
                    NewAlive.Add(NowAlive[i]);
                }
                ResNeighbors(neighbors);
                
            }
            for (int i = 0; i < CheckDead.Count; i++)
            {
                int neighborsalive = 0;

                if (CheckDeadPos[i] % 100 == 0)
                {
                    neighbors.Remove(-1 - Size);
                    neighbors.Remove(-1);
                    neighbors.Remove(-1 + Size);
                }
                if ((CheckDeadPos[i] - 99) % 100 == 0)
                {
                    neighbors.Remove(1 - Size);
                    neighbors.Remove(1);
                    neighbors.Remove(1 + Size);
                }

                foreach (int m in neighbors)
                {
                    try
                    {
                        if (Convert.ToInt32(buttons[CheckDeadPos[i] + m].Tag.ToString()) == 1)
                        {
                            neighborsalive++;
                        }
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
                if (neighborsalive == 3)
                {
                    NewAlive.Add(CheckDead[i]);
                }
                ResNeighbors(neighbors);
            }
            foreach(TextBlock b in NewAlive)
            {
                b.Tag = "1";
                b.Background = new SolidColorBrush(Colors.White);
            }
            foreach(TextBlock b in GoesDead)
            {
                b.Tag = "0";
                b.Background = new SolidColorBrush(Colors.Black);               
            }
        }
        public void ResNeighbors(SortedSet<int> n)
        {
            n.Add(-1 - Size);
            n.Add(-1);
            n.Add(-1 + Size);
            n.Add(1 - Size);
            n.Add(1);
            n.Add(1 + Size);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(uint.TryParse((sender as TextBox).Text, out uint res))
            { 
                if(res == Size)
                {
                    SetButton.IsEnabled = false;
                    return;
                }
                else
                {
                    SetButton.IsEnabled = true;
                }
            }
            else
            {
                SetButton.IsEnabled = false;
            }
        }

        private void SetButton_Click(object sender, RoutedEventArgs e)
        {
            App.Size = int.Parse(AreaSetter.Text);
            MainWindow newWindow = new MainWindow();
            newWindow.Show();
            this.Close();
        }
    }
}
