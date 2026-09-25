using StudentManagement.Dal.Interface;
using StudentManagement.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagement.Dal.Implementation
{
    public class TeacherDaoLocalStorage : ITeacherDAO
    {
        private static readonly SemaphoreSlim _fileLock = new(1, 1);
        private readonly string _filePath;

        public TeacherDaoLocalStorage(string storageDirectory)
        {
            if (string.IsNullOrWhiteSpace(storageDirectory))
                throw new ArgumentException("storageDirectory must be provided", nameof(storageDirectory));
            Directory.CreateDirectory(storageDirectory);
            _filePath = Path.Combine(storageDirectory, "Teacher.txt");
        }

        public async Task<int> CountTeachersAsync(CancellationToken cancellationToken)
        {
            await _fileLock.WaitAsync(cancellationToken);
            try
            {
                if (!File.Exists(_filePath))
                    return 0;

                var lines = await File.ReadAllLinesAsync(_filePath, cancellationToken);
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

        public async Task<Teacher?> GetTeacherById(int id, CancellationToken cancellationToken)
        {
            await _fileLock.WaitAsync(cancellationToken);
            try
            {
                if (!File.Exists(_filePath))
                    return null;
                var lines = await File.ReadAllLinesAsync(_filePath, cancellationToken);
                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;
                    var teacher = ParseLine(line);
                    if (teacher != null && teacher.Id == id)
                        return teacher;
                }
                return null;
            }
            finally
            {
                _fileLock.Release();
            }

        }

        private static Teacher? ParseLine(string line)
        {
            var parts = line.Split('|');
            if (parts.Length < 3)
                return null;
            if (!int.TryParse(parts[0], out int id))
                return null;
            if (!DateTime.TryParse(parts[2], out DateTime dob))
                return null;
            return new Teacher
            {
                Id = id,
                Name = parts[1],
                DateOfBirth = dob
            };
        }
    }
}
