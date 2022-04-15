using NetworkService.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using NetworkService.Views;
using System.Windows.Input;

namespace NetworkService.ViewModel
{
    public class NetworkDisplayViewModel : BindableBase
    {
        public static void RemoveFromList(Struja s)
        {
            foreach (Struja struja in StrujaList)
                if (struja.Id == s.Id)
                {
                    StrujaList.Remove(struja);
                    return;
                }

            for (int i = 0; i < 12; i++)
                if(Canvases[i].Struja.Id == s.Id)
                {
                    foreach (int id in Canvases[i].Lines)
                        RemoveLine(id);
                    Canvases[i] = new CanvasInfo(i);
                    return;
                }
        }

        public static void UpdateList(Struja s)
        {
            for (int i = 0; i < StrujaList.Count; i++)
                if(StrujaList[i].Id == s.Id)
                {
                    StrujaList[i].Valued = s.Valued;
                    return;
                }

            for (int i = 0; i < 12; i++)
                if (Canvases[i].Struja.Id == s.Id)
                {
                    Canvases[i].Struja = s;
                    return;
                }
        }
        public static ObservableCollection<Struja> StrujaList { get; set; }
        public static ObservableCollection<CanvasInfo> Canvases { get; set; }
        public static ObservableCollection<Line> Lines { get; set; }
        public MyICommand<ListView> SelectionChangedCommand { get; set; }
        public MyICommand MouseLeftButtonUpCommand { get; set; }
        public MyICommand<Canvas> ButtonCommand { get; set; }
        public MyICommand<Canvas> DragOverCommand { get; set; }
        public MyICommand<Canvas> DropCommand { get; set; }
        public MyICommand<Canvas> MouseLeftButtonDownCommand { get; set; }
        public MyICommand AutoPlaceCommand { get; set; }
        public MyICommand HelpCommand { get; set; }
        bool toolTipsBool;
        public bool ToolTipsBool
        {
            get => toolTipsBool;
            set
            {
                toolTipsBool = value;
                MainWindowViewModel.UseToolTips = value;
                OnPropertyChanged("ToolTipsBool");
            }
        }

        string helpText;
        static string saveHelp = "";
        public string HelpText { get => helpText; 
            set {
                helpText = value;
                saveHelp = value;
                OnPropertyChanged("HelpText"); 
            } 
        }
        bool dragging = false;
        Struja selectedStruja;
        public Struja SelectedStruja { get => selectedStruja;
            set
            {
                selectedStruja = value;
                OnPropertyChanged("SelectedStruja");
            }
                
        }

        CanvasInfo currentCanvas;
        public CanvasInfo CurrentCanvas
        {
            get => currentCanvas;
            set
            {
                currentCanvas = value;
                OnPropertyChanged("CurrentCanvas");
            }
        }

        

        bool Cmp(CanvasInfo c)
        {
            return CurrentCanvas.Struja == c.Struja && CurrentCanvas.Taken == c.Taken && CurrentCanvas.Text == c.Text;
        }

        private void OnAutoPlace()
        {
            List<Struja> temp = new List<Struja>();
            foreach (Struja s in StrujaList)
            {
                for (int i = 0; i < 12; i++)
                {
                    if (!Canvases[i].Taken)
                    {
                        Canvases[i] = new CanvasInfo(s, true, i);
                        temp.Add(s);
                        break;
                    }
                }
            }

            foreach (Struja s in temp)
               StrujaList.Remove(s);
        }

        public NetworkDisplayViewModel()
        {
            if (StrujaList == null)
                StrujaList = new ObservableCollection<Struja>();
            if (Canvases == null)
            {
                Canvases = new ObservableCollection<CanvasInfo>();
                for (int i = 0; i < 12; i++)
                    Canvases.Add(new CanvasInfo(i));
            }
            if (Lines == null)
                Lines = new ObservableCollection<Line>();

            DragOverCommand = new MyICommand<Canvas>(DragOver);
            DropCommand = new MyICommand<Canvas>(Drop);
            ButtonCommand = new MyICommand<Canvas>(ButtonCommandFreeing);
            SelectionChangedCommand = new MyICommand<ListView>(SelectionChanged);
            MouseLeftButtonUpCommand = new MyICommand(MouseLeftButtonUp);
            MouseLeftButtonDownCommand = new MyICommand<Canvas>(MouseLeftButtonDown);
            AutoPlaceCommand = new MyICommand(OnAutoPlace);
            HelpCommand = new MyICommand(OnHelp);
            helpText = saveHelp;
            ToolTipsBool = MainWindowViewModel.UseToolTips;
        }

