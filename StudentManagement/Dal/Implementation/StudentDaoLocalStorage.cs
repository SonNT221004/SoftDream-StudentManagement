using StudentManagement.Dal.Interface;
using StudentManagement.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StudentManagement.Dal.Implementation
{
    public class StudentDaoLocalStorage : IStudentDAO
    {
        private static readonly SemaphoreSlim _fileLock = new(1, 1);
        private readonly string _filePath;

        // storageDirectory is injected/configurable
        public StudentDaoLocalStorage(string storageDirectory)
        {
            if (string.IsNullOrWhiteSpace(storageDirectory))
                throw new ArgumentException("storageDirectory must be provided", nameof(storageDirectory));

            Directory.CreateDirectory(storageDirectory);
            _filePath = Path.Combine(storageDirectory, "Student.txt");
        }

        public async Task<List<Student>> GetAllStudentsAsync(int page, int pageSize)
        {
            await _fileLock.WaitAsync();
            try
            {
                if (!File.Exists(_filePath))
                    return new List<Student>();

                var lines = await File.ReadAllLinesAsync(_filePath);
                var list = new List<Student>();
                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;
                    var s = ParseLine(line);
                    if (s != null)
                        list.Add(s);
                }
                list = list.OrderBy(s => s.Id).ToList();
                if (pageSize > 0)
                {
                    if (page < 1)
                        page = 1;
                    var skip = (page - 1) * pageSize;
                    list = list.Skip(skip).Take(pageSize).ToList();
                }
                return list;
            }
            finally
            {
                _fileLock.Release();
            }
        }

        public async Task<Student?> GetStudentByIdAsync(int id)
        {
            await _fileLock.WaitAsync();
            try
            {
                if (!File.Exists(_filePath))
                    return null;

                var lines = await File.ReadAllLinesAsync(_filePath);
                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;
                    var s = ParseLine(line);
                    if (s != null && s.Id == id)
                        return s;
                }

                return null;
            }
            finally
            {
                _fileLock.Release();
            }
        }

        public async Task<Student> AddStudentAsync(Student student)
        {
            await _fileLock.WaitAsync();
            try
            {
                int nextId = 1;
                if (File.Exists(_filePath))
                {
                    var lines = await File.ReadAllLinesAsync(_filePath);
                    var maxId = lines.Select(l =>
                    {
                        var parts = l.Split('|');
                        return int.TryParse(parts[0], out var id) ? id : 0;
                    }).DefaultIfEmpty(0).Max();

                    nextId = maxId + 1;
                }

                student.Id = nextId;
                var line = FormatStudentLine(student);
                await File.AppendAllTextAsync(_filePath, line + Environment.NewLine);
                return student;
            }
            finally
            {
                _fileLock.Release();
            }
        }

        public async Task<Student?> UpdateStudentAsync(Student student)
        {
            await _fileLock.WaitAsync();
            try
            {
                if (!File.Exists(_filePath))
                    return null;

                var lines = (await File.ReadAllLinesAsync(_filePath)).ToList();
                bool found = false;
                for (int i = 0; i < lines.Count; i++)
                {
                    var parts = lines[i].Split('|');
                    if (int.TryParse(parts[0], out var id) && id == student.Id)
                    {
                        lines[i] = FormatStudentLine(student);
                        found = true;
                        break;
                    }
                }

                if (!found)
                    return null;

                await File.WriteAllLinesAsync(_filePath, lines);
                return student;
            }
            finally
            {
                _fileLock.Release();
            }
        }

        public async Task<bool> DeleteStudentAsync(int id)
        {
            await _fileLock.WaitAsync();
            try
            {
                if (!File.Exists(_filePath))
                    return false;

                var lines = (await File.ReadAllLinesAsync(_filePath)).ToList();
                var initialCount = lines.Count;
                lines = lines.Where(l =>
                {
                    var parts = l.Split('|');
                    return !(int.TryParse(parts[0], out var lineId) && lineId == id);
                }).ToList();

                if (lines.Count == initialCount)
                    return false;

                await File.WriteAllLinesAsync(_filePath, lines);
                return true;
            }
            finally
            {
                _fileLock.Release();
            }
        }

        private Student? ParseLine(string line)
        {
            var parts = line.Split('|');
            if (parts.Length < 4)
                return null;

            if (!int.TryParse(parts[0], out var id))
                return null;

            var name = Unescape(parts[1]);
            var dobStr = parts[2];
            DateTime dob;
            if (!DateTime.TryParseExact(dobStr, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dob))
            {
                dob = DateTime.MinValue;
            }

            var address = Unescape(parts[3]);
            var classes = new List<Class>();
            if (parts.Length >= 5 && !string.IsNullOrWhiteSpace(parts[4]))
            {
                var classIds = parts[4].Split(',', StringSplitOptions.RemoveEmptyEntries);
                foreach (var cid in classIds)
                {
                    if (int.TryParse(cid, out var classId))
                    {
                        classes.Add(new Class { Id = classId });
                    }
                }
            }

            return new Student
            {
                Id = id,
                Name = name,
                DateOfBirth = dob,
                Address = address,
                Classes = classes
            };
        }

        private string FormatStudentLine(Student student)
        {
            var cids = student.Classes != null && student.Classes.Any() ? string.Join(",", student.Classes.Select(c => c.Id)) : string.Empty;
            var name = Escape(student.Name);
            var address = Escape(student.Address);
            var dob = student.DateOfBirth.ToString("yyyy-MM-dd");
            return $"{student.Id}|{name}|{dob}|{address}|{cids}";
        }

        private static string Escape(string s) => s?.Replace("|", " ")?.Replace("\r", " ")?.Replace("\n", " ") ?? string.Empty;
        private static string Unescape(string s) => s?.Trim() ?? string.Empty;

        public async Task<int> CountStudentsAsync()
        {
            await _fileLock.WaitAsync();
            try
            {
                if (!File.Exists(_filePath))
                    return 0;

                var lines = await File.ReadAllLinesAsync(_filePath);
                var count = 0;

                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    if (ParseLine(line) != null)
                        count++;
                }

                return count;
            }
            finally
            {
                _fileLock.Release();
            }
        }
    }
}
