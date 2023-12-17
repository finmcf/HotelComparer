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

        public TestHotelDataService(ILogger<TestHotelDataService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _testDataDirectory = Path.Combine(Directory.GetCurrentDirectory(), "data", "TestHotelData");
            _logger.LogInformation($"Test data directory set to: {_testDataDirectory}");
            _fileNames = GenerateFileNames();
            _logger.LogInformation($"Generated file names: {string.Join(", ", _fileNames)}");
        }

        private List<string> GenerateFileNames()
        {
            var fileNames = new List<string>();
            var startDate = new DateTime(2024, 02, 21);
            var endDate = new DateTime(2024, 02, 26);

            for (var date = startDate; date < endDate; date = date.AddDays(1))
            {
                for (var nextDate = date.AddDays(1); nextDate <= endDate; nextDate = nextDate.AddDays(1))
                {
                    string fileName = $"2024-{date:MM-dd}-2024-{nextDate:MM-dd}.json";
                    fileNames.Add(fileName);
                }
            }
            return fileNames;
        }

        public async Task<IEnumerable<string>> GetAllTestHotelDataAsync()
        {
            var responses = new List<string>();
            foreach (var fileName in _fileNames)
            {
                string response = await ReadFileAsync(fileName);
                if (response != null)
                {
                    responses.Add(response);
                }
            }
            return responses;
        }

        private async Task<string> ReadFileAsync(string fileName)
        {
            string filePath = Path.Combine(_testDataDirectory, fileName);
            _logger.LogInformation($"Attempting to read file asynchronously: {filePath}");

            if (!File.Exists(filePath))
            {
                _logger.LogWarning($"Test data file not found: {filePath}");
                return null;
            }

            try
            {
                var fileContent = await File.ReadAllTextAsync(filePath);
                _logger.LogInformation($"File content of {fileName}: {fileContent}");
                return fileContent;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error reading test data file asynchronously: {filePath}");
                return null;
            }
        }
    }
}
