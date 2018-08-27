Imports System.Windows.Markup

Public Class Frame
    ' Total Score
    Private frameTotalScore As Integer
    Public Property TotalScore() As Integer
        Get
            Return frameTotalScore
        End Get
        Set(value As Integer)
            frameTotalScore = value
        End Set
    End Property

    ' Previous frame so the score can be compounded
    Private previousFrame As Frame

    ' Bonus score from compounding (strikes / spares)
    Private bonusScore As Integer
    Public Property bonus As Integer
        Get
            Return bonusScore
        End Get
        Set(value As Integer)
            bonusScore = value
            UpdateDisplay()
        End Set
    End Property

    ' Scores stored in the bowl labels
    Private bowlScores As New List(Of Integer)
    Public Property bowlScore(index As Integer) As Integer
        Get
            If index < bowlScores.Count Then
                Return bowlScores.ElementAt(index)
            Else
                Return 0
            End If
        End Get
        Set(value As Integer)
            If index < bowlScores.Count Then
                bowlScores(index) = value
            Else
                Throw New System.Exception("Attempted to reset bowl score out of bounds")
            End If
        End Set
    End Property

    ' Score to display map
    ' Used to convert a bowl score of 10 to an X for the display
    Private ReadOnly scoreToDisplay() As Char = {"-"c, "1"c, "2"c, "3"c, "4"c, "5"c, "6"c, "7"c, "8"c, "9"c, "X"c}

    ' Dimensions of every frame, EXCEPT FOR THREE BOWL FRAMES
    Public Const FRAME_WIDTH As Integer = 65
    Public Const FRAME_HEIGHT As Integer = 65

    ' Is the last frame, which grants 3 bowls on a strike
    Private isThreeBowlFrame As Boolean
    Public ReadOnly Property ThreeBowlFrame As Boolean
        Get
            Return isThreeBowlFrame
        End Get
    End Property

    ' If a spare or strike was achieved, allow final bowl
    Private canBowlBonusBowl As Boolean

    ' Used for bowl() to know which bowl label to add the new score to
    Private scoresIndex As Integer
    Public ReadOnly Property bowlsMade() As Integer
        Get
            Return scoresIndex
        End Get
    End Property


    ' Is framed filled?
    Private frameFilled As Boolean
    Public ReadOnly Property Filled() As Boolean
        Get
            Return frameFilled
        End Get
    End Property

    ' WPF Panel
    Private wpfPanel As Panel

    ' WPF Element XAML templates
    Private Const frameRectangleTemp As String = "<Rectangle xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' 
xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' Fill=""#FFF4F4F5"" HorizontalAlignment=""Left"" 
 Stroke=""Black"" VerticalAlignment=""Top""/>"
    Private Const frameRectangle3BowlTemp As String = "<Rectangle xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' 
                            xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' Fill=""#FFF4F4F5"" 
HorizontalAlignment=""Left"" Height=""65"" Stroke=""Black"" VerticalAlignment=""Top"" Width=""87"" Grid.ColumnSpan=""2""/>"
    Private Const bowlRectangleTemp As String = "<Rectangle xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' 
                            xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' Fill=""#FFF4F4F5"" HorizontalAlignment=""Left"" Height=""33"" 
Stroke=""Black"" VerticalAlignment=""Top"" Width=""22""/>"
    Private Const bowlLabelTemp As String = "<Label xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' 
                            xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' Content="""" HorizontalAlignment=""Left"" 
Height=""29"" VerticalAlignment=""Top"" Width=""22""/>"
    Private Const frameLabelTemp As String = "<Label xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' 
                            xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' Content="""" HorizontalAlignment=""Left"" 
