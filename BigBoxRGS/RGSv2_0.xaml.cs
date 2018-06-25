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
            ShowPlayModeMenu();
        }

        public bool OnUp(bool held)
        {
            if (!this.focused)
            {
                return false;
            }

            if (PlayModeMenu.Visibility == Visibility.Visible)
            {
                if (MenuItems.HasItems)
                {
                    MenuItems.SelectedIndex = MenuItems.SelectedIndex - 1;
                    if (MenuItems.SelectedIndex < 0)
                    {
                        MenuItems.SelectedIndex = 0;
                    }
                    MenuItems.SelectedItem = MenuItems.SelectedIndex;
                    MenuItems.ScrollIntoView(MenuItems.SelectedItem);
                }
            }
            else if (GameDetailMenu.Visibility == Visibility.Visible)
            {
                GameMenuItems.SelectedIndex = GameMenuItems.SelectedIndex - 1;
                if (GameMenuItems.SelectedIndex < 0)
                {
                    GameMenuItems.SelectedIndex = 0;
                }
                GameMenuItems.SelectedItem = GameMenuItems.SelectedIndex;
                GameMenuItems.ScrollIntoView(GameMenuItems.SelectedItem);
            }

            return true;
        }

        public bool OnDown(bool held)
        {
            if (!this.focused)
            {
                return false;
            }

            if (PlayModeMenu.Visibility == Visibility.Visible)
            {
                if (MenuItems.HasItems)
                {
                    MenuItems.SelectedIndex = MenuItems.SelectedIndex + 1;
                    MenuItems.SelectedItem = MenuItems.SelectedIndex;
                    MenuItems.ScrollIntoView(MenuItems.SelectedItem);
                }
            }
            else if (GameDetailMenu.Visibility == Visibility.Visible)
            {
                GameMenuItems.SelectedIndex = 1;
                GameMenuItems.SelectedItem = GameMenuItems.SelectedIndex;
                GameMenuItems.ScrollIntoView(GameMenuItems.SelectedItem);
            }

            return true;
        }

        public bool OnEnter()
        {
            if (!this.focused)
            {
                return false;
            }

            if (PlayModeMenu.Visibility == Visibility.Visible)
            {
                if (!fullLibrary)
                {
                    if (MenuItems.SelectedIndex == 0)
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
                        _playmode = MenuItems.SelectedItem.ToString();

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
                    if (MenuItems.SelectedIndex == 0)
                    {
                        AllGamesNoPlaymodeRGS();
                    }
                    else
                    {
                        _playmode = MenuItems.SelectedItem.ToString();

                        AllGamesRGS();
                    }
                }
            }
            else if (GameDetailMenu.Visibility == Visibility.Visible)
            {
                if (GameMenuItems.SelectedIndex == 0)
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
                ShowPlayModeMenu();
            }
            else
            {
                this.Visibility = Visibility.Hidden;
                this.focused = false;
                return true;
            }

            return true;
        }

        public bool OnRight(bool held)
        {
            if (held & !this.focused & _platform != null)
            {
                ShowPlayModeMenu();
                MenuItems.Items.Clear();
                MenuTitle.Text = _platform.Name;

                IGame[] _allGames = _platform.GetAllGames(false, false);
                foreach (IGame g in _allGames) //this splits the playmodes and puts 1 entry for each in the listbox
                {
                    char[] delimiterChars = { ';' };
                    foreach (string p in g.PlayModes)
                    {
                        p.Split(delimiterChars);
                        if (!MenuItems.Items.Contains(p))
                        {
                            MenuItems.Items.Add(p);
                        }
                    }
                }
                MenuItems.Items.Add("*No preferred play mode");
                MenuItems.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("", System.ComponentModel.ListSortDirection.Ascending));
                MenuItems.SelectedIndex = 0;

                this.Visibility = Visibility.Visible;
                this.focused = true;
                this.fullLibrary = false;
                return true;
            }
            else if (held & !this.focused & _playlist != null)
            {
                ShowPlayModeMenu();
                MenuItems.Items.Clear();
                MenuTitle.Text = _playlist.Name;

                IGame[] _allGames = _playlist.GetAllGames(false);
                foreach (IGame g in _allGames) //this splits the playmodes and puts 1 entry for each in the listbox
                {
                    char[] delimiterChars = { ';' };
                    foreach (string p in g.PlayModes)
                    {
                        p.Split(delimiterChars);
                        if (!MenuItems.Items.Contains(p))
                        {
                            MenuItems.Items.Add(p);
                        }
                    }
                }
                MenuItems.Items.Add("*No preferred play mode");
                MenuItems.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("", System.ComponentModel.ListSortDirection.Ascending));
                MenuItems.SelectedIndex = 0;

                this.Visibility = Visibility.Visible;
                this.focused = true;
                this.fullLibrary = false;
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
            if (held & !this.focused)
            {
                ShowPlayModeMenu();
                MenuItems.Items.Clear();
                MenuTitle.Text = "Entire Library";

                IGame[] _allGames = PluginHelper.DataManager.GetAllGames();
                foreach (IGame g in _allGames) //this splits the playmodes and puts 1 entry for each in the listbox
                {
                    char[] delimiterChars = { ';' };
                    foreach (string p in g.PlayModes)
                    {
                        p.Split(delimiterChars);
                        if (!MenuItems.Items.Contains(p))
                        {
                            MenuItems.Items.Add(p);
                        }
                    }
                }
                MenuItems.Items.Add("*No preferred play mode");
                MenuItems.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("", System.ComponentModel.ListSortDirection.Ascending));
                MenuItems.SelectedIndex = 0;

                this.Visibility = Visibility.Visible;
                this.focused = true;
                this.fullLibrary = true;
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

            GameTitle.Text = _randomSelectedGame.Title;
            GamePlayMode.Text = _randomSelectedGame.PlayMode;
            if (_randomSelectedGame.FrontImagePath != null)
            {
                GameImage.Source = new BitmapImage(new Uri(_randomSelectedGame.FrontImagePath));
            }
            else
            {
                GameImage.Source = null;
            }
            GameMenuItems.SelectedIndex = 0;
            this.noPlayMode = false;
            ShowGameDetailMenu();
        }

        private void PlatformNoPlaymodeRGS()
        {
            IGame[] _gameList = _platform.GetAllGames(false,false);
            Random _randomGame = new Random();
            IGame _randomSelectedGame = _gameList[_randomGame.Next(0, _gameList.Length)];

            _game = _randomSelectedGame;

            GameTitle.Text = _randomSelectedGame.Title;
            GamePlayMode.Text = _randomSelectedGame.PlayMode;
            if (_randomSelectedGame.FrontImagePath != null)
            {
                GameImage.Source = new BitmapImage(new Uri(_randomSelectedGame.FrontImagePath));
            }
            else
            {
                GameImage.Source = null;
            }
            GameMenuItems.SelectedIndex = 0;
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

            GameTitle.Text = _randomSelectedGame.Title;
            GamePlayMode.Text = _randomSelectedGame.PlayMode;
            if (_randomSelectedGame.FrontImagePath != null)
            {
                GameImage.Source = new BitmapImage(new Uri(_randomSelectedGame.FrontImagePath));
            }
            else
            {
                GameImage.Source = null;
            }
            GameMenuItems.SelectedIndex = 0;
            this.noPlayMode = false;
            ShowGameDetailMenu();
        }

        private void PlaylistNoPlaymodeRGS()
        {
            IGame[] _gameList = _playlist.GetAllGames(false);
            Random _randomGame = new Random();
            IGame _randomSelectedGame = _gameList[_randomGame.Next(0, _gameList.Length)];

            _game = _randomSelectedGame;

            GameTitle.Text = _randomSelectedGame.Title;
            GamePlayMode.Text = _randomSelectedGame.PlayMode;
            if (_randomSelectedGame.FrontImagePath != null)
            {
                GameImage.Source = new BitmapImage(new Uri(_randomSelectedGame.FrontImagePath));
            }
            else
            {
                GameImage.Source = null;
            }
            GameMenuItems.SelectedIndex = 0;
            this.noPlayMode = true;
            ShowGameDetailMenu();
        }

        private void AllGamesNoPlaymodeRGS()
        {
            IGame[] _gameList = PluginHelper.DataManager.GetAllGames();
            Random _randomGame = new Random();
            IGame _randomSelectedGame = _gameList[_randomGame.Next(0, _gameList.Length)];

            _game = _randomSelectedGame;

            GameTitle.Text = _randomSelectedGame.Title;
            GamePlayMode.Text = _randomSelectedGame.PlayMode;
            if (_randomSelectedGame.FrontImagePath != null)
            {
                GameImage.Source = new BitmapImage(new Uri(_randomSelectedGame.FrontImagePath));
            }
            else
            {
                GameImage.Source = null;
            }
            GameMenuItems.SelectedIndex = 0;
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

            GameTitle.Text = _randomSelectedGame.Title;
            GamePlayMode.Text = _randomSelectedGame.PlayMode;
            if (_randomSelectedGame.FrontImagePath != null)
            {
                GameImage.Source = new BitmapImage(new Uri(_randomSelectedGame.FrontImagePath));
            }
            else
            {
                GameImage.Source = null;
            }
            GameMenuItems.SelectedIndex = 0;
            this.noPlayMode = false;
            ShowGameDetailMenu();
        }

        private bool MatchesSelectedPlayMode(IGame g)
        {
            if (g.PlayMode.Contains(_playmode)) //need to add logic here to make sure game isn't marked broken or hidden
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void ShowPlayModeMenu()
        {
            PlayModeMenu.Visibility = Visibility.Visible;
            ErrorMessageMenu.Visibility = Visibility.Collapsed;
            GameDetailMenu.Visibility = Visibility.Collapsed;
        }

        private void ShowGameDetailMenu()
        {
            PlayModeMenu.Visibility = Visibility.Collapsed;
            ErrorMessageMenu.Visibility = Visibility.Collapsed;
            GameDetailMenu.Visibility = Visibility.Visible;
        }
    }
}
