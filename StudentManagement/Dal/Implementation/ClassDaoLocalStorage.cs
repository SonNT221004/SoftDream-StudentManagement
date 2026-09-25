using StudentManagement.Dal.Interface;
using StudentManagement.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StudentManagement.Dal.Implementation
{
    public class ClassDaoLocalStorage : IClassDAO
    {
        private static readonly SemaphoreSlim _fileLock = new(1, 1);
        private readonly string _filePath;

        public ClassDaoLocalStorage(string storageDirectory)
        {
            if (string.IsNullOrWhiteSpace(storageDirectory))
                throw new ArgumentException("storageDirectory must be provided", nameof(storageDirectory));

            Directory.CreateDirectory(storageDirectory);
            _filePath = Path.Combine(storageDirectory, "Class.txt");
        }

        public async Task<int> CountClassesAsync(CancellationToken cancellationToken)
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

        public async Task<List<Class>> GetAllClassesAsync(CancellationToken cancellationToken)
        {
            await _fileLock.WaitAsync(cancellationToken);
            try
            {
                if (!File.Exists(_filePath))
                    return new List<Class>();

                var lines = await File.ReadAllLinesAsync(_filePath, cancellationToken);
                var list = new List<Class>();
                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;
                    var c = ParseLine(line);
                    if (c != null)
                        list.Add(c);
                }

                return list;
            }
            finally
            {
                _fileLock.Release();
            }
        }

        public async Task<List<Class>> GetClassesByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken)
        {
            var idSet = new HashSet<int>(ids ?? Array.Empty<int>());
            if (idSet.Count == 0)
                return new List<Class>();

            var all = await GetAllClassesAsync(cancellationToken);
            return all.Where(c => idSet.Contains(c.Id)).ToList();
        }

        private Class? ParseLine(string line)
        {
            var parts = line.Split('|', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2)
                return null;

            if (!int.TryParse(parts[0].Trim(), out var id))
                return null;

            var name = parts[1].Trim();
            var teacherId = parts.Length > 2 && int.TryParse(parts[2].Trim(), out var tid) ? tid : 0;
            return new Class { Id = id, Name = name, Teacher = new Teacher { Id = teacherId } };
        }
    }
}
