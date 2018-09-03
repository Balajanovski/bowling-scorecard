Imports System.Windows.Media.Animation

Class MainWindow
    Private sound As New SoundUnit()

    Private NumRows As Integer
    Private Rows As List(Of Row) = New List(Of Row)

    Private CurrentRow As Integer

    Private Const FRAMES_PER_ROW As Integer = 10

    Private Const FIRST_ROW_X_POS As Integer = 0
    Private Const FIRST_ROW_Y_POS As Integer = 0

    ' Default dimensions so on restart the window can be ordered back to normal
    Private ReadOnly defaultWindowHeight As Integer
    Private ReadOnly defaultGridHeight As Integer
    Private ReadOnly defaultScrollViewerHeight As Integer
    Private ReadOnly defaultAddPlayerMargin As Thickness
    Private ReadOnly defaultRemovePlayerMargin As Thickness
    Private ReadOnly defaultStartGameMargin As Thickness
    Private ReadOnly defaultScoreTextBoxMargin As Thickness
    Private ReadOnly defaultNextBowlMargin As Thickness
    Private ReadOnly defaultScoreFeedbackMargin As Thickness
    Private ReadOnly defaultValidationCrossMargin As Thickness
    Private ReadOnly defaultBottomBorderMargin As Thickness
    Private ReadOnly defaultWinnerLabelMargin As Thickness
    Private ReadOnly defaultPlayAgainMargin As Thickness
    Private ReadOnly defaultAnimationImageMargin As Thickness

    ' Number of rows till the scroll wheel appears
    Private Const ROWS_TILL_SCROLL As Integer = 4

    Private Enum GameState
        Setup
        Running
        GameOver
    End Enum
    Private state As GameState

    Dim fadeInOutAnimation As New DoubleAnimation()

    Private Sub AddPlayerButton_Click(sender As Object, e As RoutedEventArgs) Handles AddPlayerButton.Click
        AddPlayer()
    End Sub

    Private Sub AddPlayer()
        Dim changeInHeight As Single = Frame.FRAME_HEIGHT

        grid.Height += changeInHeight

        If NumRows < ROWS_TILL_SCROLL Then
            Height += changeInHeight
            scrollViewer.Height += changeInHeight
            AddPlayerButton.Margin = New Thickness(AddPlayerButton.Margin.Left,
                                                    AddPlayerButton.Margin.Top + changeInHeight,
                                                    AddPlayerButton.Margin.Right,
                                                    AddPlayerButton.Margin.Bottom)
            RemovePlayerButton.Margin = New Thickness(RemovePlayerButton.Margin.Left,
                                                    RemovePlayerButton.Margin.Top + changeInHeight,
                                                    RemovePlayerButton.Margin.Right,
                                                    RemovePlayerButton.Margin.Bottom)
            StartGameButton.Margin = New Thickness(StartGameButton.Margin.Left,
                                                    StartGameButton.Margin.Top + changeInHeight,
                                                    StartGameButton.Margin.Right,
                                                    StartGameButton.Margin.Bottom)
            ScoreTextBox.Margin = New Thickness(ScoreTextBox.Margin.Left,
                                                    ScoreTextBox.Margin.Top + changeInHeight,
                                                    ScoreTextBox.Margin.Right,
                                                    ScoreTextBox.Margin.Bottom)
            NextBowlButton.Margin = New Thickness(NextBowlButton.Margin.Left,
                                                    NextBowlButton.Margin.Top + changeInHeight,
                                                    NextBowlButton.Margin.Right,
                                                    NextBowlButton.Margin.Bottom)
            ScoreFeedback.Margin = New Thickness(ScoreFeedback.Margin.Left,
                                                    ScoreFeedback.Margin.Top + changeInHeight,
                                                    ScoreFeedback.Margin.Right,
                                                    ScoreFeedback.Margin.Bottom)
            ValidationCross.Margin = New Thickness(ValidationCross.Margin.Left,
                                                    ValidationCross.Margin.Top + changeInHeight,
                                                    ValidationCross.Margin.Right,
                                                    ValidationCross.Margin.Bottom)
            bottomBorder.Margin = New Thickness(bottomBorder.Margin.Left,
                                                    bottomBorder.Margin.Top + changeInHeight,
                                                    bottomBorder.Margin.Right,
                                                    bottomBorder.Margin.Bottom)
            winnerLabel.Margin = New Thickness(winnerLabel.Margin.Left,
                                                winnerLabel.Margin.Top + changeInHeight,
                                                winnerLabel.Margin.Right,
                                                winnerLabel.Margin.Bottom)
            PlayAgainButton.Margin = New Thickness(PlayAgainButton.Margin.Left,
                                                PlayAgainButton.Margin.Top + changeInHeight,
                                                PlayAgainButton.Margin.Right,
                                                PlayAgainButton.Margin.Bottom)
            animationImage.Margin = New Thickness(animationImage.Margin.Left,
                                                scrollViewer.Margin.Top + (changeInHeight / 2),
                                                animationImage.Margin.Right,
                                                animationImage.Margin.Bottom)
        Else
            scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Visible
        End If

        NumRows += 1

        ' Add new row
        Rows.Add(New Row("Player " & CStr(NumRows), FIRST_ROW_X_POS,
                         FIRST_ROW_Y_POS + (NumRows - 1) * Frame.FRAME_HEIGHT, grid))
        Rows.ElementAt(Rows.Count - 1).FocusRowTitle()

        ' Enable the remove player button since there is more than 1 row
        RemovePlayerButton.IsEnabled = True
    End Sub

    Private Sub RemovePlayerButton_Click(sender As Object, e As RoutedEventArgs) Handles RemovePlayerButton.Click
        RemovePlayer()
    End Sub

    Private Sub RemovePlayer()
        If NumRows > 1 Then
            Dim changeInHeight As Single = Frame.FRAME_HEIGHT

            grid.Height -= changeInHeight

            If NumRows <= ROWS_TILL_SCROLL Then
                Height -= changeInHeight
                scrollViewer.Height -= changeInHeight
                AddPlayerButton.Margin = New Thickness(AddPlayerButton.Margin.Left,
                                                        AddPlayerButton.Margin.Top - changeInHeight,
                                                        AddPlayerButton.Margin.Right,
                                                        AddPlayerButton.Margin.Bottom)
                RemovePlayerButton.Margin = New Thickness(RemovePlayerButton.Margin.Left,
                                                        RemovePlayerButton.Margin.Top - changeInHeight,
                                                        RemovePlayerButton.Margin.Right,
                                                        RemovePlayerButton.Margin.Bottom)
                StartGameButton.Margin = New Thickness(StartGameButton.Margin.Left,
                                                        StartGameButton.Margin.Top - changeInHeight,
                                                        StartGameButton.Margin.Right,
                                                        StartGameButton.Margin.Bottom)
                ScoreTextBox.Margin = New Thickness(ScoreTextBox.Margin.Left,
                                                    ScoreTextBox.Margin.Top - changeInHeight,
                                                    ScoreTextBox.Margin.Right,
                                                    ScoreTextBox.Margin.Bottom)
                NextBowlButton.Margin = New Thickness(NextBowlButton.Margin.Left,
                                                    NextBowlButton.Margin.Top - changeInHeight,
                                                    NextBowlButton.Margin.Right,
                                                    NextBowlButton.Margin.Bottom)
                ScoreFeedback.Margin = New Thickness(ScoreFeedback.Margin.Left,
                                                    ScoreFeedback.Margin.Top - changeInHeight,
                                                    ScoreFeedback.Margin.Right,
                                                    ScoreFeedback.Margin.Bottom)
                ValidationCross.Margin = New Thickness(ValidationCross.Margin.Left,
                                                    ValidationCross.Margin.Top - changeInHeight,
                                                    ValidationCross.Margin.Right,
                                                    ValidationCross.Margin.Bottom)
                bottomBorder.Margin = New Thickness(bottomBorder.Margin.Left,
                                                    bottomBorder.Margin.Top - changeInHeight,
                                                    bottomBorder.Margin.Right,
                                                    bottomBorder.Margin.Bottom)
                winnerLabel.Margin = New Thickness(winnerLabel.Margin.Left,
                                                    winnerLabel.Margin.Top - changeInHeight,
                                                    winnerLabel.Margin.Right,
                                                    winnerLabel.Margin.Bottom)
                PlayAgainButton.Margin = New Thickness(PlayAgainButton.Margin.Left,
                                                    PlayAgainButton.Margin.Top - changeInHeight,
                                                    PlayAgainButton.Margin.Right,
                                                    PlayAgainButton.Margin.Bottom)
                animationImage.Margin = New Thickness(animationImage.Margin.Left,
                                                    scrollViewer.Margin.Top - (changeInHeight / 2),
                                                    animationImage.Margin.Right,
                                                    animationImage.Margin.Bottom)
            End If

            NumRows -= 1

            If NumRows <= ROWS_TILL_SCROLL Then
                scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden
            End If

            ' Remove last row
            Rows(Rows.Count - 1).MakeInvisible()
            Rows.RemoveAt(Rows.Count - 1)

            ' Disable button if there is 1 row or less
            If NumRows <= 1 Then
                RemovePlayerButton.IsEnabled = False
            End If
        End If
    End Sub

    Public Sub New()
        InitializeComponent()

        NumRows = 1
        CurrentRow = 0
        state = GameState.Setup

        Rows.Add(New Row("Player " & CStr(NumRows), FIRST_ROW_X_POS, FIRST_ROW_Y_POS, grid))
        Rows.ElementAt(Rows.Count - 1).FocusRowTitle()

        ' Initialize animation
        fadeInOutAnimation.From = 0
        fadeInOutAnimation.To = 1
        fadeInOutAnimation.Duration = New Duration(TimeSpan.FromSeconds(0.7))
        fadeInOutAnimation.AutoReverse = True

        ' Set default dimensions
        defaultWindowHeight = Window.Height
        defaultGridHeight = grid.Height
        defaultScrollViewerHeight = scrollViewer.Height
        defaultAddPlayerMargin = AddPlayerButton.Margin
        defaultRemovePlayerMargin = RemovePlayerButton.Margin
        defaultStartGameMargin = StartGameButton.Margin
        defaultScoreTextBoxMargin = ScoreTextBox.Margin
        defaultNextBowlMargin = NextBowlButton.Margin
        defaultScoreFeedbackMargin = ScoreFeedback.Margin
        defaultValidationCrossMargin = ValidationCross.Margin
        defaultBottomBorderMargin = bottomBorder.Margin
        defaultWinnerLabelMargin = winnerLabel.Margin
        defaultPlayAgainMargin = PlayAgainButton.Margin
        defaultAnimationImageMargin = animationImage.Margin

        ' Start music
        sound.playMusic()

    End Sub

    Private Sub StartGameButton_Click(sender As Object, e As RoutedEventArgs) Handles StartGameButton.Click
        StartGame()
    End Sub

    Private Sub StartGame()
        state = GameState.Running

        ' Disable and make invisible the old buttons
        AddPlayerButton.IsEnabled = False
        RemovePlayerButton.IsEnabled = False
        StartGameButton.IsEnabled = False
        AddPlayerButton.Visibility = Visibility.Collapsed
        RemovePlayerButton.Visibility = Visibility.Collapsed
        StartGameButton.Visibility = Visibility.Collapsed

        ' Make visible the new button and textbox
        NextBowlButton.IsEnabled = False
        ScoreTextBox.IsEnabled = True
        NextBowlButton.Visibility = Visibility.Visible
        ScoreTextBox.Visibility = Visibility.Visible
        ScoreFeedback.Visibility = Visibility.Visible

        ' Make visible back button
        backButton.IsEnabled = True
        backButton.Visibility = Visibility.Visible
        backButtonImage.IsEnabled = True
        backButtonImage.Visibility = Visibility.Visible

        ' Focus the score textbox
        ScoreTextBox.Focus()

        ' Set all the names to uneditable
        ' and unfocus all the rows
        For i = 0 To Rows.Count() - 1
            Rows.ElementAt(i).MakeTitleUneditable()
            Rows.ElementAt(i).UnFocus()
        Next

        ' Focus current row
        Rows(CurrentRow).Focus(System.Windows.Media.Colors.LightSkyBlue)

        ' Route events for special strikes from rows
        For Each row In Rows
            AddHandler row.AchievedSpecialScore, AddressOf Me.Row_AchievedSpecialScore
        Next
    End Sub

    ' Animations
    Private Sub Row_AchievedSpecialScore(ByVal sender As Object, ByVal e As SpecialScoreEventArgs)

        Dim score As Row.SpecialScores = e.score

        Select Case score
            Case Row.SpecialScores.Spare
                animationImage.Source = New BitmapImage(New Uri("pack://application:,,,/Resources/spare.png"))
                animationImage.BeginAnimation(Image.OpacityProperty, fadeInOutAnimation)
            Case Row.SpecialScores.Strike
                sound.playPinsFalling()
                animationImage.Source = New BitmapImage(New Uri("pack://application:,,,/Resources/strike.png"))
                animationImage.BeginAnimation(Image.OpacityProperty, fadeInOutAnimation)
            Case Row.SpecialScores._Double
                sound.playPinsFalling()
                animationImage.Source = New BitmapImage(New Uri("pack://application:,,,/Resources/double.png"))
                animationImage.BeginAnimation(Image.OpacityProperty, fadeInOutAnimation)
            Case Row.SpecialScores.Turkey
                sound.playPinsFalling()
                animationImage.Source = New BitmapImage(New Uri("pack://application:,,,/Resources/turkey.png"))
                animationImage.BeginAnimation(Image.OpacityProperty, fadeInOutAnimation)
            Case Row.SpecialScores.SquareBall
                sound.playPinsFalling()
                animationImage.Source = New BitmapImage(New Uri("pack://application:,,,/Resources/squareBall.png"))
                animationImage.BeginAnimation(Image.OpacityProperty, fadeInOutAnimation)
            Case Row.SpecialScores.HighFive
                sound.playPinsFalling()
                animationImage.Source = New BitmapImage(New Uri("pack://application:,,,/Resources/highFive.png"))
                animationImage.BeginAnimation(Image.OpacityProperty, fadeInOutAnimation)
            Case Row.SpecialScores.SixPack
                sound.playPinsFalling()
                animationImage.Source = New BitmapImage(New Uri("pack://application:,,,/Resources/sixPack.png"))
                animationImage.BeginAnimation(Image.OpacityProperty, fadeInOutAnimation)
            Case Row.SpecialScores.LuckySeven
                sound.playPinsFalling()
                animationImage.Source = New BitmapImage(New Uri("pack://application:,,,/Resources/luckySeven.png"))
                animationImage.BeginAnimation(Image.OpacityProperty, fadeInOutAnimation)
            Case Row.SpecialScores.Octopus
                sound.playPinsFalling()
                animationImage.Source = New BitmapImage(New Uri("pack://application:,,,/Resources/octopus.png"))
                animationImage.BeginAnimation(Image.OpacityProperty, fadeInOutAnimation)
            Case Row.SpecialScores.GoldenTurkey
                sound.playPinsFalling()
                animationImage.Source = New BitmapImage(New Uri("pack://application:,,,/Resources/goldenTurkey.png"))
                animationImage.BeginAnimation(Image.OpacityProperty, fadeInOutAnimation)
            Case Row.SpecialScores.DimeBag
                sound.playPinsFalling()
                animationImage.Source = New BitmapImage(New Uri("pack://application:,,,/Resources/dimeBag.png"))
                animationImage.BeginAnimation(Image.OpacityProperty, fadeInOutAnimation)
            Case Row.SpecialScores.AcesUp
                sound.playPinsFalling()
                animationImage.Source = New BitmapImage(New Uri("pack://application:,,,/Resources/acesUp.png"))
                animationImage.BeginAnimation(Image.OpacityProperty, fadeInOutAnimation)
            Case Row.SpecialScores.PerfectGame
                sound.playPinsFalling()
                animationImage.Source = New BitmapImage(New Uri("pack://application:,,,/Resources/perfectGame.png"))
                animationImage.BeginAnimation(Image.OpacityProperty, fadeInOutAnimation)
        End Select
    End Sub

    Private Sub NextBowlButton_Click(sender As Object, e As RoutedEventArgs) Handles NextBowlButton.Click
        InputScore()
    End Sub

    Private Sub MainWindow_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If (e.Key = Key.Enter Or e.Key = Key.Return) Then
            If state = GameState.Running And NextBowlButton.IsEnabled Then
                InputScore()
            ElseIf state = GameState.Setup Then
                StartGame()
            End If
        End If
    End Sub

    Private Sub MainWindow_KeyUp(sender As Object, e As KeyEventArgs) Handles Me.KeyUp
        If state = GameState.Running Then
            AssessScoreTextboxValidity()
        End If
    End Sub

    Private Sub AssessScoreTextboxValidity()
        Dim rawScore As String = ScoreTextBox.Text

        If rawScore.All(AddressOf Char.IsDigit) And rawScore <> "" Then
            If rawScore.Length > 2 Then
                ScoreFeedback.Content = "Input score must be between 0 and 10 inclusive"
                NextBowlButton.IsEnabled = False
                ValidationCross.Visibility = Visibility.Visible
            Else
                Dim score As Integer = CInt(rawScore)
                If score < 0 Or score > 10 Then
                    ScoreFeedback.Content = "Input score must be between 0 and 10 inclusive"
                    NextBowlButton.IsEnabled = False
                    ValidationCross.Visibility = Visibility.Visible
                Else
                    If 10 - Rows.ElementAt(CurrentRow).CurrFrameScore >= score Or Rows.ElementAt(CurrentRow).IsCurrFrameThreeBowl Then
                        ScoreFeedback.Content = ""
                        NextBowlButton.IsEnabled = True
                        ValidationCross.Visibility = Visibility.Collapsed
                    Else
                        ScoreFeedback.Content = "Illegal score. Score sum of frame must equal 10."
                        NextBowlButton.IsEnabled = False
                        ValidationCross.Visibility = Visibility.Visible
                    End If
                End If
            End If
        ElseIf rawScore = "" Then
            ScoreFeedback.Content = ""
            NextBowlButton.IsEnabled = False
            ValidationCross.Visibility = Visibility.Collapsed
        Else
            ScoreFeedback.Content = "Input must only consist of digits"
            NextBowlButton.IsEnabled = False
            ValidationCross.Visibility = Visibility.Visible
        End If
    End Sub

    Private Sub GameOver()
        state = GameState.GameOver
        NextBowlButton.IsEnabled = False
        ScoreTextBox.IsEnabled = False
        NextBowlButton.Visibility = Visibility.Collapsed
        ScoreTextBox.Visibility = Visibility.Collapsed

        ' Unfocus all rows
        For i = 0 To Rows.Count() - 1
            Rows.ElementAt(i).UnFocus()
        Next

        ' Determine winner
        Dim winnerPlayers As New List(Of Integer)
        Dim highestScore As Integer = -1
        For i = 0 To Rows.Count() - 1
            If Rows(i).TotalScore > highestScore Then
                winnerPlayers.Clear()
                winnerPlayers.Add(i)
                highestScore = Rows(i).TotalScore
            ElseIf Rows(i).TotalScore = highestScore Then
                winnerPlayers.Add(i)
            End If
        Next

        If winnerPlayers.Count = 1 Then
            winnerLabel.Content = "The winner is: " & Rows(winnerPlayers(0)).Title
        ElseIf winnerPlayers.Count = 0 Then
            winnerLabel.Content = "There are no winners"
        ElseIf winnerPlayers.Count > 1 Then
            Dim winnerPlayerNames As New List(Of String)
            For Each winnerIndex In winnerPlayers
                winnerPlayerNames.Add(Rows(winnerIndex).Title)
            Next

            winnerLabel.Content = "The winners are: " & String.Join(", ", winnerPlayerNames)
        End If
        winnerLabel.Visibility = Visibility.Visible

        ' Make play again button visible
        PlayAgainButton.Visibility = Visibility.Visible
        PlayAgainButton.IsEnabled = True

    End Sub

    Private Sub InputScore()
        AssessScoreTextboxValidity()

        If NextBowlButton.IsEnabled Then
            ' If current frame was filled
            If Rows.ElementAt(CurrentRow).Bowl(CInt(ScoreTextBox.Text)) Then
                ' Unfocus previous row
                Rows.ElementAt(CurrentRow).UnFocus()

                CurrentRow = CurrentRow + 1
                If CurrentRow >= Rows.Count Then
                    CurrentRow = 0
                End If

                ' Focus current row
                Rows.ElementAt(CurrentRow).Focus(System.Windows.Media.Colors.LightSkyBlue)

                If CurrentRow = 0 And Rows.ElementAt(CurrentRow).Filled Then
                    GameOver()
                End If
            End If

            ' Clear score textbox contents
            ScoreTextBox.Text = ""
        End If
    End Sub

    Private Sub PlayAgainButton_Click(sender As Object, e As RoutedEventArgs) Handles PlayAgainButton.Click
        Reset()
    End Sub

    Private Sub Reset()
        ' Make textbox invisible
        ScoreTextBox.Visibility = Visibility.Collapsed
        ScoreTextBox.IsEnabled = False

        ' Make next bowl button invisible
        NextBowlButton.Visibility = Visibility.Collapsed
        NextBowlButton.IsEnabled = False

        ' Make play again button invisible
        PlayAgainButton.IsEnabled = False
        PlayAgainButton.Visibility = Visibility.Collapsed

        ' Make validation elements invisible
        ValidationCross.Visibility = Visibility.Collapsed
        ScoreFeedback.Visibility = Visibility.Collapsed
        ScoreFeedback.Content = ""

        ' Clear winner label, and make invisible
        winnerLabel.Content = ""
        winnerLabel.Visibility = Visibility.Collapsed

        ' Detach event handlers from old rows
        For Each row In Rows
            RemoveHandler row.AchievedSpecialScore, AddressOf Me.Row_AchievedSpecialScore
        Next

        ' Clear all scores from previous game
        For Each row In Rows
            row.MakeInvisible()
        Next
        Rows.Clear()

        ' Reset game state
        NumRows = 1
        CurrentRow = 0
        state = GameState.Setup

        ' Create new, default player 1 row
        Rows.Add(New Row("Player " & CStr(NumRows), FIRST_ROW_X_POS, FIRST_ROW_Y_POS, grid))
        Rows.ElementAt(Rows.Count - 1).FocusRowTitle()

        ' Set add player, start game and remove player buttons to be visible
        AddPlayerButton.IsEnabled = True
        AddPlayerButton.Visibility = Visibility.Visible
        StartGameButton.IsEnabled = True
        StartGameButton.Visibility = Visibility.Visible
        RemovePlayerButton.IsEnabled = False
        RemovePlayerButton.Visibility = Visibility.Visible

        ' Set dimensions back to normal
        Window.Height = defaultWindowHeight
        grid.Height = defaultGridHeight
        scrollViewer.Height = defaultScrollViewerHeight
        AddPlayerButton.Margin = defaultAddPlayerMargin
        StartGameButton.Margin = defaultStartGameMargin
        RemovePlayerButton.Margin = defaultRemovePlayerMargin
        ScoreTextBox.Margin = defaultScoreTextBoxMargin
        NextBowlButton.Margin = defaultNextBowlMargin
        ScoreFeedback.Margin = defaultScoreFeedbackMargin
        ValidationCross.Margin = defaultValidationCrossMargin
        bottomBorder.Margin = defaultBottomBorderMargin
        winnerLabel.Margin = defaultWinnerLabelMargin
        PlayAgainButton.Margin = defaultPlayAgainMargin
        animationImage.Margin = defaultAnimationImageMargin

        ' Clear score textbox contents
        ScoreTextBox.Text = ""

        ' Make invisible back button
        backButton.IsEnabled = False
        backButton.Visibility = Visibility.Collapsed
        backButtonImage.IsEnabled = False
        backButtonImage.Visibility = Visibility.Collapsed
    End Sub

    Private Sub animationButton_Click(sender As Object, e As RoutedEventArgs) Handles animationButton.Click
        If animationImage.Visibility = Visibility.Visible Then
            animationImage.Visibility = Visibility.Collapsed

            animationButtonImage.Source = New BitmapImage(New Uri("pack://application:,,,/Resources/animationDisabled.png"))
            animationButton.ToolTip = "Click to enable animations"
        Else
            animationImage.Visibility = Visibility.Visible

            animationButtonImage.Source = New BitmapImage(New Uri("pack://application:,,,/Resources/animationEnabled.png"))
            animationButton.ToolTip = "Click to disable animations"
        End If
    End Sub

    Private Sub soundButton_Click(sender As Object, e As RoutedEventArgs) Handles soundButton.Click
        sound.toggleMute()

        If sound.isMuted Then
            soundButtonImage.Source = New BitmapImage(New Uri("pack://application:,,,/Resources/soundDisable.png"))
            soundButton.ToolTip = "Click to enable sound"
        Else
            soundButtonImage.Source = New BitmapImage(New Uri("pack://application:,,,/Resources/soundEnable.png"))
            soundButton.ToolTip = "Click to disable sound"
        End If
    End Sub

    Private Sub MainWindow_Exit() Handles Me.Closing
        sound.FreeResources()
    End Sub

    Private Sub backButton_Click(sender As Object, e As RoutedEventArgs) Handles backButton.Click
        Dim result As System.Windows.Forms.DialogResult = MessageBox.Show("By going back you will start a new game" & vbCrLf & "Are you sure you want to go back?",
                                                      "Go Back?",
                                                      System.Windows.Forms.MessageBoxButtons.YesNo,
                                                      System.Windows.Forms.MessageBoxIcon.Warning,
                                                      System.Windows.Forms.MessageBoxDefaultButton.Button1)

        If result = System.Windows.Forms.DialogResult.Yes Then
            Reset()
        End If
    End Sub
End Class