        private void OnHelp()
        {
            if(HelpText == "")
            {
                HelpText = "Prečice: CTRL+D -> Automatsko stavljanje entiteta na mesta, F1->Help\n" +
                    "Prevlačenjem iz liste u neko polje ce rezultirati prebacivanjem entiteta iz liste" +
                    "u to polje za prikaz trenutnog stanja tog entiteta.\nPrevlačenjem entiteta iz polja" +
                    " u polje ce rezultirati prebacivanjem entiteta iz polja u polje.\nPovlačenje linije" +
                    "izmedju 2 entiteta se radi povlačenjem prvog zauzetog polja na drugo polje.";
            }
            else
            {
                HelpText = "";
            }
        }

        void ChangeLine(int id, int x, int y, int nx, int ny)
        {
            for (int i = 0; i < Lines.Count; i++)
            {
                if(Lines[i].Id == id)
                {
                    if(Lines[i].X1 == x && Lines[i].Y1 == y)
                    {
                        Lines[i].X1 = nx;
                        Lines[i].Y1 = ny;
                    }
                    else
                    {
                        Lines[i].X2 = nx;
                        Lines[i].Y2 = ny;
                    }
                    return;
                }
            }
        }

        private void Drop(Canvas obj)
        {
            if(SelectedStruja != null)
            {
                int id = int.Parse(obj.Name.Substring(1));
                if (!Canvases[id].Taken)
                {
                    Canvases[id] = new CanvasInfo(SelectedStruja, true, id);
                    StrujaList.Remove(SelectedStruja);
                }
            }
            else if(CurrentCanvas != null)
            {
                int id = int.Parse(obj.Name.Substring(1));
                if (!Canvases[id].Taken)
                {
                    for (int i = 0; i < 12; i++)
                        if (Cmp(Canvases[i]))
                        {
                            Canvases[i] = new CanvasInfo(i);
                            break;
                        }
                    Canvases[id] = new CanvasInfo(CurrentCanvas.Struja, true, id);
                    foreach (int i in CurrentCanvas.Lines)
                    {
                        ChangeLine(i, CurrentCanvas.X, CurrentCanvas.Y, Canvases[id].X, Canvases[id].Y);
                        Canvases[id].Lines.Add(i);
                    }
                }
                else
                {
                    for (int i = 0; i < 12; i++)
                        if (Cmp(Canvases[i]))
                        {
                            Line line = new Line(Canvases[i].X, Canvases[id].X, Canvases[i].Y, Canvases[id].Y);
                            Lines.Add(line);
                            Canvases[i].Lines.Add(line.Id);
                            Canvases[id].Lines.Add(line.Id);
                            break;
                        }
                }
            }
            MouseLeftButtonUp();
        }

        private void DragOver(Canvas obj)
        {
            int id = int.Parse(obj.Name.Substring(1));
            if (!Canvases[id].Taken)
                obj.AllowDrop = true;
            else
                obj.AllowDrop = false;
        }

        private void MouseLeftButtonUp()
        {
            SelectedStruja = null;
            CurrentCanvas = null;
            dragging = false;
        }


        private void MouseLeftButtonDown(Canvas c)
        {
            int id = int.Parse(c.Name.Substring(1));
            if (Canvases[id].Taken)
            {
                CurrentCanvas = Canvases[id];
                if (!dragging)
                {
                    dragging = true;
                    DragDrop.DoDragDrop(c, CurrentCanvas, DragDropEffects.Copy | DragDropEffects.Move);
                }
            }
        }

        static void RemoveLine(int id)
        {
            for (int i = 0; i < Lines.Count; i++)
            {
                if(Lines[i].Id == id)
                {
                    Lines.RemoveAt(i);
                    return;
                }
            }
        }
        private void ButtonCommandFreeing(Canvas obj)
        {
            int id = int.Parse(obj.Name.Substring(1));
            if (Canvases[id].Taken)
            {
                foreach (int i in Canvases[id].Lines)
                    RemoveLine(i);
                StrujaList.Add(Canvases[id].Struja);
                Canvases[id] = new CanvasInfo(id);
            }
        }

        private void SelectionChanged(ListView obj)
        {
            if (!dragging)
            {
                dragging = true;
                DragDrop.DoDragDrop(obj, SelectedStruja, DragDropEffects.Copy | DragDropEffects.Move);
            }
        }
    }
}
