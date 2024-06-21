Imports System.Net.Http
Imports System.Text
Imports Newtonsoft.Json

Public Class ViewSubmissionsForm
    Private currentIndex As Integer = 0
    Private submissions As List(Of Submission)
    Private isEditing As Boolean = False

    Private Async Sub ViewSubmissionsForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Set KeyPreview to True to capture key events
        Me.KeyPreview = True

        submissions = Await LoadSubmissions()
        If submissions.Count > 0 Then
            ShowSubmission(currentIndex)
        End If
    End Sub

    Private Sub btnPrevious_Click(sender As Object, e As EventArgs) Handles btnPrevious.Click
        If currentIndex > 0 Then
            currentIndex -= 1
            ShowSubmission(currentIndex)
        End If
    End Sub

    Private Sub btnNext_Click(sender As Object, e As EventArgs) Handles btnNext.Click
        If currentIndex < submissions.Count - 1 Then
            currentIndex += 1
            ShowSubmission(currentIndex)
        End If
    End Sub

    Private Async Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        If currentIndex >= 0 AndAlso currentIndex < submissions.Count Then
            ' Confirm deletion with the user
            Dim result As DialogResult = MessageBox.Show("Are you sure you want to delete this submission?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Question)

            If result = DialogResult.Yes Then
                ' Call the DeleteSubmission method
                Await DeleteSubmission(currentIndex)
                ' Remove the submission from the list
                submissions.RemoveAt(currentIndex)
                ' Update the UI
                If submissions.Count = 0 Then
                    ClearSubmissionFields()
                ElseIf currentIndex >= submissions.Count Then
                    currentIndex -= 1
                    ShowSubmission(currentIndex)
                Else
                    ShowSubmission(currentIndex)
                End If
            End If
        End If
    End Sub

    Private Async Sub btnEdit_Click(sender As Object, e As EventArgs) Handles btnEdit.Click
        If isEditing Then
            ' Save changes and disable editing
            If currentIndex >= 0 AndAlso currentIndex < submissions.Count Then
                Dim updatedSubmission As New Submission With {
                    .Name = txtName.Text,
                    .Email = txtEmail.Text,
                    .Phone = txtPhone.Text,
                    .GithubLink = txtGithubLink.Text,
                    .StopwatchTime = txtStopwatchTime.Text
                }
                Await UpdateSubmission(currentIndex, updatedSubmission)
                submissions(currentIndex) = updatedSubmission
                ShowSubmission(currentIndex)
            End If
            ToggleReadOnly(True)
            btnEdit.Text = "Edit"
            isEditing = False
        Else
            ' Enable editing
            ToggleReadOnly(False)
            btnEdit.Text = "Save"
            isEditing = True
        End If
    End Sub

    Private Sub ToggleReadOnly(isReadOnly As Boolean)
        txtName.ReadOnly = isReadOnly
        txtEmail.ReadOnly = isReadOnly
        txtPhone.ReadOnly = isReadOnly
        txtGithubLink.ReadOnly = isReadOnly
        txtStopwatchTime.ReadOnly = isReadOnly
    End Sub

    Private Sub ClearSubmissionFields()
        txtName.Text = ""
        txtEmail.Text = ""
        txtPhone.Text = ""
        txtGithubLink.Text = ""
        txtStopwatchTime.Text = ""
    End Sub

    Private Sub ShowSubmission(index As Integer)
        Dim submission As Submission = submissions(index)
        txtName.Text = submission.Name
        txtEmail.Text = submission.Email
        txtPhone.Text = submission.Phone
        txtGithubLink.Text = submission.GithubLink
        txtStopwatchTime.Text = submission.StopwatchTime
        ToggleReadOnly(True)
    End Sub

    Private Async Function LoadSubmissions() As Task(Of List(Of Submission))
        Using client As New HttpClient()
            Dim response As HttpResponseMessage = Await client.GetAsync("http://localhost:3000/submissions")

            If response.IsSuccessStatusCode Then
                Dim jsonResponse As String = Await response.Content.ReadAsStringAsync()
                Return JsonConvert.DeserializeObject(Of List(Of Submission))(jsonResponse)
            Else
                MessageBox.Show("Error loading submissions", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return New List(Of Submission)()
            End If
        End Using
    End Function

    Private Async Function DeleteSubmission(index As Integer) As Task
        Using client As New HttpClient()
            Dim response As HttpResponseMessage = Await client.DeleteAsync($"http://localhost:3000/delete?index={index}")

            If Not response.IsSuccessStatusCode Then
                MessageBox.Show("Error deleting submission", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        End Using
    End Function

    Private Async Function UpdateSubmission(index As Integer, submission As Submission) As Task
        Using client As New HttpClient()
            Dim json As String = JsonConvert.SerializeObject(submission)
            Dim content As New StringContent(json, Encoding.UTF8, "application/json")
            Dim response As HttpResponseMessage = Await client.PutAsync($"http://localhost:3000/update?index={index}", content)

            If Not response.IsSuccessStatusCode Then
                MessageBox.Show("Error updating submission", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        End Using
    End Function

    ' Handle the KeyDown event to add keyboard shortcuts
    Private Sub ViewSubmissionsForm_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If e.Control AndAlso e.KeyCode = Keys.P Then
            ' Ctrl + P for Previous
            btnPrevious.PerformClick()
        ElseIf e.Control AndAlso e.KeyCode = Keys.N Then
            ' Ctrl + N for Next
            btnNext.PerformClick()
        End If
    End Sub

    Private Async Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click
        Dim emailToSearch As String = txtSearchEmail.Text
        If String.IsNullOrWhiteSpace(emailToSearch) Then
            MessageBox.Show("Please enter an email ID to search.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim submission As Submission = Await SearchSubmissionByEmail(emailToSearch)
        If submission IsNot Nothing Then
            ' Display the found submission
            txtName.Text = submission.Name
            txtEmail.Text = submission.Email
            txtPhone.Text = submission.Phone
            txtGithubLink.Text = submission.GithubLink
            txtStopwatchTime.Text = submission.StopwatchTime
        Else
            MessageBox.Show("Submission not found for the provided email ID.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    Private Async Function SearchSubmissionByEmail(email As String) As Task(Of Submission)
        Using client As New HttpClient()
            Dim response As HttpResponseMessage = Await client.GetAsync($"http://localhost:3000/search?email={email}")

            If response.IsSuccessStatusCode Then
                Dim jsonResponse As String = Await response.Content.ReadAsStringAsync()
                Return JsonConvert.DeserializeObject(Of Submission)(jsonResponse)
            Else
                Return Nothing
            End If
        End Using
    End Function
End Class
