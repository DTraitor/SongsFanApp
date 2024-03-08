using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using InteractionLogic;

namespace Interface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            RegenerateWindow();
            Closed += (sender, args) =>
            {
                logic.Dispose();
            };
        }

        private void RegenerateWindow()
        {
            ObjectsList.Children.Clear();
            BottomGrid.Children.Clear();
            BottomGrid.RowDefinitions.Clear();
            BottomGrid.ColumnDefinitions.Clear();
            switch (currentTab)
            {
                case DisplayTab.Songs:
                    GenerateSongsTab();
                    break;
                case DisplayTab.Singers:
                    GenerateSingersTab();
                    break;
                case DisplayTab.Disks:
                    GenerateDisksTab();
                    break;
                case DisplayTab.Custom:
                    if (generateCustomControls is null)
                        break;
                    List<Grid> controls = generateCustomControls();
                    foreach (Grid control in controls)
                    {
                        ObjectsList.Children.Add(control);
                    }
                    break;
            }
        }

        private void GenerateSongsTab()
        {
            foreach (Grid control in GenerateControls(logic.GetSongs()))
            {
                ObjectsList.Children.Add(control);
            }

            BottomGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) });
            BottomGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) });
            BottomGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            BottomGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(70, GridUnitType.Pixel) });

            TextBox songNameBox = new TextBox() { Text = "Song name" };
            Grid.SetColumn(songNameBox, 0);

            TextBox songGenreBox = new TextBox() { Text = "Song genre" };
            songGenreBox.Margin = new Thickness(5, 0, 2.5, 0);
            Grid.SetColumn(songGenreBox, 1);

            ComboBox songArtistBox = new ComboBox()
            {
                ItemsSource = logic.GetSingers(),
                DisplayMemberPath = "Name",
                SelectedValuePath = "ID",
            };
            songArtistBox.Margin = new Thickness(2.5, 0, 5, 0);
            Grid.SetColumn(songArtistBox, 2);

            Button addSongButton = new Button() { Content = "Add song" };
            addSongButton.Click += (sender, args) =>
            {
                if (songArtistBox.SelectedValue is null)
                    logic.CreateSong(songNameBox.Text, songGenreBox.Text, Guid.Empty);
                else
                    logic.CreateSong(songNameBox.Text, songGenreBox.Text, (Guid)songArtistBox.SelectedValue);
                RegenerateWindow();
            };
            Grid.SetColumn(addSongButton, 3);

            BottomGrid.Children.Add(songNameBox);
            BottomGrid.Children.Add(songGenreBox);
            BottomGrid.Children.Add(songArtistBox);
            BottomGrid.Children.Add(addSongButton);
        }

        private void GenerateSingersTab()
        {
            foreach (Grid control in GenerateControls(logic.GetSingers()))
            {
                ObjectsList.Children.Add(control);
            }

            BottomGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) });
            BottomGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(70, GridUnitType.Pixel) });

            TextBox singerNameBox = new TextBox() { Text = "Artist name" };
            Grid.SetColumn(singerNameBox, 0);

            Button addSingerButton = new Button() { Content = "Add artist" };
            addSingerButton.Margin = new Thickness(5, 0, 0, 0);
            addSingerButton.Click += (sender, args) =>
            {
                logic.CreateSinger(singerNameBox.Text);
                RegenerateWindow();
            };
            Grid.SetColumn(addSingerButton, 3);

            BottomGrid.Children.Add(singerNameBox);
            BottomGrid.Children.Add(addSingerButton);
        }

        private void GenerateDisksTab()
        {
            foreach (Grid control in GenerateControls(logic.GetDisks()))
            {
                ObjectsList.Children.Add(control);
            }

            BottomGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) });
            BottomGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(70, GridUnitType.Pixel) });

            TextBox diskNameBox = new TextBox() { Text = "Disk name" };
            Grid.SetColumn(diskNameBox, 0);

            Button addDiskButton = new Button() { Content = "Add disk" };
            addDiskButton.Margin = new Thickness(5, 0, 0, 0);
            addDiskButton.Click += (sender, args) =>
            {
                logic.CreateDisk(diskNameBox.Text);
                RegenerateWindow();
            };
            Grid.SetColumn(addDiskButton, 3);

            BottomGrid.Children.Add(diskNameBox);
            BottomGrid.Children.Add(addDiskButton);
        }

        private InteractLogic logic = new ("database.json");
        private DisplayTab currentTab = DisplayTab.Songs;
        private GenerateCustomControlsDelegate? generateCustomControls;

        private enum DisplayTab
        {
            Songs,
            Singers,
            Disks,
            Custom
        }

        private delegate List<Grid> GenerateCustomControlsDelegate();

        private List<Grid> GenerateControls(List<Song> songs)
        {
            List<Grid> controls = new();

            foreach (Song song in songs)
            {
                Grid songGrid = new Grid();

                songGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                songGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                songGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(100, GridUnitType.Pixel) });
                songGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
                songGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });

                TextBox songNameBox = new TextBox() { Text = song.Name };
                songNameBox.TextChanged += (sender, args) =>
                {
                    logic.ChangeName(song, songNameBox.Text);
                };
                TextBox songGenreBox = new TextBox() { Text = song.Genre };
                songGenreBox.TextChanged += (sender, args) =>
                {
                    logic.ChangeGenre(song, songGenreBox.Text);
                };
                ComboBox songArtistBox = new ComboBox()
                {
                    ItemsSource = logic.GetSingers(),
                    DisplayMemberPath = "Name",
                    SelectedValuePath = "ID",
                    SelectedValue = song.ArtistID
                };
                songArtistBox.SelectionChanged += (sender, args) =>
                {
                    if (songArtistBox.SelectedValue is null)
                        logic.ChangeArtist(song, Guid.Empty);
                    else
                        logic.ChangeArtist(song, (Guid)songArtistBox.SelectedValue);
                };
                Button listDisksButton = new Button() { Content = "List Disks" };
                listDisksButton.Click += (sender, args) =>
                {
                    generateCustomControls = () =>
                    {
                        return GenerateControls(logic.GetDisks(disk => disk.SongID.Contains(song.ID)));
                    };
                    currentTab = DisplayTab.Custom;
                    UpdateButtons();
                    RegenerateWindow();
                };
                Button deleteSongButton = new Button() { Content = "Delete song" };
                deleteSongButton.Click += (sender, args) =>
                {
                    logic.DeleteSong(song);
                    RegenerateWindow();
                };

                Grid.SetColumn(songNameBox, 0);
                Grid.SetColumn(songGenreBox, 1);
                Grid.SetColumn(songArtistBox, 2);
                Grid.SetColumn(listDisksButton, 3);
                Grid.SetColumn(deleteSongButton, 4);

                songGrid.Children.Add(songNameBox);
                songGrid.Children.Add(songGenreBox);
                songGrid.Children.Add(songArtistBox);
                songGrid.Children.Add(listDisksButton);
                songGrid.Children.Add(deleteSongButton);

                controls.Add(songGrid);
            }

            return controls;
        }

        private List<Grid> GenerateControls(List<Singer> singers)
        {
            List<Grid> controls = new();

            foreach (Singer singer in singers)
            {
                Grid singerGrid = new Grid();

                singerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                singerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
                singerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });

                TextBox singerNameBox = new TextBox() { Text = singer.Name };
                singerNameBox.TextChanged += (sender, args) =>
                {
                    logic.ChangeName(singer, singerNameBox.Text);
                };
                Button listSongsButton = new Button() { Content = "List Songs" };
                listSongsButton.Click += (sender, args) =>
                {
                    generateCustomControls = () =>
                    {
                        return GenerateControls(logic.GetSongs(song => song.ArtistID == singer.ID));
                    };
                    currentTab = DisplayTab.Custom;
                    UpdateButtons();
                    RegenerateWindow();
                };
                Button deleteSingerButton = new Button() { Content = "Delete artist" };
                deleteSingerButton.Click += (sender, args) =>
                {
                    logic.DeleteSinger(singer);
                    RegenerateWindow();
                };

                Grid.SetColumn(singerNameBox, 0);
                Grid.SetColumn(listSongsButton, 1);
                Grid.SetColumn(deleteSingerButton, 3);

                singerGrid.Children.Add(singerNameBox);
                singerGrid.Children.Add(listSongsButton);
                singerGrid.Children.Add(deleteSingerButton);

                controls.Add(singerGrid);
            }

            return controls;
        }

        private List<Grid> GenerateControls(List<Disk> disks)
        {
            List<Grid> controls = new();

            foreach (Disk disk in disks)
            {
                Grid diskGrid = new Grid();

                diskGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                diskGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
                diskGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
                diskGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });

                TextBox diskNameBox = new TextBox() { Text = disk.Name };
                diskNameBox.TextChanged += (sender, args) =>
                {
                    logic.ChangeName(disk, diskNameBox.Text);
                };
                Button listSongsButton = new Button() { Content = "List Songs" };
                listSongsButton.Click += (sender, args) =>
                {
                    generateCustomControls = () =>
                    {
                        return GenerateControls(logic.GetSongs(song => song.DiskID.Contains(disk.ID)));
                    };
                    currentTab = DisplayTab.Custom;
                    UpdateButtons();
                    RegenerateWindow();
                };
                Button editSongsButton = new Button() { Content = "Edit Songs" };
                editSongsButton.Click += (sender, args) =>
                {
                    generateCustomControls = () =>
                    {
                        // CheckBox, TextBlock, TextBlock, TextBlock
                        List<Grid> controls = new();

                        foreach (Song song in logic.GetSongs())
                        {
                            Grid songGrid = new Grid();

                            songGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
                            songGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                            songGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                            songGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

                            CheckBox songCheckBox = new CheckBox() { IsChecked = disk.SongID.Contains(song.ID) };
                            songCheckBox.Checked += (sender, args) =>
                            {
                                logic.AddSongToDisk(song, disk);
                            };
                            songCheckBox.Unchecked += (sender, args) =>
                            {
                                logic.RemoveSongFromDisk(song, disk);
                            };
                            TextBlock songNameBlock = new TextBlock() { Text = song.Name };
                            TextBlock songGenreBlock = new TextBlock() { Text = song.Genre };
                            TextBlock songArtistBlock = new TextBlock() { Text = logic.GetSinger(song.ArtistID)?.Name ?? "Unknown" };

                            Grid.SetColumn(songCheckBox, 0);
                            Grid.SetColumn(songNameBlock, 1);
                            Grid.SetColumn(songGenreBlock, 2);
                            Grid.SetColumn(songArtistBlock, 3);

                            songGrid.Children.Add(songCheckBox);
                            songGrid.Children.Add(songNameBlock);
                            songGrid.Children.Add(songGenreBlock);
                            songGrid.Children.Add(songArtistBlock);

                            controls.Add(songGrid);
                        }

                        return controls;
                    };
                    currentTab = DisplayTab.Custom;
                    UpdateButtons();
                    RegenerateWindow();
                };
                Button deleteDiskButton = new Button() { Content = "Delete disk" };
                deleteDiskButton.Click += (sender, args) =>
                {
                    logic.DeleteDisk(disk);
                    RegenerateWindow();
                };

                Grid.SetColumn(diskNameBox, 0);
                Grid.SetColumn(listSongsButton, 1);
                Grid.SetColumn(editSongsButton, 2);
                Grid.SetColumn(deleteDiskButton, 3);

                diskGrid.Children.Add(diskNameBox);
                diskGrid.Children.Add(listSongsButton);
                diskGrid.Children.Add(editSongsButton);
                diskGrid.Children.Add(deleteDiskButton);

                controls.Add(diskGrid);
            }

            return controls;
        }

        private void UpdateButtons()
        {
            SongsButton.IsEnabled = currentTab != DisplayTab.Songs;
            SingersButton.IsEnabled = currentTab != DisplayTab.Singers;
            DisksButton.IsEnabled = currentTab != DisplayTab.Disks;
        }

        private void OnSwitchToSongs(object sender, RoutedEventArgs e)
        {
            currentTab = DisplayTab.Songs;
            UpdateButtons();
            RegenerateWindow();
        }

        private void OnSwitchToSingers(object sender, RoutedEventArgs e)
        {
            currentTab = DisplayTab.Singers;
            UpdateButtons();
            RegenerateWindow();
        }

        private void OnSwitchToDisks(object sender, RoutedEventArgs e)
        {
            currentTab = DisplayTab.Disks;
            UpdateButtons();
            RegenerateWindow();
        }
    }
}