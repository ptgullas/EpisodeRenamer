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
                try {
                    EpisodeForComparingDto epFromFilenameDto = _titleComparer.CreateEpisodeObjectFromPath(file);
                    if (epFromFilenameDto != null) {
                        epsToCompare.Add(epFromFilenameDto);
                    }
                }
                catch (Exception e) {
                    Log.Warning(e, "Can't create EpisodeForComparingFromDto object from file {a}", file);
                }
            }
            return epsToCompare;
        }

        public void RenameFile(string filePath, EpisodeForComparingDto ep) {
            try {
                string extension = Path.GetExtension(filePath);
                string sourceDirectory = Path.GetDirectoryName(filePath);
                string newFilename = _titleComparer.GetFormattedFilename(ep);
                string newPath = Path.Combine(sourceDirectory, $"{newFilename}{extension}");
                File.Move(filePath, newPath);
                Log.Information("Renamed {a} to {b}", Path.GetFileName(filePath), Path.GetFileName(newPath));
            }
            catch (Exception e) {
                Log.Error(e.Message);
            }

        }
    }
}
