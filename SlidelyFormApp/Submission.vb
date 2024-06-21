Imports Newtonsoft.Json

Public Class Submission
    <JsonProperty("name")>
    Public Property Name As String
    <JsonProperty("email")>
    Public Property Email As String
    <JsonProperty("phone")>
    Public Property Phone As String
    <JsonProperty("github_link")>
    Public Property GithubLink As String

    <JsonProperty("stopwatch_time")>
    Public Property StopwatchTime As String
End Class
