using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace HotelComparer.Services
{
    public class TestHotelDataService
    {
        private readonly ILogger<TestHotelDataService> _logger;
        private readonly string _testDataDirectory;
        private readonly List<string> _fileNames;
        private int _currentFileIndex = 0;

        public TestHotelDataService(ILogger<TestHotelDataService> logger, string testDataDirectory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _testDataDirectory = testDataDirectory ?? throw new ArgumentNullException(nameof(testDataDirectory));

            _fileNames = GenerateFileNames(new DateTime(2024, 02, 21), new DateTime(2024, 02, 26));
        }

        private List<string> GenerateFileNames(DateTime startDate, DateTime endDate)
        {
            var fileNames = new List<string>();
            for (var date = startDate; date < endDate; date = date.AddDays(1))
            {
                for (var nextDate = date.AddDays(1); nextDate <= endDate; nextDate = nextDate.AddDays(1))
                {
                    string fileName = $"{date:yyyy-MM-dd}-{nextDate:yyyy-MM-dd}.json";
                    fileNames.Add(fileName);
                }
            }
            return fileNames;
        }

        public async Task<string> GetNextTestHotelData()
        {
            if (_currentFileIndex >= _fileNames.Count)
            {
                _logger.LogWarning("No more test data files available.");
                return null;
            }

            string fileName = _fileNames[_currentFileIndex++];
            string filePath = Path.Combine(_testDataDirectory, fileName);

            if (!File.Exists(filePath))
            {
                _logger.LogWarning($"Test data file not found: {filePath}");
                return null;
            }

            try
            {
                return await File.ReadAllTextAsync(filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error reading test data file: {filePath}");
                return null;
            }
        }
    }
}
