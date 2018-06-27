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
        private bool fullLibrary;
        private bool noPlayMode;
        IPlatform _platform;
        IPlaylist _playlist;
        IGame _game;
        private string _playmode;
        private String _appPath;
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
                    ShowPlayModeMenu();
                    pmmItems.Items.Clear();
                    pmmTitle.Text = "Entire Collection";

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

                    if (pmmItems.Items.Count > 0)
                    {
                        pmmItems.Items.Add("* No preferred play mode *");
                        pmmItems.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("", System.ComponentModel.ListSortDirection.Ascending));
                        pmmItems.SelectedIndex = 0;
                    }

                    this.fullLibrary = true;
                }

                if (mmItems.SelectedIndex == 1) // Selected Platform/Playlist
                {
                    if (_platform != null)
                    {
                        ShowPlayModeMenu();
                        pmmItems.Items.Clear();
                        pmmTitle.Text = _platform.Name;

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

                        if (pmmItems.Items.Count > 0)
                        {
                            pmmItems.Items.Add("* No preferred play mode *");
                            pmmItems.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("", System.ComponentModel.ListSortDirection.Ascending));
                            pmmItems.SelectedIndex = 0;
                        }

                        this.fullLibrary = false;
                    }

                    if (_playlist != null)
                    {
                        ShowPlayModeMenu();
                        pmmItems.Items.Clear();
                        pmmTitle.Text = _playlist.Name;

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

                        if (pmmItems.Items.Count > 0)
                        {
                            pmmItems.Items.Add("* No preferred play mode *");
                            pmmItems.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("", System.ComponentModel.ListSortDirection.Ascending));
                            pmmItems.SelectedIndex = 0;
                        }

                        this.fullLibrary = false;
                    }
                }

                return true;
            }

            if (PlayModeMenu.Visibility == Visibility.Visible)
            {
                if (pmmItems.HasItems)
                {
                    if (!fullLibrary)
                    {
                        if (pmmItems.SelectedIndex == 0)
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
                            _playmode = pmmItems.SelectedItem.ToString();

                            if (_platform != null)
                            {
                                PlatformRGS();
                            }
                            if (_playlist != null)
                            {
                                PlaylistRGS();
                            }
                        }
                    }
                    else
                    {
                        if (pmmItems.SelectedIndex == 0)
                        {
                            AllGamesNoPlaymodeRGS();
                        }
                        else
                        {
                            _playmode = pmmItems.SelectedItem.ToString();

                            AllGamesRGS();
                        }
                    }
                }

                return true;
            }

            if (GameDetailMenu.Visibility == Visibility.Visible)
            {
                if (gdmItems.SelectedIndex == 0)
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
                else
                {
                    if (!fullLibrary)
                    {
                        if (!this.noPlayMode)
                        {
                            if (_platform != null)
                            {
                                PlatformRGS();
                            }
                            if (_playlist != null)
                            {
                                PlaylistRGS();
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
                        if (!this.noPlayMode)
                        {
                            AllGamesRGS();
                        }
                        else
                        {
                            AllGamesNoPlaymodeRGS();
                        }
                    }
                }

                return true;
            }

            return false;
        }

        public bool OnEscape()
        {
            if (!this.focused)
            {
                return false;
            }

            if (GameDetailMenu.Visibility == Visibility.Visible)
            {
                ShowPlayModeMenu();
            }
            else if (PlayModeMenu.Visibility == Visibility.Visible)
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

        private void PlatformRGS()
        {
            IGame[] _gameList = _platform.GetAllGames(false, false);
            IGame[] _filteredList = Array.FindAll(_gameList, MatchesSelectedPlayMode);
            Random _randomGame = new Random();
            IGame _randomSelectedGame = _filteredList[_randomGame.Next(0, _filteredList.Length)];

            _game = _randomSelectedGame;

            FillGameDetails();

            gdmItems.SelectedIndex = 0;
            this.noPlayMode = false;
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
            this.noPlayMode = true;
            ShowGameDetailMenu();
        }

        private void PlaylistRGS()
        {
            IGame[] _gameList = _playlist.GetAllGames(false);
            IGame[] _filteredList = Array.FindAll(_gameList, MatchesSelectedPlayMode);
            Random _randomGame = new Random();
            IGame _randomSelectedGame = _filteredList[_randomGame.Next(0, _filteredList.Length)];

            _game = _randomSelectedGame;

            FillGameDetails();

            gdmItems.SelectedIndex = 0;
            this.noPlayMode = false;
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
            this.noPlayMode = true;
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
            this.noPlayMode = true;
            ShowGameDetailMenu();
        }

        private void AllGamesRGS()
        {
            IGame[] _gameList = PluginHelper.DataManager.GetAllGames();
            IGame[] _filteredList = Array.FindAll(_gameList, MatchesSelectedPlayMode);
            Random _randomGame = new Random();
            IGame _randomSelectedGame = _filteredList[_randomGame.Next(0, _filteredList.Length)];

            _game = _randomSelectedGame;

            FillGameDetails();

            gdmItems.SelectedIndex = 0;
            this.noPlayMode = false;
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

        //private bool MatchesSelectedGenre(IGame g)
        //{
        //    if (g.GenresString.Contains(_genre))
        //    {

        //    }
        //}

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
            PlayModeMenu.Visibility = Visibility.Collapsed;
            GameDetailMenu.Visibility = Visibility.Collapsed;
        }

        private void ShowPlayModeMenu()
        {
            MainMenu.Visibility = Visibility.Collapsed;
            PlayModeMenu.Visibility = Visibility.Visible;
            GameDetailMenu.Visibility = Visibility.Collapsed;
        }

        private void ShowGameDetailMenu()
        {
            MainMenu.Visibility = Visibility.Collapsed;
            PlayModeMenu.Visibility = Visibility.Collapsed;
            GameDetailMenu.Visibility = Visibility.Visible;
        }
    }
}
