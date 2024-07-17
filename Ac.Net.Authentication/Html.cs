namespace Ac.Net.Authentication
{
    internal class Html
    {
        internal static string successHtml = @"<!DOCTYPE html>
<html>
<title>Online HTML Editor</title>
<style>
    #ft {
        font-family: Arial, Helvetica, sans-serif;
        border-collapse: collapse;
        width: 50%;
        align=center
    }
    #ft td {
        border: 1px solid #ddd;
        padding: 8px;
        text-align: center;
        font-size:x-small;
    }
    #ft th {
        padding-top: 12px;
        padding-bottom: 12px;
        text-align: center;
        background-color: green;
        color: white;
    }
</style>
<head>
<script>
    window.close();
</head>
<body>
    <table id=""ft"" align=""center"">>
        <tr>
            <th>
                <h1>Forge Authentication Successful</h1>
            </th>
        </tr>
        <tr>
            <td>
                <h1>This window can be closed</h1>
            </td>
        </tr>
    </table>
</body>
</html>";

        internal static string errorHtml = @"<!DOCTYPE html>
<html>
<title>Online HTML Editor</title>
<style>
    #ft {
        font-family: Arial, Helvetica, sans-serif;
        border-collapse: collapse;
        width: 50%;
        align=center
    }
    #ft td {
        border: 1px solid #ddd;
        padding: 8px;
        text-align: center;
        font-size:x-small;
    }
    #ft th {
        padding-top: 12px;
        padding-bottom: 12px;
        text-align: center;
        background-color: red;
        color: white;
    }
</style>
<head>
</head>
<body>
    <table id=""ft"" align=""center"">>
        <tr>
            <th>
                <h1>Forge Authentication Failed</h1>
            </th>
        </tr>
        <tr>
            <td>
                <h1>User is not authorized</h1>
            </td>
        </tr>
        <tr>
            <td>
                <h1>This window can be closed</h1>
            </td>
        </tr>
    </table>
</body>
</html>";
    }
}