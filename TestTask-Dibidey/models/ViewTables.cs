using DapperMvcApp.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.IO;
using TestTask_Dibidey.models;
using System.Windows.Input;

namespace TestTask_Dibidey.ViewModels
{
    public class AbonentViewModel
    {
        public string FullName { get; set; }
        public string HomePhone { get; set; }
        public string WorkPhone { get; set; }
        public string PersonalPhone { get; set; }
        public string Street { get; set; }
    }
    public class StreetViewModel
    {
        public string Street { get; set; }
        public int Amount { get; set; }
    }
    
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<AbonentViewModel> _abonents;
        public ObservableCollection<AbonentViewModel> Abonents
        {
            get { return _abonents; }
            set
            {
                _abonents = value;
                OnPropertyChanged(nameof(Abonents));
            }
        }
        public MainWindowViewModel()
        {
            Abonents = new ObservableCollection<AbonentViewModel>();
            ConstructAbonents();
        }

        public void ConstructAbonents()
        {
            Abonents.Clear();

            var db = new UserRepository();
            var baseAbonents = db.All<abonent>();
            var numbers = db.All<phone_number>();
            var addresses = db.All<address>();

            foreach (var abonent in baseAbonents)
            {
                var abonentNumbers = numbers.FirstOrDefault(n => n.abonent == abonent.uuid);
                var address = addresses.FirstOrDefault(a => a.abonent == abonent.uuid);

                Abonents.Add(new AbonentViewModel
                {
                    FullName = abonent.full_name,
                    HomePhone = abonentNumbers.home_number,
                    PersonalPhone = abonentNumbers.personal_number,
                    Street = address.address_text,
                    WorkPhone = abonentNumbers.work_number
                });
            }
        }

        public void Search(string argument)
        {
            ConstructAbonents();

            var filteredAbonents = Abonents.Where(abonent => abonent.PersonalPhone == argument ||
                                                             abonent.WorkPhone == argument ||
                                                             abonent.HomePhone == argument).ToList();

            if (filteredAbonents.Count > 0) Abonents = new ObservableCollection<AbonentViewModel>(filteredAbonents);
            else System.Windows.Forms.MessageBox.Show("No abonents were found");
        }

        public void ExportToCSV()
        {
            var csv = new StringBuilder();
            csv.AppendLine(string.Join(",", typeof(AbonentViewModel).GetProperties().Select(p => p.Name)));

            foreach (var abonent in Abonents)
            {
                var newLine = string.Format("{0},{1},{2},{3},{4}", abonent.FullName, abonent.Street, abonent.HomePhone, abonent.WorkPhone, abonent.PersonalPhone);
                csv.AppendLine(newLine);
            }

            File.WriteAllText($"./report_{DateTime.Now.ToString("dd.MM.yyyy_HH-mm-ss")}.csv", csv.ToString());
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class SearchWindowViewModel : INotifyPropertyChanged
    {
        private string _text;
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                OnPropertyChanged(nameof(Text));
            }
        }

        public ICommand OkCommand { get; private set; }

        public SearchWindowViewModel()
        {
            OkCommand = new RelayCommand(Ok);
        }

        private void Ok(object parameter)
        {
            CloseWindow();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void CloseWindow()
        {
            foreach (var window in System.Windows.Application.Current.Windows)
            {
                if (window is SearchWindow searchWindow)
                {
                    searchWindow.DialogResult = true;
                    searchWindow.Close();
                    break;
                }
            }
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }

    public class StreetsWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<StreetViewModel> _streets;
        public ObservableCollection<StreetViewModel> Streets
        {
            get { return _streets; }
            set
            {
                _streets = value;
                OnPropertyChanged(nameof(Streets));
            }
        }

        public StreetsWindowViewModel()
        {
            Streets = new ObservableCollection<StreetViewModel>();
            GenerateStreets();
        }

        public void GenerateStreets()
        {
            Streets.Clear();

            var db = new UserRepository();
            var addresses = db.All<address>();
            var streets = db.All<streets>();

            foreach (var street in streets)
            {
                var amount = addresses.Count(a => a.address_text == street.address);
                var view = new StreetViewModel
                {
                    Street = street.address,
                    Amount = amount
                };
                Streets.Add(view);
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

    