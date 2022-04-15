using NetworkService.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace NetworkService.ViewModel
{
    public class MeasurementGraphViewModel : BindableBase
    {
        public static ObservableCollection<Measured> Measure { get; set; }
        public static ObservableCollection<Struja> Struje { get; set; }
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

        static Struja saveSelected = null;
        Struja selected;
        public Struja Selected { get => selected;
            set 
            {
                if (selected != value)
                {
                    selected = value;
                    saveSelected = value;
                    OnPropertyChanged("Selected");
                    ButtonCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public static void RemoveFromList(Struja s)
        {
            foreach (Struja struja in Struje)
                if (struja.Id == s.Id)
                {
                    Struje.Remove(struja);
                    return;
                }

        }

        static void Cut(string name)
        {
            for(int i = 0; i < Measure.Count; i++)
                if(Measure[i].Name == name)
                    if(Measure[i].Entities.Count > 5)
                    {
                        Measure[i].Entities.RemoveAt(0);
                        return;
                    }
        }
        public static void Update(string name, Entity e)
        {
            if (ExistEnt(name))
            {
                AddEnt(name, e, false);
                Cut(name);
            }
            else
            {
                Measure.Add(new Measured(name, new ObservableCollection<Entity>()));
                AddEnt(name, e);
            }
        }
        public MyICommand ButtonCommand { get; set; }

        static Measured savePicked = null;
        Measured picked;
        public Measured Picked { get => picked; 
            set
            {
                picked = value;
                savePicked = value;
                OnPropertyChanged("Picked");
            }
        }

        static bool ExistEnt(string name)
        {
            foreach (Measured m in Measure)
                if (m.Name == name)
                    return true;
            return false;
        }

        static void AddEnt(string name, Entity e, bool cut = true)
        {

            for (int i = 0; i < Measure.Count; i++)
            {
                if (Measure[i].Name == name)
                {
                    if (Measure[i].Entities.Count >= 5 && cut)
                        return;
                    Measure[i].Entities.Add(e);
                    return;
                }
            }
        }

        string EntId()
        {
            for (int i = 0; i < NetworkEntitiesViewModel.Struje.Count; i++)
                if (NetworkEntitiesViewModel.Struje[i].Id == Selected.Id)
                    return "Entitet_" + i;

            return null;
        }
        void ReadFile()
        {
            if (!File.Exists("Log.txt"))
                return;
            var read = File.ReadAllLines("Log.txt").Reverse();
            foreach (string s in read)
            {
                string[] splited = s.Split(' ');
                splited[1] = splited[1].Substring(0, splited[1].Length - 1);
                splited[2] = splited[2].Substring(0, splited[2].Length - 1);
                if (!ExistEnt(splited[2]))
                    Measure.Add(new Measured(splited[2], new ObservableCollection<Entity>()));

                DateTime dt = DateTime.Parse(splited[0] + " " + splited[1]);
                AddEnt(splited[2], new Entity(double.Parse(splited[3]), dt));
            }

            for (int i = 0; i < Measure.Count; i++)
                Measure[i].Entities = new ObservableCollection<Entity>(Measure[i].Entities.Reverse().ToList());
        }

        Measured ForPicked()
        {
            string id = EntId();
            foreach (Measured m in Measure)
                if (m.Name == id)
                    return m;
            return null;
        }
   

        bool ExistsInEntitites(int id)
        {
            foreach (Struja s in Struje)
            {
                if (s.Id == id)
                    return true;
            }
            return false;
        }
        public MeasurementGraphViewModel()
        {
            if (Measure == null)
            {
                Measure = new ObservableCollection<Measured>();
                ReadFile();
            }
            if(Struje == null)
                Struje = new ObservableCollection<Struja>();
            
            ButtonCommand = new MyICommand(OnButton, CanButton);
            ToolTipsBool = MainWindowViewModel.UseToolTips;



            if (saveSelected != null)
            {
                if (ExistsInEntitites(saveSelected.Id))
                {
                    Selected = saveSelected;
                    if (savePicked != null)
                        Picked = savePicked;
                }
            }
        }

        private bool CanButton()
        {
            return Selected != null;
        }

        private void OnButton()
        {
            Picked = ForPicked();
        }
    }
}
