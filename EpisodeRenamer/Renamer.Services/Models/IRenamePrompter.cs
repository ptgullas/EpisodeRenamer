using System;
using System.Collections.Generic;
using System.Text;

namespace Renamer.Services.Models {
    public interface IRenamePrompter {
        bool PromptForRename(string localFile, string targetName);
    }
}
