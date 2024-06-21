Imports System.Net.Http
Imports System.Text
Imports Newtonsoft.Json

Public Class ApiClient
    Private ReadOnly baseUrl As String = "http://localhost:3000"

    ' Method to save a submission
    Public Async Function SaveSubmission(submission As Submission) As Task
        Using client As New HttpClient()
            Dim json As String = JsonConvert.SerializeObject(submission)
            Dim content As New StringContent(json, Encoding.UTF8, "application/json")

            ' Log the serialized JSON data
            Console.WriteLine("Sending request to server with JSON: " & json)

            Dim response As HttpResponseMessage = Await client.PostAsync($"{baseUrl}/submit", content)
            If response.IsSuccessStatusCode Then
                MessageBox.Show("Submission saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Else
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Console.WriteLine("Error response: " & errorContent)
                MessageBox.Show("Error saving submission: " & response.ReasonPhrase, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        End Using
    End Function

    ' Method to retrieve a submission
    Public Async Function GetSubmission(index As Integer) As Task(Of Submission)
        Using client As New HttpClient()
            Dim response As HttpResponseMessage = Await client.GetAsync($"{baseUrl}/read?index={index}")

            If response.IsSuccessStatusCode Then
                Dim json As String = Await response.Content.ReadAsStringAsync()
                Dim submission As Submission = JsonConvert.DeserializeObject(Of Submission)(json)
                Return submission
            Else
                Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                Console.WriteLine("Error response: " & errorContent)
                MessageBox.Show("Error retrieving submission: " & response.ReasonPhrase, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return Nothing
            End If
        End Using
    End Function
End Class
