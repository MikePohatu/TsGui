option explicit

' Dim newExe: newExe = "TsGui.exe"
' Dim referenceExe: referenceExe = "TsGui.1.0.0.0.exe"

Dim objShell: Set objShell = CreateObject("Wscript.Shell")
Dim objFSO: Set objFSO = CreateObject("Scripting.FileSystemObject")
Dim objScriptFile: Set objScriptFile = objFSO.GetFile(Wscript.ScriptFullName)
Dim strScriptParentFolder: strScriptParentFolder = objScriptFile.ParentFolder
Dim objScriptParentFolder: set objScriptParentFolder = objFSO.GetFolder(strScriptParentFolder)

Dim intFileNumber, strCompareYn, bolCompare,strRefExe,strTestExe
Dim file, currentfile, allFiles

strRefExe = strScriptParentFolder & "\\Reference\\TsGui.exe"
strTestExe = strScriptParentFolder & "\\Test\\TsGui.exe"

'objShell.Run strPath
WScript.Echo "Which number config do you want to start with: "
intFileNumber = WScript.StdIn.ReadLine()

WScript.Echo "Do you want to run comparison tests (y/n): "
strCompareYn = WScript.StdIn.ReadLine()

WScript.Echo "****" & VbCrLf & VbCrLf
if strCompareYn = "y" Then 
	bolCompare = true
	WScript.Echo "Reference: " & strRefExe
else 
	bolCompare = false
end if
WScript.Echo "Test: " & strRefExe & VbCrLf & VbCrLf


set allFiles = objScriptParentFolder.Files
Dim bolStart: bolStart = false
Dim strStartFilePrefix: strStartFilePrefix = "Config." & IntToString(intFileNumber)

wscript.echo "Starting with file: " & strStartFilePrefix & "*.xml"



For Each file in allFiles
	currentfile = file.Name
	'wscript.echo currentfile
	If InStr(1,currentfile, strStartFilePrefix,1) > 0 Then
		bolStart = true
	End if

	if bolStart And InStr(1,currentfile, ".xml") > 0 Then
		wscript.echo currentfile

		if bolCompare then
			wscript.echo "Launching reference config: " & file.path
			objShell.Run strRefExe & " -config " &  file.path
			WScript.Echo "Press [ENTER] to continue..."
			WScript.StdIn.ReadLine
		end if

		wscript.echo "Launching test config: " & file.path
		objShell.Run strTestExe & " -config " &  file.path
		WScript.Echo "Press [ENTER] to continue..."
		WScript.StdIn.ReadLine
	end if
	intFileNumber = intFileNumber + 1
Next


Function IntToString(intVal)
	Dim s: s = CStr(intVal)

	if Len(s) < 2 then
		s = "0" & s
	end if

	IntToString = s
End Function