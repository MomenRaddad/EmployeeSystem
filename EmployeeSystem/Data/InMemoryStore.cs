using System.Text.Json;
using EmployeeSystem.Models;

namespace EmployeeSystem.Data
{

    public class InMemoryStore
    {
        public List<DepartmentModel> Departments { get; } = new();
        public List<EmployeeModel> Employees { get; } = new();

        private readonly string _depsPath;
        private readonly string _empsPath;

        private int _depId;
        private int _empId;

        public InMemoryStore(string depsPath, string empsPath)
        {
            _depsPath = depsPath;
            _empsPath = empsPath;

            LoadFromDisk();     
            InitializeCounters();
        }

        private void LoadFromDisk()
        {
            var depsDir = Path.GetDirectoryName(_depsPath)!;
            var empsDir = Path.GetDirectoryName(_empsPath)!;
            Directory.CreateDirectory(depsDir);
            Directory.CreateDirectory(empsDir);

            if (!File.Exists(_depsPath))
                File.WriteAllText(_depsPath, "[]");
            if (!File.Exists(_empsPath))
                File.WriteAllText(_empsPath, "[]");

            var depsJson = File.ReadAllText(_depsPath);
            var empsJson = File.ReadAllText(_empsPath);

            var deps = JsonSerializer.Deserialize<List<DepartmentModel>>(depsJson) ?? new();
            var emps = JsonSerializer.Deserialize<List<EmployeeModel>>(empsJson) ?? new();

            Departments.AddRange(deps);
            Employees.AddRange(emps);
        }

        private void InitializeCounters()
        {
            _depId = Departments.Count == 0 ? 1 : Departments.Max(d => d.Id) + 1;
            _empId = Employees.Count == 0 ? 1 : Employees.Max(e => e.Id) + 1;
        }

        public int NextDepartmentId() => _depId++;
        public int NextEmployeeId() => _empId++;


        public void SaveToDisk()
        {
            // Make sure the directory exists
            Directory.CreateDirectory(Path.GetDirectoryName(_depsPath)!);
            Directory.CreateDirectory(Path.GetDirectoryName(_empsPath)!);

            var opts = new JsonSerializerOptions { WriteIndented = true };

            File.WriteAllText(_depsPath, JsonSerializer.Serialize(Departments, opts));
            File.WriteAllText(_empsPath, JsonSerializer.Serialize(Employees, opts));
        }
    }
}
