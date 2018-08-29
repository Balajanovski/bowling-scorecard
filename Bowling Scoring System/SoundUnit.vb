Imports System.Windows.Media

Public Class SoundUnit
    Private sfxPlayer As New MediaPlayer()
    Private WithEvents musicPlayer As New MediaPlayer()

    Private SFX_VOLUME As Double = 0.4
    Private MUSIC_VOLUME As Double = 0.18

    Private muted As Boolean
    Public ReadOnly Property isMuted As Boolean
        Get
            Return muted
        End Get
    End Property

    Public Sub New()
        muted = False

        sfxPlayer.Volume = SFX_VOLUME
        musicPlayer.Volume = MUSIC_VOLUME
    End Sub

    Public Sub playPinsFalling()
        sfxPlayer.Open(New Uri("../../Resources/pinsFalling.wav", UriKind.RelativeOrAbsolute))
        sfxPlayer.Play()
    End Sub

    Public Sub playMusic()
        musicPlayer.Open(New Uri("../../Resources/backingMusic.mp3", UriKind.RelativeOrAbsolute))
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
End Class
