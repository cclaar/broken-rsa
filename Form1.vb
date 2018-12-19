Imports System.IO
Imports System.Xml
Imports System.Net
Imports System.Text
Imports System.Security.Cryptography
Imports System.Security.Cryptography.RSACryptoServiceProvider

Public Class Form1

    Private publicKey As String = "<RSAKeyValue><Modulus>MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAzeOWTgP2+X7rXggUdg1qdOaEtl9jK3Uz+YtI8To7e2jbWi2r5apfJDUUBMCPWlP5hTQs3xKSCAUbQ6ykxL9sVraYBdCZuMfdkUcVsA2mF0owKnOXuuHkV6JwmBQ1OFop5uX08hQ82c1gMDsTUkM0U5MjF7Vq8YR38yctZVDfN5busmatCNAapNuwpSoc8TvRGUcpsc24VNy/cwo4Cz+spHqKIT4fTyHljgF0hVliaKaszjfJKc6OMFdHqOnn+oIFfbGWN7s7oqRuoKOQWHpl/ThtHGdPVSpC/+Oh42psHmy65h+yx04rF9OoRcrtZMCf5Vl7uwSG4jCzrpU7yLa77wIDAQAB</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>"
    Private publicKey2 As RSAParameters
    Dim pubkey As String

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Dim message As String = "xxxx"
        pubkey = File.ReadAllText(Application.StartupPath & "/pub.xml")
        'Dim encrypted As Byte() = encrypt(Encoding.UTF8.GetBytes(message))
        Dim EncryptedMessage As RSAResult = Encrypt(Encoding.UTF8.GetBytes(message), publicKey)
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 Or SecurityProtocolType.Tls12

        Using client As New Net.WebClient
            client.Proxy = New WebProxy()
            Dim reqparm As New Specialized.NameValueCollection
            reqparm.Add("data", EncryptedMessage.AsBase64String)
            Dim responsebytes = client.UploadValues("https://safenet.ninja/api/_handle_info.php", "POST", reqparm)
            Dim responsebody = (New System.Text.UTF8Encoding).GetString(responsebytes)
            Clipboard.SetText(EncryptedMessage.AsBase64String)
            MsgBox(responsebody.ToString)

            Application.Exit()

        End Using

    End Sub

    Public Shared Function Encrypt(ByVal Data() As Byte, ByVal Publickey As String) As RSAResult
        Try
            Dim RSA As RSACryptoServiceProvider = New RSACryptoServiceProvider(2048)
            RSA.FromXmlString(Publickey)
            Return New RSAResult(RSAEncrypt(Data, RSA.ExportParameters(False), False))
        Catch ex As Exception
            Throw New Exception("Encrypt(Bytes): " & ex.Message, ex)
        End Try
    End Function

    Public Shared Function Decrypt(ByVal Data() As Byte, ByVal Privatekey As String) As RSAResult
        Try
            Dim RSA As RSACryptoServiceProvider = New RSACryptoServiceProvider(2048)
            RSA.FromXmlString(Privatekey)
            Dim Result As New RSAResult(RSADecrypt(Data, RSA.ExportParameters(True), False))
            Return Result
        Catch ex As Exception
            Throw New Exception("Decrypt(): " & ex.Message, ex)
        End Try
    End Function

    Private Shared Function RSAEncrypt(ByVal DataToEncrypt() As Byte, ByVal RSAKeyInfo As RSAParameters, ByVal DoOAEPPadding As Boolean) As Byte()
        Try
            Dim encryptedData() As Byte
            Using RSA As New RSACryptoServiceProvider(2048)
                RSA.ImportParameters(RSAKeyInfo)
                encryptedData = RSA.Encrypt(DataToEncrypt, DoOAEPPadding)
            End Using
            Return encryptedData
        Catch e As CryptographicException
            Throw New Exception("RSAEncrypt(): " & e.Message, e)
        End Try
    End Function

    Private Shared Function RSADecrypt(ByVal DataToDecrypt() As Byte, ByVal RSAKeyInfo As RSAParameters, ByVal DoOAEPPadding As Boolean) As Byte()
        Try
            Dim decryptedData() As Byte
            Using RSA As New RSACryptoServiceProvider(2048)
                RSA.ImportParameters(RSAKeyInfo)
                decryptedData = RSA.Decrypt(DataToDecrypt, DoOAEPPadding)
            End Using
            Return decryptedData
        Catch e As CryptographicException
            Throw New Exception("RSADecrypt(): " & e.Message, e)
        End Try
    End Function

End Class