Height=""32"" VerticalAlignment=""Top"" Width=""55""/>"


    ' WPF Elements
    Private frameRectangle As Rectangle
    Private frameLabel As Label
    Private bowlRectangles As New List(Of Rectangle)
    Private bowlLabels As New List(Of Label)

    Public Sub New(ByVal xPos As Integer,
                   ByVal yPos As Integer, ByRef panel As Panel, ByVal isThreeBowl As Boolean, ByRef prevFrame As Frame)
        scoresIndex = 0
        isThreeBowlFrame = isThreeBowl
        wpfPanel = panel
        frameFilled = False
        previousFrame = prevFrame
        bonusScore = 0
        canBowlBonusBowl = False

        ''''''''''''''''''''
        ' Initialise bowls '
        ''''''''''''''''''''
        Dim bowlRectangle As Rectangle = DirectCast(XamlReader.Parse(bowlRectangleTemp), System.Windows.Shapes.Rectangle)
        Dim bowlLabel As Label = DirectCast(XamlReader.Parse(bowlLabelTemp), Label)
        Dim bowlRectangleWidth As Integer = bowlRectangle.Width
        Dim bowlLabelWidth As Integer = bowlLabel.Width

        Dim numBowls As Integer
        If isThreeBowl Then
            numBowls = 3
        Else
            numBowls = 2
        End If

        ' Generate bowl rectangles and labels according to template
        For i = 1 To numBowls
            ' Change position of bowl rectangle then copy to list
            bowlRectangle.Margin = New Thickness(xPos + (bowlRectangleWidth * i), yPos, 0, 0)
            bowlRectangles.Add(DirectCast(XamlReader.Parse(XamlWriter.Save(bowlRectangle)), Rectangle))

            ' Change position of bowl label then copy to list
            bowlLabel.Margin = New Thickness(xPos + (bowlLabelWidth * i), yPos, 0, 0)
            bowlLabels.Add(DirectCast(XamlReader.Parse(XamlWriter.Save(bowlLabel)), Label))
        Next


        ''''''''''''''''''''
        ' Initialize frame '
        ''''''''''''''''''''

        ' Generate frame rectangle
        If isThreeBowl Then
            frameRectangle = DirectCast(XamlReader.Parse(frameRectangle3BowlTemp), Rectangle)
        Else
            frameRectangle = DirectCast(XamlReader.Parse(frameRectangleTemp), Rectangle)
            frameRectangle.Width = FRAME_WIDTH
            frameRectangle.Height = FRAME_HEIGHT
        End If
        frameRectangle.Margin = New Thickness(xPos, yPos, 0, 0)

        ' Generate frame label
        Dim bowlRectangleHeight As Integer = bowlRectangle.Height
        frameLabel = DirectCast(XamlReader.Parse(frameLabelTemp), Label)
        frameLabel.Margin = New Thickness(xPos, yPos + bowlRectangleHeight, 0, 0)


        ''''''''''''''''''''''''''''''''
        ' Attach controls to WPF Panel '
        ''''''''''''''''''''''''''''''''

        wpfPanel.Children.Add(frameRectangle)
        wpfPanel.Children.Add(frameLabel)

        For Each rect In bowlRectangles
            wpfPanel.Children.Add(rect)
        Next

        For Each label In bowlLabels
            wpfPanel.Children.Add(label)
        Next

    End Sub

    Public Sub MakeInvisible()
        ' Remove WPF controls
        wpfPanel.Children.Remove(frameRectangle)
        wpfPanel.Children.Remove(frameLabel)

        For Each rect In bowlRectangles
            wpfPanel.Children.Remove(rect)
        Next

        For Each label In bowlLabels
            wpfPanel.Children.Remove(label)
        Next
    End Sub

    Public Sub Bowl(ByVal score As Integer)
        If scoresIndex < bowlLabels.Count And Not isThreeBowlFrame _
            Or scoresIndex < bowlLabels.Count - 1 And isThreeBowlFrame _
            Or scoresIndex < bowlLabels.Count And isThreeBowlFrame And canBowlBonusBowl Then

            bowlScores.Add(score)

            Dim totalScore As Integer = 0
            For Each score In bowlScores
                totalScore += score
            Next
            If totalScore = 10 And score <> 10 Then
                ' On a spare, display that a spare was achieved
                bowlLabels(scoresIndex).Content = "/"

                If isThreeBowlFrame Then
                    canBowlBonusBowl = True
                End If
            Else
                ' Else, just use the lookup table
                bowlLabels(scoresIndex).Content = scoreToDisplay(score)
            End If

            ' Add bonus
            totalScore += bonusScore

            If IsNothing(previousFrame) Then
                frameTotalScore = totalScore
            Else
                frameTotalScore = previousFrame.TotalScore + totalScore
            End If
            frameLabel.Content = CStr(frameTotalScore)

            scoresIndex += 1

            ' If a strike was bowled the frame must be filled (unless 3 bowl frame)
            If score = 10 And scoresIndex >= (bowlLabels.Count - 1) Then
                If isThreeBowlFrame Then
                    canBowlBonusBowl = True
                Else
                    frameFilled = True
                End If
            End If

            If scoresIndex >= bowlLabels.Count Or
                scoresIndex >= bowlLabels.Count - 1 And isThreeBowlFrame And Not canBowlBonusBowl Then
                frameFilled = True
            End If
        End If
    End Sub

    Public Sub Focus(ByVal color As Color)
        frameRectangle.Fill = New SolidColorBrush(color)

        For i = 0 To bowlRectangles.Count() - 1
            bowlRectangles(i).Fill = New SolidColorBrush(color)
        Next
    End Sub

    Public Sub UnFocus()
        frameRectangle.Fill = New SolidColorBrush(System.Windows.Media.Colors.LightGray)

        For i = 0 To bowlRectangles.Count() - 1
            bowlRectangles(i).Fill = New SolidColorBrush(System.Windows.Media.Colors.LightGray)
        Next
    End Sub

    Public Sub UpdateDisplay()
        Dim totalScore As Integer = 0
        For Each score In bowlScores
            totalScore += score
        Next

        ' Add bonus
        totalScore += bonusScore

        If IsNothing(previousFrame) Then
            frameTotalScore = totalScore
        Else
            frameTotalScore = previousFrame.TotalScore + totalScore
        End If
        frameLabel.Content = CStr(frameTotalScore)
    End Sub
End Class
