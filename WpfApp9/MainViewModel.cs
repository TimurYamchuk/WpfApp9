using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows.Input;
using System.Windows;
using WpfApp9;

public class MainViewModel : INotifyPropertyChanged
{
    private ObservableCollection<Person>? _people;
    private Person? _selectedPerson;
    private string? _newPersonFullName;
    private string? _newPersonAddress;
    private string? _newPersonPhoneNumber;
    public event PropertyChangedEventHandler? PropertyChanged;
    public ObservableCollection<Person> People
    {
        get => _people ??= new ObservableCollection<Person>(); 
        set
        {
            if (_people != value)
            {
                _people = value;
                OnPropertyChanged();
            }
        }
    }

    public Person? SelectedPerson
    {
        get => _selectedPerson;
        set
        {
            if (_selectedPerson != value)
            {
                _selectedPerson = value;
                OnPropertyChanged();
                if (_selectedPerson != null)
                {
                    NewPersonFullName = _selectedPerson.FullName;
                    NewPersonAddress = _selectedPerson.Address;
                    NewPersonPhoneNumber = _selectedPerson.PhoneNumber;
                }
                else
                {
                    ClearNewPersonFields();
                }
            }
        }
    }

    // Свойства для данных нового человека
    public string? NewPersonFullName
    {
        get => _newPersonFullName;
        set
        {
            if (_newPersonFullName != value)
            {
                _newPersonFullName = value;
                OnPropertyChanged();
            }
        }
    }

    public string? NewPersonAddress
    {
        get => _newPersonAddress;
        set
        {
            if (_newPersonAddress != value)
            {
                _newPersonAddress = value;
                OnPropertyChanged();
            }
        }
    }

    public string? NewPersonPhoneNumber
    {
        get => _newPersonPhoneNumber;
        set
        {
            if (_newPersonPhoneNumber != value)
            {
                _newPersonPhoneNumber = value;
                OnPropertyChanged();
            }
        }
    }

    // Команды
    public ICommand AddCommand { get; }
    public ICommand EditCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand LoadCommand { get; }

    public MainViewModel()
    {
        AddCommand = new RelayCommand(AddPerson, CanAddPerson);
        EditCommand = new RelayCommand(EditPerson, CanEditPerson);
        DeleteCommand = new RelayCommand(DeletePerson, CanDeletePerson);
        SaveCommand = new RelayCommand(SavePeople, CanSavePeople);
        LoadCommand = new RelayCommand(LoadPeople, CanLoadPeople);
    }

    private void AddPerson()
    {
        if (!string.IsNullOrEmpty(NewPersonFullName) && !string.IsNullOrEmpty(NewPersonAddress) && !string.IsNullOrEmpty(NewPersonPhoneNumber))
        {
            People.Add(new Person { FullName = NewPersonFullName ?? string.Empty, Address = NewPersonAddress ?? string.Empty, PhoneNumber = NewPersonPhoneNumber ?? string.Empty });
            ClearNewPersonFields();
        }
    }

    private bool CanAddPerson() => !string.IsNullOrEmpty(NewPersonFullName) && !string.IsNullOrEmpty(NewPersonAddress) && !string.IsNullOrEmpty(NewPersonPhoneNumber);

    private void EditPerson()
    {
        if (SelectedPerson != null)
        {
            SelectedPerson.FullName = NewPersonFullName ?? string.Empty;
            SelectedPerson.Address = NewPersonAddress ?? string.Empty;
            SelectedPerson.PhoneNumber = NewPersonPhoneNumber ?? string.Empty;
            ClearNewPersonFields();
        }
        else
        {
            MessageBox.Show("Пожалуйста, выберите контакт для редактирования.");
        }
    }

    private bool CanEditPerson() => SelectedPerson != null && !string.IsNullOrEmpty(NewPersonFullName) && !string.IsNullOrEmpty(NewPersonAddress) && !string.IsNullOrEmpty(NewPersonPhoneNumber);

    private void DeletePerson()
    {
        if (SelectedPerson != null)
        {
            People.Remove(SelectedPerson);
            SelectedPerson = null;
        }
    }

    private bool CanDeletePerson() => SelectedPerson != null;

    private void SavePeople()
    {
        SaveFileDialog saveFileDialog = new SaveFileDialog
        {
            Filter = "JSON Files (*.json)|*.json|Text Files (*.txt)|*.txt|All Files (*.*)|*.*",
            DefaultExt = "json",
            AddExtension = true
        };

        if (saveFileDialog.ShowDialog() == true)
        {
            try
            {
                string filePath = saveFileDialog.FileName;
                var options = new JsonSerializerOptions { WriteIndented = true };
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    JsonSerializer.Serialize(stream, People, options);
                }
                MessageBox.Show("Данные сохранены");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}");
            }
        }
    }

    private bool CanSavePeople() => People.Count > 0;

    private void LoadPeople()
    {
        OpenFileDialog openFileDialog = new OpenFileDialog
        {
            Filter = "JSON Files (*.json)|*.json|Text Files (*.txt)|*.txt|All Files (*.*)|*.*",
            DefaultExt = "json"
        };

        if (openFileDialog.ShowDialog() == true)
        {
            try
            {
                string filePath = openFileDialog.FileName;
                var options = new JsonSerializerOptions { WriteIndented = true };
                using (var stream = new FileStream(filePath, FileMode.Open))
                {
                    People = JsonSerializer.Deserialize<ObservableCollection<Person>>(stream) ?? new ObservableCollection<Person>();
                }
                MessageBox.Show("Данные загружены");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}");
            }
        }
    }

    private bool CanLoadPeople() => true;

    private void ClearNewPersonFields()
    {
        NewPersonFullName = string.Empty;
        NewPersonAddress = string.Empty;
        NewPersonPhoneNumber = string.Empty;
    }


    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
