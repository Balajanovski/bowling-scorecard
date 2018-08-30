Imports System.Windows.Media
Imports System.Reflection
Imports System.IO

Public Class SoundUnit
    Private sfxPlayer As New MediaPlayer()
    Private WithEvents musicPlayer As New MediaPlayer()

    Private SFX_VOLUME As Double = 0.4
    Private MUSIC_VOLUME As Double = 0.14

    Private muted As Boolean
    Public ReadOnly Property isMuted As Boolean
        Get
            Return muted
        End Get
    End Property

    Private tempDir As String

    Public Sub New()
        muted = False

        sfxPlayer.Volume = SFX_VOLUME
        musicPlayer.Volume = MUSIC_VOLUME

        tempDir = CreateTempDirectory("temp-")
        ExportResource(Assembly.GetExecutingAssembly, "Bowling_Scoring_System", "backingMusic.mp3")
        ExportResource(Assembly.GetExecutingAssembly, "Bowling_Scoring_System", "pinsFalling.wav")
    End Sub

    ' Exports music resources from dll into temp folder so they can be played via the media player
    Private Sub ExportResource(ByRef assembly As Assembly, ByVal assemblyNamespace As String, ByVal mediaFile As String)
        Dim fullFileName As String = assemblyNamespace + "." + mediaFile
        Dim tmpFile = Path.Combine(Me.tempDir, fullFileName)

        Using input As Stream = assembly.GetManifestResourceStream(fullFileName)
            Using fileStream As Stream = File.OpenWrite(tmpFile)
                input.CopyTo(fileStream)
            End Using
        End Using
    End Sub

    Private Shared Function CreateTempDirectory(ByVal Optional prefix As String = "") As String
        While True
            Dim folder As String = Path.Combine(Path.GetTempPath(), prefix + Guid.NewGuid().ToString())
            If Not Directory.Exists(folder) Then
                Directory.CreateDirectory(folder)
                Return folder
            End If
        End While
    End Function

    Public Sub playPinsFalling()
        sfxPlayer.Open(New Uri(Path.Combine(tempDir, "Bowling_Scoring_System.pinsFalling.wav")))
        sfxPlayer.Play()
    End Sub

    Public Sub playMusic()
        musicPlayer.Open(New Uri(Path.Combine(tempDir, "Bowling_Scoring_System.backingMusic.mp3")))
        musicPlayer.Play()
    End Sub

    Public Sub stopMusic()
        musicPlayer.Stop()
    End Sub

    Public Sub toggleMute()
        muted = Not muted

        If muted Then
            sfxPlayer.Volume = 0
            musicPlayer.Volume = 0
        Else
            sfxPlayer.Volume = SFX_VOLUME
            musicPlayer.Volume = MUSIC_VOLUME
        End If
    End Sub

    Private Sub musicPlayer_Ended() Handles musicPlayer.MediaEnded
        musicPlayer.Position = TimeSpan.Zero
        musicPlayer.Play()
    End Sub

    Public Sub FreeResources()
        sfxPlayer.Stop()
        musicPlayer.Stop()

        sfxPlayer.Close()
        musicPlayer.Close()

        ' Delete all temporary music files and the temp directory
        Dim files() As String = Directory.GetFiles(tempDir)
        For Each fileSrc In files
            File.Delete(fileSrc)
        Next

        While Directory.GetFiles(tempDir).Count > 0
            ' Loop till files are deleted
            ' Added for thread safety as file deletion is asynchronous
        End While
        Directory.Delete(tempDir)
    End Sub
End Class
