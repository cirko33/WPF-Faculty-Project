using NetworkService.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetworkService.ViewModel
{
    public class MainWindowViewModel : BindableBase
    {
        public MyICommand<string> NavCommand { get; private set; }
        public MyICommand ChangeCommand { get; set; }

        static NetworkEntitiesViewModel networkEntitiesViewModel = new NetworkEntitiesViewModel();
        static MeasurementGraphViewModel measurementGraphViewModel = new MeasurementGraphViewModel();
        static NetworkDisplayViewModel networkDisplayViewModel = new NetworkDisplayViewModel();

        BindableBase currentViewModel;

        public BindableBase CurrentViewModel
        {
            get => currentViewModel;
            set
            {
                SetProperty(ref currentViewModel, value);
            }
        }

        public static bool UseToolTips { get; set; } = true;
        public MainWindowViewModel()
        {
            createListener(); //Povezivanje sa serverskom aplikacijom
            NavCommand = new MyICommand<string>(OnNav);
            ChangeCommand = new MyICommand(Change);
            CurrentViewModel = networkEntitiesViewModel;
        }

        private void Change()
        {
            if (CurrentViewModel == networkDisplayViewModel)
                CurrentViewModel = measurementGraphViewModel;
            else if (CurrentViewModel == measurementGraphViewModel)
                CurrentViewModel = networkEntitiesViewModel;
            else if (CurrentViewModel == networkEntitiesViewModel)
                CurrentViewModel = networkDisplayViewModel;
        }

        private void OnNav(string dest)
        {
            switch (dest)
            {
                case "NetEnt":
                    CurrentViewModel = networkEntitiesViewModel;
                    break;
                case "NetDis":
                    CurrentViewModel = networkDisplayViewModel;
                    break;
                case "MesGraph":
                    CurrentViewModel = measurementGraphViewModel;
                    break;
            }
        }

        private void createListener()
        { 
            var tcp = new TcpListener(IPAddress.Any, 25565);
            tcp.Start();

            var listeningThread = new Thread(() =>
            {
                while (true)
                {
                    var tcpClient = tcp.AcceptTcpClient();
                    ThreadPool.QueueUserWorkItem(param =>
                    {
                        //Prijem poruke
                        NetworkStream stream = tcpClient.GetStream();
                        string incomming;
                        byte[] bytes = new byte[1024];
                        int i = stream.Read(bytes, 0, bytes.Length);
                        //Primljena poruka je sacuvana u incomming stringu
                        incomming = System.Text.Encoding.ASCII.GetString(bytes, 0, i);

                        //Ukoliko je primljena poruka pitanje koliko objekata ima u sistemu -> odgovor
                        if (incomming.Equals("Need object count"))
                        {
                            //Response
                            /* Umesto sto se ovde salje count.ToString(), potrebno je poslati 
                             * duzinu liste koja sadrzi sve objekte pod monitoringom, odnosno
                             * njihov ukupan broj (NE BROJATI OD NULE, VEC POSLATI UKUPAN BROJ)
                             * */
                            Byte[] data = System.Text.Encoding.ASCII.GetBytes(NetworkEntitiesViewModel.Struje.Count.ToString());
                            stream.Write(data, 0, data.Length);
                        }
                        else
                        {
                            //U suprotnom, server je poslao promenu stanja nekog objekta u sistemu
                            Console.WriteLine(incomming); //Na primer: "Entitet_1:272"

                            //################ IMPLEMENTACIJA ####################
                            // Obraditi poruku kako bi se dobile informacije o izmeni
                            // Azuriranje potrebnih stvari u aplikaciji

                            if (NetworkEntitiesViewModel.Struje.Count > 0)
                            {
                                var splited = incomming.Split(':');
                                DateTime dt = DateTime.Now;
                                using (StreamWriter sw = File.AppendText("Log.txt"))
                                    sw.WriteLine(dt + ": " + splited[0] + ", " + splited[1]);

                                int id = Int32.Parse(splited[0].Split('_')[1]);
                                NetworkEntitiesViewModel.Struje[id].Valued = Double.Parse(splited[1]);
                                NetworkDisplayViewModel.UpdateList(NetworkEntitiesViewModel.Struje[id]);
                                MeasurementGraphViewModel.Update(splited[0], new Entity(Double.Parse(splited[1]), dt));
                            }
                        }
                    }, null);
                }
            });

            listeningThread.IsBackground = true;
            listeningThread.Start();
        }
    }
}
