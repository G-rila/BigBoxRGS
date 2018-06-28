using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Xml;
using Unbroken.LaunchBox.Plugins;
using Unbroken.LaunchBox.Plugins.Data;

namespace BigBoxRGS
{
    public partial class RGSv2_0 : UserControl, IBigBoxThemeElementPlugin
    {
        private bool focused;
        private bool byEntireCollection;
        private bool byPlayMode;
        private bool byGenre;
        IPlatform _platform;
        IPlaylist _playlist;
        IGame _game;
        private string _playmode;
        private string _genre;
        private string _appPath;
        private string _skipGameDetailsScreen;

        public RGSv2_0()
        {
            this.InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _appPath = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);

            XmlDocument _doc = new XmlDocument();
            String filePath = _appPath + "\\Data\\BigBoxSettings.xml";

            _doc.Load(filePath);

            XmlNode _node = _doc.SelectSingleNode("/LaunchBox/BigBoxSettings/SkipGameDetailsScreen");
            _skipGameDetailsScreen = _node.InnerText;

            this.Visibility = Visibility.Hidden;
            ShowMainMenu();
        }

        public bool OnUp(bool held)
        {
            if (!this.focused)
            {
                return false;
            }

            if (MainMenu.Visibility == Visibility.Visible)
            {
                mmItems.SelectedIndex = mmItems.SelectedIndex - 1;
                if (mmItems.SelectedIndex < 0)
                {
                    mmItems.SelectedIndex = 0;
                }
                mmItems.SelectedItem = mmItems.SelectedIndex;
                mmItems.ScrollIntoView(mmItems.SelectedItem);
            }

            if (FilterMenu.Visibility == Visibility.Visible)
            {
                fmItems.SelectedIndex = fmItems.SelectedIndex - 1;
                if (fmItems.SelectedIndex < 0)
                {
                    fmItems.SelectedIndex = 0;
                }
                fmItems.SelectedItem = fmItems.SelectedIndex;
                fmItems.ScrollIntoView(fmItems.SelectedItem);
            }

            if (PlayModeMenu.Visibility == Visibility.Visible)
            {
                if (pmmItems.HasItems)
                {
                    pmmItems.SelectedIndex = pmmItems.SelectedIndex - 1;
                    if (pmmItems.SelectedIndex < 0)
                    {
                        pmmItems.SelectedIndex = 0;
                    }
                    pmmItems.SelectedItem = pmmItems.SelectedIndex;
                    pmmItems.ScrollIntoView(pmmItems.SelectedItem);
                }
            }

            if (GameDetailMenu.Visibility == Visibility.Visible)
            {
                gdmItems.SelectedIndex = gdmItems.SelectedIndex - 1;
                if (gdmItems.SelectedIndex < 0)
                {
                    gdmItems.SelectedIndex = 0;
                }
                gdmItems.SelectedItem = gdmItems.SelectedIndex;
                gdmItems.ScrollIntoView(gdmItems.SelectedItem);
            }
            return true;
        }

        public bool OnDown(bool held)
        {
            if (!this.focused)
            {
                return false;
            }

            if (MainMenu.Visibility == Visibility.Visible)
            {
                mmItems.SelectedIndex = mmItems.SelectedIndex + 1;
                mmItems.SelectedItem = mmItems.SelectedIndex;
                mmItems.ScrollIntoView(mmItems.SelectedItem);
            }

            if (FilterMenu.Visibility == Visibility.Visible)
            {
                fmItems.SelectedIndex = fmItems.SelectedIndex + 1;
                fmItems.SelectedItem = fmItems.SelectedIndex;
                fmItems.ScrollIntoView(fmItems.SelectedItem);
            }

            if (PlayModeMenu.Visibility == Visibility.Visible)
            {
                if (pmmItems.HasItems)
                {
                    pmmItems.SelectedIndex = pmmItems.SelectedIndex + 1;
                    pmmItems.SelectedItem = pmmItems.SelectedIndex;
                    pmmItems.ScrollIntoView(pmmItems.SelectedItem);
                }
            }

            if (GameDetailMenu.Visibility == Visibility.Visible)
            {
                gdmItems.SelectedIndex = gdmItems.SelectedIndex + 1;
                gdmItems.SelectedItem = gdmItems.SelectedIndex;
                gdmItems.ScrollIntoView(gdmItems.SelectedItem);
            }
            return true;
        }

