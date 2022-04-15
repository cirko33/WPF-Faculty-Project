using NetworkService.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace NetworkService.ViewModel
{
    public class NetworkEntitiesViewModel : BindableBase
    {
        public static ObservableCollection<Struja> Struje { get; set; }
        public static ICollectionView PrikazStruja { get; set; }
        public static List<TypeStruja> Tipovi { get; set; }
        public MyICommand AddCommand { get; set; }
        public MyICommand DeleteCommand { get; set; }
        public MyICommand FilterCommand { get; set; }
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

        Struja novaStruja = new Struja();
        public Struja NovaStruja { get => novaStruja;
            set
            {
                novaStruja = value;
                OnPropertyChanged("NovaStruja");
            }
        }

        Struja izabran;
        public Struja Izabran { get => izabran;
            set
            {
                izabran = value;
                DeleteCommand.RaiseCanExecuteChanged();
            }
        }

        bool filterL;
        public bool FilterL
        {
            get => filterL;
            set
            {
                if (filterL != value)
                {
                    filterL = value;
                    OnPropertyChanged("FilterL");
                    FilterCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private bool filterM;
        public bool FilterM
        {
            get => filterM;
            set
            {
                if (filterM != value)
                {
                    filterM = value;
                    OnPropertyChanged("FilterM");
                    FilterCommand.RaiseCanExecuteChanged();
                }
            }
        }

        int filterID;
        public int FilterID
        {
            get => filterID;
            set
            {
                filterID = value;
                OnPropertyChanged("FilterID");
                FilterCommand.RaiseCanExecuteChanged();
            }
        }

        string filterIDGreska;
        public string FilterIDGreska
        {
            get => filterIDGreska;
            set
            {
                filterIDGreska = value;
                OnPropertyChanged("FilterIDGreska");
            }
        }

        TypeStruja filterTip;
        public TypeStruja FilterTip
        {
            get => filterTip;
            set
            {
                filterTip = value;
                OnPropertyChanged("Tip");
                FilterCommand.RaiseCanExecuteChanged();
            }
        }

        public NetworkEntitiesViewModel()
        {
            if(Struje == null)
                Struje = new ObservableCollection<Struja>();
            PrikazStruja = CollectionViewSource.GetDefaultView(Struje);
            Tipovi = new List<TypeStruja> { new TypeStruja("Interval Meter"), new TypeStruja("Smart Meter") };

            AddCommand = new MyICommand(OnAdd);
            DeleteCommand = new MyICommand(OnDelete, CanDelete);
            FilterCommand = new MyICommand(OnFilter, CanFilter);
            HelpCommand = new MyICommand(OnHelp);
            FilterID = 1;
            NovaStruja.Id = 1;
            HelpText = saveHelp;
            ToolTipsBool = MainWindowViewModel.UseToolTips;
        }

        string helpText;
        static string saveHelp = "";
        public string HelpText
        {
            get => helpText;
            set
            {
                helpText = value;
                saveHelp = value;
                OnPropertyChanged("HelpText");
            }
        }

        private void OnHelp()
        {
            if (HelpText == "")
            {
                HelpText = "Prečice: CTRL+D -> Dodavanje entiteta u listu, CTRL+TAB -> pomeranje" +
                    " između prozora CTRL+F -> Filtracija sa zadatim parametrima, F1 -> Help \n" +
                    "Za dodavanje novog entiteta potrebno je uneti jedinstveni id, naziv i izabrati " +
                    "kog je tipa entitet. Nakon toga pritisak na dugme \"Add\" ili gestikulacijom CTRL+D " +
                    "se doda u listu entiteta.\nOznačavanjem entiteta u listi i pritiskom na \"Delete\" se ukloni " +
                    "entitet iz liste.\nFiltracija je moguća na 3 načina: samo po tipu, samo po id-u i po tipu i id-u.\n" +
                    "Nakon izabranih parametara filtraciju je moguće pokrenuti na dugme \"Filter\" ili " +
                    "gestikulacijom CTRL+F";
            }
            else
            {
                HelpText = "";
            }
        }

        bool IDLM => FilterIDCheck() && (FilterL || FilterM);
        private bool CanFilter()
        {
            return IDLM || FilterTip != null;
        }

        private void OnFilter()
        {
            if (FilterTip != null)
                PrikazStruja.Filter = FilterWithType;
            else
                PrikazStruja.Filter = FilterWithoutType;
        }

        private bool FilterWithType(object obj)
        {
            Struja s = obj as Struja;
            if (IDLM)
                return (FilterL ? s.Id < FilterID : s.Id > FilterID) && FilterTip.Name == s.Type.Name;
            else
                return FilterTip.Name == s.Type.Name;
        }

        private bool FilterWithoutType(object obj)
        {
            Struja s = obj as Struja;
            return FilterL ? s.Id < FilterID : s.Id > FilterID;
        }

        private bool CanDelete()
        {
            return Izabran != null;
        }

        private void OnDelete()
        {
            NetworkDisplayViewModel.RemoveFromList(Izabran);
            MeasurementGraphViewModel.RemoveFromList(Izabran);
            Struje.Remove(Izabran);
        }

        private void OnAdd()
        {
            NovaStruja.Validate();
            if (NovaStruja.IsValid)
            {
                if (ExistsID(NovaStruja.Id))
                {
                    NovaStruja.ValidationErrors["Id"] = "ID exists in list";
                    return;
                }
                Struje.Add(new Struja(NovaStruja));
                NetworkDisplayViewModel.StrujaList.Add(new Struja(NovaStruja));
                MeasurementGraphViewModel.Struje.Add(new Struja(NovaStruja));
                NovaStruja.Id++;
            }
        }

        bool ExistsID(int id)
        {
            foreach (Struja s in Struje)
                if (s.Id == id)
                    return true;
            return false;
        }

        bool FilterIDCheck()
        {
            if (FilterID > 0)
                FilterIDGreska = "";
            else
                FilterIDGreska = "ID must be more then 0 and must be number";
            return FilterID > 0;
        }
    }
}
