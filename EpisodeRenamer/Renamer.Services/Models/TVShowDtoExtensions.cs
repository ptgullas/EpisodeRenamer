using Renamer.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Renamer.Services.Models {
    public static class TVShowDtoExtensions {
        public static TVShow ToTVShow(this TVShowDto dto) {
            return new TVShow() {
                SeriesId = dto.SeriesId,
                SeriesName = dto.SeriesNameTVDB,
                SeriesNamePreferred = dto.SeriesNamePreferred
            };
        }
    }
}
