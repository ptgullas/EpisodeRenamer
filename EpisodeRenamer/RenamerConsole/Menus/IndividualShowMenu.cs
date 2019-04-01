using Renamer.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RenamerConsole.Menus {
    public class IndividualShowMenu : IMenu {
        public Task DisplayMenu() {
            throw new NotImplementedException();
        }

        private async void DisplayIndividualShowMenu(TVShow selectedShow) {
            Console.WriteLine($"Selected: {selectedShow.SeriesName} (SeriesId: {selectedShow.SeriesId}). Preferred Name: {selectedShow.SeriesNamePreferred}");
            Console.WriteLine("1. Display all episodes in database");
            Console.WriteLine("2. Display seasons in database");
            Console.WriteLine("3. Check for new/updated episodes in TVDB");
        }
    }
}
