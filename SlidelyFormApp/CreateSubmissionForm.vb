Imports System.Net.Http
Imports System.Text
Imports Newtonsoft.Json

Public Class CreateSubmissionForm
    Private stopwatch As New Stopwatch()
    Private WithEvents timer As New Timer()

    Private Sub CreateSubmissionForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Set KeyPreview to True to capture key events
        Me.KeyPreview = True

        ' Set up the timer
        timer.Interval = 1000 ' Update the display every second
        AddHandler timer.Tick, AddressOf Timer_Tick
    End Sub

    Private Sub Timer_Tick(sender As Object, e As EventArgs)
        UpdateStopwatchDisplay()
    End Sub

    Private Sub btnToggleStopwatch_Click(sender As Object, e As EventArgs) Handles btnToggleStopwatch.Click
        If stopwatch.IsRunning Then
            stopwatch.Stop()
            timer.Stop()
        Else
            stopwatch.Start()
            timer.Start()
        End If
        UpdateStopwatchDisplay()
    End Sub

    Private Async Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click
        Dim submission As New Submission With {
             .Name = txtName.Text,
             .Email = txtEmail.Text,
             .Phone = txtPhone.Text,
             .GithubLink = txtGithubLink.Text,
             .StopwatchTime = stopwatch.Elapsed.ToString("hh\:mm\:ss")
         }

        ' Log the data being sent
        Dim json As String = JsonConvert.SerializeObject(submission)
        Console.WriteLine("Submitting data: " & json)

        Await SaveSubmission(submission)
        MessageBox.Show("Submission saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Private Sub UpdateStopwatchDisplay()
        lblStopwatchTime.Text = stopwatch.Elapsed.ToString("hh\:mm\:ss")
    End Sub

    Private Async Function SaveSubmission(submission As Submission) As Task
        Using client As New HttpClient()
            Dim json As String = JsonConvert.SerializeObject(submission)
            Dim content As New StringContent(json, Encoding.UTF8, "application/json")
            Dim response As HttpResponseMessage = Await client.PostAsync("http://localhost:3000/submit", content)

            If Not response.IsSuccessStatusCode Then
                MessageBox.Show("Error saving submission", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        End Using
    End Function

    ' Handle the KeyDown event to add keyboard shortcuts
    Private Sub CreateSubmissionForm_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If e.Control AndAlso e.KeyCode = Keys.T Then
            ' Ctrl + T for Toggle Stopwatch
            btnToggleStopwatch.PerformClick()
        ElseIf e.Control AndAlso e.KeyCode = Keys.S Then
            ' Ctrl + S for Submit
            btnSubmit.PerformClick()
        End If
    End Sub
End Class
