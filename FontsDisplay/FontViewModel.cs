﻿using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Caliburn.Micro;
using SharpFont;

namespace FontsDisplay
{
    public class FontViewModel : PropertyChangedBase
    {
        public FontViewModel()
            : this(@"Resources/fontawesome-webfont.ttf") {}

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
                this.NotifyOfPropertyChange(() => this.Font);
            }
        }

        private bool copyUnicode;

        public bool CopyUnicode
        {
            get { return this.copyUnicode; }
            set
            {
                if (value == this.copyUnicode)
                    return;
                this.copyUnicode = value;
                this.NotifyOfPropertyChange(() => this.CopyUnicode);
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
                this.Initialize();
                this.NotifyOfPropertyChange();
            }
        }

        private string colorText;
        public string ColorText
        {
            get { return this.colorText; }
            set
            {
                if (value == this.colorText)
                    return;
                this.colorText = value;
                this.NotifyOfPropertyChange(() => this.ColorText);
                int color = 0;
                if (int.TryParse(this.colorText.Trim().Replace("#", ""), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out color))
                {
                    byte a = (byte) (color >> 24 & 0xFF);
                    byte r = (byte) (color >> 16 & 0xFF);
                    byte g = (byte) (color >> 8 & 0xFF);
                    byte b = (byte) (color & 0xFF);
                    this.ColorBrush = new SolidColorBrush(Color.FromArgb(a == 0 ? (byte) 0xFF : a, r, g, b));
                }
                else
                {
                    this.ColorBrush = Brushes.Black;
                }
            }
        }

        private Brush colorBrush = Brushes.Black;
        public Brush ColorBrush
        {
            get { return this.colorBrush; }
            set
            {
                if (Equals(value, this.colorBrush))
                    return;
                this.colorBrush = value;
                this.NotifyOfPropertyChange(() => this.ColorBrush);
            }
        }

        private void UpdateFontFamily(string path)
        {
            var file = new FileInfo(path);
            var dir = file.DirectoryName;
            var library = new Library();
            using (var font = new Face(library, path))
            {
                this.Font = Path.Combine(dir, "#" + font.FamilyName);
            }
        }

        public string Font { get; private set; }

        private ObservableCollection<GlyphInfo> glyphInfos;
        public ObservableCollection<GlyphInfo> GlyphInfos
        {
            get { return this.glyphInfos; }
            set
            {
                if (object.Equals(value, this.glyphInfos))
                    return;
                this.glyphInfos = value;
                this.NotifyOfPropertyChange(() => this.GlyphInfos);
            }
        }

        public void Initialize()
        {
            if (File.Exists(this.FontPath))
            {
                var library = new Library();
                using (var font = new Face(library, this.FontPath))
                {
                    var infos = new ObservableCollection<GlyphInfo>();
                    uint glyphIndex = 0;
                    uint character = font.GetFirstChar(out glyphIndex);
                    while (glyphIndex != 0)
                    {
                        if (String.IsNullOrWhiteSpace(this.SearchCharacters) || this.SearchCharacters.Contains((char) character) ||
                            (font.HasGlyphNames && font.GetGlyphName(glyphIndex, 32).Contains(this.SearchCharacters)))
                        {
                            infos.Add(new GlyphInfo
                            {
                                Character = char.ConvertFromUtf32((int) character),
                                Unicode = character,
                                Name = font.HasGlyphNames ? font.GetGlyphName(glyphIndex, 32) : String.Empty
                            });
                        }
                        character = font.GetNextChar(character, out glyphIndex);
                    }
                    this.GlyphInfos = infos;
                }
            }
        }

        public void CopyToClipboard(GlyphInfo info)
        {
            if (this.CopyUnicode)
            {
                Clipboard.SetText(info.Unicode.ToString("X4"), TextDataFormat.UnicodeText);
            }
            else
            {
                Clipboard.SetText(info.Character, TextDataFormat.UnicodeText);
            }
        }
    }
}