using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using SharpFont;

namespace FontsDisplay
{
    public class FontViewModel : PropertyChangedBase
    {
        public FontViewModel()
            : this(@"Resources/erdf2_v3.ttf") {}

        public FontViewModel(string path)
        {
            this.FontPath = path;
        }

        private string fontPath;

        public string FontPath
        {
            get { return this.fontPath; }
            set
            {
                if (value == this.fontPath)
                {
                    return;
                }
                this.fontPath = value;
                this.UpdateFontFamily(this.fontPath);
                this.Initialize();
                this.NotifyOfPropertyChange(() => this.FontPath);
                this.NotifyOfPropertyChange(() => this.Characters);
                this.NotifyOfPropertyChange(() => this.Font);
            }
        }

        private string searchCharacters;
        public string SearchCharacters
        {
            get { return this.searchCharacters; }
            set
            {
                if (value == this.searchCharacters)
                {
                    return;
                }
                this.searchCharacters = value;
                this.NotifyOfPropertyChange();
                this.Initialize();
                this.NotifyOfPropertyChange(nameof(Characters));
            }
        }

        private void UpdateFontFamily(string path)
        {
            var file = new FileInfo(path);
            var dir = file.DirectoryName;
            var filename = Path.GetFileNameWithoutExtension(path);
            this.Font = Path.Combine(dir, "#" + filename);
        }

        public string Font { get; private set; }

        public Dictionary<string, uint> CharToGlyph { get; set; }

        public IList<string> Characters
        {
            get { return this.CharToGlyph.Keys.ToList(); }
        }

        public void Initialize()
        {
            var library = new Library();
            if (File.Exists(this.FontPath))
            {
                using (var font = new Face(library, this.FontPath))
                {
                    this.CharToGlyph = new Dictionary<string, uint>();
                    uint glyphIndex = 0;
                    uint character = font.GetFirstChar(out glyphIndex);
                    while (glyphIndex != 0)
                    {
                        if (String.IsNullOrWhiteSpace(this.SearchCharacters) || this.SearchCharacters.Contains((char) character))
                        {
                            this.CharToGlyph.Add(char.ConvertFromUtf32((int)character), character);
                        }
                        character = font.GetNextChar(character, out glyphIndex);
                    }
                }
            }
        }

        public void CopyToClipboard(string character)
        {
            Clipboard.SetText(character, TextDataFormat.UnicodeText);
        }
    }
}