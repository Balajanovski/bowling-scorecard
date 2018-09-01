Imports System.Windows.Markup

Public Class SpecialScoreEventArgs
    Inherits System.EventArgs

    Public Sub New(ByVal score As Row.SpecialScores)
        specialScore = score
    End Sub

    Public ReadOnly Property score As Row.SpecialScores
        Get
            Return specialScore
        End Get
    End Property

    Private specialScore As Row.SpecialScores
End Class

Public Class Row
    Private Const NUM_FRAMES As Integer = 10

    Private frames(NUM_FRAMES - 1) As Frame

    Public Enum SpecialScores
        Spare
        Strike
        _Double
        Turkey
        SquareBall
        HighFive
        SixPack
        LuckySeven
        Octopus
        GoldenTurkey
        DimeBag
        AcesUp
        PerfectGame
    End Enum
    Public Event AchievedSpecialScore(ByVal sender As Object, ByVal e As SpecialScoreEventArgs)

    Private consecutiveStrikes As Integer


    ' To access and edit the score of a specific bowl
    Private Property BowlVal(ByVal bowlNum As Integer) As Integer
        Get
            If bowlNum <= bowlsMade Then
                Dim frameIndex As Integer = 0
                Dim bowlIndex As Integer = 0

                While bowlNum > 0
                    bowlNum -= frames(frameIndex).bowlsMade
                    frameIndex += 1
                End While

                ' If odd num of bowls made, adjust
                If bowlNum < 0 Then
                    bowlIndex = 1
                    frameIndex -= 1
                End If

                Return frames(frameIndex).bowlScore(bowlIndex)
            Else
                Return 0
            End If
        End Get
        Set(value As Integer)
            If bowlNum <= bowlsMade Then
                Dim frameIndex As Integer = 0
                Dim bowlIndex As Integer = 0

                While bowlNum > 0
                    bowlNum -= frames(frameIndex).bowlsMade
                    frameIndex += 1
                End While

                ' If odd num of bowls made, adjust
                If bowlNum < 0 Then
                    bowlIndex = 1
                    frameIndex -= 1
                End If

                frames(frameIndex).bowlScore(bowlIndex) = value
            Else
                Throw New System.Exception("Attempted to reset bowl score out of bounds")
            End If
        End Set
    End Property

    ' Find out if bowl is in the specified frame
    Private ReadOnly Property IsInFrame(ByVal bowlNum As Integer, ByVal frame As Integer) As Boolean
        Get
            If bowlNum <= bowlsMade Then
                Dim frameIndex As Integer = 0
                Dim bowlIndex As Integer = 0

                While bowlNum > 0
                    bowlNum -= frames(frameIndex).bowlsMade
                    frameIndex += 1
                End While

                ' If odd num of bowls made, adjust
                If bowlNum < 0 Then
                    bowlIndex = 1
                    frameIndex -= 1
                End If

                Return (frameIndex = frame)
            Else
                Return False
            End If
        End Get
    End Property

    ' Used for determining which frame to input scores
    Private currentFrame As Integer

    ' Bowls made in this row
    Private bowlsMade As Integer

    ' Represents a score such as a strike or spare
    ' which has a bonus compounded onto the frame's score
    Private Class Compounder
        Public bowlsMadeTillCompound As Integer
        Public numCompounds As Integer
        Public frameIndex As Integer

        Public Sub New(ByVal bowlsMade As Integer, ByVal fIndex As Integer, ByVal numComp As Integer)
            bowlsMadeTillCompound = bowlsMade
            frameIndex = fIndex
            numCompounds = numComp
        End Sub
    End Class

    ' All frame indices with all strikes / spares
    ' So their scores can be compounded
    Private compoundFrames As New List(Of Compounder)

    ' Total score of all the frames added together
    Private rowTotalScore As Integer
    Public ReadOnly Property TotalScore() As Integer
        Get
            Return rowTotalScore
        End Get
    End Property

    ' Has row been filled?
    Private rowFilled As Boolean
    Public ReadOnly Property Filled() As Boolean
        Get
            Return rowFilled
        End Get
    End Property

    ' Get row title
    Public ReadOnly Property Title() As String
        Get
            Return titleTextBox.Text
        End Get
    End Property

    ' Total scores of all the fully filled frames summed together
    Private previousFrameTotalScore As Integer
    ' Total score of current frame
    Private currentFrameScore As Integer
    Public ReadOnly Property CurrFrameScore As Integer
        Get
            Return currentFrameScore
        End Get
    End Property

    Public ReadOnly Property IsCurrFrameThreeBowl As Boolean
        Get
            Return frames.ElementAt(currentFrame).ThreeBowlFrame
        End Get
    End Property

    ' WPF Xaml Templates
    Private Const titleRectangleTemp As String = "<Rectangle xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' 
            xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' Fill=""#FFF4F4F5"" HorizontalAlignment=""Left"" Height=""65""
            Stroke=""Black"" VerticalAlignment=""Top"" Width=""65""/>"
    Private Const titleLabelTemp As String = "<Label xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' 
            xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' HorizontalAlignment="" Left"" Height="" 33""
            VerticalAlignment=""Top"" Width=""65""/>"
    Private Const titleTextBoxTemp As String = "<TextBox xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' 
            xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' HorizontalAlignment="" Left"" Height="" 32""
            VerticalAlignment=""Top"" Width=""63"" BorderThickness=""0"" Background=""Transparent"" Padding=""2, 4, 2, 4"" />"

    ' WPF Elements
    Private titleRect As Rectangle
    Private titleLabel As Label
    Private WithEvents titleTextBox As TextBox

    ' WPF Panel
    Private wpfPanel As Panel

    Public Sub New(ByRef rowTitle As String, ByVal xPos As Integer, ByVal yPos As Integer, ByRef panel As Panel)
        wpfPanel = panel
        currentFrame = 0
        rowTotalScore = 0
        bowlsMade = 0
        previousFrameTotalScore = 0
        currentFrameScore = 0
        rowFilled = False
        consecutiveStrikes = 0

        ' Create row title WPF display
        titleRect = DirectCast(XamlReader.Parse(titleRectangleTemp), Rectangle)
        titleRect.Margin = New Thickness(xPos, yPos, 0, 0)
        titleLabel = DirectCast(XamlReader.Parse(titleLabelTemp), Label)
        titleTextBox = DirectCast(XamlReader.Parse(titleTextBoxTemp), TextBox)
        titleLabel.Margin = New Thickness(xPos, yPos, 0, 0)
        titleTextBox.Margin = New Thickness(xPos + 1, yPos + 1, 0, 0)
        titleLabel.Content = ""
        titleTextBox.Text = rowTitle

        wpfPanel.Children.Add(titleRect)
        wpfPanel.Children.Add(titleLabel)
        wpfPanel.Children.Add(titleTextBox)

        ' Create non-three bowl frames
        frames(0) = New Frame(xPos + Frame.FRAME_WIDTH, yPos, wpfPanel, False, Nothing)
        For i = 2 To (NUM_FRAMES - 1)
            frames(i - 1) = New Frame(xPos + (i * Frame.FRAME_WIDTH), yPos, wpfPanel, False, frames(i - 2))
        Next

        ' Create three bowl frame
        frames(NUM_FRAMES - 1) = New Frame(xPos + (NUM_FRAMES * Frame.FRAME_WIDTH), yPos, wpfPanel, True, frames(NUM_FRAMES - 2))
    End Sub

    Public Sub MakeTitleUneditable()
        titleLabel.Content = titleTextBox.Text
        titleTextBox.Visibility = Visibility.Collapsed
        titleTextBox.IsEnabled = False

        wpfPanel.Children.Remove(titleTextBox)
    End Sub

    Public Sub FocusRowTitle()
        ' Highlight the row title
        titleTextBox.Focus()
        titleTextBox.Select(0, titleTextBox.Text.Length)
    End Sub

    Private Sub titleTextBox_GotFocus() Handles titleTextBox.GotFocus
        Focus(System.Windows.Media.Colors.LightSkyBlue)
    End Sub

    Private Sub titleTextBox_LostFocus() Handles titleTextBox.LostFocus
        UnFocus()
    End Sub

    Public Sub Focus(ByVal color As Color)
        titleRect.Fill = New SolidColorBrush(color)

        For i = 0 To NUM_FRAMES - 1
            frames(i).Focus(color)
        Next
    End Sub

    Public Sub UnFocus()
        titleRect.Fill = New SolidColorBrush(System.Windows.Media.Colors.LightGray)

        For i = 0 To NUM_FRAMES - 1
            frames(i).UnFocus()
        Next
    End Sub

    Public Sub MakeInvisible()
        titleRect.Visibility = Visibility.Collapsed
        titleLabel.Visibility = Visibility.Collapsed
        titleTextBox.Visibility = Visibility.Collapsed

        wpfPanel.Children.Remove(titleRect)
        wpfPanel.Children.Remove(titleLabel)
        wpfPanel.Children.Remove(titleTextBox)

        For i = 0 To (NUM_FRAMES - 1)
            frames(i).MakeInvisible()
        Next
    End Sub

    ' Returns if current frame was cleared
    Public Function Bowl(ByVal score As Integer) As Boolean
        If score < 0 Or score > 10 Then
            Return False
        End If

        If currentFrame < NUM_FRAMES Then
            rowTotalScore += score

            Dim achievedSpare As Boolean = False
            If score + currentFrameScore >= 10 Then
                If IsCurrFrameThreeBowl Then
                    If frames(currentFrame).bowlsMade < 2 Then
                        achievedSpare = True
                    Else
                        achievedSpare = False
                    End If
                Else
                    achievedSpare = True
                End If
            End If

            ' If current frame has been cleared
            Dim frameFilled As Boolean
            If frames(currentFrame).Filled Then
                currentFrameScore = score

                currentFrame += 1

                If currentFrame = (NUM_FRAMES - 1) Then
                    rowFilled = True
                End If

                If currentFrame < NUM_FRAMES Then
                    frames(currentFrame).Bowl(score)
                End If

                ' Return if the progressed current frame is filled
                frameFilled = frames(currentFrame).Filled
                If frameFilled Then
                    currentFrameScore = 0 ' Reset current frame score, since frame has progressed
                End If
            Else
                currentFrameScore += score

                frames(currentFrame).Bowl(score)

                If frames(currentFrame).Filled Then
                    frameFilled = True
                    currentFrameScore = 0 ' Reset current frame score, since frame has progressed
                Else
                    frameFilled = False ' Frame not cleared
                End If
            End If

            ' Compound scores
            For i = compoundFrames.Count - 1 To 0 Step -1

                If bowlsMade >= compoundFrames(i).bowlsMadeTillCompound + compoundFrames(i).numCompounds Then

                    ' Compound the scores of the number of bowls infront of the compound frame
                    For j = 1 To compoundFrames(i).numCompounds + 1
                        frames(compoundFrames(i).frameIndex).bonus +=
                                BowlVal(compoundFrames(i).bowlsMadeTillCompound + j)
                    Next

                    ' Update displays of frames ahead of compounded frame
                    For k = compoundFrames(i).frameIndex + 1 To currentFrame
                        frames(k).UpdateDisplay()
                    Next

                    compoundFrames.RemoveAt(i)
                End If

            Next


            If score = 10 Then
                ' On strike add to compounders
                ' 2 bonus scores added
                If Not IsInFrame(bowlsMade, 9) Then
                    compoundFrames.Insert(0, New Compounder(bowlsMade, currentFrame, 2))
                End If

                consecutiveStrikes += 1

                ' Handle animations for consecutive strikes
                Select Case consecutiveStrikes
                    Case 1
                        RaiseEvent AchievedSpecialScore(Me, New SpecialScoreEventArgs(SpecialScores.Strike))
                    Case 2
                        RaiseEvent AchievedSpecialScore(Me, New SpecialScoreEventArgs(SpecialScores._Double))
                    Case 3
                        RaiseEvent AchievedSpecialScore(Me, New SpecialScoreEventArgs(SpecialScores.Turkey))
                    Case 4
                        RaiseEvent AchievedSpecialScore(Me, New SpecialScoreEventArgs(SpecialScores.SquareBall))
                    Case 5
                        RaiseEvent AchievedSpecialScore(Me, New SpecialScoreEventArgs(SpecialScores.HighFive))
                    Case 6
                        RaiseEvent AchievedSpecialScore(Me, New SpecialScoreEventArgs(SpecialScores.SixPack))
                    Case 7
                        RaiseEvent AchievedSpecialScore(Me, New SpecialScoreEventArgs(SpecialScores.LuckySeven))
                    Case 8
                        RaiseEvent AchievedSpecialScore(Me, New SpecialScoreEventArgs(SpecialScores.Octopus))
                    Case 9
                        RaiseEvent AchievedSpecialScore(Me, New SpecialScoreEventArgs(SpecialScores.GoldenTurkey))
                    Case 10
                        RaiseEvent AchievedSpecialScore(Me, New SpecialScoreEventArgs(SpecialScores.DimeBag))
                    Case 11
                        RaiseEvent AchievedSpecialScore(Me, New SpecialScoreEventArgs(SpecialScores.AcesUp))
                    Case 12
                        RaiseEvent AchievedSpecialScore(Me, New SpecialScoreEventArgs(SpecialScores.PerfectGame))
                End Select
            ElseIf achievedSpare Then
                ' On spare add to compounders
                ' 1 bonus score added
                compoundFrames.Insert(0, New Compounder(bowlsMade, currentFrame, 1))
                RaiseEvent AchievedSpecialScore(Me, New SpecialScoreEventArgs(SpecialScores.Spare))
                consecutiveStrikes = 0
            Else
                consecutiveStrikes = 0
            End If

            bowlsMade += 1

            Return frameFilled
        End If

        Return False
    End Function
End Class