        public bool OnEnter()
        {
            if (!this.focused)
            {
                return false;
            }

            if (MainMenu.Visibility == Visibility.Visible)
            {
                if (mmItems.SelectedIndex == 0) // Entire Collection
                {
                    this.byEntireCollection = true;
                    fmItems.SelectedIndex = 0;
                    ShowFilterMenu();
                }

                if (mmItems.SelectedIndex == 1) // Selected Platform/Playlist
                {
                    this.byEntireCollection = false;
                    fmItems.SelectedIndex = 0;
                    ShowFilterMenu();
                }
                return true;
            }

            if (FilterMenu.Visibility == Visibility.Visible)
            {
                if (fmItems.SelectedIndex == 0) // Choose from all
                {
                    this.byPlayMode = false;
                    this.byGenre = false;

                    if (!this.byEntireCollection)
                    {
                        if (_platform != null)
                        {
                            PlatformNoPlaymodeRGS();
                        }
                        if (_playlist != null)
                        {
                            PlaylistNoPlaymodeRGS();
                        }
                    }
                    else
                    {
                        AllGamesNoPlaymodeRGS();
                    }
                }

                if (fmItems.SelectedIndex == 1) // Choose by genre
                {
                    this.byPlayMode = false;
                    this.byGenre = true;

                    if (this.byEntireCollection)
                    {
                        ShowPlayModeMenu();
                        pmmItems.Items.Clear();
                        pmmTitle.Text = "Entire Collection by Genre";

                        IGame[] _allGames = PluginHelper.DataManager.GetAllGames();
                        IGame[] _filteredList = Array.FindAll(_allGames, BrokenOrHidden);
                        foreach (IGame g in _filteredList) //this splits the genres and puts 1 entry for each in the listbox
                        {
                            char[] delimiterChars = { ';' };
                            foreach (string p in g.Genres)
                            {
                                p.Split(delimiterChars);
                                if (!pmmItems.Items.Contains(p))
                                {
                                    pmmItems.Items.Add(p);
                                }
                            }
                        }

                        pmmItems.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("", System.ComponentModel.ListSortDirection.Ascending));
                        pmmItems.SelectedIndex = 0;
                    }
                    else
                    {
                        if (_platform != null)
                        {
                            ShowPlayModeMenu();
                            pmmItems.Items.Clear();
                            pmmTitle.Text = _platform.Name + " by Genre";

                            IGame[] _allGames = _platform.GetAllGames(false, false);
                            foreach (IGame g in _allGames) //this splits the genres and puts 1 entry for each in the listbox
                            {
                                char[] delimiterChars = { ';' };
                                foreach (string p in g.Genres)
                                {
                                    p.Split(delimiterChars);
                                    if (!pmmItems.Items.Contains(p))
                                    {
                                        pmmItems.Items.Add(p);
                                    }
                                }
                            }

                            pmmItems.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("", System.ComponentModel.ListSortDirection.Ascending));
                            pmmItems.SelectedIndex = 0;
                        }

                        if (_playlist != null)
                        {
                            ShowPlayModeMenu();
                            pmmItems.Items.Clear();
                            pmmTitle.Text = _playlist.Name + " by Genre";

                            IGame[] _allGames = _playlist.GetAllGames(false);
                            IGame[] _filteredList = Array.FindAll(_allGames, BrokenOrHidden);
                            foreach (IGame g in _filteredList) //this splits the genres and puts 1 entry for each in the listbox
                            {
                                char[] delimiterChars = { ';' };
                                foreach (string p in g.Genres)
                                {
                                    p.Split(delimiterChars);
                                    if (!pmmItems.Items.Contains(p))
                                    {
                                        pmmItems.Items.Add(p);
                                    }
                                }
                            }

                            pmmItems.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("", System.ComponentModel.ListSortDirection.Ascending));
                            pmmItems.SelectedIndex = 0;
                        }
                    }
                }

                if (fmItems.SelectedIndex == 2) // Choose by play mode
                {
                    this.byPlayMode = true;
                    this.byGenre = false;

                    if (this.byEntireCollection)
                    {
                        ShowPlayModeMenu();
                        pmmItems.Items.Clear();
                        pmmTitle.Text = "Entire Collection by Play Mode";

                        IGame[] _allGames = PluginHelper.DataManager.GetAllGames();
                        IGame[] _filteredList = Array.FindAll(_allGames, BrokenOrHidden);
                        foreach (IGame g in _filteredList) //this splits the playmodes and puts 1 entry for each in the listbox
                        {
                            char[] delimiterChars = { ';' };
                            foreach (string p in g.PlayModes)
                            {
                                p.Split(delimiterChars);
                                if (!pmmItems.Items.Contains(p))
                                {
                                    pmmItems.Items.Add(p);
                                }
                            }
                        }

                        pmmItems.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("", System.ComponentModel.ListSortDirection.Ascending));
                        pmmItems.SelectedIndex = 0;
                    }
                    else
                    {
                        if (_platform != null)
                        {
                            ShowPlayModeMenu();
                            pmmItems.Items.Clear();
                            pmmTitle.Text = _platform.Name + " by Paly Mode";

                            IGame[] _allGames = _platform.GetAllGames(false, false);
                            foreach (IGame g in _allGames) //this splits the playmodes and puts 1 entry for each in the listbox
                            {
                                char[] delimiterChars = { ';' };
                                foreach (string p in g.PlayModes)
                                {
                                    p.Split(delimiterChars);
                                    if (!pmmItems.Items.Contains(p))
                                    {
                                        pmmItems.Items.Add(p);
                                    }
                                }
                            }

                            pmmItems.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("", System.ComponentModel.ListSortDirection.Ascending));
                            pmmItems.SelectedIndex = 0;
                        }

                        if (_playlist != null)
                        {
                            ShowPlayModeMenu();
                            pmmItems.Items.Clear();
                            pmmTitle.Text = _playlist.Name + " by Play Mode";

                            IGame[] _allGames = _playlist.GetAllGames(false);
                            IGame[] _filteredList = Array.FindAll(_allGames, BrokenOrHidden);
                            foreach (IGame g in _filteredList) //this splits the playmodes and puts 1 entry for each in the listbox
                            {
                                char[] delimiterChars = { ';' };
                                foreach (string p in g.PlayModes)
                                {
                                    p.Split(delimiterChars);
                                    if (!pmmItems.Items.Contains(p))
                                    {
                                        pmmItems.Items.Add(p);
                                    }
                                }
                            }

                            pmmItems.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("", System.ComponentModel.ListSortDirection.Ascending));
                            pmmItems.SelectedIndex = 0;
                        }
                    }
                }
                return true;
            }

            if (PlayModeMenu.Visibility == Visibility.Visible)
            {
                if (pmmItems.HasItems)
                {
                    if (!this.byEntireCollection)
                    {
                        if (this.byPlayMode)
                        {
                            _playmode = pmmItems.SelectedItem.ToString();

                            if (_platform != null)
                            {
                                PlatformPlaymodeRGS();
                            }
                            if (_playlist != null)
                            {
                                PlaylistPlaymodeRGS();
                            }
                        }
                        else if (this.byGenre)
                        {
                            _genre = pmmItems.SelectedItem.ToString();

                            if (_platform != null)
                            {
                                PlatformGenreRGS();
                            }
                            if (_playlist != null)
                            {
                                PlaylistGenreRGS();
                            }
                        }
                        else
                        {
                            if (_platform != null)
                            {
                                PlatformNoPlaymodeRGS();
                            }
                            if (_playlist != null)
                            {
                                PlaylistNoPlaymodeRGS();
                            }
                        }
                    }
                    else
                    {
                        if (this.byPlayMode)
                        {
                            _playmode = pmmItems.SelectedItem.ToString();

                            AllGamesPlaymodeRGS();
                        }
                        else if (this.byGenre)
                        {
                            _genre = pmmItems.SelectedItem.ToString();

                            AllGamesGenreRGS();
                        }
                        else
                        {
                            AllGamesNoPlaymodeRGS();
                        }
                    }
                }
                return true;
            }

            if (GameDetailMenu.Visibility == Visibility.Visible)
            {
                if (gdmItems.SelectedIndex == 0) // Play this game
                {
                    if (_skipGameDetailsScreen == "true")
                    {
                        this.Visibility = Visibility.Hidden;
                        this.focused = false;
                        _game.Play();
                    }
                    else
                    {
                        PluginHelper.BigBoxMainViewModel.ShowGame(_game, FilterType.PlatformOrCategoryOrPlaylist);
                        this.Visibility = Visibility.Hidden;
                        this.focused = false;
                    }
                }
                else // Choose another game
                {
                    if (!this.byEntireCollection)
                    {
                        if (this.byPlayMode)
                        {
                            if (_platform != null)
                            {
                                PlatformPlaymodeRGS();
                            }
                            if (_playlist != null)
                            {
                                PlaylistPlaymodeRGS();
                            }
                        }
                        else if (this.byGenre)
                        {
                            if (_platform != null)
                            {
                                PlatformGenreRGS();
                            }
                            if (_playlist != null)
                            {
                                PlaylistGenreRGS();
                            }
                        }
                        else
                        {
                            if (_platform != null)
                            {
                                PlatformNoPlaymodeRGS();
                            }
                            if (_playlist != null)
                            {
                                PlaylistNoPlaymodeRGS();
                            }
                        }
                    }
                    else
                    {
                        if (this.byPlayMode)
                        {
                            AllGamesPlaymodeRGS();
                        }
                        else if (this.byGenre)
                        {
                            AllGamesGenreRGS();
                        }
                        else
                        {
                            AllGamesNoPlaymodeRGS();
                        }
                    }
                }
                return true;
            }
            return true;
        }

        public bool OnEscape()
        {
            if (!this.focused)
            {
                return false;
            }

            if (GameDetailMenu.Visibility == Visibility.Visible)
            {
                if (!this.byPlayMode & !this.byGenre)
                {
                    ShowFilterMenu();
                }
                else
                {
                    ShowPlayModeMenu();
                }
            }
            else if (PlayModeMenu.Visibility == Visibility.Visible)
            {
                ShowFilterMenu();
            }
            else if (FilterMenu.Visibility == Visibility.Visible)
            {
                ShowMainMenu();
            }
            else
            {
                this.Visibility = Visibility.Hidden;
                this.focused = false;
            }
            return true;
        }

        public bool OnRight(bool held)
        {
            if (held & !this.focused)
            {
                ShowMainMenu();
                mmItems.SelectedIndex = 0;
                this.Visibility = Visibility.Visible;
                this.focused = true;
                return true;
            }
            else if (this.focused)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool OnLeft(bool held)
        {
            if (!this.focused)
            {
                return false;
            }
            return true;
        }

        public bool OnPageUp()
        {
            if (!this.focused)
            {
                return false;
            }
            return true;
        }

        public bool OnPageDown()
        {
            if (!this.focused)
            {
                return false;
            }
            return true;
        }

        public void OnSelectionChanged(FilterType filterType, string filterValue, IPlatform platform, IPlatformCategory category, IPlaylist playlist, IGame game)
        {
            _platform = platform;
            _playlist = playlist;
        }

        private void PlatformPlaymodeRGS()
        {
            IGame[] _gameList = _platform.GetAllGames(false, false);
            IGame[] _filteredList = Array.FindAll(_gameList, MatchesSelectedPlayMode);
            Random _randomGame = new Random();
            IGame _randomSelectedGame = _filteredList[_randomGame.Next(0, _filteredList.Length)];

            _game = _randomSelectedGame;

            FillGameDetails();

            gdmItems.SelectedIndex = 0;

            this.byEntireCollection = false;
            this.byPlayMode = true;
            this.byGenre = false;

            ShowGameDetailMenu();
        }

        private void PlatformGenreRGS()
        {
            IGame[] _gameList = _platform.GetAllGames(false, false);
            IGame[] _filteredList = Array.FindAll(_gameList, MatchesSelectedGenre);
            Random _randomGame = new Random();
            IGame _randomSelectedGame = _filteredList[_randomGame.Next(0, _filteredList.Length)];

            _game = _randomSelectedGame;

            FillGameDetails();

            gdmItems.SelectedIndex = 0;

            this.byEntireCollection = false;
            this.byPlayMode = false;
            this.byGenre = true;

            ShowGameDetailMenu();
        }

        private void PlatformNoPlaymodeRGS()
        {
            IGame[] _gameList = _platform.GetAllGames(false, false);
            Random _randomGame = new Random();
            IGame _randomSelectedGame = _gameList[_randomGame.Next(0, _gameList.Length)];

            _game = _randomSelectedGame;

            FillGameDetails();

            gdmItems.SelectedIndex = 0;

            this.byEntireCollection = false;
            this.byPlayMode = false;
            this.byGenre = false;

            ShowGameDetailMenu();
        }

        private void PlaylistPlaymodeRGS()
        {
            IGame[] _gameList = _playlist.GetAllGames(false);
            IGame[] _filteredList = Array.FindAll(_gameList, MatchesSelectedPlayMode);
            Random _randomGame = new Random();
            IGame _randomSelectedGame = _filteredList[_randomGame.Next(0, _filteredList.Length)];

            _game = _randomSelectedGame;

            FillGameDetails();

            gdmItems.SelectedIndex = 0;

            this.byEntireCollection = false;
            this.byPlayMode = true;
            this.byGenre = false;

            ShowGameDetailMenu();
        }

        private void PlaylistGenreRGS()
        {
            IGame[] _gameList = _playlist.GetAllGames(false);
            IGame[] _filteredList = Array.FindAll(_gameList, MatchesSelectedGenre);
            Random _randomGame = new Random();
            IGame _randomSelectedGame = _filteredList[_randomGame.Next(0, _filteredList.Length)];

            _game = _randomSelectedGame;

            FillGameDetails();

            gdmItems.SelectedIndex = 0;

            this.byEntireCollection = false;
            this.byPlayMode = false;
            this.byGenre = true;

            ShowGameDetailMenu();
        }

        private void PlaylistNoPlaymodeRGS()
        {
            IGame[] _gameList = _playlist.GetAllGames(false);
            IGame[] _filteredList = Array.FindAll(_gameList, BrokenOrHidden);
            Random _randomGame = new Random();
            IGame _randomSelectedGame = _filteredList[_randomGame.Next(0, _filteredList.Length)];

            _game = _randomSelectedGame;

            FillGameDetails();

            gdmItems.SelectedIndex = 0;

            this.byEntireCollection = false;
            this.byPlayMode = false;
            this.byGenre = false;

            ShowGameDetailMenu();
        }

        private void AllGamesNoPlaymodeRGS()
        {
            IGame[] _gameList = PluginHelper.DataManager.GetAllGames();
            IGame[] _filteredList = Array.FindAll(_gameList, BrokenOrHidden);
            Random _randomGame = new Random();
            IGame _randomSelectedGame = _filteredList[_randomGame.Next(0, _filteredList.Length)];

            _game = _randomSelectedGame;

            FillGameDetails();

            gdmItems.SelectedIndex = 0;

            this.byEntireCollection = true;
            this.byPlayMode = false;
            this.byGenre = false;

            ShowGameDetailMenu();
        }

        private void AllGamesPlaymodeRGS()
        {
            IGame[] _gameList = PluginHelper.DataManager.GetAllGames();
            IGame[] _filteredList = Array.FindAll(_gameList, MatchesSelectedPlayMode);
            Random _randomGame = new Random();
            IGame _randomSelectedGame = _filteredList[_randomGame.Next(0, _filteredList.Length)];

            _game = _randomSelectedGame;

            FillGameDetails();

            gdmItems.SelectedIndex = 0;

            this.byEntireCollection = true;
            this.byPlayMode = true;
            this.byGenre = false;

            ShowGameDetailMenu();
        }

        private void AllGamesGenreRGS()
        {
            IGame[] _gameList = PluginHelper.DataManager.GetAllGames();
            IGame[] _filteredList = Array.FindAll(_gameList, MatchesSelectedGenre);
            Random _randomGame = new Random();
            IGame _randomSelectedGame = _filteredList[_randomGame.Next(0, _filteredList.Length)];

            _game = _randomSelectedGame;

            FillGameDetails();

            gdmItems.SelectedIndex = 0;

            this.byEntireCollection = true;
            this.byPlayMode = false;
            this.byGenre = true;

            ShowGameDetailMenu();
        }

        private bool MatchesSelectedPlayMode(IGame g)
        {
            if (g.PlayMode.Contains(_playmode))
            {
                if (g.Broken == true | g.Hide == true)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        private bool MatchesSelectedGenre(IGame g)
        {
            if (g.GenresString.Contains(_genre))
            {
                if (g.Broken == true | g.Hide == true)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        private bool BrokenOrHidden(IGame g)
        {
            if (g.Broken == true | g.Hide == true)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void FillGameDetails()
        {
            gdmTitle.Text = _game.Title;

            if (_game.PlayMode != string.Empty)
            {
                gdmPlayMode.Text = _game.PlayMode;
                gdmPlayMode.Visibility = Visibility.Visible;
            }
            else
            {
                gdmPlayMode.Visibility = Visibility.Collapsed;
            }

            if (_game.GenresString != string.Empty)
            {
                gdmGenre.Text = _game.GenresString;
                gdmGenre.Visibility = Visibility.Visible;
            }
            else
            {
                gdmGenre.Visibility = Visibility.Collapsed;
            }

            if (_game.FrontImagePath != null)
            {
                gdmImage.Source = new BitmapImage(new Uri(_game.FrontImagePath));
            }
            else
            {
                gdmImage.Source = null;
            }
        }

        private void ShowMainMenu()
        {
            MainMenu.Visibility = Visibility.Visible;
            FilterMenu.Visibility = Visibility.Collapsed;
            PlayModeMenu.Visibility = Visibility.Collapsed;
            GameDetailMenu.Visibility = Visibility.Collapsed;
        }

        private void ShowFilterMenu()
        {
            MainMenu.Visibility = Visibility.Collapsed;
            FilterMenu.Visibility = Visibility.Visible;
            PlayModeMenu.Visibility = Visibility.Collapsed;
            GameDetailMenu.Visibility = Visibility.Collapsed;
        }

        private void ShowPlayModeMenu()
        {
            MainMenu.Visibility = Visibility.Collapsed;
            FilterMenu.Visibility = Visibility.Collapsed;
            PlayModeMenu.Visibility = Visibility.Visible;
            GameDetailMenu.Visibility = Visibility.Collapsed;
        }

        private void ShowGameDetailMenu()
        {
            MainMenu.Visibility = Visibility.Collapsed;
            FilterMenu.Visibility = Visibility.Collapsed;
            PlayModeMenu.Visibility = Visibility.Collapsed;
            GameDetailMenu.Visibility = Visibility.Visible;
        }
    }
}
