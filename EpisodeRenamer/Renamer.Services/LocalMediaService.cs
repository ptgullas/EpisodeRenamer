using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using Renamer.Services.Models;
using Renamer.Data.Entities;
using Serilog;

namespace Renamer.Services {
    public class LocalMediaService {
        private TitleComparer _titleComparer;
        private string _directoryPath;
        public LocalMediaService(string directoryPath) {
            _titleComparer = new TitleComparer();
            _directoryPath = directoryPath;
        }

        private List<string> GetFiles() {
            return Directory.GetFiles(_directoryPath, "*.mkv", SearchOption.TopDirectoryOnly).ToList();
        }

        public List<EpisodeForComparingDto> GetFilesAsDtosToCompare() {
            List<string> files = GetFiles();
            List<EpisodeForComparingDto> epsToCompare = new List<EpisodeForComparingDto>();
            foreach (string file in files) {
                epsToCompare.Add(_titleComparer.CreateEpisodeObjectFromFilename(file));
            }
            return epsToCompare;
        }

        public void RenameFile(string filePath, EpisodeForComparingDto ep) {
            try {
                string extension = Path.GetExtension(filePath);
                string newFilename = _titleComparer.GetFormattedFilename(ep);
                File.Move(filePath, $"{newFilename}.{extension}");
            }
            catch (Exception e) {
                Log.Error(e.Message);
            }

        }
    }
}
