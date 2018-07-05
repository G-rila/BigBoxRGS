# BigBoxRGS
## BigBox Random Game Selector v2
This plugin adds the ability to choose a random game from the selected platform/playlist or from your entire collection based on play-mode or genre.

## Installation
Place the .DLL in your LaunchBox\Plugins folder.

## Setup
1. Add the namespace to the view(s) you want to be able to call the plugin from. Ideally, this would just be whatever platform view you are using (ie. PlatformWheel1FiltersView.xaml) but it can be added to the game view(s) also.
```xaml
xmlns:bbrgs="clr-namespace:BigBoxRGS;assembly=BigBoxRGS"
```

#### Example of namespace added to existing code
```xaml
<UserControl xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
             xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
             xmlns:transitions="clr-namespace:Unbroken.LaunchBox.Wpf.Transitions;assembly=Unbroken.LaunchBox.Wpf"
             xmlns:coverFlow="clr-namespace:Unbroken.LaunchBox.Wpf.Controls.CoverFlow;assembly=Unbroken.LaunchBox.Wpf"
             xmlns:cal="http://www.caliburnproject.org"
             xmlns:i="http://schemas.microsoft.com/expression/2010/interactivity"
             xmlns:bbrgs="clr-namespace:BigBoxRGS;assembly=BigBoxRGS"
             mc:Ignorable="d"
             d:DesignHeight="562" d:DesignWidth="1000" HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Style="{DynamicResource UserControlStyle}">
```

2. Add this code after the closing ```</Grid>``` tag, before the closing ```</Canvas>``` tag.
```xaml
<Grid Height="{Binding ElementName=Canvas, Path=ActualHeight}" Width="{Binding ElementName=Canvas, Path=ActualWidth}">
  <bbrgs:RGSv2_0 HorizontalAlignment="Center" VerticalAlignment="Center" ShowGameDetails="False" ShowGameNotes="False" CheckForUpdates="True"/>
</Grid>
```

#### Example of entire code with namespace declared at top and plugin code added in correct location
```xaml
<UserControl xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
             xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
             xmlns:transitions="clr-namespace:Unbroken.LaunchBox.Wpf.Transitions;assembly=Unbroken.LaunchBox.Wpf"
             xmlns:coverFlow="clr-namespace:Unbroken.LaunchBox.Wpf.Controls.CoverFlow;assembly=Unbroken.LaunchBox.Wpf"
             xmlns:cal="http://www.caliburnproject.org"
             xmlns:i="http://schemas.microsoft.com/expression/2010/interactivity"
             xmlns:bbrgs="clr-namespace:BigBoxRGS;assembly=BigBoxRGS"
             mc:Ignorable="d"
             d:DesignHeight="562" d:DesignWidth="1000" HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Style="{DynamicResource UserControlStyle}">
	<Canvas Name="Canvas">
        <transitions:TransitionPresenter TransitionSelector="{Binding BackgroundTransitionSelector}" Content="{Binding BackgroundView}" Height="{Binding ElementName=Canvas, Path=ActualHeight}" Width="{Binding ElementName=Canvas, Path=ActualWidth}" IsContentVideo="true" />
        <Grid Height="{Binding ElementName=Canvas, Path=ActualHeight}" Width="{Binding ElementName=Canvas, Path=ActualWidth}">
			<Grid.Background>
				<SolidColorBrush Color="Black" Opacity="{Binding BackgroundFade}" />
			</Grid.Background>
            <Grid.ColumnDefinitions>
                <ColumnDefinition Width="42*" />
                <ColumnDefinition Width="4*" />
                <ColumnDefinition Width="92*" />
            </Grid.ColumnDefinitions>
            <coverFlow:FlowControl x:Name="FlowControl" HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Grid.Column="0" CoverFactory="{Binding CoverFactory}" ImageType="Clear Logo"
                CurveAmount="4.5" CameraZPosition="3.0" VisibleCount="14" PageSize="6" Spacing="1.0" ItemZPosition="1.0" SelectedItemZPosition="2.0" />
            <Grid Grid.Column="2">
                <Grid.RowDefinitions>
                    <RowDefinition Height="5*" />
                    <RowDefinition Height="50*" />
                    <RowDefinition Height="2*" />
                    <RowDefinition Height="25*" />
                    <RowDefinition Height="2*" />
                    <RowDefinition Height="25*" />
                    <RowDefinition Height="5*" />
                </Grid.RowDefinitions>
                <Grid Grid.Row="1">
                    <Grid.ColumnDefinitions>
                        <ColumnDefinition Width="44*" />
                        <ColumnDefinition Width="4*" />
                        <ColumnDefinition Width="57*" />
                        <ColumnDefinition Width="4*" />
                    </Grid.ColumnDefinitions>
                    <transitions:TransitionPresenter Grid.Column="0" TransitionSelector="{Binding ImageTransitionSelector}" Content="{Binding ImageView}" />
                    <transitions:TransitionPresenter Grid.Column="2" TransitionSelector="{Binding DetailsTransitionSelector}" Content="{Binding DetailsView}" />
                </Grid>
                <transitions:TransitionPresenter Grid.Row="3" TransitionSelector="{Binding TopBoxesTransitionSelector}" Content="{Binding TopBoxesView}" />
                <transitions:TransitionPresenter Grid.Row="5" TransitionSelector="{Binding BottomBoxesTransitionSelector}" Content="{Binding BottomBoxesView}" />
            </Grid>
        </Grid><!-- CLOSING GRID TAG -->
        <Grid Height="{Binding ElementName=Canvas, Path=ActualHeight}" Width="{Binding ElementName=Canvas, Path=ActualWidth}">
          <bbrgs:RGSv2_0 HorizontalAlignment="Center" VerticalAlignment="Center" ShowGameDetails="False" ShowGameNotes="False" CheckForUpdates="True"/>
        </Grid> 
    </Canvas><!-- CLOSING CANVAS TAG -->
</UserControl>
```

## Configure
* You can control if the plugin shows game details/notes by changing the boolean properties ```ShowGameNotes=""``` and ```ShowGameDetails=""```.
* You can control if the plugin checks for updates by changing the boolean property ```CheckForUpdates=""```. To conform with GitHub's API policy, the query will only happen once every 30 minutes.

## Usage
In BigBox, while you are in the view(s) you added the plugin to, hold right to call the plugin.
